using GalaSoft.MvvmLight.Messaging;
using MotorControl.Commons.Controls.Common;
using MotorControl.Commons.Controls.Common.NoData;
using MotorControl.Commons.Models.Messages;
using MotorControl.Commons.ViewModels;
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

namespace MotorControl.Commons.Views.Readbacks
{
    /// <summary>
    /// Interaction logic for DigitalReadbacksControl.xaml
    /// </summary>
    public partial class DigitalReadbacksControl : UserControl
    {
        public DigitalReadbacksControl()
        {
            InitializeComponent();
            Messenger.Default.Register<MessageToDigitalReadbacksControl>(this, MessageHandler);
        }

        private DigitalReadbacksViewModel ViewModel
        {
            get => this.Resources[nameof(ViewModel)] as DigitalReadbacksViewModel;
        }

        private void MessageHandler(MessageToDigitalReadbacksControl message)
        {
            if (message.Connected == false)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.Content = new Tile() { Title = "Digital Readbacks", Content = new NoDataControl_1() };
                });
            }
            else
            {
                ViewModel.FillDigitalOutput(message.ModBus);
            }
        }
    }
}
