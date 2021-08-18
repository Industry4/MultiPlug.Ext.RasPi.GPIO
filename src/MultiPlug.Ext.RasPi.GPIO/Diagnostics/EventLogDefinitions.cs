using MultiPlug.Base.Diagnostics;

namespace MultiPlug.Ext.RasPi.GPIO.Diagnostics
{
    internal class EventLogDefinitions
    {
        internal const string DefinitionsId = "MultiPlug.Ext.RasPi.GPIO.EN";

        internal static EventLogDefinition[] Definitions { get; set; } = new EventLogDefinition[]
        {
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.SourceWiringPi,                           Source = (uint) EventLogEntryCodes.Reserved, StringFormat = "WiringPiLib",                                                  Type = EventLogEntryType.Information  },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.SourceGPIO,                               Source = (uint) EventLogEntryCodes.Reserved, StringFormat = "GPIO",                                                         Type = EventLogEntryType.Information  },

            new EventLogDefinition { Code = (uint) EventLogEntryCodes.MissingDllUnosquareWiringPi,              Source = (uint) EventLogEntryCodes.SourceWiringPi, StringFormat = "Missing file: Unosquare.WiringPi.dll",                   Type = EventLogEntryType.Error },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.MissingDllUnosquareRaspberryIO,           Source = (uint) EventLogEntryCodes.SourceWiringPi,  StringFormat = "Missing file: Unosquare.RaspberryIO.dll",               Type = EventLogEntryType.Error },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.MissingDllUnosquareRaspberryAbstractions, Source = (uint) EventLogEntryCodes.SourceWiringPi,  StringFormat = "Missing file: Unosquare.Raspberry.Abstractions.dll",    Type = EventLogEntryType.Error },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.MissingDllSwanLite,                       Source = (uint) EventLogEntryCodes.SourceWiringPi,  StringFormat = "Missing file: Swan.Lite.dll",                           Type = EventLogEntryType.Error },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.MissingDllSwan,                           Source = (uint) EventLogEntryCodes.SourceWiringPi,  StringFormat = "Missing file: Swan.dll",                                Type = EventLogEntryType.Error },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.PlatformNotSupportedException,            Source = (uint) EventLogEntryCodes.SourceWiringPi,  StringFormat = "Platform Not Supported Exception",                      Type = EventLogEntryType.Warning },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.GenericExceptionLib,                      Source = (uint) EventLogEntryCodes.SourceWiringPi,  StringFormat = "Exception Message: {0} Inner Message: {1}",             Type = EventLogEntryType.Error },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.MissingWiringPiLib,                      Source = (uint) EventLogEntryCodes.SourceWiringPi,  StringFormat = "WiringPi 2.52 Prerequisite Not Met",                     Type = EventLogEntryType.Error },

            new EventLogDefinition { Code = (uint) EventLogEntryCodes.PinInLow,                                 Source = (uint) EventLogEntryCodes.SourceGPIO,  StringFormat = "IN BCM Pin: {0} State: LOW",                                Type = EventLogEntryType.Information },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.PinInHigh,                                Source = (uint) EventLogEntryCodes.SourceGPIO,  StringFormat = "IN BCM Pin: {0} State: HIGH",                               Type = EventLogEntryType.Information },

            new EventLogDefinition { Code = (uint) EventLogEntryCodes.PinOutLow,                                Source = (uint) EventLogEntryCodes.SourceGPIO,  StringFormat = "OUT BCM Pin: {0} State: LOW",                               Type = EventLogEntryType.Information },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.PinOutHigh,                               Source = (uint) EventLogEntryCodes.SourceGPIO,  StringFormat = "OUT BCM Pin: {0} State: HIGH",                              Type = EventLogEntryType.Information },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.GenericExceptionGPIO,                      Source = (uint) EventLogEntryCodes.SourceGPIO,  StringFormat = "Exception Message: {0} Inner Message: {1}",                Type = EventLogEntryType.Error },
        };
    }
}
