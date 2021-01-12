using MotorControl.Commons.Commanding.RelayCommand;
using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MotorControl.Commons.Controls.Common.ViewModels
{
    public class NumericViewModel : BaseViewModel
    {
        public static DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(string), typeof(Numeric));

        private string _value;

        private Action IncreaseValue;
        private Action DecreaseValue;
        private Action<string> SetValue;

        public NumericViewModel()
        {
            IncreaseCommand = new RelayCommand(IncreaseCommandHandler);
            DecreaseCommand = new RelayCommand(DecreaseCommandHandler);
        }

        public void Initialize(bool isFloatNumeric)
        {
            if (isFloatNumeric)
            {
                IncreaseValue = IncreaseDoubleValueHandler;
                DecreaseValue = DecreaseDoubleValueHandler;
                SetValue = SetDoubleValue;
                Value = (0.0m).ToString();
                
            }
            else
            {
                IncreaseValue = IncreaseIntegerValueHandler;
                DecreaseValue = DecreaseIntegerValueHandler;
                SetValue = SetIntegerValue;
                Value = 0.ToString();
            }
        }

        public RelayCommand IncreaseCommand { get; }
        public RelayCommand DecreaseCommand { get; }

        private void IncreaseCommandHandler() => IncreaseValue();
        private void DecreaseCommandHandler() => DecreaseValue();

        public string Value
        {
            get => _value;
            set => SetValue?.Invoke(value);
        }

        private void IncreaseIntegerValueHandler()
        {
            int.TryParse(Value, out int value);

            Value = (++value).ToString();
        }

        private void IncreaseDoubleValueHandler()
        {
            decimal.TryParse(Value, out decimal value);

            Value = (value + 0.1m).ToString();
        }

        private void DecreaseIntegerValueHandler()
        {
            int.TryParse(Value, out int value);

            Value = (--value).ToString();
        }

        private void DecreaseDoubleValueHandler()
        {
            decimal.TryParse(Value, out decimal value);

            Value = (value - 0.1m).ToString();
        }

        private void SetIntegerValue(string value)
        {
            if (int.TryParse(value ?? 0.ToString(), out int val))
            {
                _value = val.ToString();
                OnPropertyChanged(nameof(Value));
            }
        }

        private void SetDoubleValue(string value)
        {
            if (decimal.TryParse(value ?? 0.ToString(), out decimal val))
            {
                _value = val.ToString();
                OnPropertyChanged(nameof(Value));
            }
        }

        public void SetDoubleValue(double value) => _value = value.ToString();
        public double GetDoubleValue() => double.Parse(_value);
    }
}
