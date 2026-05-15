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
            if(theName.Equals("wiringpi_3.18.deb", StringComparison.OrdinalIgnoreCase))
            {
                if (Core.Instance.RaspberryPi.OSRaspbianBullseye)
                {
                    return new Response
                    {
                        RawBytes = Resources.wiringpi_3_18_bullseye_armhf,
                        MediaType = "application/octet-stream"
                    };
                }
                else
                {
                    if (Core.Instance.RaspberryPi.IsArm64OS)
                    {
                        return new Response
                        {
                            RawBytes = Resources.wiringpi_3_18_arm64,
                            MediaType = "application/octet-stream"
                        };
                    }
                    else
                    {
                        return new Response
                        {
                            RawBytes = Resources.wiringpi_3_18_armhf,
                            MediaType = "application/octet-stream"
                        };
                    }
                }
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
        }
    }
}
