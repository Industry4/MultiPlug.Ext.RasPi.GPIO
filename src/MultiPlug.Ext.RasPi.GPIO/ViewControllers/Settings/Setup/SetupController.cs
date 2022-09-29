using System;
using System.Collections.Generic;
using System.Linq;

using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.SharedRazor;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi;

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
                    Outputs = Core.Instance.RaspberryPi.GPIO.Select(Pin => new PinModel
                    {
                        BcmPinNumber = Pin.BcmPinNumber,
                        EventHigh = Pin.Event.HighValue,
                        EventKey = Pin.Event.Subjects[0],
                        EventLow = Pin.Event.LowValue,
                        Output = Pin.Output,
                        PullMode = Pin.PullMode,
                        SubscriptionsCount = Pin.Subscriptions.Length.ToString(),
                        EventId = Pin.Event.Id,
                        EventDescription = Pin.Event.Description,
                        InitState = Pin.InitState,
                        ShutdownState = Pin.ShutdownState,
                        Debounce = Pin.Debounce.Value
                    }).ToArray()
                },
                Template = Templates.Setup
            };
        }

        public Response Post(SetupPostModel theModel)
        {
            if (theModel != null &&
                theModel.BcmPinNumber != null &&
                theModel.IsOutput != null &&
                theModel.PullMode != null &&
                theModel.InitState != null &&
                theModel.ShutdownState != null &&
                theModel.Debounce != null &&
                new[]
                {
                    theModel.BcmPinNumber.Length,
                    theModel.IsOutput.Length,
                    theModel.PullMode.Length,
                    theModel.InitState.Length,
                    theModel.ShutdownState.Length,
                    theModel.Debounce.Length
                }.All(x => x == theModel.BcmPinNumber.Length)
                )
            {
                var Properties = new List<RasPiPinProperties>();

                for (int i = 0; i < theModel.BcmPinNumber.Length; i++)
                {
                    Properties.Add(new RasPiPinProperties
                    {
                        BcmPinNumber = theModel.BcmPinNumber[i],
                        Output = theModel.IsOutput[i],
                        PullMode = theModel.PullMode[i],
                        InitState = theModel.InitState[i],
                        ShutdownState = theModel.ShutdownState[i],
                        Debounce = theModel.Debounce[i]
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