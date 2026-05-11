using System;
using MultiPlug.Ext.RasPi.GPIO.Utils.WiringPi;
using Unosquare.RaspberryIO.Abstractions;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    internal class GpioPin
    {
        readonly IGpioPinV2 m_GpioPin;

        internal Action StateChange;


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

        internal void RegisterInterruptCallback(EdgeDetection edgeDetection, Action callback)
        {
            m_GpioPin.RegisterInterruptCallback(edgeDetection, callback);
        }


        internal void RegisterInterruptCallback(EdgeDetection edgeDetection, ulong debounceperiod)
        {
            m_GpioPin.RegisterInterruptCallback(edgeDetection, debounceperiod);
        }

        internal void RemoveInterruptCallback()
        {
            m_GpioPin.RemoveInterruptCallback();
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
