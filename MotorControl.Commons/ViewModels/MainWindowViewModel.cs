using GalaSoft.MvvmLight.Messaging;
using MotorControl.Commons.Models.Messages;
using MotorControl.Commons.Models.Messages.Tabs;
using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MotorControl.Commons.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        private TabsControl _control;
        public TabsControl Control
        {
            get => _control;
            set
            {
                _control = value;
                OnPropertyChanged(nameof(Control));
            }
        }
    }

    public partial class MainWindowViewModel
    {
        public override void OnLoaded(object sender, RoutedEventArgs args)
        {
            //SendDataToMotorInformation();
            base.OnLoaded(sender, args);
        }

        public void SendDataToMotorInformation()
        {
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                Messenger.Default.Send<MessageToMotorInformationControl>(new MessageToMotorInformationControl { Connected = false });
                Messenger.Default.Send<MessageToDigitalReadbacksControl>(new MessageToDigitalReadbacksControl { Connected = false });
                Messenger.Default.Send<MessageToMotorControlManagementPanelControl>(new MessageToMotorControlManagementPanelControl { Connected = false });

                Messenger.Default.Send<MessageToGraphicalReadbacksControl>(new MessageToGraphicalReadbacksControl { Connected = false });
                Messenger.Default.Send<MessageToMonitorWindowControl>(new MessageToMonitorWindowControl { Connected = false });
                Messenger.Default.Send<MessageToFirmwareUpdateControl>(new MessageToFirmwareUpdateControl { Connected = false });
                Messenger.Default.Send<MessageToEngineeringRestrictedControl>(new MessageToEngineeringRestrictedControl { Connected = false });

                Messenger.Default.Send<MessageToWarningControl>(new MessageToWarningControl { Connected = false });
                Messenger.Default.Send<MessageToParameterSettingsControl>(new MessageToParameterSettingsControl { Connected = false });
            });
            
        }
    }
}
