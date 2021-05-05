using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unosquare.RaspberryIO.Abstractions;

using MultiPlug.Base.Exchange;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.Output.Subscription;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    public class RasPiPin
    {
        private GpioPin m_GpioPin = null;

        public event EventHandler SubscriptionsUpdated;
        public event EventHandler EventUpdated;

        internal static bool FireEvents = false;
        private bool m_FireEvents = true;

        [DataMember]
        public Models.Components.Output.Properties Properties { private set; get; } = new Models.Components.Output.Properties();

        public bool CurrentState { get { return m_GpioPin.LastValue; } }

        public RasPiPin(IGpioPin theGpioPin )
        {
            m_GpioPin = new GpioPin( theGpioPin );

            var NewEventGuid = Guid.NewGuid().ToString();
            Properties.Event = new Base.Exchange.Event { Guid = NewEventGuid, Id = NewEventGuid, Description = "BCM Pin: " + theGpioPin.BcmPinNumber.ToString() };
            Properties.Event.CachedPayload = new Func<Payload>(CachedValue);


            Properties.BcmPinNumber = theGpioPin.BcmPinNumber.ToString();

        }

        private void SetOutput(string theValue)    // Using String to use Empty as a unset value
        {
            Properties.Output = theValue;
            try
            {
                if (!string.IsNullOrEmpty(theValue))
                {

                    SuppressEvents();
                    if (string.Equals(theValue, "true", StringComparison.OrdinalIgnoreCase))
                    {
                        m_GpioPin.PinMode = GpioPinDriveMode.Output;
                    }
                    else
                    {
                        m_GpioPin.PinMode = GpioPinDriveMode.Input;
                        m_GpioPin.InputPullMode = (GpioPinResistorPullMode)Properties.PullMode;

                        m_GpioPin.Read();

                        //  if (m_GpioPin.InterruptCallback == null )
                        //  { 
                        m_GpioPin.RegisterInterruptCallback(EdgeDetection.FallingAndRisingEdge, ReadGpioPin);
                        //  }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception " + e.Message);
            }
        }

        public void shutdown()
        {

        }

        private void SetPullMode(int theMode)
        {
            Properties.PullMode = theMode;

            if (Properties.Output == "false")
            {
                SuppressEvents();
                m_GpioPin.InputPullMode = (GpioPinResistorPullMode)Properties.PullMode;

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

        private void ReadGpioPin()
        {
            if (FireEvents && m_FireEvents && Properties.Output == "false")
            {
                bool LastOutputState = m_GpioPin.LastValue;
                bool CurrentPinState = m_GpioPin.Read();

                if (CurrentPinState != LastOutputState)
                {
                    Console.WriteLine(DateTime.Now.ToString("h:mm:ss") + "[I/O] IN [Bcm Pin] " + m_GpioPin.BcmPinNumber + (CurrentPinState ? " [state] High" : " [state] Low"));

                    Properties.Event.Fire(CreateGroupData());
                }
            }
        }

        private Payload CreateGroupData()
        {
            return new Payload(Properties.Event.Id, new PayloadSubject[] { new PayloadSubject(Properties.EventKey, m_GpioPin.LastValue ? Properties.EventHigh : Properties.EventLow) });
        }

        private Payload CachedValue()
        {
            return new Payload(Properties.Event.Id, new PayloadSubject[] { new PayloadSubject(Properties.EventKey, m_GpioPin.LastValue ? Properties.EventHigh : Properties.EventLow) });
        }

        internal void Update (Models.Components.Output.Properties theUpdatedProperties)
        {
            if( Properties.Output != theUpdatedProperties.Output)
            {
                SetOutput(theUpdatedProperties.Output);
            }

            if( Properties.PullMode != theUpdatedProperties.PullMode)
            {
                SetPullMode(theUpdatedProperties.PullMode);
            }

            if ( Properties.InitState != theUpdatedProperties.InitState )
            {
                Properties.InitState = theUpdatedProperties.InitState;
            }

            if( Properties.ShutdownState != theUpdatedProperties.ShutdownState)
            {
                Properties.ShutdownState = theUpdatedProperties.ShutdownState;
            }

            if(theUpdatedProperties.Event != null)
            {
                Update( new EventModel
                {
                    BcmPinNumber = theUpdatedProperties.BcmPinNumber,
                    Id = theUpdatedProperties.Event.Id,
                    Description = theUpdatedProperties.Event.Description,
                    Key = theUpdatedProperties.EventKey,
                    High = theUpdatedProperties.EventHigh,
                    Low = theUpdatedProperties.EventLow
                });
            }

            if( theUpdatedProperties.Subscriptions != null)
            {
                Update(theUpdatedProperties.Subscriptions);
            }
        }

        internal void Update(List<Models.Components.Output.Subscription.Subscription> theSubscriptions)
        {
            var NewSubscriptions = new List<Models.Components.Output.Subscription.Subscription>();

            bool SubscriptionChanged = false;

            foreach (var Subscription in theSubscriptions)
            {
                var SubSearch = Properties.Subscriptions.FirstOrDefault(s => s.Guid == Subscription.Guid);

                if (SubSearch == null)
                {
                    TheEventConsumer EventConsumer = new TheEventConsumer(m_GpioPin, Subscription.Properties, Properties);

                    EventConsumer.ReadGpioPin = new Action(ReadGpioPin);

                    Subscription.EventConsumer = EventConsumer;

                    if (Subscription.Properties == null) // TODO Temp
                    {
                    Subscription.Properties = new Models.Components.Output.Subscription.Properties { High = "1", Low = "0", KeyId = "value" };
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

                    SubSearch.Properties.KeyId = Subscription.Properties.KeyId;
                    SubSearch.Properties.High = Subscription.Properties.High;
                    SubSearch.Properties.Low = Subscription.Properties.Low;
                }
            }

            if( NewSubscriptions.Any())
            {
                Properties.Subscriptions.AddRange(NewSubscriptions);
                SubscriptionChanged = true;
            }

            if( SubscriptionChanged )
            {
                SubscriptionsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        internal void Update(EventModel theModel)
        {
            Properties.EventKey = theModel.Key;
            Properties.EventHigh = theModel.High;
            Properties.EventLow = theModel.Low;

            bool EventChanged = false;

            if( Properties.Event.Id != theModel.Id)
            {
                Properties.Event.Id = theModel.Id;
                EventChanged = true;
            }
            if( Properties.Event.Description != theModel.Description)
            {
                Properties.Event.Description = theModel.Description;
                EventChanged = true;
            }

            if( EventChanged && Properties.Output == "false")
            {
                EventUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        internal void Remove(string theSubscriptionGuid)
        {
            var SearchSubscription = Properties.Subscriptions.FirstOrDefault(s => s.Guid == theSubscriptionGuid);

            if (SearchSubscription != null)
            {
                Properties.Subscriptions.Remove(SearchSubscription);
                SubscriptionsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
