using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Load
{
    public class RaspberryPiComponent
    {
        [DataMember]
        public RasPiPin[] Outputs { get; set; }
    }
}
