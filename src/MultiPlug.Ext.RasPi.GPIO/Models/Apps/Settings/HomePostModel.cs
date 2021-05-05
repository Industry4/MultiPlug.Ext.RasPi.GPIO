namespace MultiPlug.Ext.RasPi.GPIO.Models.Apps.Settings
{
    public class HomePostModel
    {
        public string[] BcmPinNumber { get; set; }
        public string[] IsOutput { get; set; }
        public int[] PullMode { get; set; }
        public int[] InitState { get; set; }
        public int[] ShutdownState { get; set; }
    }
}
