
namespace MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings
{
    public class PinModel
    {
        public string BcmPinNumber { get; set; }

        public string Output { get; set; } = string.Empty;


        public string EventKey { get; set; } = "value";

        public string EventHigh { get; set; } = "1";

        public string EventLow { get; set; } = "0";

        public int PullMode { get; set; } = 0;

        public string SubscriptionsCount { get; set; }
        public string EventId { get; set; }
        public string EventDescription { get; set; }
        public int InitState { get; set; }
        public int ShutdownState { get; set; }
    }
}
