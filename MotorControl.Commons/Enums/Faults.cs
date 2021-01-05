using System;

namespace MotorControl.Commons
{
    //public enum ChartItem
    //{
    //    Voltage,
    //    Current,
    //    StatorTemp,
    //    Vibration,
    //    PowerInput,
    //    PowerOutput,
    //    Torque,
    //    Efficiency
    //}

    /// <summary>
    /// Fault bits
    /// </summary>
    [Flags]
    public enum Faults
    {
        /// <summary>
        /// No faults
        /// </summary>
        NoFaults = 0,
        /// <summary>
        /// Hardware overcurrent fault bit
        /// </summary>
        HWOvercurrent = 1,
        /// <summary>
        /// Software overcurrent fault bit
        /// </summary>
        SWOvercurrent = 2,
        /// <summary>
        /// Overvoltage fault bit
        /// </summary>
        Overvoltage = 4,
        /// <summary>
        /// Undervoltage fault bit
        /// </summary>
        Undervoltage = 8,
        /// <summary>
        /// IGBT overtemp fault bit
        /// </summary>
        IGBTOvertemp = 16,
        /// <summary>
        /// Stator overtemp fault bit
        /// </summary>
        StatorOvertemp = 32,
        /// <summary>
        /// Board overtemp fault bit
        /// </summary>
        BoardOvertemp = 64,
    };


}