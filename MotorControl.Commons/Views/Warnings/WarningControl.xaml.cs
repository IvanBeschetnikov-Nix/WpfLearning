using GalaSoft.MvvmLight.Messaging;
using MotorControl.Commons.Controls.Common;
using MotorControl.Commons.Controls.Common.NoData;
using MotorControl.Commons.Models.Messages;
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

namespace MotorControl.Commons.Views.Warnings
{
    /// <summary>
    /// Interaction logic for WarningControl.xaml
    /// </summary>
    public partial class WarningControl : UserControl
    {
        public WarningControl()
        {
            InitializeComponent();
            Messenger.Default.Register<MessageToWarningControl>(this, MessageHandler);
        }

        private void MessageHandler(MessageToWarningControl obj)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Content = new Tile() { Title = "Fault/Warnings", Content = new NoDataControl_3() };
            });
        }
    }
}
