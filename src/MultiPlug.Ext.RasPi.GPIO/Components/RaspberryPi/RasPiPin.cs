using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.Output.Subscription;
using Unosquare.RaspberryIO.Abstractions;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    public class RasPiPin
    {
        private IGpioPin m_GpioPin = null;

        private EventCreator m_EventCreator = null;

        public event EventHandler SubscriptionsUpdated;
        public event EventHandler EventUpdated;

        [DataMember]
        public Models.Components.Output.Properties Properties { private set; get; } = new Models.Components.Output.Properties();

        public RasPiPin(IGpioPin theGpioPin )
        {
            m_GpioPin = theGpioPin;

            m_EventCreator = new EventCreator(m_GpioPin, Properties);
            m_EventCreator.SetOutput(false);

            var NewEventGuid = Guid.NewGuid().ToString();
            Properties.Event = new Base.Exchange.Event { Guid = NewEventGuid, Id = NewEventGuid, Description = "BCM Pin: " + theGpioPin.BcmPinNumber.ToString(), Object = m_EventCreator };

            Properties.BcmPinNumber = theGpioPin.BcmPinNumber.ToString();
        }

        public void Update (Models.Components.Output.Properties theUpdatedProperties)
        {
            if( Properties.Output != theUpdatedProperties.Output)
            {
                m_EventCreator.SetOutput(theUpdatedProperties.Output);
            }

            if( Properties.PullMode != theUpdatedProperties.PullMode)
            {
                m_EventCreator.SetPullMode(theUpdatedProperties.PullMode);
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

        public void Update(List<Subscription> theSubscriptions)
        {
            var NewSubscriptions = new List<Subscription>();

            bool SubscriptionChanged = false;

            foreach (var Subscription in theSubscriptions)
            {
                var SubSearch = Properties.Subscriptions.FirstOrDefault(s => s.Guid == Subscription.Guid);

                if (SubSearch == null)
                {
                    Subscription.EventConsumer = new TheEventConsumer(m_GpioPin, Subscription.Properties, m_EventCreator);

                    if(Subscription.Properties == null) // TODO Temp
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

            if( EventChanged && ! Properties.Output)
            {
                EventUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Remove(string theSubscriptionGuid)
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
