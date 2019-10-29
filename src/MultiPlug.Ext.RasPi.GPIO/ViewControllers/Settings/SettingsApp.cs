using System.Collections.Generic;

using MultiPlug.Base.Http;
using MultiPlug.Extension.Core.Http;
using MultiPlug.Extension.Core.Attribute;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings
{
    [ViewAs(ViewAs.Partial)]
    [HttpEndpointType(HttpEndpointType.Settings)]
    class SettingsApp : HttpEndpoint
    {
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
    }
}
