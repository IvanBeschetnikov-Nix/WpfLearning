using MotorControl.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.Models.Messages
{
    public class MessageToMainWindow_MotorDirectionChanged
    {
        public RotationAllowed DirectionAllowed { get; set; }
    }
}
