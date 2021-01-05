namespace MotorControl.Commons
{

    /// <summary>
    /// Fixed ModBus registers
    /// </summary>
    public enum RegisterIndex
    {
        // NOTE: Register values are internally zero-based, 
        // although the spreadsheet lists them as 1-based

        // Motor Control
        MotorControlBase = 1000,
        MotorStartStop = MotorControlBase,
        MotorDirection,
        MotorSpeed,
        MotorControlOffset,

        // Operating Limits
        OperatingLimitsBase = 1100,
        LimitsSpeedUserMin = OperatingLimitsBase,
        LimitsSpeedUserMax,
        LimitsSpeedAbsoluteMin,
        LimitsSpeedAbsoluteMax,
        LimitsAccelRampDuration,
        LimitsDecelRampDuration,
        LimitsSkipSpeed1,
        LimitsSkipSpeed2,
        LimitsSkipSpeed3,
        LimitsSkipSpeedBandwidth,
        LimitsDirectionAllowed,
        OperatingLimitsOffset,

        // Operation Type
        OperationTypeBase = 1200,
        OperationControlMode = OperationTypeBase,
        OperationSpeedSource,
        OperationStartStopSource,
        OperationDirectionSource,
        OperationClearFaultSource,
        OperationStartFunction,
        OperationStopFunction,
        OperationTypeOffset,

        // Fault Reset
        FaultResetBase = 1300,
        FaultResetAttempts = FaultResetBase,
        FaultResetDelay,
        FaultResetOffset,

        // Terminal Settings
        TerminalSettingsAIBase = 2000,
        TerminalSettingsAI1Function = TerminalSettingsAIBase,
        TerminalSettingsAI1MinSetting,
        TerminalSettingsAI1MaxSetting,
        TerminalSettingsAI1Filter,
        TerminalSettingsAI1FaultAction,
        TerminalSettingsAI1TypeSelection,
        TerminalSettingsAIOffset,

        TerminalSettingsDIBase = 2100,
        TerminalSettingsDI1Function = TerminalSettingsDIBase,
        TerminalSettingsDI2Function,
        TerminalSettingsDI3Function,
        TerminalSettingsDI4Function,
        TerminalSettingsDIOffset,

        TerminalSettingsConstantSpeedBase = 2200,
        TerminalSettingsConstantSpeed1 = TerminalSettingsConstantSpeedBase,
        TerminalSettingsConstantSpeed2,
        TerminalSettingsConstantSpeed3,
        TerminalSettingsConstantSpeed4,
        TerminalSettingsConstantSpeedOffset,

        TerminalSettingsAOBase = 2300,
        TerminalSettingsAO1Function = TerminalSettingsAOBase,
        TerminalSettingsAO1ScalingMinimum,
        TerminalSettingsAO1ScalingMaximum,
        TerminalSettingsAO1Filter,
        TerminalSettingsAO1TypeSelection,
        TerminalSettingsAOOffset,

        TerminalSettingsDOBase = 2400,
        TerminalSettingsDO1Function = TerminalSettingsDOBase,
        TerminalSettingsDO2Function,
        TerminalSettingsDOOffset,

        //Monitor Electrical
        MonitorElectricalBase = 3000,
        ElectricalBusVoltage = MonitorElectricalBase,
        ElectricalBusCurrent,
        ElectricalBusInputPower,
        ElectricalAverageCurrent,
        //ElectricalPhaseBCurrent,
        //ElectricalPhaseCCurrent,
        MonitorElectricalOffset,

        // Monitor Environment
        MonitorEnvironmentBase = 3100,
        EnvironmentTemperature1 = MonitorEnvironmentBase,
        EnvironmentTemperature2,
        EnvironmentTemperature3,
        EnvironmentTemperature4,
        EnvironmentTemperature5,
        EnvironmentTemperature6,
        EnvironmentTemperature7,
        EnvironmentTemperature8,
        EnvironmentTemperature9,
        EnvironmentTemperature10,
        EnvironmentTemperature11,
        EnvironmentTemperature12,
        EnvironmentTemperature13,
        EnvironmentTemperature14,
        EnvironmentInverterBoardHumidity,
        MonitorEnvironmentOffset,

        // Monitor Temperature
        MonitorTemperatureBase = 3151,
        MonitorTemperatureLabel1 = MonitorTemperatureBase,
        MonitorTemperatureLabel2,
        MonitorTemperatureLabel3,
        MonitorTemperatureLabel4,
        MonitorTemperatureLabel5,
        MonitorTemperatureLabel6,
        MonitorTemperatureLabel7,
        MonitorTemperatureLabel8,
        MonitorTemperatureLabel9,
        MonitorTemperatureLabel10,
        MonitorTemperatureLabel11,
        MonitorTemperatureLabel12,
        MonitorTemperatureLabel13,
        MonitorTemperatureLabel14,
        MonitorTemperatureOffset,

        // Monitor Performance
        MonitorPerformanceBase = 3200,
        PerformanceOutputTorque = MonitorPerformanceBase,
        PerformanceOutputPower,
        PerformanceInputPower,
        PerformanceMotorEfficiency,
        MonitorPerformanceOffset,

        // Monitor Operating
        MonitorOperatingBase = 3300,
        OperatingStartStopActual = MonitorOperatingBase,
        OperatingDirectionActual,
        OperatingSpeedActual,
        MonitorOperatingOffset,

        // Monitor Sensor
        MonitorSensorBase = 3400,
        SensorAccelerometerStatus = MonitorSensorBase,
        SensorAccelerometerX,
        SensorAccelerometerY,
        SensorAccelerometerZ,
        MonitorSensorOffset,

        // Monitor IO
        MonitorIOAIBase = 3500,
        MonitorIOAI1Value = MonitorIOAIBase,
        MonitorIOAI2Value,
        MonitorIOAIOffset,

        MonitorIOAOBase = 3600,
        MonitorIOAO1Value = MonitorIOAOBase,
        MonitorIOAO2Value,
        MonitorIOAOOffset,

        MonitorIODIBase = 3700,
        MonitorIODI1Value = MonitorIODIBase,
        MonitorIODI2Value,
        MonitorIODI3Value,
        MonitorIODI4Value,
        MonitorIODIOffset,

        MonitorIODOBase = 3800,
        MonitorIODO1Value = MonitorIODOBase,
        MonitorIODO2Value,
        MonitorIODO3Value,
        MonitorIODO4Value,
        MonitorIODOOffset,

        // Faults / Warnings
        FaultsWarningsBase = 4000,
        FaultsActiveFaults = FaultsWarningsBase,
        FaultsActiveWarnings,
        FaultsWarningsOffset,
        // FaultsWarningsOffset is an invisible insertion
        FaultsClearFaults = FaultsWarningsOffset,   // write-only
        FaultsClearWarnings,                        // write-only

        FaultsFaultWord = 4010,
        FaultsWarningWord = 4020,

        // Device Information
        DeviceInformationBase = 7000,
        DeviceParameterTableVersion = DeviceInformationBase,
        DeviceMotorMaxCurrent,
        DeviceMotorVoltage,
        DeviceInformationOffset,

        DeviceSerialNumberBase = 7010,
        DeviceSerialNumberWord1 = DeviceSerialNumberBase,
        DeviceSerialNumberWord2,
        DeviceSerialNumberWord3,
        DeviceSerialNumberWord4,
        DeviceSerialNumberOffset,

        DeviceModelNumberBase = 7020,
        DeviceModelNumber = DeviceModelNumberBase,
        DeviceModelNumberOffset,

        DeviceFWVersionBase = 7030,
        DeviceFWVersionInvMajor = DeviceFWVersionBase,
        DeviceFWVersionInvMinor,
        DeviceFWVersionCIMMajor,
        DeviceFWVersionCIMMinor,
        DeviceFWVersionOffset,

        // Device Information - Lifetime
        DeviceLifetimeBase = 7100,
        DeviceDriveRuntime = DeviceLifetimeBase,
        DeviceMotorRuntime,
        DeviceMotorMWh,
        DeviceLifetimeOffset,

        SystemParameterSave = 8700,

        // Engineering Restricted
        EngineeringRestrictedBase = 8800,
        EngineeringPassword = EngineeringRestrictedBase,
        EngineeringRestrictedOffset,

    };


}