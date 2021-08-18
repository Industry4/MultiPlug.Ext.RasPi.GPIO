using System;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Properties;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Assets
{
    [Route("downloads/*")]
    public class DownloadsController : AssetsEndpoint
    {
        public Response Get(string theName)
        {
            if(theName.Equals("wiringpi-latest.deb", StringComparison.OrdinalIgnoreCase))
            {
                return new Response
                {
                    RawBytes = Resources.wiringpi_latest,
                    MediaType = "application/octet-stream"
                };
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
        }
    }
}
