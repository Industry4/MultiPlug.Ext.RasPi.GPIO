using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unosquare.RaspberryIO.Abstractions;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Event;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Subscription;
using MultiPlug.Ext.RasPi.GPIO.Utils.WiringPi;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    public class RasPiPin : RasPiPinProperties
    {
        private GpioPin m_GpioPin = null;

        public event Action SubscriptionsUpdated;
        public event Action EventUpdated;

        internal static bool FireEvents = false;
        private bool m_FireEvents = true;
        private bool m_InitStateSet = false;
        private readonly object m_StateChangeLock = new object();
        private Stopwatch m_StateChangeStopWatch = new Stopwatch();
        private int m_TooQuickCounter = 0;

        internal event Action<Diagnostics.EventLogEntryCodes, string[]> LogVerbose;
        internal event Action<Diagnostics.EventLogEntryCodes, string[]> LogError;

        public bool CurrentState { get { return m_GpioPin.LastValue; } }

        public int PinNumber { get { return m_GpioPin.BcmPinNumber; } }

        public void SetState(bool theState)
        {
            m_GpioPin.Write(theState);
        }

        public RasPiPin(IGpioPinV2 theGpioPin )
        {
            Debounce = 40;

            m_GpioPin = new GpioPin( theGpioPin );
            m_GpioPin.StateChange += new Action(OnPinWrite);

            var NewEventGuid = Guid.NewGuid().ToString();
            Event = new RasPiPinEvent { Guid = NewEventGuid, Id = NewEventGuid, Description = "BCM Pin: " + theGpioPin.BcmPinNumber.ToString(), Subjects = new string[] { "value" } };
            Event.CachedPayload = new Func<Payload>(CreateGroupData);

            base.BcmPinNumber = theGpioPin.BcmPinNumber.ToString();

            m_StateChangeStopWatch.Start();
        }

        private void OnPinWrite()
        {
            if (m_GpioPin.LastValue)
            {
                LogVerbose?.Invoke(Diagnostics.EventLogEntryCodes.PinOutHigh, new string[] { base.BcmPinNumber });
            }
            else
            {
                LogVerbose?.Invoke(Diagnostics.EventLogEntryCodes.PinOutLow, new string[] { base.BcmPinNumber });
            }

            Event.Invoke(CreateGroupData());
        }

        private void SetOutput(string theValue)    
        {
            try
            {
                if (string.IsNullOrEmpty(theValue) == false) // Using String to use Empty as a unset value
                {
                    SuppressEvents();

                    // Is OutPut
                    if (string.Equals(theValue, c_True, StringComparison.OrdinalIgnoreCase))
                    {
                        // Currently in Input, now being Output.
                        if (string.Equals(Output, c_False, StringComparison.OrdinalIgnoreCase))
                        {
                            m_GpioPin.StopListening();
                        }

                        m_GpioPin.PinMode = GpioPinDriveMode.Output;

                        Output = theValue;
                    }
                    // Is InPut
                    else
                    {
                        m_GpioPin.PinMode = GpioPinDriveMode.Input;
                        m_GpioPin.InputPullMode = (GpioPinResistorPullMode)PullMode;

                        m_GpioPin.Read();
                        m_GpioPin.StartListening(EdgeDetection.FallingAndRisingEdge, Convert.ToUInt64(Debounce.Value), OnInputChange, OnWaitForInterruptError);

                        Output = theValue;
                    }
                }
                else // Unset
                {
                    // Currently a Input, now being Unset.
                    if(string.Equals(Output, c_False, StringComparison.OrdinalIgnoreCase))
                    {
                        m_GpioPin.StopListening();
                    }

                    Output = theValue;
                }
            }
            catch (Exception theException)
            {
                LogError?.Invoke(Diagnostics.EventLogEntryCodes.GenericExceptionGPIO, new string[] { theException.Message, theException.InnerException != null ? theException.InnerException.Message : string.Empty });
                Output = theValue;
            }
        }

        private void OnWaitForInterruptError()
        {
            LogError?.Invoke(Diagnostics.EventLogEntryCodes.WaitForInterruptError, new string[] { PinNumber.ToString() });
        }

        private void Init()
        {
            if (!m_InitStateSet)
            {
                m_InitStateSet = true;

                if (isOutput && InitState != 0)
                {
                    m_GpioPin.Write(InitState == 1 ? true : false);
                }
            }
        }


        internal void Shutdown()
        {
            if (isOutput && ShutdownState != 0)
            {
                m_GpioPin.Write(ShutdownState == 1 ? true : false);
            }

            if (string.IsNullOrEmpty(Output) == false) // String Empty is Unset
            {
                if (isOutput == false)
                {
                    m_GpioPin.StopListening();
                }
            }
        }

        private void SetPullMode(int theMode)
        {
            PullMode = theMode;

            if (Output == c_False)
            {
                SuppressEvents();
                m_GpioPin.InputPullMode = (GpioPinResistorPullMode)PullMode;

            }
        }

        private void SuppressEvents()
        {
            m_FireEvents = false;
            System.Timers.Timer RunOnce = new System.Timers.Timer(400);
            RunOnce.Elapsed += (s, e) => { m_FireEvents = true; };
            RunOnce.AutoReset = false;
            RunOnce.Start();
        }

        internal void OnInputChange(int theEdge)
        {
            lock (m_StateChangeLock)
            {
                m_StateChangeStopWatch.Stop();
                TimeSpan ts = m_StateChangeStopWatch.Elapsed;

                // Has the State toggled too quickly indicating a setup or hardware fault, eg floating pin.
                if (ts.Days == 0 && ts.Hours == 0 && ts.Seconds == 0 && ts.Milliseconds < 50)
                {
                    m_TooQuickCounter++;

                    if (m_TooQuickCounter == 5)
                    {
                        m_TooQuickCounter = 0;
                        LogError?.Invoke(Diagnostics.EventLogEntryCodes.PinDisabled, new string[] { BcmPinNumber });
                        var lv = m_GpioPin.LastValue;
                        m_GpioPin.LastValue = false;
                        InvokeEvent(lv, m_GpioPin.LastValue); // Set the Output to Low
                        Output = string.Empty; // Set pin to Unset so User has to investigate issue.
                        m_GpioPin.StopListening();
                        return;
                    }
                }
                else
                {
                    m_TooQuickCounter = 0;
                }

                m_StateChangeStopWatch.Restart();

                var LastValue = m_GpioPin.LastValue;
                m_GpioPin.LastValue = theEdge == 2;
                InvokeEvent(LastValue, m_GpioPin.LastValue);
            }
        }

        private void EventHandler()
        {
            InvokeEvent(m_GpioPin.LastValue, m_GpioPin.Read());
        }

        private void InvokeEvent(bool theLastOutputState, bool theCurrentPinState)
        {
            if (FireEvents && m_FireEvents && Output == c_False)
            {
                if (theCurrentPinState != theLastOutputState)
                {
                    if (theCurrentPinState)
                    {
                        LogVerbose?.Invoke(Diagnostics.EventLogEntryCodes.PinInHigh, new string[] { base.BcmPinNumber });
                    }
                    else
                    {
                        LogVerbose?.Invoke(Diagnostics.EventLogEntryCodes.PinInLow, new string[] { base.BcmPinNumber });
                    }

                    Event.Invoke(CreateGroupData());
                }
            }
        }

        private Payload CreateGroupData()
        {
            return new Payload(Event.Id, new PayloadSubject[] { new PayloadSubject(Event.Subjects[0], m_GpioPin.LastValue ? Event.HighValue : Event.LowValue) });
        }

        internal void Update(string theDescription)
        {
            if( theDescription != Event.Description)
            {
                Event.Description = theDescription;
                EventUpdated?.Invoke();
            }
        }

        internal void Update (RasPiPinProperties theUpdatedProperties)
        {
            if( Output != theUpdatedProperties.Output)
            {
                SetOutput(theUpdatedProperties.Output);
            }

            if( PullMode != theUpdatedProperties.PullMode)
            {
                SetPullMode(theUpdatedProperties.PullMode);
            }

            if ( InitState != theUpdatedProperties.InitState )
            {
                InitState = theUpdatedProperties.InitState;
            }

            if( ShutdownState != theUpdatedProperties.ShutdownState)
            {
                ShutdownState = theUpdatedProperties.ShutdownState;
            }

            if(theUpdatedProperties.Debounce != null && 
                theUpdatedProperties.Debounce.Value != Debounce.Value && 
                theUpdatedProperties.Debounce.Value >= 0)
            {
                Debounce = theUpdatedProperties.Debounce;
            }

            if(theUpdatedProperties.Event != null)
            {
                Update( new EventModel
                {
                    BcmPinNumber = theUpdatedProperties.BcmPinNumber,
                    EventId = theUpdatedProperties.Event.Id,
                    EventDescription = theUpdatedProperties.Event.Description,
                    Subject = theUpdatedProperties.Event.Subjects[0],
                    High = theUpdatedProperties.Event.HighValue,
                    Low = theUpdatedProperties.Event.LowValue
                });
            }

            Init();

            if( theUpdatedProperties.Subscriptions != null)
            {
                Update(theUpdatedProperties.Subscriptions);
            }
        }

        internal void Update(RasPiPinSubscription[] theSubscriptions)
        {
            var NewSubscriptions = new List<RasPiPinSubscription>();

            bool SubscriptionChanged = false;

            foreach (var Subscription in theSubscriptions)
            {
                var SubSearch = Subscriptions.FirstOrDefault(s => s.Guid == Subscription.Guid);

                if (SubSearch == null)
                {
                    TheEventConsumer EventConsumer = new TheEventConsumer(m_GpioPin, Subscription, this, new Action(EventHandler));

                    if (string.IsNullOrEmpty(Subscription.High))
                    {
                        Subscription.High = "1";
                    }

                    if (string.IsNullOrEmpty(Subscription.Low))
                    {
                        Subscription.Low = "0";
                    }

                    NewSubscriptions.Add(Subscription);
                }
                else
                {
                    if( SubSearch.Id != Subscription.Id)
                    {
                        SubSearch.Id = Subscription.Id;
                        SubscriptionChanged = true;
                    }

                    SubSearch.High = Subscription.High;
                    SubSearch.Low = Subscription.Low;
                }
            }

            if( NewSubscriptions.Any())
            {
                List<RasPiPinSubscription> CurrentSubscriptions = new List<RasPiPinSubscription>(Subscriptions);
                CurrentSubscriptions.AddRange(NewSubscriptions);
                Subscriptions = CurrentSubscriptions.ToArray();
                SubscriptionChanged = true;
            }

            if( SubscriptionChanged )
            {
                SubscriptionsUpdated?.Invoke();
            }
        }

        internal void Update(EventModel theModel)
        {
            Event.HighValue = theModel.High;
            Event.LowValue = theModel.Low;

            bool EventChanged = false;

            if(Event.Subjects[0] != theModel.Subject)
            {
                Event.Subjects[0] = theModel.Subject;
                EventChanged = true;
            }

            if ( Event.Id != theModel.EventId)
            {
                Event.Id = theModel.EventId;
                EventChanged = true;
            }
            if( Event.Description != theModel.EventDescription)
            {
                Event.Description = theModel.EventDescription;
                EventChanged = true;
            }

            if( EventChanged && Output == c_False)
            {
                EventUpdated?.Invoke();
            }
        }

        internal void Remove(string theSubscriptionGuid)
        {
            var SearchSubscription = Subscriptions.FirstOrDefault(Subscription => Subscription.Guid == theSubscriptionGuid);

            if (SearchSubscription != null)
            {
                List<RasPiPinSubscription> CurrentSubscriptions = new List<RasPiPinSubscription>(Subscriptions);
                CurrentSubscriptions.Remove(SearchSubscription);
                Subscriptions = CurrentSubscriptions.ToArray();
                SubscriptionsUpdated?.Invoke();
            }
        }
    }
}
