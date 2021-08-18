using System.Linq;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Models.API.IO;
using MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.API.IO
{
    [Route("iostate/*")]
    public class IOStateController : APIEndpoint
    {
        public Response Get(string io)
        {
            RasPiPin[] Pins = Core.Instance.RaspberryPi.GPIO;

            if (!string.IsNullOrEmpty(io))
            {

                Pins = Core.Instance.RaspberryPi.GPIO.Where(Pin => Pin.BcmPinNumber == io).ToArray();
            }

            return new Response
            {
                MediaType = "application/json",
                Model = Pins.Select(Output => new IOModel
                {
                    bcm = Output.BcmPinNumber,
                    state = Output.CurrentState ? "1" : "0"
                }).ToArray()
            };
        }

        public Response Post(string io, string state )
        {
            if( ! (string.IsNullOrEmpty( io ) || string.IsNullOrEmpty( state ) ) )
            {
                RasPiPin Search = Core.Instance.RaspberryPi.GPIO.FirstOrDefault(Pin => Pin.BcmPinNumber == io);

                if(Search != null)
                {
                    if(state == "1" || string.Equals(state, "true", System.StringComparison.OrdinalIgnoreCase) || string.Equals(state, "on", System.StringComparison.OrdinalIgnoreCase))
                    {
                        Search.SetState(true);
                        return Good();
                    }
                    else if (state == "0" || string.Equals(state, "false", System.StringComparison.OrdinalIgnoreCase) || string.Equals(state, "off", System.StringComparison.OrdinalIgnoreCase))
                    {
                        Search.SetState(true);
                        return Good();
                    }
                    else if (string.Equals(state, "t", System.StringComparison.OrdinalIgnoreCase) || string.Equals(state, "toggle", System.StringComparison.OrdinalIgnoreCase) )
                    {
                        Search.SetState( ! Search.CurrentState );
                        return Good();
                    }
                }
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        private Response Good()
        {
            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }
    }
}
