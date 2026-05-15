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
            string targetPath = Path.Combine(basePath, "wiringpi_3.18.deb");

            if (Core.Instance.RaspberryPi.OSRaspbianBullseye)
            {
                File.WriteAllBytes(targetPath, Properties.Resources.wiringpi_3_18_bullseye_armhf);
            }
            else
            {
                if(Core.Instance.RaspberryPi.IsArm64OS)
                {
                    File.WriteAllBytes(targetPath, Properties.Resources.wiringpi_3_18_arm64);
                }
                else
                {
                    File.WriteAllBytes(targetPath, Properties.Resources.wiringpi_3_18_armhf);  
                }
         
            }
        }
        /// <summary>
        /// Extracts all the file resources to the specified base path.
        /// </summary>
        public static void ExtractAll()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var executablePermissions = SysCall.StringToInteger("0777", IntPtr.Zero, 8);

            if(Core.Instance.RaspberryPi.OSRaspbianBullseye)
            {
                string targetPath = Path.Combine(basePath, "gpio");
                File.WriteAllBytes(targetPath, Properties.Resources.gpio_bullseye);

                try
                {
                    SysCall.Chmod(targetPath, (uint)executablePermissions);
                }
                catch
                {
                    /* Ignore */
                }

                targetPath = Path.Combine(basePath, "libwiringPi.so.3.18");
                File.WriteAllBytes(targetPath, Properties.Resources.libwiringPi_so_3_bullseye);

                try
                {
                    SysCall.Chmod(targetPath, (uint)executablePermissions);
                }
                catch
                {
                    /* Ignore */
                }
            }
            else
            {
                if(Core.Instance.RaspberryPi.IsArm64OS)
                {
                    string targetPath = Path.Combine(basePath, "gpio");
                    File.WriteAllBytes(targetPath, Properties.Resources.gpio_arm64);

                    try
                    {
                        SysCall.Chmod(targetPath, (uint)executablePermissions);
                    }
                    catch
                    {
                        /* Ignore */
                    }

                    targetPath = Path.Combine(basePath, "libwiringPi.so.3.18");
                    File.WriteAllBytes(targetPath, Properties.Resources.libwiringPi_so_3_arm64);

                    try
                    {
                        SysCall.Chmod(targetPath, (uint)executablePermissions);
                    }
                    catch
                    {
                        /* Ignore */
                    }
                }
                else
                {
                    string targetPath = Path.Combine(basePath, "gpio");
                    File.WriteAllBytes(targetPath, Properties.Resources.gpio);

                    try
                    {
                        SysCall.Chmod(targetPath, (uint)executablePermissions);
                    }
                    catch
                    {
                        /* Ignore */
                    }

                    targetPath = Path.Combine(basePath, "libwiringPi.so.3.18");
                    File.WriteAllBytes(targetPath, Properties.Resources.libwiringPi_so_3);

                    try
                    {
                        SysCall.Chmod(targetPath, (uint)executablePermissions);
                    }
                    catch
                    {
                        /* Ignore */
                    }
                }
            }
        }
    }
}