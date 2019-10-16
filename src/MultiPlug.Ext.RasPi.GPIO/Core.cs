using MultiPlug.Base;
using MultiPlug.Ext.RasPi.GPIO.Components.RaspberryPi;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.RasPi.GPIO
{
    public class Core : MultiPlugBase
    {
        private static Core m_Instance = null;

        [DataMember]
        public RaspberryPiComponent RaspberryPi { get; private set; }

        public static Core Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Core();
                }
                return m_Instance;
            }
        }

        private Core()
        {
            RaspberryPi = new RaspberryPiComponent();
        }
    }
}
