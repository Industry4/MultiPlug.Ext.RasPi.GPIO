using System;
using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings
{
    [Route("event")]
    public class SettingsEventController : SettingsApp
    {
        public Response Get(string id)
        {
            var SearchResults = Core.Instance.RaspberryPi.Outputs.FirstOrDefault(o => string.Equals(o.Properties.BcmPinNumber, id, System.StringComparison.OrdinalIgnoreCase));

            if (SearchResults == null)
            {
                return null;
            }

            var Model = new Models.Apps.Settings.EventModel
            {
                BcmPinNumber = id,
                Id = SearchResults.Properties.Event.Id,
                Description = SearchResults.Properties.Event.Description,
                High = SearchResults.Properties.EventHigh,
                Low = SearchResults.Properties.EventLow,
                Key = SearchResults.Properties.EventKey
            };

            return new Response
            {
                Model = Model,
                Template = "GetSettingsEvent"
            };
        }

        public Response Post(EventModel theModel)
        {
            Core.Instance.RaspberryPi.Update(theModel);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = new Uri(Context.Request.AbsoluteUri.Replace(Context.Request.PathAndQuery, "") + string.Join("", Context.Referrer.Segments.Take(Context.Referrer.Segments.Length - 1)))
            };
        }
    }
}
