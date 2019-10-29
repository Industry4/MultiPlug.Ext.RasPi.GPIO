using System.Collections.Generic;

using MultiPlug.Base.Http;
using MultiPlug.Extension.Core.Http;
using MultiPlug.Extension.Core.Attribute;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Assets
{
    [HttpEndpointType(HttpEndpointType.Assets)]
    class AssetsEndpoint : HttpEndpoint
    {
        private Controller[] m_Controllers = new Controller[] { new ImageController() };

        public override IEnumerable<Controller> Controllers
        {
            get
            {
                return m_Controllers;
            }
        }
    }
}
