///
/// Note: Code taken from Unosquare.WiringPi which is now in Public archive state on github.
/// MIT License. Copyright(c) 2018 Unosquare Labs. https://github.com/unosquare/wiringpi-dotnet
/// 
using System;

namespace MultiPlug.Ext.RasPi.GPIO.Utils.WiringPi.Native
{
    /// <summary>
    /// A delegate defining a callback for an Interrupt Service Routine.
    /// </summary>
    public delegate void InterruptServiceRoutineCallback();

    /// <summary>
    /// Defines the body of a thread worker.
    /// </summary>
    public delegate void ThreadWorker();


    public struct WPIWfiStatus
    {
        public int statusOK;        // -1: error (return of 'poll' command), 0: timeout, 1: irq processed, next data values are valid if needed
        public uint pinBCM;         // gpio as BCM pin
        public int edge;            // INT_EDGE_FALLING or INT_EDGE_RISING
        public Int64 timeStamp_us;  // time stamp in microseconds
    };


    public delegate void InterruptServiceRoutineCallbackV2(WPIWfiStatus wfiStatus, IntPtr userdata);
}
