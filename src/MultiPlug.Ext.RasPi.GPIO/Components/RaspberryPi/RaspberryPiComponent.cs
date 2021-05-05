using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi
{
    public class RaspberryPiComponent
    {
        public event EventHandler SubscriptionsUpdated;
        public event EventHandler EventsUpdated;

        [DataMember]
        public List<RasPiPin> Outputs { get; } = new List<RasPiPin>();

        public bool PlatformSupported { get; set; } = true;

        public RaspberryPiComponent()
        {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unosquare.WiringPi.dll")))
            {
                LogException(new Exception("Missing file: Unosquare.WiringPi.dll"));
                return;
            }

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unosquare.RaspberryIO.dll")))
            {
                LogException(new Exception("Missing file: Unosquare.RaspberryIO.dll"));
                return;
            }

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Unosquare.Raspberry.Abstractions.dll")))
            {
                LogException(new Exception("Missing file: Unosquare.Raspberry.Abstractions.dll"));
                return;
            }

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Swan.Lite.dll")))
            {
                LogException(new Exception("Missing file: Swan.Lite.dll"));
                return;
            }

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Swan.dll")))
            {
                LogException(new Exception("Missing file: Swan.dll"));
                return;
            }

            try
            {
                Pi.Init<BootstrapWiringPi>();
                Init();
            }
            catch (PlatformNotSupportedException)
            {
                PlatformSupported = false;
            }
            catch (Exception theException)
            {
                LogException(theException);
            }
        }
        
        public void Shutdown()
        {
            foreach (var output in Outputs)
            {
                output.shutdown();
            }
        }

        private void Init()
        {
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
                        RasPiPin.SubscriptionsUpdated += OnSubscriptionsUpdated;
                        RasPiPin.EventUpdated += OnEventUpdated;
                        Outputs.Add(RasPiPin);
                    }
                }
            }
            catch (Exception theException)
            {
                LogException(theException);
            }
        }

        private void LogException(Exception theException)
        {
            var ErrorFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.txt");

            using (StreamWriter Writer = new StreamWriter(ErrorFilePath, true))
            {
                Writer.WriteLine("Message: " + theException.Message);
                Writer.WriteLine("Stacktrace: " + theException.StackTrace);

                if(theException.InnerException != null)
                {
                    Writer.WriteLine("Inner Message: " + theException.InnerException.Message);
                    Writer.WriteLine("Inner Stacktrace: " + theException.InnerException.StackTrace);
                }

                Writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        internal void Update(Models.Load.RasPiPin[] thePins)
        {
            if ( thePins == null)
            {
                return;
            }
            EnableUpdateEvents(false);

            foreach ( var LoadedPin in thePins)
            {
                if ( LoadedPin.Properties.BcmPinNumber == null)
                {
                    continue;
                }

                var OutputSearch = Outputs.FirstOrDefault(o => o.Properties.BcmPinNumber == LoadedPin.Properties.BcmPinNumber);

                if (OutputSearch != null)
                {
                    OutputSearch.Update(LoadedPin.Properties);
                }
            }

            EnableUpdateEvents(true);
        }


        internal void Update(EventModel theModel)
        {
            var OutputSearch = Outputs.FirstOrDefault(o => o.Properties.BcmPinNumber == theModel.BcmPinNumber);

            if (OutputSearch != null)
            {
                OutputSearch.Update(theModel);
            }
        }
        private void OnEventUpdated(object sender, EventArgs e)
        {
            EventsUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void OnSubscriptionsUpdated(object sender, EventArgs e)
        {
            if(m_UpdatesEnabled)
            {
                SubscriptionsUpdated?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                m_UpdatesBuffered = true;
            }

        }

        private bool m_UpdatesEnabled = true;
        private bool m_UpdatesBuffered = false;

        private void EnableUpdateEvents( bool isTrue )
        {
            if( isTrue )
            {
                if( m_UpdatesBuffered )
                {
                    SubscriptionsUpdated?.Invoke(this, EventArgs.Empty);
                    m_UpdatesBuffered = false;
                }

            }

            m_UpdatesEnabled = isTrue;
        }


        /// <summary>
        /// Updates or Adds to the Subscriptions of a Pin
        /// </summary>
        /// <param name="theBcmPinNumber"></param>
        /// <param name="theSubscriptions"></param>
        internal void Update(string theBcmPinNumber, List<Models.Components.Output.Subscription.Subscription> theSubscriptions)
        {
            EnableUpdateEvents(false);

            var OutputSearch = Outputs.FirstOrDefault(o => o.Properties.BcmPinNumber == theBcmPinNumber);

            if ( OutputSearch != null)
            {
                OutputSearch.Update(theSubscriptions);
            }

            EnableUpdateEvents(true);
        }

        internal void Update(Models.Components.Output.Properties[] theProperties)
        {
            EnableUpdateEvents(false);

            foreach ( var Property in theProperties)
            {
                var OutputSearch = Outputs.FirstOrDefault(o => o.Properties.BcmPinNumber == Property.BcmPinNumber);

                if (OutputSearch != null)
                {
                    OutputSearch.Update(Property);
                }
            }

            EnableUpdateEvents(true);
        }

        /// <summary>
        /// Removes a Subscription from a Pin
        /// </summary>
        /// <param name="theBcmPinNumber"></param>
        /// <param name="theSubscriptionGuid"></param>
        internal void Remove(string theBcmPinNumber, string theSubscriptionGuid)
        {
            var OutputSearch = Outputs.FirstOrDefault(o => o.Properties.BcmPinNumber == theBcmPinNumber);

            if (OutputSearch != null)
            {
                OutputSearch.Remove(theSubscriptionGuid);
            }
        }

        public List<Base.Exchange.Subscription> Subscriptions
        {
            get
            {
                var list = new List<Base.Exchange.Subscription>();
                Outputs.ForEach(o => list.AddRange(o.Properties.Subscriptions));
                return list;
            }
        }

        public List<Base.Exchange.Event> Events
        {
            get
            {
                return Outputs.Where(o => o.Properties.Output == "false" ).Select( o => o.Properties.Event).ToList();
            }
        }
    }
}
