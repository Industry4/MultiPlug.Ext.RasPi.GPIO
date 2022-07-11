
namespace MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings
{
    public class SubscriptionsPostModel
    {
        public string WiringPiId { get; set; }
        public string[] SubscriptionGuid { get; set; }
        public string[] SubscriptionId { get; set; }
        public string[] SubscriptionHigh { get; set; }
        public string[] SubscriptionLow { get; set; }
    }
}
