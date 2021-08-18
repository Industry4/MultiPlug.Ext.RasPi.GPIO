using System;
using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings;
using MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.SharedRazor;
using MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi;

namespace MultiPlug.Ext.RasPi.GPIO.ViewControllers.Settings.Events
{
    [Route("event")]
    public class SettingsEventController : SettingsApp
    {
        public Response Get(string id)
        {
            RasPiPin SearchResults = Core.Instance.RaspberryPi.GPIO.FirstOrDefault(Pin => string.Equals(Pin.BcmPinNumber, id, System.StringComparison.OrdinalIgnoreCase));

            if (SearchResults == null)
            {
                return null;
            }

            EventModel Model = new EventModel
            {
                BcmPinNumber = id,
                Id = SearchResults.Event.Id,
                Description = SearchResults.Event.Description,
                High = SearchResults.Event.HighValue,
                Low = SearchResults.Event.LowValue,
                Key = SearchResults.Event.Subjects[0]
            };

            return new Response
            {
                Model = Model,
                Template = Templates.Event
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
