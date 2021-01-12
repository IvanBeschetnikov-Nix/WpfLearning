using GalaSoft.MvvmLight.Messaging;
using MotorControl.Commons.Controls.Common.NoData;
using MotorControl.Commons.Models.Messages.Tabs;
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
using System.Windows.Shapes;

namespace MotorControl.Commons.Views.Tabs
{
    /// <summary>
    /// Interaction logic for MonitorWindowControl.xaml
    /// </summary>
    public partial class MonitorWindowControl : UserControl
    {
        public MonitorWindowControl()
        {
            InitializeComponent();
            Messenger.Default.Register<MessageToMonitorWindowControl>(this, MessageHandler);
        }

        private void MessageHandler(MessageToMonitorWindowControl obj)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Content = new NoDataControl_2();
            });
        }
    }
}
