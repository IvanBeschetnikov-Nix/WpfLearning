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
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MotorControl.Commons.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using MotorControl.Commons.Models.Messages;

namespace MotorControl.Commons.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(TabsControl tabsControl)
        {
            InitializeComponent();

            Messenger.Default.Register<MessageToMainWindow_MotorDirectionChanged>(this, MotorDirrecitonChanged);

            Loaded += ViewModel.OnLoaded;
            Unloaded += ViewModel.OnUnloaded;
            Closing += ViewModel.OnClosing;
            this.ViewModel.Control = tabsControl;
        }

        private void MotorDirrecitonChanged(MessageToMainWindow_MotorDirectionChanged message)
        {
            this.ViewModel.DirectionAllowed = message.DirectionAllowed;
        }

        private MainWindowViewModel ViewModel
        {
            get => this.Resources["ViewModel"] as MainWindowViewModel;
        }
    }
}