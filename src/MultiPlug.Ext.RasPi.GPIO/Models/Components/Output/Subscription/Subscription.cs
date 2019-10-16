using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Components.Output.Subscription
{
    public class Subscription : Base.Exchange.Subscription
    {
        [DataMember]
        public Properties Properties { get; set; }
    }
}
