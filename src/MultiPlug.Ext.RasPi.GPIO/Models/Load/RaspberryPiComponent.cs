using System.Runtime.Serialization;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Load
{
    public class RaspberryPiComponent
    {
        [DataMember]
        public RasPiPinProperties[] GPIO { get; set; }
    }
}
