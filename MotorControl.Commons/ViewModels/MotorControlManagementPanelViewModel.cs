using MotorControl.Commons.Enums;
using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.ViewModels
{
    public class MotorControlManagementPanelViewModel : BaseViewModel
    {
        private int _readback;
        private int _speed, _speedMax, _speedMin;
        private bool _motorRunning;
        private bool _isEnabledMotorDirection;
        private Rotation _motorDirection;
        private RotationAllowed _directionAllowed;

        public MotorControlManagementPanelViewModel()
        {
        }

        public int Readback { get => _readback; set => SetPropertyValue(ref _readback, value, nameof(Readback)); }
        public Rotation MotorDirection { get => _motorDirection; set => SetPropertyValue(ref _motorDirection, value, nameof(MotorDirection)); }
        public int Speed { get => _speed; set => SetPropertyValue(ref _speed, value, nameof(Speed)); }
        public int SpeedMax { get => _speedMax; set => SetPropertyValue(ref _speedMax, value, nameof(SpeedMax)); }
        public int SpeedMin { get => _speedMin; set => SetPropertyValue(ref _speedMin, value, nameof(SpeedMin)); }
        public bool MotorRunning { get => _motorRunning; set => SetPropertyValue(ref _motorRunning, value, nameof(MotorRunning)); }
        public RotationAllowed DirectionAllowed { get => _directionAllowed; set => SetPropertyValue(ref _directionAllowed, value, nameof(DirectionAllowed)); }
        public bool IsEnabledMotorDirection { get => _isEnabledMotorDirection; set => SetPropertyValue(ref _isEnabledMotorDirection, value, nameof(IsEnabledMotorDirection)); }

        public void UpdateData(ModBusWrapper modBus, bool motorRunning, RotationAllowed dirrrectionAllowed)
        {
            this.Speed = Math.Max(modBus.LimitsSpeedUserMin, modBus.MotorSpeedSetting);
            this.SpeedMax = modBus.LimitsSpeedUserMax;
            this.SpeedMin = modBus.LimitsSpeedUserMin;
            this.Readback = modBus.MotorSpeedReadback;
            this.MotorRunning = motorRunning;

            this.IsEnabledMotorDirection = !motorRunning && dirrrectionAllowed == RotationAllowed.Both;
            this.MotorDirection = MotorDirectionMapper.GetFromRotationAllowed(dirrrectionAllowed) ?? this.MotorDirection;
        }
    }
}
