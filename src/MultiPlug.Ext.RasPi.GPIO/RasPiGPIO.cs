using System;

using MultiPlug.Base.Exchange;
using MultiPlug.Extension.Core;
using MultiPlug.Extension.Core.Http;

using MultiPlug.Ext.RasPi.GPIO.Properties;
using MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.SharedRazor;

namespace MultiPlug.Ext.RasPi.GPIO
{
    public class RasPiGPIO : MultiPlugExtension
    {
        private Models.Load.Root m_LoadModel = null;

        public RasPiGPIO()
        {
            Core.Instance.RaspberryPi.SubscriptionsUpdated += OnSubscriptionsUpdated;
            Core.Instance.RaspberryPi.EventsUpdated += OnEventsUpdated;
        }

        private void OnEventsUpdated(object sender, EventArgs e)
        {
            MultiPlugActions.Extension.Updates.Events();
        }

        private void OnSubscriptionsUpdated(object sender, EventArgs e)
        {
            MultiPlugActions.Extension.Updates.Subscriptions();
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
                    new RazorTemplate(Templates.Navigation, Resources.SettingsNavigation),
                    new RazorTemplate(Templates.Home, Resources.SettingsHome),
                    new RazorTemplate(Templates.Setup, Resources.SettingsSetup),
                    new RazorTemplate(Templates.Event, Resources.SettingsEvent),
                    new RazorTemplate(Templates.Subscriptions, Resources.SettingsSubscriptions),
                    new RazorTemplate(Templates.About, Resources.SettingsAbout)
                };
            }
        }

        public override void Initialise()
        {
            if(m_LoadModel != null)
            {
                RasPiPin.FireEvents = false;
                Core.Instance.RaspberryPi.Update(m_LoadModel.RaspberryPi.Outputs);
                m_LoadModel = null;
                RasPiPin.FireEvents = true;
            }
        }

        public void Load(Models.Load.Root theLoadModel)
        {
            if (theLoadModel.RaspberryPi == null)
            {
                return;
            }

            m_LoadModel = theLoadModel;
        }

        public override object Save()
        {
            return Core.Instance;
        }

        public override void Start()
        {
            RasPiPin.FireEvents = true;
        }

        public override void Shutdown()
        {
            RasPiPin.FireEvents = false;
            Core.Instance.RaspberryPi.Shutdown();
        }
    }
}
