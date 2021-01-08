using GalaSoft.MvvmLight.Messaging;
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

namespace MotorControl.Commons.Views.MotorManagement
{
    /// <summary>
    /// Interaction logic for MotorInformationControl.xaml
    /// </summary>
    public partial class MotorInformationControl : UserControl
    {
        public MotorInformationControl()
        {
            InitializeComponent();
            Messenger.Default.Register<MessageToMotorInformationControl>(this, MessageHandler);
        }

        private void MessageHandler(MessageToMotorInformationControl message)
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                this.Content = new NoDataControl_1();
            });
        }
    }
}
