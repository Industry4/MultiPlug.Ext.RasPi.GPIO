using System;
using MultiPlug.Base.Exchange;
using Unosquare.RaspberryIO.Abstractions;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    public class EventCreator : EventableBase
    {
        public override event MPEventHandler Update;

        readonly IGpioPin m_GpioPin = null;
        readonly Models.Components.Output.Properties m_OutputProperties;

        public static bool FireEvents = false;
        private bool m_FireEvents = true;

        private bool m_LastOutputState = true;

        public EventCreator(IGpioPin theGpioPin, Models.Components.Output.Properties theOutputProperties )
        {
            m_OutputProperties = theOutputProperties;
            m_GpioPin = theGpioPin;
        }

        public void SetOutput(bool isTrue)
        {
            m_OutputProperties.Output = isTrue;
            try
            {
                SuppressEvents();
                if (isTrue)
                {
                    m_GpioPin.PinMode = GpioPinDriveMode.Output;
                }
                else
                {
                    m_GpioPin.PinMode = GpioPinDriveMode.Input;
                    m_GpioPin.InputPullMode = (GpioPinResistorPullMode) m_OutputProperties.PullMode;

                    m_LastOutputState = m_GpioPin.Read();

                  //  if (m_GpioPin.InterruptCallback == null )
                  //  { 
                        m_GpioPin.RegisterInterruptCallback(EdgeDetection.FallingAndRisingEdge, ReadGpioPin);
                  //  }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception " + e.Message);
            }
        }

        public void SetPullMode(int theMode)
        {
            m_OutputProperties.PullMode = theMode;

            if (!m_OutputProperties.Output)
            {
                SuppressEvents();
                m_GpioPin.InputPullMode = (GpioPinResistorPullMode)m_OutputProperties.PullMode;

            }
        }

        public bool isOutput { get { return m_OutputProperties.Output; } }

        private void SuppressEvents()
        {
            m_FireEvents = false;
            System.Timers.Timer RunOnce = new System.Timers.Timer(400);
            RunOnce.Elapsed += (s, e) => { m_FireEvents = true; };
            RunOnce.AutoReset = false;
            RunOnce.Start();
        }

        public void ReadGpioPin()
        {
            if (FireEvents && m_FireEvents && ! m_OutputProperties.Output)
            {
                bool PinState = m_GpioPin.Read();

                if(PinState != m_LastOutputState)
                {
                    m_LastOutputState = PinState;
                    Console.WriteLine("ReadGpioPin() BcmPin: " + m_GpioPin.BcmPin);
                    Update?.Invoke(CreateGroupData());
                }
            }
        }

        private Payload CreateGroupData()
        {
            EventCreator Event = (EventCreator)m_OutputProperties.Event.Object;

            // TODO May not need the uncommented  - It may be a way of getting the current pin value if it is a output.? 
            //if (m_OutputProperties.Output)
            //{
            //    return new Group
            //    {
            //        Id = m_OutputProperties.Event.Id,
            //        Pairs = new Pair[0]
            //    };
            //}
            //else
            //{
                return new Payload(m_OutputProperties.Event.Id, new Pair[] { new Pair( m_OutputProperties.EventKey, m_LastOutputState ? m_OutputProperties.EventHigh : m_OutputProperties.EventLow ) } );
            //}
        }

        public override Payload CachedValue()
        {
            return CreateGroupData();
        }
    }
}
