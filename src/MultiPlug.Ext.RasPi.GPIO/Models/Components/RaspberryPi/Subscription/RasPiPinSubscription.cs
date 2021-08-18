using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Subscription
{
    public class RasPiPinSubscription : Base.Exchange.Subscription
    {
        //[DataMember]
        //public Properties Properties { get; set; }

        [DataMember]
        public string KeyId { get; set; }
        [DataMember]
        public string High { get; set; }
        [DataMember]
        public string Low { get; set; }
    }
}
