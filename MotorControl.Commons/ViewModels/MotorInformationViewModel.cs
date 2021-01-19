using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.ViewModels
{
    public class MotorInformationViewModel : BaseViewModel
    {
        private string _motorSerialNo = "7011 - 7014";
        private string _model = "7033";
        private string _voltage = "7033";
        private string _maxCurrent = "7033";
        private string _driveRunTime = "7033";
        private string _motorRunTime = "7033";

        public MotorInformationViewModel()
        {
        }

        public string Model { get => _model; set => SetPropertyValue(ref _model, value, nameof(Model)); }
        public string MotorSerialNo { get => _motorSerialNo; set => SetPropertyValue(ref _motorSerialNo, value, nameof(MotorSerialNo)); }
        public string Voltage { get => _voltage; set => _voltage = value; }
        public string MaxCurrent { get => _maxCurrent; set => SetPropertyValue(ref _maxCurrent, value, nameof(MaxCurrent)); }
        public string DriveRunTime { get => _driveRunTime; set => SetPropertyValue(ref _driveRunTime, value, nameof(DriveRunTime)); }
        public string MotorRunTime { get => _motorRunTime; set => SetPropertyValue(ref _motorRunTime, value, nameof(MotorRunTime)); }
    }
}
