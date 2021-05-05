using System.Linq;
using MultiPlug.Base.Http;
using MultiPlug.Base.Attribute;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.SharedRazor;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.Home
{
    [Route("")]
    public class SettingsHomeController : SettingsApp
    {
        public Response Get()
        {
            return new Response
            {
                Model = new SetupModel
                {
                    Outputs = Core.Instance.RaspberryPi.Outputs.Where(Output => Output.Properties.Output != string.Empty )
                        .Select(Output => new PinStateModel
                        {
                            BcmPinNumber = Output.Properties.BcmPinNumber,
                            State = Output.CurrentState,
                            isOutput = Output.Properties.isOutput
                        }).ToArray()
                },
                Template = Templates.Home
            };
        }


    }
}