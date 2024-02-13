using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unosquare.RaspberryIO.Abstractions;

using MultiPlug.Base.Exchange;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Subscription;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Event;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    public class RasPiPin : RasPiPinProperties
    {
        private GpioPin m_GpioPin = null;

        public event Action SubscriptionsUpdated;
        public event Action EventUpdated;

        internal static bool FireEvents = false;
        private bool m_FireEvents = true;
        private bool InitStateSet = false;
        private bool m_Debouncing = false;

        internal event Action<Diagnostics.EventLogEntryCodes, string[]> Log;

        public bool CurrentState { get { return m_GpioPin.LastValue; } }

        public void SetState(bool theState)
        {
            m_GpioPin.Write(theState);
        }

        public RasPiPin(IGpioPin theGpioPin )
        {
            Debounce = 40;

            m_GpioPin = new GpioPin( theGpioPin );
            m_GpioPin.StateChange += new Action(OnPinWrite);

            var NewEventGuid = Guid.NewGuid().ToString();
            Event = new RasPiPinEvent { Guid = NewEventGuid, Id = NewEventGuid, Description = "BCM Pin: " + theGpioPin.BcmPinNumber.ToString(), Subjects = new string[] { "value" } };
            Event.CachedPayload = new Func<Payload>(CreateGroupData);

            BcmPinNumber = theGpioPin.BcmPinNumber.ToString();
        }

        private void OnPinWrite()
        {
            if (m_GpioPin.LastValue)
            {
                Log?.Invoke(Diagnostics.EventLogEntryCodes.PinOutHigh, new string[] { BcmPinNumber });
            }
            else
            {
                Log?.Invoke(Diagnostics.EventLogEntryCodes.PinOutLow, new string[] { BcmPinNumber });
            }

            Event.Invoke(CreateGroupData());
        }

        private void SetOutput(string theValue)    // Using String to use Empty as a unset value
        {
            Output = theValue;
            try
            {
                if (!string.IsNullOrEmpty(theValue))
                {

                    SuppressEvents();
                    if (string.Equals(theValue, c_True, StringComparison.OrdinalIgnoreCase))
                    {
                        m_GpioPin.PinMode = GpioPinDriveMode.Output;
                    }
                    else
                    {
                        m_GpioPin.PinMode = GpioPinDriveMode.Input;
                        m_GpioPin.InputPullMode = (GpioPinResistorPullMode)PullMode;

                        m_GpioPin.Read();

                        m_GpioPin.RegisterInterruptCallback(EdgeDetection.FallingAndRisingEdge, InterruptHandler);
                    }
                }
            }
            catch (Exception theException)
            {
                Log?.Invoke(Diagnostics.EventLogEntryCodes.GenericExceptionGPIO, new string[] { theException.Message, theException.InnerException != null ? theException.InnerException.Message : string.Empty });
            }
        }

        private void Init()
        {
            if (!InitStateSet)
            {
                InitStateSet = true;

                if (isOutput && InitState != 0)
                {
                    m_GpioPin.Write(InitState == 1 ? true : false);
                }
            }
        }


        internal void Shutdown()
        {
            if( isOutput && ShutdownState != 0)
            {
                m_GpioPin.Write(ShutdownState == 1 ? true : false);
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


        private void InterruptHandler()
        {
            if (m_Debouncing)
            {
                return;
            }

            if ( Debounce.Value > 0 )
            {
                m_Debouncing = true;
                Task.Run(async delegate
                {
                    bool LastValue = m_GpioPin.LastValue;

                    bool ValuePrevious = m_GpioPin.Read();

                    bool DoubleChecked = false;   // Check one more time for good measure

                    int DebouceDelay = Debounce.Value;

                    while ( true )
                    {
                        await Task.Delay(DebouceDelay);

                        bool ValueNow = m_GpioPin.Read();

                        if (ValueNow == ValuePrevious)
                        {
                            if(DoubleChecked)
                            {
                                ValuePrevious = ValueNow;
                                break;
                            }
                            else
                            {
                                DebouceDelay = 10;
                                DoubleChecked = true;
                            }
                        }
                        else
                        {
                            DoubleChecked = false;
                        }

                        ValuePrevious = ValueNow;
                    }

                    InvokeEvent(LastValue, ValuePrevious);
                    m_Debouncing = false;
                });
            }
            else
            {
                InvokeEvent(m_GpioPin.LastValue, m_GpioPin.Read());
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
                        Log?.Invoke(Diagnostics.EventLogEntryCodes.PinInHigh, new string[] { BcmPinNumber });
                    }
                    else
                    {
                        Log?.Invoke(Diagnostics.EventLogEntryCodes.PinInLow, new string[] { BcmPinNumber });
                    }

                    Event.Invoke(CreateGroupData());
                }
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString("ReadGpioPin() Suppressed"));
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
