using System;
using System.Linq;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Subscription;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    class TheEventConsumer : EventConsumer
    {
        readonly GpioPin m_GpioPin = null;
        readonly RasPiPinSubscription m_Subscription;
        readonly RasPiPinProperties m_Properties;

        internal Action ReadGpioPin;

        public TheEventConsumer(GpioPin theGpioPin, RasPiPinSubscription theSubscription, RasPiPinProperties theProperties )
        {
            m_Subscription = theSubscription;
            m_Properties = theProperties;
            m_GpioPin = theGpioPin;
        }

        public override void OnEvent(Payload thePayload)
        {
            if (m_Properties.isOutput)
            {
                var Value = thePayload.Subjects.FirstOrDefault();

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
            else if(m_Properties.isInput)
            {
                ReadGpioPin?.Invoke();
            }
        }
    }
}
