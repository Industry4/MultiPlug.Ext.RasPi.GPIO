using MultiPlug.Base.Http;
using MultiPlug.Extension.Core.Attribute;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings
{
    [ViewAs(ViewAs.Partial)]
    [HttpEndpointType(HttpEndpointType.Settings)]
    public class SettingsApp : Controller
    {
    }
}
