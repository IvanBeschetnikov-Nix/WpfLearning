using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MotorControl.Commons.Views.CompanySupport
{
    /// <summary>
    /// Interaction logic for CompanySupportControl.xaml
    /// </summary>
    public partial class CompanySupportControl : UserControl
    {
        public CompanySupportControl()
        {
            InitializeComponent();
        }

        private void ProcessOnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        }
    }
}
