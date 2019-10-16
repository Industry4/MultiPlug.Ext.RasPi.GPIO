
using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Components.Output.Subscription
{
    public class Properties
    {
        [DataMember]
        public string KeyId { get; set; }
        [DataMember]
        public string High { get; set; }
        [DataMember]
        public string Low { get; set; }
    }
}
