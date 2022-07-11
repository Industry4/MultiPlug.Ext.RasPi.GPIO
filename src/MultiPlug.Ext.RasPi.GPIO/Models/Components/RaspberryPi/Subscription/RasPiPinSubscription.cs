using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Subscription
{
    public class RasPiPinSubscription : Base.Exchange.Subscription
    {
        [DataMember]
        public string High { get; set; }
        [DataMember]
        public string Low { get; set; }
    }
}
