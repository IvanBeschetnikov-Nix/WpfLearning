using GalaSoft.MvvmLight.Messaging;
using MotorControl.Commons.Models.Messages;
using MotorControl.Commons.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorControl.Commons.ViewModels
{
    public class DigitalReadbacksViewModel : BaseViewModel
    {
        private string _dCVoltage;
        private string _current;
        private string _statorTemp;
        private string _efficency;
        private string _powerInput;
        private string _powerOutput;
        private string _torque;

        public string DCVoltage { get => _dCVoltage; set => SetPropertyValue(ref _dCVoltage, value, nameof(DCVoltage)); }
        public string Current { get => _current; set => SetPropertyValue(ref _current, value, nameof(Current)); }
        public string StatorTemp { get => _statorTemp; set => SetPropertyValue(ref _statorTemp, value, nameof(StatorTemp)); }
        public string Efficency { get => _efficency; set => SetPropertyValue(ref _efficency, value, nameof(Efficency)); }
        public string PowerInput { get => _powerInput; set => SetPropertyValue(ref _powerInput, value, nameof(PowerInput)); }
        public string PowerOutput { get => _powerOutput; set => SetPropertyValue(ref _powerOutput, value, nameof(PowerOutput)); }
        public string Torque { get => _torque; set => SetPropertyValue(ref _torque, value, nameof(Torque)); }

        /// <summary>
        /// Displays dynamic values recieved from the ModBus
        /// </summary>
        /// <param name="modBus"></param>
        public void FillDigitalOutput(ModBusWrapper modBus)
        {
            //TODO: toolStripStatusLabel1.Text = DateTime.Now.Ticks.ToString();

            // Speed control
            //TODO: changingValue = true;t

            // during development, Speed Setting may come back zero, which is invalid
            //TODO: numericSpeed.Value = Math.Max(numericSpeed.Minimum, modBus.MotorSpeedSetting);
            //TODO: changingValue = false;

            // update the Speed readback
            //TODO: textBoxSpeedReadback.Text = modBus.MotorSpeedReadback.ToString();

            // Digital Output
            DCVoltage = modBus.BusVoltage.ToString("f1");
            Current = modBus.AverageCurrent.ToString("f1");
            StatorTemp = modBus.Temperature2.ToString();

            //textBoxVibration.Text = modBus
            PowerInput = modBus.PowerInput.ToString();
            PowerOutput = modBus.PowerOutput.ToString();
            Torque = modBus.MotorTorque.ToString("f3");
            Efficency = modBus.MotorEfficiency.ToString();

            // status

            //TODO:
            //if (modBus.HasActiveFaults || modBus.HasActiveWarnings)
            //{
            //    labelStatus.Text = string.Format("Fault Bits: {0}, Warning Bits: {1}", modBus.FaultBits, modBus.WarningBits);
            //}
            //else
            //{
            //    labelStatus.Text = "No Faults or Warnings";
            //}
        }


    }
}
