using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.Models.Messages.Tabs
{
    public class MessageToMonitorWindowControl : MotorState
    {
        public ModBusWrapper ModBus { get; set; }
    }
}
