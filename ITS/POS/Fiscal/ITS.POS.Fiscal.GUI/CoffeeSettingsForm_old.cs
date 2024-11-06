using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;
using ITS.Common.Communication;
using ITS.POS.Fiscal.Common;
using ITS.Common.Utilities.Forms;
using System.Threading;
using ITS.POS.Fiscal.GUI.Properties;
using System.Reflection;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;

namespace ITS.POS.Fiscal.GUI
{
    public partial class CoffeeSettingsForm : XtraForm
    {

        protected MessageClient Client { get; set; }
        protected FiscalServiceSettings Settings { get; set; }
        public const int ServiceTimeout = 10000;

        protected ServiceController Service
        {
            get
            {
                return ServiceController.GetServices().FirstOrDefault(sv => sv.ServiceName == Constants.ServiceName);
            }
        }


        protected ServiceControllerStatus ServiceRunning
        {
            get
            {
                if (Service != null)
                    return Service.Status;
                return ServiceControllerStatus.Stopped;
            }
        }

        public readonly string SettingsFilePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Service\\config.xml";


        public CoffeeSettingsForm()
        {
            InitializeComponent();
            //Read Settings
            if (File.Exists(SettingsFilePath))
            {
                Settings = ConfigurationHelper.LoadSettings<FiscalServiceSettings>(SettingsFilePath, false);
            }
            else
            {
                Settings = new FiscalServiceSettings();
            }
            lblAlgoSN.DataBindings.Add("Text", Settings, "SerialNumber");
            txtListeningPort.DataBindings.Add("EditValue", Settings, "Port");
            txtIPAddress.DataBindings.Add("EditValue", Settings, "Ethernet_IPAddress");
            txtComPort.DataBindings.Add("EditValue", Settings, "COM_PortName");
            radCOM.DataBindings.Add("Checked", Settings, "IsCOM");
            radEthernet.DataBindings.Add("Checked", Settings, "IsEthernet");
            chkBoxInError.DataBindings.Add("Checked", Settings, "FiscalOnError");
            txtAbcFolderPath.DataBindings.Add("EditValue", Settings, "AbcFolder");
            txtRbsRegistrationNumber.DataBindings.Add("EditValue", Settings, "RegistrationNumber");
            mnuOpen.Text = "Ελαχιστοποίηση";
            
            
            this.Text = Resources.FISCAL_SERVICE_NAME + " Configuration v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            this.taskBarIcon.Text = Resources.FISCAL_SERVICE_NAME;
            timer = new System.Threading.Timer(UpdateStatuses, null, 1000, 1000);
            switch(Settings.FiscalDevice)
            {
                case Retail.Platform.Enumerations.eFiscalDevice.ALGOBOX_NET:
                    comboBox1.SelectedIndex = 0;
                    break;
                case Retail.Platform.Enumerations.eFiscalDevice.RBS_NET:
                    comboBox1.SelectedIndex = 1;
                    break;
            }

            
        }
        System.Threading.Timer timer;
        protected void UpdateStatuses( object state)
        {
            this.Invoke((MethodInvoker)delegate
            {

                btnInstallService.Text = (Service == null) ? "Install Service" : "Uninstall Service";
                switch (ServiceRunning)
                {
                    case ServiceControllerStatus.ContinuePending:
                        btnStartService.Enabled = false;
                        lblServiceStatus.Text = "Pending";
                        break;
                    case ServiceControllerStatus.Paused:
                        btnStartService.Enabled = false;
                        lblServiceStatus.Text = "Paused";
                        break;
                    case ServiceControllerStatus.PausePending:
                        btnStartService.Enabled = false;
                        lblServiceStatus.Text = "Pause Pending";
                        break;
                    case ServiceControllerStatus.Running:
                        btnStartService.Enabled = true;
                        btnStartService.Text = "Stop Service";
                        lblServiceStatus.Text = "Running";
                        break;
                    case ServiceControllerStatus.StartPending:
                        btnStartService.Enabled = false;
                        lblServiceStatus.Text = "Start Pending";
                        break;
                    case ServiceControllerStatus.Stopped:
                        btnStartService.Enabled = Service != null;
                        btnStartService.Text = "Start Service";
                        lblServiceStatus.Text = "Stopped";
                        break;
                    case ServiceControllerStatus.StopPending:
                        btnStartService.Enabled = false;
                        lblServiceStatus.Text = "Stop Pending";
                        break;
                }

                if (Client != null && Client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Connected)
                {
                    picServiceStatus.Image = Resources.online_dot;
                    btnConnect.Text = "Disconnect from Service";
                }
                else
                {
                    picServiceStatus.Image = Resources.offline_dot;
                    taskBarIcon.Icon = Resources.disign_offline1;
                    btnConnect.Text = "Connect to Service";
                }

                if (Client == null)
                {
                    lblServiceConnected.Text = "Disconnected";
                }
                else
                {
                    lblServiceConnected.Text = Client.ConnectionStatus.ToString();
                }
            });
            if (Client != null && Client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Connected)
            {
                FiscalGetOnlineResponse response = Client.SendMessageAndWaitResponse<FiscalGetOnlineResponse>(new FiscalGetOnlineRequest(), 1000);
                if (response != null && response.Result == eFiscalResponseType.SUCCESS)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        picAlgoboxStatus.Image = Resources.online_dot;
                        lblFiscalStatus.Image = Resources.online_dot;
                        taskBarIcon.Icon = Resources.disign1;
                        if(response.ReloadSettings)
                        {
                            if (File.Exists(SettingsFilePath))
                            {
                                Settings = ConfigurationHelper.LoadSettings<FiscalServiceSettings>(SettingsFilePath, false);
                            }
                            else
                            {
                                Settings = new FiscalServiceSettings();
                            }
                            //Binding is not working for some reason...
                            chkBoxInError.Checked = Settings.FiscalOnError;
                        }
                    });
                    return;

                }
            }
            this.Invoke((MethodInvoker)delegate
            {

                picAlgoboxStatus.Image = Resources.offline_dot;
                lblFiscalStatus.Image = Resources.offline_dot;
                taskBarIcon.Icon = Resources.disign_offline1;
            });
            
        }

        private void radEthernet_CheckedChanged(object sender, EventArgs e)
        {
            tabCOM.PageVisible = radCOM.Checked;
            tabEthernet.PageVisible = radEthernet.Checked;
        }

        private void btnInstallService_Click(object sender, EventArgs e)
        {
            String DirectoryExe = Path.GetDirectoryName(Application.ExecutablePath) + "\\Service\\DiSign Service.exe";
            if (Service == null)
            {
                Process.Start(DirectoryExe, "-SERVICE");
            }
            else
            {
                if (Service.Status != ServiceControllerStatus.Stopped)
                {
                    Service.Stop();
                }
                Process.Start(DirectoryExe, "-NOSERVICE");
            }
        }
        

        private void btnStartService_Click(object sender, EventArgs e)
        {
            switch (ServiceRunning)
            {
                case ServiceControllerStatus.Running:
                    Service.Stop();
                    break;
                case ServiceControllerStatus.Stopped:
                    Service.Start();
                    break;
            }

        }

        private void btnGetVersion_Click(object sender, EventArgs e)
        {
            if (Client != null && Client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Connected)
            {
                FiscalGetVersionInfoResponse response = Client.SendMessageAndWaitResponse<FiscalGetVersionInfoResponse>(new FiscalGetVersionInfoRequest(), ServiceTimeout);
                if (response != null)
                {
                    string message = "";
                    if (response.Result == eFiscalResponseType.SUCCESS)
                    {
                        message = "VERSION INFO: " + response.ExResult;
                    }
                    else
                    {
                        message = "FAILURE: " + Environment.NewLine + "Error Code:" + response.ErrorCode + Environment.NewLine + "Result: " + response.ExResult;
                    }
                    MessageBox.Show(message);
                }
                else
                {
                    MessageBox.Show("Response Timeout..");
                }

            }
            else
            {
                MessageBox.Show("Not connected. Please connect first.");
            }
        }

        private void btnIssueZ_Click(object sender, EventArgs e)
        {
            if (Client != null && Client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Connected)
            {
                FiscalIssueZResponse response = Client.SendMessageAndWaitResponse<FiscalIssueZResponse>(new FiscalIssueZRequest(), ServiceTimeout);
                if (response != null)
                {
                    string message = "";
                    if (response.Result == eFiscalResponseType.SUCCESS)
                    {
                        message = "SUCCESS!";
                    }
                    else
                    {
                        message = "FAILURE: " + Environment.NewLine + "Error Code:" + response.ErrorCode + Environment.NewLine + "Result: " + response.ExResult;
                    }
                    MessageBox.Show(message);
                }
                else
                {
                    MessageBox.Show("Response Timeout..");
                }

            }
            else
            {
                MessageBox.Show("Not connected. Please connect first.");
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (Client == null)
            {
                Client = new MessageClient(Constants.ApplicationIdentifier, "localhost", Settings.Port);
            }

            if (Client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Disconnected || Client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.None)
            {
                Client.Connect();
            }
            else if (Client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Connected)
            {
                //MessageBox.Show("Already Connected");
                Client.Shutdown();
            }
            else
            {
                MessageBox.Show(Client.ConnectionStatus + ". Please try again.");
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            ConfigurationHelper.SaveSettingsFile(Settings, SettingsFilePath);
        }

        private void btnGetIDSN_Click(object sender, EventArgs e)
        {
            if (Client != null && Client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Connected)
            {
                FiscalGetSerialNumberResponse resp = this.Client.SendMessageAndWaitResponse<FiscalGetSerialNumberResponse>(new FiscalGetSerialNumberRequest(), 1000);
                if (resp != null && resp.Result == eFiscalResponseType.SUCCESS)
                {
                    MessageBox.Show("Serial Number:" + resp.SerialNumber, "Serial Number", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("An error occured", "Serial Number", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Not connected. Please connect first.");
            }
        }


        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm frm = new AboutForm())
            {
                frm.ShowDialog();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shouldTerminate = true;
            taskBarIcon.Visible = false;
            this.Close();
        }

        bool shouldTerminate=false;
        private void CoffeeSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !shouldTerminate)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                timer.Dispose();
            }
        }

        private void CoffeeSettingsForm_Shown(object sender, EventArgs e)
        {
            //this.ShowInTaskbar = true;
        }



        private void taskBarIcon_Click(object sender, EventArgs e)
        {

        }

        private void taskBarIcon_DoubleClick(object sender, EventArgs e)
        {
             MouseEventArgs me = e as MouseEventArgs;
             if (me != null && me.Button == System.Windows.Forms.MouseButtons.Left)
             {
                 mnuOpen_Click(sender, e);
             }
        } 

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            else
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void CoffeeSettingsForm_ClientSizeChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Minimized:
                    ShowInTaskbar = false;
                    mnuOpen.Text = "Εμφάνιση";
                    break;
                default:
                    ShowInTaskbar = true;
                    mnuOpen.Text = "Ελαχιστοποίηση";
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox1.SelectedIndex)
            {
                case 0:
                    Settings.FiscalDevice = Retail.Platform.Enumerations.eFiscalDevice.ALGOBOX_NET;
                    break;
                case 1:
                    Settings.FiscalDevice = Retail.Platform.Enumerations.eFiscalDevice.RBS_NET;
                    break;
            }
            tabRbsNet.PageVisible = Settings.FiscalDevice == Retail.Platform.Enumerations.eFiscalDevice.RBS_NET;
        }

        private void btnSearchFolder_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtAbcFolderPath.EditValue = this.folderBrowserDialog1.SelectedPath;
            }   
        }

        private void issueZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnIssueZ_Click(sender, e);
        }

        private void mnuExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Είστε σίγουροι ότι θέλετε να κλείσετε την εφαρμογή;","Ερώτηση", MessageBoxButtons.YesNo, MessageBoxIcon.Question)== System.Windows.Forms.DialogResult.Yes)
            {
                shouldTerminate = true;
                taskBarIcon.Visible = false;
                this.Close();
            }
        }

  
    }

}
