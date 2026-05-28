using System;
using System.Threading.Tasks;
using MultiPlug.Ext.RasPi.GPIO.Utils.WiringPi;
using Unosquare.RaspberryIO.Abstractions;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    internal class GpioPin
    {
        readonly IGpioPinV2 m_GpioPin;

        internal Action StateChange;

        private bool m_PollInterrupt = false;

        public GpioPin(IGpioPinV2 theGpioPin)
        {
            m_GpioPin = theGpioPin;
        }

        internal int BcmPinNumber { get { return m_GpioPin.BcmPinNumber; } }

        internal GpioPinResistorPullMode InputPullMode
        {
            get
            {
                return m_GpioPin.InputPullMode;
            }
            set
            {
                m_GpioPin.InputPullMode = value;
            }
        }

        internal GpioPinDriveMode PinMode
        {
            get
            {
                return m_GpioPin.PinMode;
            }
            set
            {
                m_GpioPin.PinMode = value;
            }
        }

        internal bool LastValue { get; set; }

        internal bool Read()
        {
            LastValue = m_GpioPin.Read();
            return LastValue;
        }

        internal void StartListening(EdgeDetection theEdgeDetection, ulong theDebouncePeriod, Action<int> theCallback, Action theErrorLog)
        {
            if(m_PollInterrupt)
            {
                return;
            }

            m_PollInterrupt = true;

            Task.Run(() =>
            {
                bool LoggedOnce = false;

                while (m_PollInterrupt)
                {
                    var result = m_GpioPin.WaitForInterrupt(theEdgeDetection, theDebouncePeriod);

                    if(m_PollInterrupt)
                    {
                        if (result.statusOK == 1)
                        {
                            Task.Run(() =>
                            {
                                theCallback(result.edge);
                            });
                        }
                        else
                        {
                            if(LoggedOnce == false)
                            {
                                LoggedOnce = true;

                                if( theErrorLog != null )
                                {
                                    theErrorLog();
                                }

                            }
                        }
                    }
                }
            });
        }

        internal void StopListening()
        {
            if(m_PollInterrupt)
            {
                m_PollInterrupt = false;
                m_GpioPin.WaitForInterruptClose();
            }
        }

        internal void Write(bool value)
        {
            m_GpioPin.Write(value);

            if(LastValue != value )
            {
                LastValue = value;
                StateChange?.Invoke();
            }
        }
    }
}
