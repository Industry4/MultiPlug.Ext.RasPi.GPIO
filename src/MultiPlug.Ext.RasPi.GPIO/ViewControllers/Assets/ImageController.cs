using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Properties;
using System.Drawing;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Assets
{
    [Route("images/*")]
    class ImageController : Controller
    {
        public Response Get(string theName)
        {
            ImageConverter converter = new ImageConverter();
            return new Response { RawBytes = (byte[])converter.ConvertTo(Resources.raspberry_pi, typeof(byte[])), MediaType = "image/png" };
        }
    }
}
