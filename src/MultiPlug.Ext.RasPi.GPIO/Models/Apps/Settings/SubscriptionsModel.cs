using MultiPlug.Ext.RasPi.GPIO.Models.Components.Output.Subscription;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings
{
    public class SubscriptionsModel
    {
        public string WiringPiId { get; set; }
        public Subscription[] Subscriptions { set; get; }
        public string KeyIdDefault { get; set; }
        public string HighDefault { get; set; }
        public string LowDefault { get; set; }
    }
}
