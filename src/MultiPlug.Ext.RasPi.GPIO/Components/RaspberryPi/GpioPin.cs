using System;
using Unosquare.RaspberryIO.Abstractions;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    internal class GpioPin
    {
        readonly IGpioPin m_GpioPin;

        public GpioPin(IGpioPin theGpioPin)
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

        internal bool LastValue { get; private set; }

        internal bool Read()
        {
            LastValue = m_GpioPin.Read();
            return LastValue;
        }

        internal void RegisterInterruptCallback(EdgeDetection edgeDetection, Action callback)
        {
            m_GpioPin.RegisterInterruptCallback(edgeDetection, callback);
        }

        internal void Write(bool value)
        {
            LastValue = value;
            m_GpioPin.Write(value);
        }
    }
}
