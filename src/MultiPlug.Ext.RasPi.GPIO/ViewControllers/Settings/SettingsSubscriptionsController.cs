﻿using System;
using System.Linq;
using System.Collections.Generic;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.Output.Subscription;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings
{
    [Route("subscriptions")]
    public class SettingsSubscriptionsController : SettingsApp
    {
        public Response Get( string id )
        {
            var SearchResults = Core.Instance.RaspberryPi.Outputs.FirstOrDefault(o => string.Equals(o.Properties.BcmPinNumber, id, System.StringComparison.OrdinalIgnoreCase) );

            var Model = new SubscriptionsModel
            {
                WiringPiId = id,
                Subscriptions = (SearchResults != null) ? SearchResults.Properties.Subscriptions.ToArray() : new Subscription[0],
                KeyIdDefault = "value",
                HighDefault = "1",
                LowDefault = "0"
            };

            return new Response
            {
                Model = Model,
                Template = "GetSettingsSubscriptions"
            };
        }

        public Response Post(SubscriptionsPostModel theModel)
        {
            if (theModel != null &&
                theModel.WiringPiId != null &&
                theModel.SubscriptionGuid != null &&
                theModel.SubscriptionHigh != null &&
                theModel.SubscriptionId != null &&
                theModel.SubscriptionKeyId != null &&
                theModel.SubscriptionLow != null &&
                new[] { theModel.SubscriptionHigh.Length, theModel.SubscriptionLow.Length }.All(x => x == theModel.SubscriptionKeyId.Length)
                )
            {
                var Subscriptions = new List<Subscription>();

                for (int i = 0; i < theModel.SubscriptionGuid.Length; i++)
                {
                    if (string.IsNullOrEmpty(theModel.SubscriptionId[i]))
                    {
                        continue;
                    }

                    Subscriptions.Add(new Subscription
                    {
                        Guid = (string.IsNullOrEmpty(theModel.SubscriptionGuid[i])) ? Guid.NewGuid().ToString() : theModel.SubscriptionGuid[i],
                        Id = theModel.SubscriptionId[i],
                        Properties = new Models.Components.Output.Subscription.Properties
                        {
                            KeyId = theModel.SubscriptionKeyId[i],
                            High = theModel.SubscriptionHigh[i],
                            Low = theModel.SubscriptionLow[i]
                        }
                    });
                }
          
                Core.Instance.RaspberryPi.Update(theModel.WiringPiId, Subscriptions);
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = new Uri(Context.Request.AbsoluteUri.Replace(Context.Request.PathAndQuery, "") + string.Join("", Context.Referrer.Segments.Take(Context.Referrer.Segments.Length - 1)))
            };
        }
    }
}
