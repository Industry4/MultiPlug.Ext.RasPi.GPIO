using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Event
{
    public class RasPiPinEvent : Base.Exchange.Event
    {
        [DataMember]
        public string HighValue { get; set; } = "1";
        [DataMember]
        public string LowValue { get; set; } = "0";
    }
}
