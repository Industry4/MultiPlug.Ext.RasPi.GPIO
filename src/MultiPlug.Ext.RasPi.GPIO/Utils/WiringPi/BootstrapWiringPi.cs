///
/// Note: Code taken from Unosquare.WiringPi which is now in Public archive state on github.
/// MIT License. Copyright(c) 2018 Unosquare Labs. https://github.com/unosquare/wiringpi-dotnet
/// 
namespace MultiPlug.Ext.RasPi.GPIO.Utils.WiringPi
{
    using Unosquare.RaspberryIO.Abstractions;
    using global::Swan.DependencyInjection;

    /// <summary>
    /// Represents the Bootstrap class to extract resources.
    /// </summary>
    /// <seealso cref="Unosquare.RaspberryIO.Abstractions.IBootstrap" />
    public class BootstrapWiringPi : IBootstrap
    {
        private static readonly object SyncLock = new object();

        /// <inheritdoc />
        public void Bootstrap()
        {
            lock (SyncLock)
            {
                Resources.EmbeddedResources.ExtractAll();

                DependencyContainer.Current.Register<IGpioController>(new GpioController());
                DependencyContainer.Current.Register<ISpiBus>(new SpiBus());
                DependencyContainer.Current.Register<II2CBus>(new I2CBus());
                DependencyContainer.Current.Register<ISystemInfo>(new SystemInfo());
                DependencyContainer.Current.Register<ITiming>(new Timing());
                DependencyContainer.Current.Register<IThreading>(new Threading());
            }
        }
    }
}
