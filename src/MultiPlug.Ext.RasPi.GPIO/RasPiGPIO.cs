using System;

using MultiPlug.Base.Exchange;
using MultiPlug.Extension.Core;
using MultiPlug.Extension.Core.Http;

using MultiPlug.Ext.RasPi.GPIO.Properties;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Assets;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings;
using MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi;

namespace MultiPlug.Ext.RasPi.GPIO
{
    public class RasPiGPIO : MultiPlugExtension
    {
        private Models.Load.Root m_LoadModel = null;

        private HttpEndpoint[] m_Apps;

        public RasPiGPIO()
        {
            m_Apps = new HttpEndpoint[] { new SettingsApp(), new AssetsEndpoint() };

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

        public override HttpEndpoint[] HttpEndpoints
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

        public override event EventHandler<Event[]> EventsUpdated;
        public override event EventHandler<Subscription[]> SubscriptionsUpdated;
        #pragma warning disable 0067
        public override event EventHandler<HttpEndpoint[]> HttpEndpointsUpdated;
        public override event EventHandler<RazorTemplate[]> NewRazorTemplates;
        #pragma warning restore 0067

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
            EventCreator.FireEvents = true;
        }

        public override void Shutdown()
        {
            EventCreator.FireEvents = false;
            Core.Instance.RaspberryPi.Shutdown();
        }
    }
}
