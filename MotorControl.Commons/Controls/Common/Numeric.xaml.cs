using MotorControl.Commons.Commanding.RelayCommand;
using MotorControl.Commons.Controls.Common.ViewModels;
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
    /// Interaction logic for Numeric.xaml
    /// </summary>
    public partial class Numeric : UserControl
    {
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(decimal), typeof(Numeric));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(decimal), typeof(Numeric));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(Numeric));
        public static readonly DependencyProperty IsFloatProperty = DependencyProperty.Register(nameof(IsFloat), typeof(bool), typeof(Numeric));

        public Numeric()
        {
            IncreaseCommand = new RelayCommand(IncreaseCommandHandler);
            DecreaseCommand = new RelayCommand(DecreaseCommandHandler);

            InitializeComponent();
        }

        public bool IsFloat { get => (bool)GetValue(IsFloatProperty); set => SetValue(IsFloatProperty, value); }
        public decimal MinValue { get => (decimal)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public decimal MaxValue { get => (decimal)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public decimal Value
        {
            get => IsFloat
                ? (decimal)GetValue(ValueProperty)
                : (int)((decimal)GetValue(ValueProperty));
            set
            {
                if (IsValueInRange(value)) 
                    SetValue(ValueProperty, value);
            }
        }

        public RelayCommand IncreaseCommand { get; }
        public RelayCommand DecreaseCommand { get; }


        private void DecreaseCommandHandler()
        {
            if (IsFloat) Value -= 0.1m;
            else Value -= 1;
        }

        private void IncreaseCommandHandler()
        {
            if (IsFloat) Value += 0.1m;
            else Value += 1;
        }

        private bool IsValueInRange(decimal value) => value >= MinValue && value <= MaxValue;
    }
}
