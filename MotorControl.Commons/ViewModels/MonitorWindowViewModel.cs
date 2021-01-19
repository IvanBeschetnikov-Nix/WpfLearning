using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.ViewModels
{
    public class MonitorWindowViewModel : BaseViewModel
    {
        private int
            _temperature1,
            _temperature2,
            _temperature3,
            _temperature4,
            _temperature5,
            _temperature6,
            _temperature7,
            _temperature8,
            _temperature9,
            _temperature10,
            _temperature11,
            _temperature12,
            _temperature13,
            _temperature14;

        public int Temperature1 { get => _temperature1; set => SetPropertyValue(ref _temperature1, value, nameof(Temperature1)); }
        public int Temperature2 { get => _temperature2; set => SetPropertyValue(ref _temperature2, value, nameof(Temperature2)); }
        public int Temperature3 { get => _temperature3; set => SetPropertyValue(ref _temperature3, value, nameof(Temperature3)); }
        public int Temperature4 { get => _temperature4; set => SetPropertyValue(ref _temperature4, value, nameof(Temperature4)); }
        public int Temperature5 { get => _temperature5; set => SetPropertyValue(ref _temperature5, value, nameof(Temperature5)); }
        public int Temperature6 { get => _temperature6; set => SetPropertyValue(ref _temperature6, value, nameof(Temperature6)); }
        public int Temperature7 { get => _temperature7; set => SetPropertyValue(ref _temperature7, value, nameof(Temperature7)); }
        public int Temperature8 { get => _temperature8; set => SetPropertyValue(ref _temperature8, value, nameof(Temperature8)); }
        public int Temperature9 { get => _temperature9; set => SetPropertyValue(ref _temperature9, value, nameof(Temperature9)); }
        public int Temperature10 { get => _temperature10; set => SetPropertyValue(ref _temperature10, value, nameof(Temperature10)); }
        public int Temperature11 { get => _temperature11; set => SetPropertyValue(ref _temperature11, value, nameof(Temperature11)); }
        public int Temperature12 { get => _temperature12; set => SetPropertyValue(ref _temperature12, value, nameof(Temperature12)); }
        public int Temperature13 { get => _temperature13; set => SetPropertyValue(ref _temperature13, value, nameof(Temperature13)); }
        public int Temperature14 { get => _temperature14; set => SetPropertyValue(ref _temperature14, value, nameof(Temperature14)); }

        public void SetData(ModBusWrapper modBus)
        {
            this.Temperature1  = modBus.Temperature1;
            this.Temperature2  = modBus.Temperature2;
            this.Temperature3  = modBus.Temperature3;
            this.Temperature4  = modBus.Temperature4;
            this.Temperature5  = modBus.Temperature5;
            this.Temperature6  = modBus.Temperature6;
            this.Temperature7  = modBus.Temperature7;
            this.Temperature8  = modBus.Temperature8;
            this.Temperature9  = modBus.Temperature9;
            this.Temperature10 = modBus.Temperature10;
            this.Temperature11 = modBus.Temperature11;
            this.Temperature12 = modBus.Temperature12;
            this.Temperature13 = modBus.Temperature13;
            this.Temperature14 = modBus.Temperature14;
        }
    }
}
