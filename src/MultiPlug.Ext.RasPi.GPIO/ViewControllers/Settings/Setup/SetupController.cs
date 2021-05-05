using System;
using System.Collections.Generic;
using System.Linq;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.SharedRazor;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.Setup
{
    [Route("setup")]
    public class SetupController : SettingsApp
    {
        public Response Get()
        {
            return new Response
            {
                Model = new HomeModel
                {
                    Outputs = Core.Instance.RaspberryPi.Outputs.Select(Output => new PinModel
                    {
                        BcmPinNumber = Output.Properties.BcmPinNumber,
                        EventHigh = Output.Properties.EventHigh,
                        EventKey = Output.Properties.EventKey,
                        EventLow = Output.Properties.EventLow,
                        Output = Output.Properties.Output,
                        PullMode = Output.Properties.PullMode,
                        SubscriptionsCount = Output.Properties.Subscriptions.Count.ToString(),
                        EventId = Output.Properties.Event.Id,
                        EventDescription = Output.Properties.Event.Description
                    }).ToArray()
                },
                Template = Templates.Setup
            };
        }

        public Response Post(HomePostModel theModel)
        {
            if (theModel != null &&
                theModel.BcmPinNumber != null &&
                theModel.IsOutput != null &&
                theModel.PullMode != null &&
                theModel.InitState != null &&
                theModel.ShutdownState != null &&
                new[]
                {
                    theModel.BcmPinNumber.Length,
                    theModel.IsOutput.Length,
                    theModel.PullMode.Length,
                    theModel.InitState.Length,
                    theModel.ShutdownState.Length
                }.All(x => x == theModel.BcmPinNumber.Length)
                )
            {
                var Properties = new List<Models.Components.Output.Properties>();

                for (int i = 0; i < theModel.BcmPinNumber.Length; i++)
                {
                    Properties.Add(new Models.Components.Output.Properties
                    {
                        BcmPinNumber = theModel.BcmPinNumber[i],
                        Output = theModel.IsOutput[i],
                        PullMode = theModel.PullMode[i],
                        InitState = theModel.InitState[i],
                        ShutdownState = theModel.ShutdownState[i]
                    });
                }

                Core.Instance.RaspberryPi.Update(Properties.ToArray());
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = new Uri(Context.Request.AbsoluteUri)
            };
        }
    }
}