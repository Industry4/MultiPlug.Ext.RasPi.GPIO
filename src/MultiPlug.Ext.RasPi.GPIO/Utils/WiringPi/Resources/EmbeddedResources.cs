///
/// Note: Code taken from Unosquare.WiringPi which is now in Public archive state on github.
/// MIT License. Copyright(c) 2018 Unosquare Labs. https://github.com/unosquare/wiringpi-dotnet
/// 
namespace MultiPlug.Ext.RasPi.GPIO.Utils.WiringPi.Resources
{
    using Native;
    using System;
    using System.IO;

    /// <summary>
    /// Provides access to embedded assembly files.
    /// </summary>
    internal static class EmbeddedResources
    {
        /// <summary>
        /// Initializes static members of the <see cref="EmbeddedResources"/> class.
        /// </summary>
        static EmbeddedResources()
        {
        }
        public static void ExtractDebLirary()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            string targetPath = Path.Combine(basePath, "wiringpi_3.181.deb");

            if (Core.Instance.RaspberryPi.OSRaspbianBullseye)
            {
                File.WriteAllBytes(targetPath, Properties.Resources.wiringpi_3_181_bullseye_armhf_deb);
            }
            else
            {
                if(Core.Instance.RaspberryPi.IsArm64OS)
                {
                    File.WriteAllBytes(targetPath, Properties.Resources.wiringpi_3_181_arm64_deb);
                }
                else
                {
                    File.WriteAllBytes(targetPath, Properties.Resources.wiringpi_3_181_armhf_deb);  
                }
         
            }
        }
        /// <summary>
        /// Extracts all the file resources to the specified base path.
        /// </summary>
        public static void MigrationsDeleteOldFiles()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            string[] TargetFilePath = new string[]
            {
                Path.Combine(basePath, "gpio"),
                Path.Combine(basePath, "libwiringPi.so.2.52"),
                Path.Combine(basePath, "libwiringPi.so.3.18"),
                Path.Combine(basePath, "wiringpi_3.18.deb"),
                Path.Combine(basePath, "wiringpi_3.181.deb")
            };

            foreach (var ItemPath in TargetFilePath)
            {
                try
                {
                    File.Delete(ItemPath);
                }
                catch
                {
                    /* Ignore */
                }
            }
        }
    }
}