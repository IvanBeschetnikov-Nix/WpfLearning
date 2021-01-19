using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.Models.Messages
{
    public class MessageToDigitalReadbacksControl : MotorState
    {
        public ModBusWrapper ModBus { get; set; }
    }
}
