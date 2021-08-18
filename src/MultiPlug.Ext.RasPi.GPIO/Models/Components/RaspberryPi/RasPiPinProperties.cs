using System.Runtime.Serialization;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Event;
using MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi.Subscription;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Components.RaspberryPi
{
    public class RasPiPinProperties
    {
        public const string c_True = "true";
        public const string c_False = "false";

        [DataMember]
        public string BcmPinNumber { get; set; }
        [DataMember]
        public string Output { get; set; } = string.Empty;
        [DataMember]
        public int PullMode { get; set; } = 0;
        [DataMember]
        public int InitState { get; set; } = 0;
        [DataMember]
        public int ShutdownState { get; set; } = 0;
        [DataMember]
        public RasPiPinEvent Event { get; set; }
        [DataMember]
        public RasPiPinSubscription[] Subscriptions { set; get; } = new RasPiPinSubscription[0];

        public bool isOutput { get { return Output == c_True; } }
        public bool isInput { get { return Output == c_False; } }
    }
}