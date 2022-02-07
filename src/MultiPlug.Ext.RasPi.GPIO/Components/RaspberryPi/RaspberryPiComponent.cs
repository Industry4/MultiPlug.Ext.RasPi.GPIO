using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;
using MultiPlug.Base.Exchange.API;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Subscription;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi;
using MultiPlug.Ext.RasPi.GPIO.Diagnostics;
using MultiPlug.Ext.RasPi.GPIO.Utils.Swan;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    public class RaspberryPiComponent
    {
        public ILoggingService LoggingService { get; private set; }

        public event Action SubscriptionsUpdated;
        public event Action EventsUpdated;

        [DataMember]
        public RasPiPin[] GPIO { get; set; } = new RasPiPin[0];

        internal bool PlatformSupported { get; private set; } = true;
        internal bool WiringPiInstalled { get; private set; } = true;

        private const string c_GPIOVersionDefault = "Unknown";

        internal string GPIOVersion { get; private set; } = c_GPIOVersionDefault;

        [DataMember]
        public int LoggingLevel { get; set; }



        public RaspberryPiComponent()
        {
        }

        internal void Init(IMultiPlugServices theMultiPlugServices)
        {
            theMultiPlugServices.Logging.RegisterDefinitions(EventLogDefinitions.DefinitionsId, EventLogDefinitions.Definitions, true);

            LoggingService = theMultiPlugServices.Logging.New("RasPiGPIO", EventLogDefinitions.DefinitionsId);

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unosquare.WiringPi.dll")))
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.MissingDllUnosquareWiringPi);
                return;
            }

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unosquare.RaspberryIO.dll")))
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.MissingDllUnosquareRaspberryIO);
                return;
            }

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unosquare.Raspberry.Abstractions.dll")))
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.MissingDllUnosquareRaspberryAbstractions);
                return;
            }

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Swan.Lite.dll")))
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.MissingDllSwanLite);
                return;
            }

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Swan.dll")))
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.MissingDllSwan);
                return;
            }

            Task<ProcessResult> VersionNumberTask = ProcessRunner.GetProcessResultAsync("gpio", "-v");

            try
            {
                VersionNumberTask.Wait();
            }
            catch
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.MissingWiringPiLib);
                WiringPiInstalled = false;
                return;
            }

            if (VersionNumberTask.Result.Okay())
            {
                GPIOVersion = VersionNumberTask.Result.GetOutput().Split('\n')[0].Split(':')[1].Trim();
            }

            if (GPIOVersion != "2.52")
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.MissingWiringPiLib);
                WiringPiInstalled = false;
                return;
            }

            try
            {
                Pi.Init<BootstrapWiringPi>();
                InitPins();
            }
            catch (PlatformNotSupportedException)
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.PlatformNotSupportedException);
                PlatformSupported = false;
            }
            catch (Exception theException)
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.GenericExceptionLib, theException.Message, theException.InnerException != null ? theException.InnerException.Message : string.Empty);
            }
        }
        
        public void Shutdown()
        {
            foreach (var output in GPIO)
            {
                output.Shutdown();
            }
        }

        private void InitPins()
        {
            List<RasPiPin> Pins = new List<RasPiPin>();

            try
            {
                foreach ( IGpioPin pin in Pi.Gpio )
                {
                    if( pin.BcmPinNumber == 0 ||    // EEPROM
                        pin.BcmPinNumber == 1 ||    // EEPROM
                        pin.BcmPinNumber == 2 ||    // RTC
                        pin.BcmPinNumber == 3 ||    // RTC
                        pin.BcmPinNumber == 4 ||    // Reserved Shutdown & boot pin
                        pin.BcmPinNumber == 14 ||   // Reserved UART Transmit
                        pin.BcmPinNumber == 15 ||   // Reserved UART Receive
                        pin.BcmPinNumber == 18 )    // Fan
                    {
                        continue;
                    }

                    if (pin.Header == GpioHeader.P1 )
                    {
                        var RasPiPin = new RasPiPin(pin);
                        RasPiPin.Log += OnLogWriteEntry;
                        RasPiPin.SubscriptionsUpdated += OnSubscriptionsUpdated;
                        RasPiPin.EventUpdated += OnEventUpdated;
                        Pins.Add(RasPiPin);
                    }
                }
            }
            catch (Exception theException)
            {
                LoggingService.WriteEntry((uint)EventLogEntryCodes.GenericExceptionLib, theException.Message, theException.InnerException != null ? theException.InnerException.Message : string.Empty);
            }

            GPIO = Pins.ToArray();
        }

        private void OnLogWriteEntry(EventLogEntryCodes theLogCode, string[] theArg)
        {
            if( LoggingLevel > 0)
            {
                LoggingService.WriteEntry((uint)theLogCode, theArg);
            }
        }

        internal void Update(EventModel theModel)
        {
            RasPiPin PinSearch = GPIO.FirstOrDefault(Pin => Pin.BcmPinNumber == theModel.BcmPinNumber);

            if (PinSearch != null)
            {
                PinSearch.Update(theModel);
            }
        }

        internal void Update(HomePostModel theModel)
        {
            for (int Index = 0; Index < theModel.BcmPinNumber.Length; Index++)
            {
                RasPiPin PinSearch = GPIO.FirstOrDefault(Pin => Pin.BcmPinNumber == theModel.BcmPinNumber[Index]);

                if (PinSearch != null)
                {
                    PinSearch.Update(theModel.Description[Index]);
                }
            }
        }

        private void OnEventUpdated()
        {
            EventsUpdated?.Invoke();
        }

        private void OnSubscriptionsUpdated()
        {
            SubscriptionsUpdated?.Invoke();
        }

        /// <summary>
        /// Updates or Adds to the Subscriptions of a Pin
        /// </summary>
        /// <param name="theBcmPinNumber"></param>
        /// <param name="theSubscriptions"></param>
        internal void Update(string theBcmPinNumber, RasPiPinSubscription[] theSubscriptions)
        {
            RasPiPin PinSearch = GPIO.FirstOrDefault(Pin => Pin.BcmPinNumber == theBcmPinNumber);

            if ( PinSearch != null)
            {
                PinSearch.Update(theSubscriptions);
            }
        }

        internal void Update(RasPiPinProperties[] theProperties)
        {
            Console.WriteLine("Updating");
            try
            {
                if (theProperties == null)
                {
                    return;
                }

                foreach (var Property in theProperties)
                {
                    RasPiPin PinSearch = GPIO.FirstOrDefault(Pin => Pin.BcmPinNumber == Property.BcmPinNumber);

                    if (PinSearch != null)
                    {
                        PinSearch.Update(Property);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            Console.WriteLine("Updating complete");
        }

        /// <summary>
        /// Removes a Subscription from a Pin
        /// </summary>
        /// <param name="theBcmPinNumber"></param>
        /// <param name="theSubscriptionGuid"></param>
        internal void Remove(string theBcmPinNumber, string theSubscriptionGuid)
        {
            RasPiPin PinSearch = GPIO.FirstOrDefault(Pin => Pin.BcmPinNumber == theBcmPinNumber);

            if (PinSearch != null)
            {
                PinSearch.Remove(theSubscriptionGuid);
            }
        }

        public Base.Exchange.Subscription[] Subscriptions
        {
            get
            {
                List<Base.Exchange.Subscription> List = new List<Base.Exchange.Subscription>();
                Array.ForEach(GPIO, Pin => List.AddRange(Pin.Subscriptions));
                return List.ToArray();
            }
        }

        public Base.Exchange.Event[] Events
        {
            get
            {
                return GPIO.Select( Pin => Pin.Event).ToArray();
            }
        }
    }
}
