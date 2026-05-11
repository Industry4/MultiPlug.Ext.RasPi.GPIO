///
/// Note: Code taken from Unosquare.WiringPi which is now in Public archive state on github.
/// MIT License. Copyright(c) 2018 Unosquare Labs. https://github.com/unosquare/wiringpi-dotnet
/// 
namespace MultiPlug.Ext.RasPi.GPIO.Utils.WiringPi
{
    using Unosquare.RaspberryIO.Abstractions;

    /// <summary>
    /// The SPI Bus containing the 2 SPI channels.
    /// </summary>
    public class SpiBus : ISpiBus
    {
        /// <inheritdoc />
        public int Channel0Frequency { get; set; }
        
        /// <inheritdoc />
        public int Channel1Frequency { get; set; }
        
        /// <inheritdoc />
        public int DefaultFrequency => 8000000;

        /// <inheritdoc />
        public ISpiChannel Channel0
        {
            get
            {
                if (Channel0Frequency == 0)
                    Channel0Frequency = DefaultFrequency;

                return SpiChannel.Retrieve(SpiChannelNumber.Channel0, Channel0Frequency);
            }
        }

        /// <inheritdoc />
        public ISpiChannel Channel1
        {
            get
            {
                if (Channel1Frequency == 0)
                    Channel1Frequency = DefaultFrequency;

                return SpiChannel.Retrieve(SpiChannelNumber.Channel1, Channel1Frequency);
            }
        }
    }
}
