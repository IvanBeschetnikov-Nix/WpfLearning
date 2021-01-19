using MotorControl.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.Models.Messages
{
    public class MessageToMotorControlManagementPanelControl : MotorState
    {
        public ModBusWrapper ModBus { get; set; }
        public bool MotorRunning { get; set; }
        public RotationAllowed DirectionAllowed { get; set; } = RotationAllowed.Both;
    }
}
