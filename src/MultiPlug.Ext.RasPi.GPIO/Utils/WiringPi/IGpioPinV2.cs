using Unosquare.RaspberryIO.Abstractions;

namespace MultiPlug.Ext.RasPi.GPIO.Utils.WiringPi
{
    /// <summary>
    /// Added in WiringPi V3.16 https://github.com/WiringPi/WiringPi/blob/master/wiringPi/wiringPi.h
    /// </summary>
    public interface IGpioPinV2 : IGpioPin
    {
        void RegisterInterruptCallback(EdgeDetection edgeDetection, ulong debounceperiod);

        void RemoveInterruptCallback();
    }
}
