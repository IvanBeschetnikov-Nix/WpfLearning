using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.Models.Messages
{
    public class MessageToParameterSettingsControl : MotorState
    {
        public ModBusWrapper ModBus { get; set; }
        public bool MotorRunning { get; set; }
    }
}
