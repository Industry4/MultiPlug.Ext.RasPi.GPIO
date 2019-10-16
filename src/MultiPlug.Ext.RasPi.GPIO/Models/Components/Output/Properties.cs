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
        public bool Output { get; set; }

        [DataMember]
        public string EventKey { get; set; } = "value";
        [DataMember]
        public string EventHigh { get; set; } = "1";
        [DataMember]
        public string EventLow { get; set; } = "0";

        [DataMember]
        public int PullMode { get; set; } = 0;

        [DataMember]
        public Event Event { get; set; }

        [DataMember]
        public List<Subscription.Subscription> Subscriptions { private set; get; } = new List<Subscription.Subscription>();

        ///// <summary>
        ///// Html Helper for check box hidden input
        ///// </summary>
        //public string IsOutput { get { return Output ? "checked" : "";  } }
        ///// <summary>
        ///// Html Helper for check box
        ///// </summary>
        //public string IsInput { get { return !Output ? "" : "disabled";  } }

        //public string PullModeOff { get { return PullMode == 0 ? "selected" : ""; } }
        //public string PullModeDown { get { return PullMode == 1 ? "selected" : ""; } }
        //public string PullModeUp { get { return PullMode == 2 ? "selected" : ""; } }
    }
}