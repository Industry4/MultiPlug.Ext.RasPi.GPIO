namespace MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings
{
    public class HomePostModel
    {
        public string[] BcmPinNumber { get; set; }
        public bool[] IsOutput { get; set; }
        public int[] PullMode { get; set; }
    }
}
