using System;
using System.Collections.Generic;
using MultiPlug.Base.Http;
using MultiPlug.Extension.Core.Views;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings
{
    class SettingsApp : ViewBase
    {
        readonly Guid m_Id = Guid.NewGuid();

        readonly Controller[] m_Controllers = new Controller[]{
            new SettingsHomeController(),
            new SettingsSubscriptionsController(),
            new SettingsDeleteSubscriptionController(),
            new SettingsEventController()
        };
        public override IEnumerable<Controller> Controllers
        {
            get
            {
                return m_Controllers;
            }
        }

        public override Guid Id
        {
            get
            {
                return m_Id;
            }
        }

        public override bool isPartial
        {
            get
            {
                return true;
            }
        }

        public override string Name
        {
            get
            {
                return "Raspberry Pi";
            }
        }

        public override ViewType Type
        {
            get
            {
                return ViewType.Settings;
            }
        }
    }
}
