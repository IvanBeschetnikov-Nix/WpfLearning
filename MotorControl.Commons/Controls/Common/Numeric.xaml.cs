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
        public Numeric()
        {
            InitializeComponent();

            Loaded += ViewModel.OnLoaded;
            Loaded += Numeric_Loaded;
            Unloaded += ViewModel.OnUnloaded;

            ViewModel.Initialize();
        }

        private void Numeric_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize(IsFloatNumeric);
        }

        public bool IsFloatNumeric { get; set; }

        public Brush BorderBackground { get; set; } = new BrushConverter().ConvertFromString("#FFF3F3F3") as Brush;

        public NumericViewModel ViewModel
        {
            get => this.Resources["ViewModel"] as NumericViewModel;
        }
    }
}
