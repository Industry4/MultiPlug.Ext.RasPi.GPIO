using System;
using System.Linq;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Subscription;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    internal class TheEventConsumer
    {
        readonly GpioPin m_GpioPin = null;
        readonly RasPiPinSubscription m_Subscription;
        readonly RasPiPinProperties m_Properties;

        readonly Action m_ReadGpioPin;

        internal TheEventConsumer(GpioPin theGpioPin, RasPiPinSubscription theSubscription, RasPiPinProperties theProperties, Action theAction )
        {
            m_Subscription = theSubscription;
            m_Properties = theProperties;
            m_GpioPin = theGpioPin;
            m_ReadGpioPin = theAction;
            m_Subscription.Event += OnEvent;
        }

        private void OnEvent(SubscriptionEvent obj)
        {
            if (m_Properties.isOutput)
            {
                foreach( var Value in obj.PayloadSubjects)
                {
                    if (Value == null)
                    {
                        return;
                    }

                    if (string.Equals(Value.Value, m_Subscription.High, StringComparison.OrdinalIgnoreCase))
                    {
                        m_GpioPin.Write(true);
                    }
                    else if (string.Equals(Value.Value, m_Subscription.Low, StringComparison.OrdinalIgnoreCase))
                    {
                        m_GpioPin.Write(false);
                    }
                }
            }
            else if (m_Properties.isInput)
            {
                m_ReadGpioPin?.Invoke();
            }
        }
    }
}
