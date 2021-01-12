using GalaSoft.MvvmLight.Messaging;
using MotorControl.Commons.Controls.Common.NoData;
using MotorControl.Commons.Models.Messages;
using MotorControl.Commons.ViewModels;
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

namespace MotorControl.Commons.Views.MotorManagement
{
    /// <summary>
    /// Interaction logic for MotorControlManagementPanelControl.xaml
    /// </summary>
    public partial class MotorControlManagementPanelControl : UserControl
    {
        public MotorControlManagementPanelControl()
        {
            InitializeComponent();
            Messenger.Default.Register<MessageToMotorControlManagementPanelControl>(this, MessageHandler);
        }

        private void MessageHandler(MessageToMotorControlManagementPanelControl obj)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Content = new NoDataControl_2();
            });
        }

        private MotorControlManagementPanelViewModel ViewModel
        {
            get => this.Resources[nameof(ViewModel)] as MotorControlManagementPanelViewModel;
        }
    }
}
