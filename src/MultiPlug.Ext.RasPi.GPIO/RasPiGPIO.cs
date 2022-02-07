using MultiPlug.Base.Exchange;
using MultiPlug.Extension.Core;
using MultiPlug.Extension.Core.Http;

using MultiPlug.Ext.RasPi.GPIO.Properties;
using MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.SharedRazor;
using MultiPlug.Extension.Core.Attribute;
using System;

namespace MultiPlug.Ext.RasPi.GPIO
{
    [FeatureCanUnload(false)]
    public class RasPiGPIO : MultiPlugExtension
    {
        private Models.Load.Root m_LoadModel = null;

        public RasPiGPIO()
        {
            Core.Instance.RaspberryPi.SubscriptionsUpdated += OnSubscriptionsUpdated;
            Core.Instance.RaspberryPi.EventsUpdated += OnEventsUpdated;
            Core.Instance.RaspberryPi.Init(MultiPlugServices);
        }

        private void OnEventsUpdated()
        {
            MultiPlugActions.Extension.Updates.Events();
        }

        private void OnSubscriptionsUpdated()
        {
            MultiPlugActions.Extension.Updates.Subscriptions();
        }

        public override Event[] Events
        {
            get
            {
                return Core.Instance.RaspberryPi.Events;
            }
        }

        public override Subscription[] Subscriptions
        {
            get
            {
                return Core.Instance.RaspberryPi.Subscriptions;
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
                    new RazorTemplate(Templates.HomeAlt, Resources.SettingsHomeInstallWiringPi),
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
                Core.Instance.RaspberryPi.LoggingLevel = m_LoadModel.RaspberryPi.LoggingLevel;
                Core.Instance.RaspberryPi.Update(m_LoadModel.RaspberryPi.GPIO);
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
