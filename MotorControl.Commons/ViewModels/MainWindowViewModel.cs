using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private TabsControl _control;
        public TabsControl Control
        {
            get => _control;
            set
            {
                _control = value;
                OnPropertyChanged(nameof(Control));
            }
        }
    }
}
