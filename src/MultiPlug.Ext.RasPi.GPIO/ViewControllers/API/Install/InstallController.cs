using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.API.Install
{
    [Route("install")]
    public class InstallController : APIEndpoint
    {
        public Response Post()
        {
            string Result = Core.Instance.RaspberryPi.InstallLibrary();

            if (Result == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            else
            {
                return new Response
                {
                    Model = new { ErrorMessage = Result },
                    StatusCode = System.Net.HttpStatusCode.ServiceUnavailable,
                    MediaType = "application/json"
                };
            }
        }
    }
}