using System;
using System.Linq;
using System.Collections.Generic;
using MultiPlug.Base.Http;
using MultiPlug.Base.Attribute;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.SharedRazor;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.Home
{
    [Route("")]
    public class SettingsHomeController : SettingsApp
    {
        public Response Get()
        {
            if( ! Core.Instance.RaspberryPi.WiringPiInstalled )
            {
                return new Response
                {
                    Template = Templates.HomeAlt,
                };
            }


            IEnumerable<RasPiPin> ActiveIO = Core.Instance.RaspberryPi.GPIO.Where(Pin => Pin.Output != string.Empty);

            return new Response
            {
                Model = new SetupModel
                {
                    Outputs = ActiveIO.Select(Pin => new PinStateModel
                        {
                            BcmPinNumber = Pin.BcmPinNumber,
                            State = Pin.CurrentState,
                            isOutput = Pin.isOutput,
                            EventId = Pin.Event.Id,
                            EventHigh = Pin.Event.HighValue,
                            Description = Pin.Event.Description
                    }).ToArray()
                },
                Template = Templates.Home,
                Subscriptions = ActiveIO.Select(Pin => new Subscription
                        {
                            Guid = Guid.NewGuid().ToString(),
                            Id = Pin.Event.Id
                        }).ToArray()
            };
        }

        public Response Post(HomePostModel theModel)
        {
            if (theModel != null &&
                theModel.BcmPinNumber != null &&
                theModel.Description != null &&
                theModel.BcmPinNumber.Length == theModel.Description.Length)
            {
                Core.Instance.RaspberryPi.Update(theModel);
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = new Uri(Context.Request.AbsoluteUri)
            };
        }
    }
}