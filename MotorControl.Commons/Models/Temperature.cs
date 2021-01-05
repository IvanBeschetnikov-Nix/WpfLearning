using MotorControl.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorControl.Commons.Models
{
    public class Temperature
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public TemperatureUnitMeasurement Unit { get; set; }
    }
}
