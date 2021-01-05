using MotorControl.Commons.Controls.Common.ViewModels;
using MotorControl.Commons.Enums;
using MotorControl.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MotorControl.Commons.Controls.Common
{
    /// <summary>
    /// Interaction logic for TemperatureControl.xaml
    /// </summary>
    public partial class TemperatureControl : UserControl
    {
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register(nameof(Enabled), typeof(bool), typeof(TemperatureControl));
        public static readonly DependencyProperty TemperatureNameProperty = DependencyProperty.Register(nameof(TemperatureName), typeof(string), typeof(TemperatureControl));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(TemperatureControl));
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(TemperatureUnitMeasurement), typeof(TemperatureControl));

        public TemperatureControl()
        {
            InitializeComponent();
            Loaded += TemperatureControl_Loaded;
        }

        private void TemperatureControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize(new Temperature
            {
                Enabled = Enabled,
                Name = TemperatureName,
                Unit = Unit,
                Value = Value
            });
        }

        public bool Enabled
        {
            get => (bool)GetValue(EnabledProperty);
            set => SetValue(EnabledProperty, value);
        }

        public string TemperatureName
        {
            get => (string)GetValue(TemperatureNameProperty);
            set => SetValue(TemperatureNameProperty, value);
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public TemperatureUnitMeasurement Unit
        {
            get => (TemperatureUnitMeasurement)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        public TemperatureControlViewModel ViewModel
        {
            get => this.Resources[nameof(ViewModel)] as TemperatureControlViewModel;
        }
    }
}
