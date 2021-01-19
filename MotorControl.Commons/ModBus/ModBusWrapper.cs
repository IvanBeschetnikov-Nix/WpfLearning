using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyModbus;
using MotorControl.Commons;
using MotorControl.Commons.Enums;

namespace MotorControl
{
    /// <summary>
    /// Encapsulates the ModBus component
    /// </summary>
    public class ModBusWrapper
    {
        /// <summary>
        /// Handles modbus communications with the board
        /// </summary>
        private ModbusClient modbusClient = null;

        /// <summary>
        /// Connected event delegate
        /// </summary>
        /// <param name="connected"></param>
        public delegate void ConnectedHandler(bool connected);
        /// <summary>
        /// Indicates when connection state changes
        /// </summary>
        public event ConnectedHandler OnConnected;

        /// <summary>
        /// MotorRunning event delegate
        /// </summary>
        /// <param name="motorRunning"></param>
        public delegate void MotorRunningHandler(bool motorRunning);
        /// <summary>
        /// Indicates when motor's running state changes
        /// </summary>
        public event MotorRunningHandler OnMotorRunning;

        /// <summary>
        /// MotorSpeed changed event delegate
        /// </summary>
        /// <param name="motorSpeed"></param>
        public delegate void MotorSpeedChangedHandler(int motorSpeed);
        /// <summary>
        /// Indicates when motor's speed changes
        /// </summary>
        /// <remarks>
        /// Used for Readback display, not changes in requested speed
        /// </remarks>
        public event MotorSpeedChangedHandler OnMotorSpeedChanged;

        /// <summary>
        /// MotorDirection changed event delegate
        /// </summary>
        /// <param name="motorDirection"></param>
        public delegate void MotorDirectionChangedHandler(Rotation motorDirection);
        /// <summary>
        /// Indicates when motor's Direction changes
        /// </summary>
        public event MotorDirectionChangedHandler OnMotorDirectionChanged;

        /// <summary>
        /// Rudimentary simulation mode for debugging purposes
        /// </summary>
        public bool SimulationMode = false;

        private string _LastError;
        /// <summary>
        /// Read-only value of last ModBus error message
        /// </summary>
        public string LastError
        {
            get { return _LastError; }
            private set { _LastError = value; }
        }

        //TODO: SimulatePropsRandomUpdating
        private Random rnd = new Random();
        public void SimulatePropsRandomUpdating()
        {
            this.BusVoltage = rnd.Next(0, 100) + (float)rnd.NextDouble();
            this.AverageCurrent = rnd.Next(0, 12) + (float)rnd.NextDouble();
            this.Temperature2 = rnd.Next(0, 200);
            this.PowerInput = rnd.Next(1000, 10_000);
            this.PowerOutput = rnd.Next(1000, 10_000);
            this.MotorTorque = rnd.Next(0, 5) + (float)rnd.NextDouble();
            this.MotorEfficiency = rnd.Next(0, 100);

            this.LimitsSpeedUserMin = 100;
            this.LimitsSpeedUserMax = 1000;
            this.MotorSpeedReadback = rnd.Next(0, 1000);
        }
        #region "Connection Management"

        /// <summary>
        /// Connect to ModBus using the specified protocol
        /// </summary>
        /// <param name="protocol">TCP or Serial</param>
        /// <param name="address">IP Address or Serial Port name</param>
        /// <returns>Returns the resulting connected state</returns>
        public bool Connect(Protocol protocol, string address)
        {
            try
            {
                // if not already extant
                if (modbusClient == null)
                {
                    // instantiate ModBus Client
                    modbusClient = new ModbusClient();
                    // monitor connection state
                    modbusClient.ConnectedChanged += ModbusClient_ConnectedChanged;
                }

                if (!modbusClient.Connected)
                {
                    // consider protocol for this connection
                    if (protocol == Protocol.TCP)
                    {
                        // set IP Address and well-known Port
                        modbusClient.IPAddress = address;
                        modbusClient.Port = 502;
                        //disable serial port to switch to TCP
                        modbusClient.SerialPort = null;
                    }
                    else
                    {
                        // set up ModBus using Serial port
                        modbusClient.SerialPort = address;
                        modbusClient.Baudrate = 19200;
                        modbusClient.Parity = System.IO.Ports.Parity.None;
                        modbusClient.UnitIdentifier = 1;
                    }

                    // clear last error
                    LastError = "";

                    try
                    {
                        // connect using current settings
                        modbusClient.Connect();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            // allow one retry following failure
                            modbusClient.Connect();
                        }
                        catch (Exception ex)
                        {
                            LastError = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }

            // return connected state
            return Connected;
        }

        /// <summary>
        /// Disconnect ModBus client from board
        /// </summary>
        public void Disconnect(bool force = false)
        {
            // sanity check
            if (force || Connected)
            {
                modbusClient.Disconnect();
                // remove event
                modbusClient.ConnectedChanged -= ModbusClient_ConnectedChanged;
                // destroy ModBus instance
                modbusClient = null;
            }
        }

        /// <summary>
        /// Return the current 'connected' state
        /// </summary>
        public bool Connected
        {
            // we're connected if the ModBus client is instantiated and connected
            //            get { return (modbusClient != null && modbusClient.Connected && modbusClient.Available(1000)); }
            get
            {
                bool available = false;
                if (modbusClient != null && modbusClient.Connected)
                {
                    available = modbusClient.Available(500);
                    // give a second chance
                    if (!available)
                        available = modbusClient.Available(500);
                }
                return available;
            }
        }

        /// <summary>
        /// Raised by ModBus Client when connection state changes
        /// </summary>
        /// <param name="sender"></param>
        private void ModbusClient_ConnectedChanged(object sender)
        {
            // pass event on to listeners
            if (OnConnected != null)
                OnConnected.Invoke(Connected);
        }

        #endregion // Connection Management

        /// <summary>
        /// Motor Drive enabled state
        /// </summary>
        public bool DriveEnabled { get; private set; } = false;

        private bool _MotorRunEnable = false;
        /// <summary>
        /// Motor Drive running state
        /// </summary>
        /// <remarks>
        /// Rather than standard property behavior, the Setter only submits the request,
        /// while the Getter returns the value that comes back from the hardware.
        /// </remarks>
        public bool MotorRunEnable
        {
            get { return _MotorRunEnable; }
            set
            {
                // only submit if different than current setting
                if (_MotorRunEnable != value)
                    // 0 = Stop, 1 = Start
                    WriteRegister(RegisterIndex.MotorStartStop, (value ? 1 : 0));
            }
        }

        /// <summary>
        /// Motor Enabled readback from the hardware
        /// </summary>
        public bool MotorEnableReadback { get; private set; } = false;

        private Rotation _MotorDirection = Rotation.CCW;
        /// <summary>
        /// Motor Drive running state
        /// </summary>
        /// <remarks>
        /// Rather than standard property behavior, the Setter only submits the request,
        /// while the Getter returns the value that comes back from the hardware.
        /// </remarks>
        public Rotation MotorDirection
        {
            get { return _MotorDirection; }
            set
            {
                // only submit if different than current setting
                if (_MotorDirection != value)
                    WriteRegister(RegisterIndex.MotorDirection, (int)value);
            }
        }

        /// <summary>
        /// Motor Direction readback from the hardware
        /// </summary>
        public Rotation MotorDirectionReadback { get; private set; } = Rotation.CCW;

        private int _MotorSpeedSetting = 0;
        /// <summary>
        /// Motor speed setting (RPM)
        /// </summary>
        /// <remarks>
        /// Rather than standard property behavior, the Setter only submits the request,
        /// while the Getter returns the value that comes back from the hardware.
        /// </remarks>
        public int MotorSpeedSetting
        {
            get { return _MotorSpeedSetting; }
            set
            {
                WriteRegister(RegisterIndex.MotorSpeed, 
                    (value < LimitsSpeedUserMin) ? LimitsSpeedUserMin : 
                    ((value > LimitsSpeedUserMax) ? LimitsSpeedUserMax : value));
            }
        }

        /// <summary>
        /// Motor Speed readback from the hardware (RPM)
        /// </summary>
        public int MotorSpeedReadback { get; private set; } = 0;

        //private bool _ControlOverride = false;
        ///// <summary>
        ///// Control Override enabled state
        ///// </summary>
        ///// <remarks>
        ///// False for software controlled device, True for manual override
        ///// </remarks>
        //public bool ControlOverride
        //{
        //    get { return _ControlOverride; }
        //    set
        //    {
        //        // only submit if different than current setting
        //        if (_ControlOverride != value)
        //            writeRegister(RegisterIndex.OperationControlMode, (value ? 1 : 0));
        //    }
        //}

        ///// <summary>
        ///// Force Angle enabled state
        ///// </summary>
        //public bool ForceAngleEnabled { get; private set; } = false;


        public string DeviceSerialNumber1 { get; private set; }
        public string DeviceSerialNumber2 { get; private set; }
        public string DeviceSerialNumber3 { get; private set; }
        public string DeviceSerialNumber4 { get; private set; }
        public int DeviceModelNumber { get; private set; }
        public int DeviceVoltage { get; private set; }
        public float DeviceMaxCurrent { get; private set; }
        public int DeviceDriveRunTime { get; private set; }
        public int DeviceMotorRunTime { get; private set; }

        public int DeviceFWVersionMajor { get; private set; }
        public int DeviceFWVersionMinor { get; private set; }

        public int LimitsSpeedUserMin { get; set; }
        public int LimitsSpeedUserMax { get; set; }
        public int LimitsSpeedAbsoluteMin { get; private set; }
        public int LimitsSpeedAbsoluteMax { get; private set; }
        public int LimitsAccelRampDuration { get; set; }
        public int LimitsDecelRampDuration { get; set; }
        public int LimitsSkipSpeed1 { get; set; }
        public int LimitsSkipSpeed2 { get; set; }
        public int LimitsSkipSpeed3 { get; set; }
        public int LimitsSkipSpeedBandwidth { get; set; }
        public int LimitsDirectionAllowed { get; set; }

        public int ControlOverride { get; set; }
        public int SpeedInputSource { get; set; }
        public int StartStopInputSource { get; set; }
        public int DirectionInputSource { get; set; }
        public int ClearFaultInputSource { get; set; }

        public int FaultResetAttempts { get; set; }
        public float FaultResetDelay { get; set; }

        public int AI1Function { get; set; }
        public float AI1MinSetting { get; set; }
        public float AI1MaxSetting { get; set; }
        public int AI1Filter { get; set; }
        public int AI1FaultAction { get; set; }
        public int AI1Type { get; set; }

        public int DI1Function { get; set; }
        public int DI2Function { get; set; }
        public int DI3Function { get; set; }
        public int DI4Function { get; set; }

        public int ConstantSpeed1 { get; set; }
        public int ConstantSpeed2 { get; set; }
        public int ConstantSpeed3 { get; set; }
        public int ConstantSpeed4 { get; set; }

        public int AO1Function { get; set; }
        public float AO1ScalingMinimum { get; set; }
        public float AO1ScalingMaximum { get; set; }
        public int AO1Type { get; set; }

        public int DO1Function { get; set; }
        public int DO2Function { get; set; }

        public int PowerInput { get; private set; }

        public int PowerOutput { get; private set; }

        public int Temperature1 { get; private set; }
        /// <summary>
        /// Motor stator temperature reading
        /// </summary>
        public int Temperature2 { get; private set; }
        public int Temperature3 { get; private set; }
        public int Temperature4 { get; private set; }
        public int Temperature5 { get; private set; }
        public int Temperature6 { get; private set; }
        public int Temperature7 { get; private set; }
        public int Temperature8 { get; private set; }
        public int Temperature9 { get; private set; }
        public int Temperature10 { get; private set; }
        public int Temperature11 { get; private set; }
        public int Temperature12 { get; private set; }
        public int Temperature13 { get; private set; }
        public int Temperature14 { get; private set; }
        public int BoardHumidity { get; private set; }

        public int TemperatureLabel1 { get; private set; }
        public int TemperatureLabel2 { get; private set; }
        public int TemperatureLabel3 { get; private set; }
        public int TemperatureLabel4 { get; private set; }
        public int TemperatureLabel5 { get; private set; }
        public int TemperatureLabel6 { get; private set; }
        public int TemperatureLabel7 { get; private set; }
        public int TemperatureLabel8 { get; private set; }
        public int TemperatureLabel9 { get; private set; }
        public int TemperatureLabel10 { get; private set; }
        public int TemperatureLabel11 { get; private set; }
        public int TemperatureLabel12 { get; private set; }
        public int TemperatureLabel13 { get; private set; }
        public int TemperatureLabel14 { get; private set; }

        public int AccelerometerStatus { get; private set; }
        public int AccelerometerX { get; private set; }
        public int AccelerometerY { get; private set; }
        public int AccelerometerZ { get; private set; }

        public float AI1Value { get; private set; }
        public float AI2Value { get; private set; }
        public float AO1Value { get; private set; }
        public float AO2Value { get; private set; }

        public bool DI1Value { get; private set; }
        public bool DI2Value { get; private set; }
        public bool DI3Value { get; private set; }
        public bool DI4Value { get; private set; }
        public bool DO1Value { get; private set; }
        public bool DO2Value { get; private set; }
        public bool DO3Value { get; private set; }
        public bool DO4Value { get; private set; }

        public bool HasActiveFaults { get; private set; }
        public bool HasActiveWarnings { get; private set; }
        public uint FaultBits { get; private set; }
        public uint WarningBits { get; private set; }

        /// <summary>
        /// Motor torque (MNM)
        /// </summary>
        public float MotorTorque { get; private set; }

        public int MotorEfficiency { get; private set; }

        /// <summary>
        /// Bus Voltage
        /// </summary>
        public float BusVoltage { get; private set; }
        public float BusCurrent { get; private set; }
        public int BusInputPower { get; private set; }
        public float AverageCurrent { get; private set; }

        /// <summary>
        /// Readback of heartbeat register
        /// </summary>
        public int Heartbeat { get; private set; }

        /// <summary>
        /// Fault state
        /// </summary>
        /// <remarks>
        /// Returns bitset based on <c>Faults</c> enumeration
        /// </remarks>
        public int FaultState { get; private set; }

        /// <summary>
        /// Readback of password register
        /// </summary>
        public int Password { get; private set; } = 1234;

        #region "Register Read / Write"

        /// <summary>
        /// Write the specified value to the specified register
        /// </summary>
        /// <param name="register"></param>
        /// <param name="value"></param>
        public void WriteRegister(RegisterIndex register, int value)
        {
            try
            {
                modbusClient.WriteSingleRegister((int)register, value);
            }
            catch (Exception)
            {
                LastError = "Error saving register: " + register.ToString();
                // let exception be handled by the caller
                throw;
            }
        }

        /// <summary>
        /// Read and store defined range of registers
        /// </summary>
        /// <param name="start">Index of first register in range</param>
        /// <param name="count">Index one beyond the last register in range</param>
        /// <returns></returns>
        private ModbusStatuses readRegisterRange(RegisterIndex start, int count)
        {
            // iterator
            RegisterIndex register;
            // number of register entries
            //int count = stop - start;

            // array of register results
            int[] readback;

            try
            {
                // read all registers in range
                readback = modbusClient.ReadHoldingRegisters((int)start, count);
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return ModbusStatuses.STATUS_READ_FAILED;
            }

            // unpack register results
            //for (register = start; register < stop; register++)
            for (register = start; register < (start + count); register++)
            {
                // zero-based offset into array
                int offset = register - start;
                // integer result of register
                int intResult = readback[offset];
                // boolean result of register
                bool boolResult = (intResult != 0);
                // string result of register
                string stringResult = readback[offset].ToString();

                // consider current register
                switch (register)
                {
                    case RegisterIndex.MotorStartStop:
                        // 0 = Stop, 1 = Start
                        _MotorRunEnable = (intResult == 1);
                        break;

                    case RegisterIndex.MotorDirection:
                        // update property value
                        _MotorDirection = (Rotation)intResult;
                        break;

                    case RegisterIndex.MotorSpeed:
                        _MotorSpeedSetting = intResult;
                        break;


                    case RegisterIndex.LimitsSpeedUserMin:
                        LimitsSpeedUserMin = intResult;
                        break;
                    case RegisterIndex.LimitsSpeedUserMax:
                        LimitsSpeedUserMax = intResult;
                        break;
                    case RegisterIndex.LimitsSpeedAbsoluteMin:
                        LimitsSpeedAbsoluteMin = intResult;
                        break;
                    case RegisterIndex.LimitsSpeedAbsoluteMax:
                        LimitsSpeedAbsoluteMax = intResult;
                        break;
                    case RegisterIndex.LimitsAccelRampDuration:
                        LimitsAccelRampDuration = intResult;
                        break;
                    case RegisterIndex.LimitsDecelRampDuration:
                        LimitsDecelRampDuration = intResult;
                        break;
                    case RegisterIndex.LimitsSkipSpeed1:
                        LimitsSkipSpeed1 = intResult;
                        break;
                    case RegisterIndex.LimitsSkipSpeed2:
                        LimitsSkipSpeed2 = intResult;
                        break;
                    case RegisterIndex.LimitsSkipSpeed3:
                        LimitsSkipSpeed3 = intResult;
                        break;
                    case RegisterIndex.LimitsSkipSpeedBandwidth:
                        LimitsSkipSpeedBandwidth = intResult;
                        break;
                    case RegisterIndex.LimitsDirectionAllowed:
                        LimitsDirectionAllowed = intResult;
                        break;

                    case RegisterIndex.OperationControlMode:
                        ControlOverride = intResult;
                        break;
                    case RegisterIndex.OperationSpeedSource:
                        SpeedInputSource = intResult;
                        break;
                    case RegisterIndex.OperationStartStopSource:
                        StartStopInputSource = intResult;
                        break;
                    case RegisterIndex.OperationDirectionSource:
                        DirectionInputSource = intResult;
                        break;
                    case RegisterIndex.OperationClearFaultSource:
                        ClearFaultInputSource = intResult;
                        break;

                    case RegisterIndex.FaultResetAttempts:
                        FaultResetAttempts = intResult;
                        break;
                    case RegisterIndex.FaultResetDelay:
                        FaultResetDelay = (float)intResult / 10f;
                        break;

                    case RegisterIndex.OperationStartFunction:
                        break;
                    case RegisterIndex.OperationStopFunction:
                        break;

                    case RegisterIndex.TerminalSettingsAI1Function:
                        AI1Function = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsAI1MinSetting:
                        AI1MinSetting = (float)intResult / 10f;
                        break;
                    case RegisterIndex.TerminalSettingsAI1MaxSetting:
                        AI1MaxSetting = (float)intResult / 10f;
                        break;
                    case RegisterIndex.TerminalSettingsAI1Filter:
                        AI1Filter = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsAI1FaultAction:
                        AI1FaultAction = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsAI1TypeSelection:
                        AI1Type = intResult;
                        break;

                    case RegisterIndex.TerminalSettingsDI1Function:
                        DI1Function = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsDI2Function:
                        DI2Function = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsDI3Function:
                        DI3Function = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsDI4Function:
                        DI4Function = intResult;
                        break;

                    case RegisterIndex.TerminalSettingsConstantSpeed1:
                        ConstantSpeed1 = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsConstantSpeed2:
                        ConstantSpeed2 = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsConstantSpeed3:
                        ConstantSpeed3 = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsConstantSpeed4:
                        ConstantSpeed4 = intResult;
                        break;

                    case RegisterIndex.TerminalSettingsAO1Function:
                        AO1Function = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsAO1ScalingMinimum:
                        AO1ScalingMinimum = (float)intResult / 10f;
                        break;
                    case RegisterIndex.TerminalSettingsAO1ScalingMaximum:
                        AO1ScalingMaximum = (float)intResult / 10f;
                        break;
                    case RegisterIndex.TerminalSettingsAO1Filter:
                        break;
                    case RegisterIndex.TerminalSettingsAO1TypeSelection:
                        AO1Type = intResult;
                        break;

                    case RegisterIndex.TerminalSettingsDO1Function:
                        DO1Function = intResult;
                        break;
                    case RegisterIndex.TerminalSettingsDO2Function:
                        DO2Function = intResult;
                        break;


                    case RegisterIndex.ElectricalBusVoltage:
                        BusVoltage = (float)intResult / 10f;
                        break;
                    case RegisterIndex.ElectricalBusCurrent:
                        BusCurrent = (float)intResult / 10f;
                        break;
                    case RegisterIndex.ElectricalBusInputPower:
                        BusInputPower = intResult;
                        break;
                    case RegisterIndex.ElectricalAverageCurrent:
                        AverageCurrent = (float)intResult / 100f;
                        break;
                    //case RegisterIndex.ElectricalPhaseBCurrent:
                    //    break;
                    //case RegisterIndex.ElectricalPhaseCCurrent:
                    //    break;


                    case RegisterIndex.EnvironmentTemperature1:
                        Temperature1 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature2:
                        Temperature2 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature3:
                        Temperature3 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature4:
                        Temperature4 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature5:
                        Temperature5 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature6:
                        Temperature6 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature7:
                        Temperature7 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature8:
                        Temperature8 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature9:
                        Temperature9 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature10:
                        Temperature10 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature11:
                        Temperature11 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature12:
                        Temperature12 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature13:
                        Temperature13 = intResult;
                        break;
                    case RegisterIndex.EnvironmentTemperature14:
                        Temperature14 = intResult;
                        break;
                    case RegisterIndex.EnvironmentInverterBoardHumidity:
                        BoardHumidity = intResult;
                        break;

                    case RegisterIndex.MonitorTemperatureLabel1:
                        TemperatureLabel1 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel2:
                        TemperatureLabel2 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel3:
                        TemperatureLabel3 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel4:
                        TemperatureLabel4 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel5:
                        TemperatureLabel5 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel6:
                        TemperatureLabel6 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel7:
                        TemperatureLabel7 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel8:
                        TemperatureLabel8 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel9:
                        TemperatureLabel9 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel10:
                        TemperatureLabel10 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel11:
                        TemperatureLabel11 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel12:
                        TemperatureLabel12 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel13:
                        TemperatureLabel13 = intResult;
                        break;
                    case RegisterIndex.MonitorTemperatureLabel14:
                        TemperatureLabel14 = intResult;
                        break;

                    case RegisterIndex.PerformanceOutputTorque:
                        MotorTorque = (float)intResult / 1000f;
                        break;
                    case RegisterIndex.PerformanceOutputPower:
                        PowerOutput = intResult;
                        break;
                    case RegisterIndex.PerformanceInputPower:
                        PowerInput = intResult;
                        break;
                    case RegisterIndex.PerformanceMotorEfficiency:
                        MotorEfficiency = intResult;
                        break;

                    case RegisterIndex.SensorAccelerometerStatus:
                        AccelerometerStatus = intResult;
                        break;
                    case RegisterIndex.SensorAccelerometerX:
                        AccelerometerX = intResult;
                        break;
                    case RegisterIndex.SensorAccelerometerY:
                        AccelerometerY = intResult;
                        break;
                    case RegisterIndex.SensorAccelerometerZ:
                        AccelerometerZ = intResult;
                        break;

                    case RegisterIndex.MonitorIOAI1Value:
                        AI1Value = (float)intResult / 10f;
                        break;
                    case RegisterIndex.MonitorIOAI2Value:
                        AI2Value = (float)intResult / 10f;
                        break;
                    case RegisterIndex.MonitorIOAO1Value:
                        AO1Value = (float)intResult / 10f;
                        break;
                    case RegisterIndex.MonitorIOAO2Value:
                        AO2Value = (float)intResult / 10f;
                        break;

                    case RegisterIndex.MonitorIODI1Value:
                        DI1Value = boolResult;
                        break;
                    case RegisterIndex.MonitorIODI2Value:
                        DI2Value = boolResult;
                        break;
                    case RegisterIndex.MonitorIODI3Value:
                        DI3Value = boolResult;
                        break;
                    case RegisterIndex.MonitorIODI4Value:
                        DI4Value = boolResult;
                        break;

                    case RegisterIndex.MonitorIODO1Value:
                        DO1Value = boolResult;
                        break;
                    case RegisterIndex.MonitorIODO2Value:
                        DO2Value = boolResult;
                        break;
                    case RegisterIndex.MonitorIODO3Value:
                        DO3Value = boolResult;
                        break;
                    case RegisterIndex.MonitorIODO4Value:
                        DO4Value = boolResult;
                        break;

                    //case RegisterIndex.REG_INDEX_SPEED_OVERRIDE:
                    //    ControlOverride = boolResult;
                    //    break;
                    //case RegisterIndex.REG_INDEX_FORCE_ANGLE:
                    //    ForceAngleEnabled = boolResult;
                    //    break;
                    //case RegisterIndex.REG_INDEX_ID:
                    //    //if ((UInt16)readback[0] != regModbusID) //timestamp IDs don't match
                    //    //    lblModbusMismatch.Visibility = Visibility.Visible;
                    //    //else
                    //    //    lblModbusMismatch.Visibility = Visibility.Hidden;
                    //    break;
                    //case RegisterIndex.REG_INDEX_HEARTBEAT:
                    //    Heartbeat = intResult;
                    //    break;
                    //case RegisterIndex.REG_INDEX_FAULT_STATE:
                    //    FaultState = intResult;
                    //    break;

                    case RegisterIndex.FaultsActiveFaults:
                        HasActiveFaults = boolResult;
                        break;
                    case RegisterIndex.FaultsActiveWarnings:
                        HasActiveWarnings = boolResult;
                        break;
                    case RegisterIndex.FaultsFaultWord:
                        FaultBits = (uint)intResult;
                        break;
                    case RegisterIndex.FaultsWarningWord:
                        WarningBits = (uint)intResult;
                        break;

                    case RegisterIndex.OperatingStartStopActual:
                        // update state
                        MotorEnableReadback = boolResult;
                        // watch for change of state...
                        if (MotorEnableReadback != boolResult)
                        {
                            // pass event on to listeners
                            if (OnMotorRunning != null)
                                OnMotorRunning.Invoke(boolResult);
                        }
                        break;

                    case RegisterIndex.OperatingDirectionActual:
                        // update state
                        Rotation rotationResult = (Rotation)intResult;
                        MotorDirectionReadback = rotationResult;
                        // watch for change of rotation...
                        if (MotorDirectionReadback != rotationResult)
                        {
                            // pass event on to listeners
                            if (OnMotorDirectionChanged != null)
                                OnMotorDirectionChanged.Invoke(rotationResult);
                        }
                        break;

                    case RegisterIndex.OperatingSpeedActual:
                        MotorSpeedReadback = intResult;
                        // watch for change of speed...
                        if (MotorSpeedReadback != intResult)
                        {
                            // pass speed on to listeners
                            if (OnMotorSpeedChanged != null)
                                OnMotorSpeedChanged.Invoke(intResult);
                        }
                        break;



                    case RegisterIndex.DeviceSerialNumberWord1:
                        // 2 ascii characters
                        {
                            if ((intResult >> 8) < 32 || (intResult & 0x00FF) < 32)
                                DeviceSerialNumber1 = "INVALID";
                            else
                            {
                                char c1 = (char)(intResult >> 8);
                                char c2 = (char)(intResult & 0x00FF);
                                DeviceSerialNumber1 = string.Format("{0}{1}", c1, c2);
                            }
                        }
                        break;
                    case RegisterIndex.DeviceSerialNumberWord2:
                        // 2 ascii characters
                        {
                            if (DeviceSerialNumber1 == "INVALID")
                                DeviceSerialNumber2 = "";
                            else
                            {
                                char c1 = (char)(intResult >> 8);
                                char c2 = (char)(intResult & 0x00FF);
                                DeviceSerialNumber2 = string.Format("{0}{1}", c1, c2);
                            }
                        }
                        break;
                    case RegisterIndex.DeviceSerialNumberWord3:
                        // 3 digit number
                        if (DeviceSerialNumber1 == "INVALID")
                            DeviceSerialNumber3 = "";
                        else
                            DeviceSerialNumber3 = string.Format("{0:000}", intResult);
                        break;
                    case RegisterIndex.DeviceSerialNumberWord4:
                        // 3 digit number
                        if (DeviceSerialNumber1 == "INVALID")
                            DeviceSerialNumber4 = "";
                        else
                            DeviceSerialNumber4 = string.Format("{0:000}", intResult);
                        break;
                    case RegisterIndex.DeviceModelNumber:
                        DeviceModelNumber = intResult;
                        break;
                    case RegisterIndex.DeviceMotorVoltage:
                        DeviceVoltage = intResult;
                        break;
                    case RegisterIndex.DeviceMotorMaxCurrent:
                        DeviceMaxCurrent = (float)intResult / 100f;
                        break;
                    case RegisterIndex.DeviceFWVersionInvMajor:
                        DeviceFWVersionMajor = intResult;
                        break;
                    case RegisterIndex.DeviceFWVersionInvMinor:
                        DeviceFWVersionMinor = intResult;
                        break;
                    case RegisterIndex.DeviceFWVersionCIMMajor:
                        break;
                    case RegisterIndex.DeviceFWVersionCIMMinor:
                        break;


                    case RegisterIndex.DeviceDriveRuntime:
                        DeviceDriveRunTime = intResult;
                        break;
                    case RegisterIndex.DeviceMotorRuntime:
                        DeviceMotorRunTime = intResult;
                        break;
                    case RegisterIndex.DeviceMotorMWh:
                        break;


                    case RegisterIndex.EngineeringPassword:
                        Password = intResult;
                        break;


                    default:
                        break;
                }
            }
            return ModbusStatuses.STATUS_SUCCESS;
        }

        /// <summary>
        /// Read and store all defined Register values
        /// </summary>
        /// <returns></returns>
        public ModbusStatuses ReadAllRegisters()
        {
            ModbusStatuses status = ModbusStatuses.STATUS_SUCCESS;
            bool oneAtAtTime = false;
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                if (!oneAtAtTime)
                {
                    // call based on distinct ranges of enum values
                    status = readRegisterRange(RegisterIndex.MotorControlBase, RegisterIndex.MotorControlOffset - RegisterIndex.MotorControlBase);

                    status |= readRegisterRange(RegisterIndex.OperatingLimitsBase, RegisterIndex.OperatingLimitsOffset - RegisterIndex.OperatingLimitsBase);
                    status |= readRegisterRange(RegisterIndex.OperationTypeBase, RegisterIndex.OperationTypeOffset - RegisterIndex.OperationTypeBase);
                    status |= readRegisterRange(RegisterIndex.FaultResetBase, RegisterIndex.FaultResetOffset - RegisterIndex.FaultResetBase);

                    status |= readRegisterRange(RegisterIndex.TerminalSettingsAIBase, RegisterIndex.TerminalSettingsAIOffset - RegisterIndex.TerminalSettingsAIBase);
                    status |= readRegisterRange(RegisterIndex.TerminalSettingsDIBase, RegisterIndex.TerminalSettingsDIOffset - RegisterIndex.TerminalSettingsDIBase);
                    status |= readRegisterRange(RegisterIndex.TerminalSettingsConstantSpeedBase, RegisterIndex.TerminalSettingsConstantSpeedOffset - RegisterIndex.TerminalSettingsConstantSpeedBase);
                    status |= readRegisterRange(RegisterIndex.TerminalSettingsAOBase, RegisterIndex.TerminalSettingsAOOffset - RegisterIndex.TerminalSettingsAOBase);
                    status |= readRegisterRange(RegisterIndex.TerminalSettingsDOBase, RegisterIndex.TerminalSettingsDOOffset - RegisterIndex.TerminalSettingsDOBase);

                    //status |= readRegisterRange(RegisterIndex.MonitorElectricalBase, RegisterIndex.MonitorElectricalOffset - RegisterIndex.MonitorElectricalBase);
                    status |= readRegisterRange(RegisterIndex.ElectricalBusVoltage, 1);
                    status |= readRegisterRange(RegisterIndex.ElectricalAverageCurrent, 1);
                    //
                    status |= readRegisterRange(RegisterIndex.MonitorEnvironmentBase, RegisterIndex.MonitorEnvironmentOffset - RegisterIndex.MonitorEnvironmentBase);
                    status |= readRegisterRange(RegisterIndex.MonitorTemperatureBase, RegisterIndex.MonitorTemperatureOffset - RegisterIndex.MonitorTemperatureBase);
                    status |= readRegisterRange(RegisterIndex.MonitorPerformanceBase, RegisterIndex.MonitorPerformanceOffset - RegisterIndex.MonitorPerformanceBase);
                    status |= readRegisterRange(RegisterIndex.MonitorOperatingBase, RegisterIndex.MonitorOperatingOffset - RegisterIndex.MonitorOperatingBase);
                    status |= readRegisterRange(RegisterIndex.MonitorSensorBase, RegisterIndex.MonitorSensorOffset - RegisterIndex.MonitorSensorBase);

                    status |= readRegisterRange(RegisterIndex.MonitorIOAIBase, RegisterIndex.MonitorIOAIOffset - RegisterIndex.MonitorIOAIBase);
                    status |= readRegisterRange(RegisterIndex.MonitorIOAOBase, RegisterIndex.MonitorIOAOOffset - RegisterIndex.MonitorIOAOBase);
                    status |= readRegisterRange(RegisterIndex.MonitorIODIBase, RegisterIndex.MonitorIODIOffset - RegisterIndex.MonitorIODIBase);
                    status |= readRegisterRange(RegisterIndex.MonitorIODOBase, RegisterIndex.MonitorIODOOffset - RegisterIndex.MonitorIODOBase);

                    status |= readRegisterRange(RegisterIndex.FaultsWarningsBase, RegisterIndex.FaultsWarningsOffset - RegisterIndex.FaultsWarningsBase);
                    status |= readRegisterRange(RegisterIndex.FaultsFaultWord, 1);
                    status |= readRegisterRange(RegisterIndex.FaultsWarningWord, 1);

                    status |= readRegisterRange(RegisterIndex.DeviceInformationBase, RegisterIndex.DeviceInformationOffset - RegisterIndex.DeviceInformationBase);
                    status |= readRegisterRange(RegisterIndex.DeviceSerialNumberBase, RegisterIndex.DeviceSerialNumberOffset - RegisterIndex.DeviceSerialNumberBase);
                    status |= readRegisterRange(RegisterIndex.DeviceModelNumberBase, RegisterIndex.DeviceModelNumberOffset - RegisterIndex.DeviceModelNumberBase);
                    status |= readRegisterRange(RegisterIndex.DeviceFWVersionBase, RegisterIndex.DeviceFWVersionOffset - RegisterIndex.DeviceFWVersionBase);
                    status |= readRegisterRange(RegisterIndex.DeviceLifetimeBase, RegisterIndex.DeviceLifetimeOffset - RegisterIndex.DeviceLifetimeBase);

                    status |= readRegisterRange(RegisterIndex.EngineeringRestrictedBase, RegisterIndex.EngineeringRestrictedOffset - RegisterIndex.EngineeringRestrictedBase);
                }
                else
                {
                    // call for all distinct enums (but one at a time)
                    foreach (RegisterIndex register in (RegisterIndex[])Enum.GetValues(typeof(RegisterIndex)))
                    {
                        status |= readRegisterRange(register, 1);
                    }
                }
                sw.Stop();
                Debug.WriteLine(sw.ElapsedMilliseconds.ToString());
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                // force disconnect ?
                Disconnect(true);
            }
            // at least at initial stages, status indicates that either the read was successful, 
            // or that some part of the read failed (but not specifically which part)
            return ModbusStatuses.STATUS_SUCCESS; //status;
        }

        /// <summary>
        /// Write specific registers as required by application
        /// </summary>
        public void WriteAllRegisters()
        {
            // Limits
            WriteRegister(RegisterIndex.LimitsSpeedUserMax, LimitsSpeedUserMax);
            WriteRegister(RegisterIndex.LimitsSpeedUserMin, LimitsSpeedUserMin);
            WriteRegister(RegisterIndex.LimitsAccelRampDuration, LimitsAccelRampDuration);
            WriteRegister(RegisterIndex.LimitsDecelRampDuration, LimitsDecelRampDuration);
            WriteRegister(RegisterIndex.LimitsSkipSpeed1, LimitsSkipSpeed1);
            WriteRegister(RegisterIndex.LimitsSkipSpeed2, LimitsSkipSpeed2);
            WriteRegister(RegisterIndex.LimitsSkipSpeed3, LimitsSkipSpeed3);
            WriteRegister(RegisterIndex.LimitsSkipSpeedBandwidth, LimitsSkipSpeedBandwidth);
            WriteRegister(RegisterIndex.LimitsDirectionAllowed, LimitsDirectionAllowed);

            // Operation Type
            WriteRegister(RegisterIndex.OperationControlMode, ControlOverride);
            WriteRegister(RegisterIndex.OperationSpeedSource, SpeedInputSource);
            WriteRegister(RegisterIndex.OperationStartStopSource, StartStopInputSource);
            WriteRegister(RegisterIndex.OperationDirectionSource, DirectionInputSource);
            WriteRegister(RegisterIndex.OperationClearFaultSource, ClearFaultInputSource);

            // Fault Reset
            WriteRegister(RegisterIndex.FaultResetAttempts, FaultResetAttempts);
            WriteRegister(RegisterIndex.FaultResetDelay, (int)(FaultResetDelay * 10));

            // Terminal Settings
            WriteRegister(RegisterIndex.TerminalSettingsAI1Function, AI1Function);
            WriteRegister(RegisterIndex.TerminalSettingsAI1MaxSetting, (int)(AI1MaxSetting * 10));
            WriteRegister(RegisterIndex.TerminalSettingsAI1MinSetting, (int)(AI1MinSetting * 10));
            WriteRegister(RegisterIndex.TerminalSettingsAI1Filter, AI1Filter);
            WriteRegister(RegisterIndex.TerminalSettingsAI1FaultAction, AI1FaultAction);
            WriteRegister(RegisterIndex.TerminalSettingsAI1TypeSelection, AI1Type);

            //WriteRegister(RegisterIndex.TerminalSettingsDI1Function, DI1Function);
            //WriteRegister(RegisterIndex.TerminalSettingsDI2Function, DI2Function);
            //WriteRegister(RegisterIndex.TerminalSettingsDI3Function, DI3Function);
            //WriteRegister(RegisterIndex.TerminalSettingsDI4Function, DI4Function);

            WriteRegister(RegisterIndex.TerminalSettingsConstantSpeed1, ConstantSpeed1);
            WriteRegister(RegisterIndex.TerminalSettingsConstantSpeed2, ConstantSpeed2);
            WriteRegister(RegisterIndex.TerminalSettingsConstantSpeed3, ConstantSpeed3);
            WriteRegister(RegisterIndex.TerminalSettingsConstantSpeed4, ConstantSpeed4);

            WriteRegister(RegisterIndex.TerminalSettingsAO1Function, AO1Function);
            WriteRegister(RegisterIndex.TerminalSettingsAO1ScalingMaximum, (int)(AO1ScalingMaximum * 10));
            WriteRegister(RegisterIndex.TerminalSettingsAO1ScalingMinimum, (int)(AO1ScalingMinimum * 10));
            WriteRegister(RegisterIndex.TerminalSettingsAO1TypeSelection, AO1Type);

            WriteRegister(RegisterIndex.TerminalSettingsDO1Function, DO1Function);
            WriteRegister(RegisterIndex.TerminalSettingsDO2Function, DO2Function);
        }

        #endregion // Register Read / Write

    }
}
