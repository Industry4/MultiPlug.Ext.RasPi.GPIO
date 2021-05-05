using MultiPlug.Base.Exchange;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Components.Output
{
    public class Properties
    {
        [DataMember]
        public string BcmPinNumber { get; set; }
        [DataMember]
        public string Output { get; set; } = string.Empty;
        [DataMember]
        public string EventKey { get; set; } = "value";
        [DataMember]
        public string EventHigh { get; set; } = "1";
        [DataMember]
        public string EventLow { get; set; } = "0";
        [DataMember]
        public int PullMode { get; set; } = 0;
        [DataMember]
        public int InitState { get; set; } = 0;
        [DataMember]
        public int ShutdownState { get; set; } = 0;
        [DataMember]
        public Event Event { get; set; }
        [DataMember]
        public List<Subscription.Subscription> Subscriptions { private set; get; } = new List<Subscription.Subscription>();

        public bool isOutput { get { return Output == "true"; } }
        public bool isInput { get { return Output == "false"; } }
    }
}