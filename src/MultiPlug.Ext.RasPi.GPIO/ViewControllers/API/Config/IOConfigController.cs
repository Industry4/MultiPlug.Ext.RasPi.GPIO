using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Models.API.Config;
using MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.API.Config
{
    [Route("ioconfig/*")]
    public class IOConfigController : APIEndpoint
    {
        public Response Get(string id)
        {
            RasPiPin[] Pins = Core.Instance.RaspberryPi.GPIO.Where(Pin => Pin.Output != string.Empty).ToArray();

            if (!string.IsNullOrEmpty(id))
            {

                Pins = Pins.Where(Pin => Pin.BcmPinNumber == id).ToArray();
            }

            return new Response
            {
                MediaType = "application/json",
                Model = Pins.Select(Output => new IOConfigModel
                {
                    bcm = Output.BcmPinNumber,
                    direction = (Output.isInput) ? "in" : "out"
                }).ToArray()
            };
        }
    }
}
