using GalaSoft.MvvmLight.Messaging;
using MotorControl.Commons.Controls.Common.NoData;
using MotorControl.Commons.Models.Messages.Tabs;
using System;
using System.Collections.Generic;
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

namespace MotorControl.Commons.Views.Tabs
{
    /// <summary>
    /// Interaction logic for EngineeringRestrictedControl.xaml
    /// </summary>
    public partial class EngineeringRestrictedControl : UserControl
    {
        public EngineeringRestrictedControl()
        {
            InitializeComponent();
            Messenger.Default.Register<MessageToEngineeringRestrictedControl>(this, MessageHandler);
        }

        private void MessageHandler(MessageToEngineeringRestrictedControl obj)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Content = new NoDataControl_2();
            });
        }
    }
}
