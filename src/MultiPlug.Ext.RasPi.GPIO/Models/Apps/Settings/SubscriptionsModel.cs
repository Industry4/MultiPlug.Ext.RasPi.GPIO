using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Subscription;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings
{
    public class SubscriptionsModel
    {
        public string WiringPiId { get; set; }
        public RasPiPinSubscription[] Subscriptions { set; get; }
        public string KeyIdDefault { get; set; }
        public string HighDefault { get; set; }
        public string LowDefault { get; set; }
    }
}
