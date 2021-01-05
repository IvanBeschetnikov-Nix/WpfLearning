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
    /// Interaction logic for MotorControlReadbacksCard.xaml
    /// </summary>
    public partial class MotorControlReadbacksCard : UserControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(MotorControlReadbacksCard));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(string), typeof(MotorControlReadbacksCard));

        public MotorControlReadbacksCard()
        {
            InitializeComponent();
        }

        public string Header 
        {
            get => GetValue(HeaderProperty) as string;
            set => SetValue(HeaderProperty, value); 
        }
        public string Value
        {
            get => GetValue(ValueProperty) as string;
            set => SetValue(ValueProperty, value);
        }
    }
}
