using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings
{
    public class PinStateModel
    {
        public string BcmPinNumber { get; set; }
        public bool State { get; set; }

        public bool isOutput { get; set; }
    }
}
