
namespace MultiPlug.Ext.RasPi.GPIO.Diagnostics
{
    internal enum EventLogEntryCodes
    {
        Reserved = 0,
        SourceWiringPi = 1,
        SourceGPIO = 2,

        MissingDllUnosquareWiringPi = 50,
        MissingDllUnosquareRaspberryIO = 51,
        MissingDllUnosquareRaspberryAbstractions = 52,
        MissingDllSwanLite = 53,
        MissingDllSwan = 54,
        PlatformNotSupportedException = 55,
        GenericExceptionLib = 56,
        MissingWiringPiLib = 57,

        PinOutLow = 70,
        PinOutHigh = 71,
        PinInLow = 72,
        PinInHigh = 73,
        GenericExceptionGPIO = 74,
    }
}
