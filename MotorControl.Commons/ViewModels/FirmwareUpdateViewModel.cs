using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotorControl.Commons.Commanding.RelayCommand;
using MotorControl.Commons.Enums;
using MotorControl.Commons.Shared;
using MotorControl.Commons.Structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MotorControl.Commons.ViewModels
{
    public class FirmwareUpdateViewModel : BaseViewModel
    {
        private readonly string _maCAddress;
        private readonly string _ipAddress;

        private TcpClient _client = null;
        private BackgroundWorker _worker = null;
        private string _kernelFile = "";
        private string _applicationFile = "";
        private string _tivaFile = "";

        private const int TCPPORT = 1000;

        private const byte BYTE_HEADER_KERN = 1;
        private const byte BYTE_HEADER_APP = 2;
        private const byte BYTE_HEADER_TIVA_BOOT = 3;
        private const byte BYTE_HEADER_DATA_NORM = 0;
        private const byte BYTE_HEADER_DATA_START = 1;
        private const byte BYTE_HEADER_DATA_STOP = 2;

        private int sizePacket = 150;
        private int sizeHeader = 2;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void ProgressCallback(int percent);
        delegate void MessageCallback(string msg);

        [DllImport("eFlashLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int eFlash(string strMacAddr, string strIpAddr, string strFileName, ProgressCallback progressCallback, MessageCallback messageCallback);

        private bool Connected
        {
            get { return _client != null && _client.Connected; }
        }

        public RelayCommand ConnectCommand;

        public FirmwareUpdateViewModel()
        {
            var configuration = 
                ServiceCollectionKeeper
                .ServiceProvider
                .GetRequiredService<IConfiguration>();

            this.SetupBackgroundWorker();

            this._maCAddress = configuration[ConfigKeys.MACAddress];
            this._ipAddress = configuration[ConfigKeys.IPAddress];

            this.ConnectCommand = new RelayCommand(ConnectCommandHandler);
        }

        public override void OnLoaded(object sender, RoutedEventArgs args)
        {
            this.OnConnectedChanged(false);

            base.OnLoaded(sender, args);
        }

        public override void OnUnloaded(object sender, RoutedEventArgs args)
        {
            // watch for active connection

            if (Connected)
            {
                // see what user wants to do
                if (MessageBox.Show("Connection is still active.\nClose connection and exit?", "WARNING",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ConnectCommandHandler();
                }
                else
                {
                    args.Handled = true;
                }
            }

            base.OnUnloaded(sender, args);
        }


        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ReprogTaskArgs args = (ReprogTaskArgs)e.Argument;
            NetworkStream stream;
            int numBytesFile;
            int numBytesResponse;
            int i;
            int sizeDataActual;
            byte[] bytesResponse = new byte[32];
            byte[] bytesPacket = new byte[sizePacket];
            byte[] bytesFile;
            int sizeData = sizePacket - sizeHeader;
            string strResp;

            string filename = args.filename;
            ReprogTask task = args.task;

            var bw = sender as BackgroundWorker;

            try
            {
                // special handling for Tiva
                if (task == ReprogTask.Tiva)
                {
                    // step 1: set up boot loader
                    byte[] bytes = new byte[1];

                    bytes[0] = BYTE_HEADER_TIVA_BOOT;

                    stream = _client.GetStream();
                    stream.Write(bytes, 0, 1);
                    //no response from bootloader
                    //TODO: make boot call async in Tiva so we get a response?

                    // we have to disconnect prior to using eFlash
                    _client.Close();
                    _client = null;

                    bool success = (eFlash(_maCAddress, _ipAddress, _tivaFile, progressCallback, messageCallback) == 0);
                    return;
                }
                else
                {
                    switch (task)
                    {
                        case ReprogTask.App:
                            bytesPacket[0] = BYTE_HEADER_APP;
                            break;
                        case ReprogTask.Kern:
                            bytesPacket[0] = BYTE_HEADER_KERN;
                            break;
                        default:
                            return;
                    }

                    bytesFile = File.ReadAllBytes(filename);
                    numBytesFile = bytesFile.Length;
                    //TODO: AddLogEntry(string.Format("File length = {0} bytes", numBytesFile));

                    // TCP stream
                    stream = _client.GetStream();

                    for (i = 0; i < numBytesFile; i += sizeData)
                    {
                        //TODO: AddLogEntry(string.Format("...sending packet {0}", i));
                        //support single write packet that includes start and stop
                        bytesPacket[1] = BYTE_HEADER_DATA_NORM;

                        if (i == 0)
                            bytesPacket[1] |= BYTE_HEADER_DATA_START;

                        if (numBytesFile < (i + sizeData)) //last packet
                        {
                            bytesPacket[1] |= BYTE_HEADER_DATA_STOP;
                            sizeDataActual = numBytesFile - i;
                        }
                        else
                            sizeDataActual = sizeData;

                        System.Buffer.BlockCopy(bytesFile, i, bytesPacket, sizeHeader, sizeDataActual);
                        stream.Write(bytesPacket, 0, sizeDataActual + sizeHeader);
                        try
                        {
                            //TODO: AddLogEntry(string.Format("...reading response"));
                            numBytesResponse = stream.Read(bytesResponse, 0, bytesResponse.Length);
                            //TODO: AddLogEntry(string.Format("...response bytes = {0}", numBytesResponse));
                        }
                        catch
                        {
                            //TODO: AddLogEntry("ERROR: No response from TCP server");
                            return;
                        }
                        strResp = System.Text.Encoding.ASCII.GetString(bytesResponse, 0, numBytesResponse);
                        if (strResp == "SUCCESS")
                            _worker.ReportProgress(i / (numBytesFile / 100));
                        else
                        {
                            //TODO: AddLogEntry("ERROR: Load failed:\n" + strResp);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: AddLogEntry("ERROR: " + ex.Message);
            }
        }

        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressCallback(e.ProgressPercentage);
            //progressBar1.Value = e.ProgressPercentage;
            //Application.DoEvents();
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // re-enable button
            //buttonUpload.Enabled = true;
            //TODO: Application.DoEvents();
        }

        private void progressCallback(int percent)
        {
            //TODO:
        }

        private void messageCallback(string message)
        {
            //TODO:
        }

        private void SetupBackgroundWorker()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += _worker_DoWork;
            _worker.ProgressChanged += _worker_ProgressChanged;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        }

        /// <summary>
        /// Called when TCP connection changes state
        /// </summary>
        /// <param name="connected"></param>
        private void OnConnectedChanged(bool connected)
        {
            //TODO: 
            //// update the display accordingly
            //groupBoxConnection.Text = string.Format("Connection [{0}]", Connected ? "Connected" : "Disconnected");
            //buttonConnect.Text = (Connected ? "Disconnect" : "Connect");
            //buttonConnect.BackColor = (Connected ? Color.LightSalmon : Color.PaleGreen);
            ////
            ////groupBoxUploads.Enabled = Connected;
            //buttonUpload.Enabled = Connected;
        }

        private void ConnectCommandHandler()
        {
            try
            {
                // if not already extant
                _client ??= new TcpClient();

                if (!Connected)
                {
                    IPAddress ip;
                    if (true)//TODO: (Protocol)comboBoxProtocol.SelectedIndex == Protocol.TCP)
                    {
                        try
                        {
                            ip = IPAddress.Parse(this._ipAddress);
                        }
                        catch
                        {
                            DisplayMessageBox("Invalid IP address", "Error");
                            return;
                        }

                        // try to connect
                        _client.ReceiveTimeout = 10000; // 10 sec timeout
                        try
                        {
                            _client.Connect(new IPEndPoint(ip, TCPPORT));
                        }
                        catch
                        {
                            DisplayMessageBox(string.Format("TCP unable to connect to IP {0}, Port {1}", this._ipAddress, TCPPORT), "Error");
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (_worker.IsBusy)
                        {
                            _worker.CancelAsync();
                            // reset progress
                            //progressBar1.Value = 0;
                            progressCallback(0);
                        }
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        _client.Close();
                        _client = null;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessageBox(ex.Message, "Error setting up TCP Client");
            }
            // raise pseudo-event
            OnConnectedChanged(Connected);

            //// for DEMO
            //AddLogEntry("Uploading Tiva file");
            //progressBar1.Value = 30;
        }

        /// <summary>
        /// Helper to display user message box on UI thread
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="caption"></param>
        private void DisplayMessageBox(string msg, string caption)
        {
            MessageBox.Show(msg, caption);
        }

        /// <summary>
        /// Add log message to status
        /// </summary>
        /// <param name="msg"></param>
        private void AddLogEntry(string msg)
        {
            // add item and make sure it's visible

            //TODO: 
            //listViewStatus.Items.Add(msg).EnsureVisible();
            //Application.DoEvents();
        }
    }
}
