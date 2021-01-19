using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Configuration;
using MotorControl.Commons.Enums;
using MotorControl.Commons.Models.Messages;
using MotorControl.Commons.Models.Messages.Tabs;
using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MotorControl.Commons.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModel
    {
        private TabsControl _control;
        private RotationAllowed _rotationAllowed;


        public TabsControl Control
        {
            get => _control;
            set
            {
                _control = value;
                OnPropertyChanged(nameof(Control));
            }
        }

        public RotationAllowed DirectionAllowed
        {
            get => _rotationAllowed;
            set 
            {
                FillMotorInformation(value);
                _rotationAllowed = value;
            }
        }

    }

    public partial class MainWindowViewModel
    {
        private readonly System.Timers.Timer _timer;
        private readonly IConfiguration _configuration;
        private ModBusWrapper _modBusWrapper;

        public MainWindowViewModel()
        {
            this._configuration = ServiceCollectionKeeper.ServiceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            this._timer = new System.Timers.Timer(int.Parse(_configuration["TimerIntervalMS"]));
            
        }

        public override void OnLoaded(object sender, RoutedEventArgs args)
        {
            this.SetupModBus();
            this.FillMotorInformation();
            this.SetupTimer();
            //TODO: this.SetNotConnectedState();
            base.OnLoaded(sender, args);
        }

        public override void OnClosing(object sender, CancelEventArgs e)
        {
            
            if (MessageBox.Show("Connection is still active.\nClose connection and exit ? ", "WARNING", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //TODO:
            }
            else
            {
                e.Cancel = true;
            }

            base.OnClosing(sender, e);
        }

        private void SetupModBus()
        {
            this._modBusWrapper = new ModBusWrapper();
            this._modBusWrapper.OnConnected += ModBus_OnConnected;
            this._modBusWrapper.OnMotorRunning += ModBus_OnMotorRunning;
            this._modBusWrapper.OnMotorSpeedChanged += ModBus_OnMotorSpeedChanged;
            this._modBusWrapper.OnMotorDirectionChanged += ModBus_OnMotorDirectionChanged;
        }

        private void SetupTimer()
        {
            this._timer.AutoReset = false;
            this._timer.Elapsed += Timer_Elapsed;
            this._timer.Start();
        }

        private void ModBus_OnMotorDirectionChanged(Rotation motorDirection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// No need to update motor speed readback because timer do it when elapsed
        /// </summary>
        /// <param name="motorSpeed"></param>
        private void ModBus_OnMotorSpeedChanged(int motorSpeed)
        {
        }

        private void ModBus_OnMotorRunning(bool motorRunning)
        {
            // can only change direction while motor is not running
            // AND current Parameters allow Both directions (zero)
            this.FillMotorInformation(this.DirectionAllowed);
            // additionally, operating Parameters cannot be changed while running
            this.UpdateParamterSettingsControl(motorRunning);
        }

        /// <summary>
        /// Respond to connect events from the ModBus
        /// </summary>
        /// <param name="connected"></param>
        private void ModBus_OnConnected(bool connected)
        {
            //this.UpdateParamterSettingsControl(connected);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool connected = true;//TODO: Connected;
            try
            {
                // disable prior to call
                this._timer.Enabled = false;
                this._modBusWrapper.SimulatePropsRandomUpdating();
                // if we have a valid connection, do the readings
                if (connected)
                {
                    //TODO: 
                    //if (_modBusWrapper.ReadAllRegisters() == ModbusStatuses.STATUS_READ_FAILED)
                    //{
                    //    throw new Exception("Disconnecting from ModBus");
                    //}
                    //else
                    {
                        // display digital output
                        FillDigitalOutput();
                        // fill in chart
                        //TODO: PlotToChart();
                        // fill Monitor window
                        FillMonitorWindow();
                    }
                    // logging?
                    //TODO: if (_logging) WriteLogEntry();
                }
                else
                {
                    //toolStripStatusLabel1.Text = "...ModBus unavailable";
                    throw new Exception("Lost connection to ModBus");
                }
            }
            catch (Exception ex)
            {
                string error = string.Empty;

                if (_modBusWrapper != null)
                {
                    error = _modBusWrapper.LastError;
                    _modBusWrapper.Disconnect(true);
                }
                // display error
                //TODO: DisplayMessageBox($"Lost connection or internal error.\n\nModBus error: {error}", ex.Message);
            }
            finally
            {
                // re-enable timer ?
                this._timer.Enabled = connected;
            }
        }

        private void FillMotorInformation(RotationAllowed rotationAllowed = RotationAllowed.Both)
        {
            Messenger.Default.Send<MessageToMotorControlManagementPanelControl>(new MessageToMotorControlManagementPanelControl
            {
                Connected = true,
                ModBus = this._modBusWrapper,
                DirectionAllowed = rotationAllowed
            });
        }

        private void FillDigitalOutput()
        {
            Messenger.Default.Send<MessageToDigitalReadbacksControl>(new MessageToDigitalReadbacksControl
            {
                Connected = true,
                ModBus = this._modBusWrapper
            });
        }

        private void FillMonitorWindow()
        {
            Messenger.Default.Send<MessageToMonitorWindowControl>(new MessageToMonitorWindowControl
            {
                Connected = true,
                ModBus = this._modBusWrapper
            });
        }

        private void UpdateParamterSettingsControl(bool motorRunning)
        {
            Messenger.Default.Send<MessageToParameterSettingsControl>(new MessageToParameterSettingsControl
            {
                Connected = true,
                ModBus = this._modBusWrapper,
                MotorRunning = motorRunning
            }); 
        }

        /// <summary>
        /// Sends notifiication to all user controls, to set Not Connected state
        /// </summary>
        public void SetNotConnectedState()
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
