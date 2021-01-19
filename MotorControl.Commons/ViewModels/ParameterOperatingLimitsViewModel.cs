using GalaSoft.MvvmLight.Messaging;
using MotorControl.Commons.Enums;
using MotorControl.Commons.Models.Messages;
using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.ViewModels
{
    public class ParameterOperatingLimitsViewModel : BaseViewModel
    {
        private string _selectedDirectionAllowed;
        private IDictionary<string, RotationAllowed> rotationMap;

        public ParameterOperatingLimitsViewModel()
        {
            rotationMap = new Dictionary<string, RotationAllowed>
            {
                ["CCW Only"] = RotationAllowed.CCW_Only,
                ["CW Only"] = RotationAllowed.CW_Only,
                ["Both"] = RotationAllowed.Both,
            };
        }

        public string SelectedDirectionAllowed 
        { 
            get => _selectedDirectionAllowed; 
            set
            {
                SetPropertyValue(ref _selectedDirectionAllowed, value, nameof(SelectedDirectionAllowed));
                Messenger.Default.Send<MessageToMainWindow_MotorDirectionChanged>(new MessageToMainWindow_MotorDirectionChanged { DirectionAllowed = rotationMap[value] });
            }
        }
    }
}
