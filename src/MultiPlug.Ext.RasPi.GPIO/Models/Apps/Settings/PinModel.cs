
namespace MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings
{
    public class PinModel
    {
        public string BcmPinNumber { get; set; }

        public bool Output { get; set; }


        public string EventKey { get; set; } = "value";

        public string EventHigh { get; set; } = "1";

        public string EventLow { get; set; } = "0";

        public int PullMode { get; set; } = 0;

        public string SubscriptionsCount { get; set; }
        public string EventId { get; set; }
        public string EventDescription { get; set; }
        public string IsOutput { get { return Output ? "checked" : ""; } }
        /// <summary>
        /// Html Helper for check box
        /// </summary>
        public string IsInput { get { return !Output ? "" : "disabled"; } }

        public string PullModeOff { get { return PullMode == 0 ? "selected" : ""; } }
        public string PullModeDown { get { return PullMode == 1 ? "selected" : ""; } }
        public string PullModeUp { get { return PullMode == 2 ? "selected" : ""; } }
    }
}
