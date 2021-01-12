using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.ViewModels
{
    public class MotorControlManagementPanelViewModel : BaseViewModel
    {
        private string _readback = "3003";
        private MotorDirections _motorDirection = MotorDirections.CCW;
        private int _speed = 100;

        public MotorControlManagementPanelViewModel()
        {

        }

        public string Readback 
        { 
            get => _readback; 
            set
            {
                _readback = value;
                OnPropertyChanged(nameof(Readback));
            }
        }

        public MotorDirections MotorDirection
        {
            get => _motorDirection;
            set => _motorDirection = value;
        }

        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }
    }

    public enum MotorDirections
    {
        CCW,
        ABS
    }
}
