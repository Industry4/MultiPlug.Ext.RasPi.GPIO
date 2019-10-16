using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Extension.Core;
using MultiPlug.Extension.Core.Views;
using MultiPlug.Extension.Core.Collections;
using MultiPlug.Ext.RasPi.GPIO.Properties;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Assets;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings;
using MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi;

namespace MultiPlug.Ext.RasPi.GPIO
{
    public class RasPiGPIO : MultiPlugExtension
    {
        private Models.Load.Root m_LoadModel = null;

        private ViewBase[] m_Apps;

        public RasPiGPIO()
        {
            m_Apps = new ViewBase[] { new SettingsApp(), new AssetsEndpoint() };

            Core.Instance.RaspberryPi.SubscriptionsUpdated += OnSubscriptionsUpdated;
            Core.Instance.RaspberryPi.EventsUpdated += OnEventsUpdated;
        }

        private void OnEventsUpdated(object sender, EventArgs e)
        {
            EventsUpdated?.Invoke(this, Events);
        }

        private void OnSubscriptionsUpdated(object sender, EventArgs e)
        {
            SubscriptionsUpdated?.Invoke(this, Subscriptions);
        }

        public override ViewBase[] Apps
        {
            get
            {
                return m_Apps;
            }
        }

        public override Event[] Events
        {
            get
            {
                return Core.Instance.RaspberryPi.Events.ToArray();
            }
        }

        public override Subscription[] Subscriptions
        {
            get
            {
                return Core.Instance.RaspberryPi.Subscriptions.ToArray();
            }
        }

        public override RazorTemplate[] RazorTemplates
        {
            get
            {
                return new RazorTemplate[]
                {
                    new RazorTemplate("GetSettingsEvent", Resources.SettingsEvent),
                    new RazorTemplate("GetSettingsHome", Resources.SettingsHome),
                    new RazorTemplate("GetSettingsSubscriptions", Resources.SettingsSubscriptions)
                };
            }
        }

        public override event EventHandler<ViewBase[]> AppsUpdated;
        public override event EventHandler<Event[]> EventsUpdated;
        public override event EventHandler<Subscription[]> SubscriptionsUpdated;
        public override event EventHandler<RazorTemplate[]> NewRazorTemplates;

        public override void Initialise()
        {
            if(m_LoadModel != null)
            {
                EventCreator.FireEvents = false;
                Core.Instance.RaspberryPi.Update(m_LoadModel.RaspberryPi.Outputs);
                m_LoadModel = null;
                EventCreator.FireEvents = true;
            }
        }

        public override void Load(KeyValuesJson[] config)
        {
            throw new NotImplementedException();
        }

        public void Load(Models.Load.Root theLoadModel)
        {
            if (theLoadModel.RaspberryPi == null)
            {
                return;
            }

            m_LoadModel = theLoadModel;
        }

        public override void OnUnhandledException(UnhandledExceptionEventArgs args)
        {
            throw new NotImplementedException();
        }

        public override object Save()
        {
            return Core.Instance;
        }

        public override void Start()
        {
            EventCreator.FireEvents = true;
        }

        public override void Stop()
        {
            EventCreator.FireEvents = false;
            Core.Instance.RaspberryPi.Shutdown();
        }
    }
}
