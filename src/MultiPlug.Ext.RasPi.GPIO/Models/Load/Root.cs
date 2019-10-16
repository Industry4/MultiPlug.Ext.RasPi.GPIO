using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Load
{
    public class Root
    {
        [DataMember]
        public RaspberryPiComponent RaspberryPi { get; set; }
    }
}
