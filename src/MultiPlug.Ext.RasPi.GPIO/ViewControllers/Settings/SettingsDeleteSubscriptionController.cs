using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings
{
    [Route("deletesubscription")]
    class SettingsDeleteSubscriptionController : Controller
    {
        public Response Get(DeleteModel theModel)
        {
            Core.Instance.RaspberryPi.Remove(theModel.id, theModel.subid);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }
    }
}
