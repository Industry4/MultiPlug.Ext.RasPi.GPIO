///
/// Note: Code taken from Unosquare.WiringPi which is now in Public archive state on github.
/// MIT License. Copyright(c) 2018 Unosquare Labs. https://github.com/unosquare/wiringpi-dotnet
/// 
namespace MultiPlug.Ext.RasPi.GPIO.Utils.WiringPi
{
    using Native;
    using Unosquare.RaspberryIO.Abstractions;
    using System;

    /// <summary>
    /// Represents the WiringPi system info.
    /// </summary>
    /// <seealso cref="ISystemInfo" />
    public class SystemInfo : ISystemInfo
    {
        private static readonly object Lock = new object();
        private static bool _revGetted;
        private static BoardRevision _boardRevision = BoardRevision.Rev2;

        /// <inheritdoc />
        public BoardRevision BoardRevision => GetBoardRevision();

        /// <inheritdoc />
        public Version LibraryVersion
        {
            get
            {
                var libParts = WiringPi.WiringPiLibrary.Split('.');
                var major = int.Parse(libParts[libParts.Length - 2]);
                var minor = int.Parse(libParts[libParts.Length - 1]);
                return new Version(major, minor);
            }
        }

        internal static BoardRevision GetBoardRevision()
        {
            lock (Lock)
            {
                if (_revGetted) return _boardRevision;
                var val = WiringPi.PiBoardRev();
                _boardRevision = val == 1 ? BoardRevision.Rev1 : BoardRevision.Rev2;
                _revGetted = true;
            }

            return _boardRevision;
        }
    }
}
