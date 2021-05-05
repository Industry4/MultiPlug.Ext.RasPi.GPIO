using System;
using System.Linq;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    class TheEventConsumer : EventConsumer
    {
        readonly GpioPin m_GpioPin = null;
        readonly Models.Components.Output.Subscription.Properties m_SubscriptionProperties;
        readonly Models.Components.Output.Properties m_Properties;

        internal Action ReadGpioPin;

        public TheEventConsumer(GpioPin theGpioPin, Models.Components.Output.Subscription.Properties theSubscriptionProperties, Models.Components.Output.Properties theEventCreator )
        {
            m_SubscriptionProperties = theSubscriptionProperties;
            m_Properties = theEventCreator;
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

                if (string.Equals(Value.Value, m_SubscriptionProperties.High, StringComparison.OrdinalIgnoreCase))
                {
                    m_GpioPin.Write(true);
                }
                else if (string.Equals(Value.Value, m_SubscriptionProperties.Low, StringComparison.OrdinalIgnoreCase))
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
