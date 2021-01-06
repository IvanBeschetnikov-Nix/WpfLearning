using LiveCharts;
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

namespace MotorControl.Commons.Views.Tabs
{
    /// <summary>
    /// Interaction logic for GraphicalReadbacks.xaml
    /// </summary>
    public partial class GraphicalReadbacks : UserControl
    {
        public ChartValues<int> Vals { get; set; } = new ChartValues<int> { 100, 500, 900, 200, 600, 1000, 120, 500};

        public GraphicalReadbacks()
        {
            InitializeComponent();
        }
    }
}
