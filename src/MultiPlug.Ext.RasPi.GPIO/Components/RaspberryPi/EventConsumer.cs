using System;
using System.Linq;
using MultiPlug.Base.Exchange;
using Unosquare.RaspberryIO.Abstractions;


namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    class TheEventConsumer : EventConsumer
    {
        readonly IGpioPin m_GpioPin = null;
        readonly Models.Components.Output.Subscription.Properties m_SubscriptionProperties;
        readonly EventCreator m_EventCreator;

        public TheEventConsumer(IGpioPin theGpioPin, Models.Components.Output.Subscription.Properties theSubscriptionProperties, EventCreator theEventCreator )
        {
            m_SubscriptionProperties = theSubscriptionProperties;
            m_EventCreator = theEventCreator;
            m_GpioPin = theGpioPin;
        }

        public override void OnEvent(Payload thePayload)
        {
            if (m_EventCreator.isOutput)
            {
                var Value = thePayload.Pairs.FirstOrDefault();

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
            else
            {
                m_EventCreator.ReadGpioPin();
            }
        }
    }
}
