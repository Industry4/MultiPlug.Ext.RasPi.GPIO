using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Load
{
    public class RasPiPin
    {
        [DataMember]
        public Components.Output.Properties Properties { set; get; }
    }
}
