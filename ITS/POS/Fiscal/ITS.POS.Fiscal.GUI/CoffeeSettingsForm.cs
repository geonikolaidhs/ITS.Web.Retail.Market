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
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Fiscal.GUI
{
    public partial class CoffeeSettingsForm : XtraForm
    {

        protected MessageClient Client { get; set; }
        protected FiscalServiceSettings Settings { get; set; }
        public const int ServiceTimeout = ITS.POS.Hardware.Common.DiSign.TIMEOUT;//10000;

        private ServiceController _Service;
        protected ServiceController Service
        {
            get
            {
                if (_Service == null)
                {
                    _Service = ServiceController.GetServices().FirstOrDefault(sv => sv.ServiceName == Constants.ServiceName);
                }
                else
                {

                    _Service.Refresh();
                }
                return _Service;
            }
        }


        protected ServiceControllerStatus ServiceRunning
        {
            get
            {
                try
                {
                    if (Service != null)
                    {
                        return Service.Status;
                    }
                    return ServiceControllerStatus.Stopped;
                }
                catch
                {
                    _Service = null;
                    return ServiceControllerStatus.Stopped;
                }

            }
        }

        public readonly string SettingsFilePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\Service\\config.xml";


        public CoffeeSettingsForm()
        {
            InitializeComponent();
            tabSettings.PageVisible = true;
        }

        System.Threading.Timer timer;

        protected void UpdateStatuses(object state)
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
                DisposeIfPossible(picServiceStatus.Image);
                DisposeIfPossible(taskBarIcon.Icon);
                if (Client != null)
                {
                    picServiceStatus.Image = Resources.online_dot;
                    taskBarIcon.Icon = Resources.disign1;
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
                    lblServiceConnected.Text = "Connected";
                }
            });

            DisposeIfPossible(picAlgoboxStatus.Image);
            DisposeIfPossible(lblFiscalStatus.Image);
            DisposeIfPossible(taskBarIcon.Icon);


            picAlgoboxStatus.Image = Resources.online_dot;
            lblFiscalStatus.Image = Resources.online_dot;
            //taskBarIcon.Icon = Resources.disign1;


            if (Client != null)
            {
                try
                {
                    FiscalGetOnlineResponse response = Client.SendMessageAndWaitResponse<FiscalGetOnlineResponse>(new FiscalGetOnlineRequest(), 3000);
                    if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage) && response.Result == eFiscalResponseType.SUCCESS)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            DisposeIfPossible(picAlgoboxStatus.Image);
                            DisposeIfPossible(lblFiscalStatus.Image);
                            DisposeIfPossible(taskBarIcon.Icon);
                            picAlgoboxStatus.Image = Resources.online_dot;
                            lblFiscalStatus.Image = Resources.online_dot;
                            taskBarIcon.Icon = Resources.disign1;
                            if (response.ReloadSettings)
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
                catch
                {
                    ////communication error
                }
            }
            this.Invoke((MethodInvoker)delegate
            {
                DisposeIfPossible(picAlgoboxStatus.Image);
                DisposeIfPossible(lblFiscalStatus.Image);
                DisposeIfPossible(taskBarIcon.Icon);
                picAlgoboxStatus.Image = Resources.offline_dot;
                lblFiscalStatus.Image = Resources.offline_dot;
                taskBarIcon.Icon = Resources.disign_offline1;
            });

        }

        private void radEthernet_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radCOM.Checked)
                {
                    txtComPort.Enabled = true;
                    txtIPAddress.Text = string.Empty;
                    txtIPAddress.Enabled = false;
                }
                else
                {
                    txtComPort.Text = string.Empty;
                    txtComPort.Enabled = false;
                    txtIPAddress.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
            }
        }

        private void btnInstallService_Click(object sender, EventArgs e)
        {
            try
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
                    this.Client = null;
                }
                if (_Service != null)
                {
                    _Service.Dispose();
                    _Service = null;
                }
            }
            catch
            {

            }
        }


        private void btnStartService_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
            }

        }

        private void btnGetVersion_Click(object sender, EventArgs e)
        {
            if (Client != null)
            {
                FiscalGetVersionInfoResponse response = Client.SendMessageAndWaitResponse<FiscalGetVersionInfoResponse>(new FiscalGetVersionInfoRequest(), ServiceTimeout);
                if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage))
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
                else if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage) == false)
                {
                    MessageBox.Show(response.ErrorMessage);
                }
                else
                {
                    MessageBox.Show("Response Timeout.");
                }

            }
            else
            {
                MessageBox.Show("Not connected. Please connect first.");
            }
        }

        private void btnIssueZ_Click(object sender, EventArgs e)
        {
            if (Client != null)
            {
                FiscalIssueZResponse response = Client.SendMessageAndWaitResponse<FiscalIssueZResponse>(new FiscalIssueZRequest(), ServiceTimeout);
                if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage))
                {
                    string message = "";
                    if (response.Result == eFiscalResponseType.SUCCESS)
                    {
                        message = "SUCCESS!" + " " + response.UploadZErrorMessage;
                    }
                    else
                    {
                        message = "FAILURE: " + Environment.NewLine + "Error Code:" + response.ErrorCode + Environment.NewLine + "Result: " + response.ExResult;
                    }
                    MessageBox.Show(message);
                }
                else if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage) == false)
                {
                    MessageBox.Show(response.ErrorMessage);
                }
                else
                {
                    MessageBox.Show("Response Timeout.");
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
                Client = new MessageClient("127.0.0.1", Settings.Port, Program.Logger);
            }
            else
            {
                Client.Dispose();
                Client = null;
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            string ABCFolderPath = txtAbcFolderPath.Text;
            if (String.IsNullOrWhiteSpace(ABCFolderPath) || String.IsNullOrEmpty(ABCFolderPath))
            {
                MessageBox.Show("Δεν έχει οριστεί φάκελος αποθήκευσης υπογραφών ΕΑΦΔΣΣ");
                return;
            }
            else
            {
                if (!Directory.Exists(ABCFolderPath))
                {
                    DialogResult answer = MessageBox.Show("Ο φάκελος προορισμού που ορίσατε δεν υπάρχει. Θέλετε να δημιουργηθεί αυτός ο φάκελος;", "", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                    if (answer == DialogResult.Yes)
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(ABCFolderPath);
                            Settings.AbcFolder = ABCFolderPath;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ο φάκελος προορισμού δεν υποστηρίζεται \n" + ex.Message, "Σφάλμα Αποθήκευσης", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Settings.AbcFolder = "";
                        }
                        //ConfigurationHelper.SaveSettingsFile(Settings, SettingsFilePath);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            Settings.DeviceName = Settings.FiscalDevice.ToString();
            ConfigurationHelper.SaveSettingsFile(Settings, SettingsFilePath);
        }

        private void btnGetIDSN_Click(object sender, EventArgs e)
        {
            if (Client != null)
            {
                FiscalGetSerialNumberResponse response = this.Client.SendMessageAndWaitResponse<FiscalGetSerialNumberResponse>(new FiscalGetSerialNumberRequest(), 3000);
                if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage) && response.Result == eFiscalResponseType.SUCCESS)
                {
                    MessageBox.Show("Serial Number:" + response.SerialNumber, "Serial Number", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage) == false)
                {
                    MessageBox.Show(response.ErrorMessage);
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

        bool shouldTerminate = false;
        private void CoffeeSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !shouldTerminate)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                if (timer != null)
                {
                    timer.Dispose();
                }

            }
        }

        private void CoffeeSettingsForm_Shown(object sender, EventArgs e)
        {
            //this.ShowInTaskbar = true;
            //this.comboBox1.SelectedIndex = 1;
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
            try
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        Settings.FiscalDevice = Retail.Platform.Enumerations.eFiscalDevice.ALGOBOX_NET;
                        txtRbsRegistrationNumber.Text = string.Empty;
                        txtRbsRegistrationNumber.Enabled = false;
                        break;
                    case 1:
                        Settings.FiscalDevice = Retail.Platform.Enumerations.eFiscalDevice.RBS_NET;
                        txtRbsRegistrationNumber.Enabled = true;
                        break;
                    case 2:
                        Settings.FiscalDevice = Retail.Platform.Enumerations.eFiscalDevice.ALGOBOX_NETV2;
                        txtRbsRegistrationNumber.Text = string.Empty;
                        txtRbsRegistrationNumber.Enabled = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
            }
        }


        private void issueZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnIssueZ_Click(sender, e);
        }

        private void mnuExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Είστε σίγουροι ότι θέλετε να κλείσετε την εφαρμογή;", "Ερώτηση", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                shouldTerminate = true;
                taskBarIcon.Visible = false;
                this.Close();
            }
        }

        private void btnSendOnlineCheck_Click(object sender, EventArgs e)
        {
            if (Client != null)
            {
                FiscalGetOnlineResponse response = Client.SendMessageAndWaitResponse<FiscalGetOnlineResponse>(new FiscalGetOnlineRequest(), 3000);
                if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage) && response.Result == eFiscalResponseType.SUCCESS)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        picAlgoboxStatus.Image = Resources.online_dot;
                        lblFiscalStatus.Image = Resources.online_dot;
                        taskBarIcon.Icon = Resources.disign1;
                        if (response.ReloadSettings)
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
                else if (response != null && String.IsNullOrWhiteSpace(response.ErrorMessage) == false)
                {
                    MessageBox.Show(response.ErrorMessage);
                }
            }
        }


        private void DisposeIfPossible(IDisposable obj)
        {
            if (obj != null)
            {
                obj.Dispose();
            }
        }
        private void DisposeIfPossible(ref IDisposable obj)
        {
            if (obj != null)
            {
                obj.Dispose();
            }
            obj = null;
        }

        private void CoffeeSettingsForm_Load(object sender, EventArgs e)
        {
            //Read Settings
            if (File.Exists(SettingsFilePath))
            {
                Settings = ConfigurationHelper.LoadSettings<FiscalServiceSettings>(SettingsFilePath, false);
            }
            else
            {
                Settings = new FiscalServiceSettings();
            }
            try
            {
                lblAlgoSN.DataBindings.Add("Text", Settings, "SerialNumber");
                txtListeningPort.DataBindings.Add("EditValue", Settings, "Port");
                txtIPAddress.DataBindings.Add("EditValue", Settings, "Ethernet_IPAddress");
                txtComPort.DataBindings.Add("EditValue", Settings, "COM_PortName");
                txtAesKey.DataBindings.Add("EditValue", Settings, "AesKey");
                txtEafdssSN.DataBindings.Add("EditValue", Settings, "EafdssSN");

                txtSmtpServer.DataBindings.Add("EditValue", Settings, "SmtpServer");
                txtSmtpPort.DataBindings.Add("EditValue", Settings, "SmtpPort");
                txtSmtpUser.DataBindings.Add("EditValue", Settings, "SmtpUser");
                txtSmtpPass.DataBindings.Add("EditValue", Settings, "SmtpPass");

                checkMailFail.DataBindings.Add("Checked", Settings, "SendMailOnUploadFail");
                checkSendSuccessMail.DataBindings.Add("Checked", Settings, "SendMailOnUploadSuccess");

                txtMailList.DataBindings.Add("EditValue", Settings, "MailList");

                txtGGPSUrl.DataBindings.Add("EditValue", Settings, "GgpsUrl");
                checkBoχSendFiles.DataBindings.Add("Checked", Settings, "SendFiles");
                radCOM.DataBindings.Add("Checked", Settings, "IsCOM");
                radEthernet.DataBindings.Add("Checked", Settings, "IsEthernet");
                chkBoxInError.DataBindings.Add("Checked", Settings, "FiscalOnError");
                txtAbcFolderPath.DataBindings.Add("EditValue", Settings, "AbcFolder");
                txtAbcFolderPath.Text = Settings.AbcFolder;
                txtRbsRegistrationNumber.DataBindings.Add("EditValue", Settings, "RegistrationNumber");
                checkBoxKeepLog.DataBindings.Add("Checked", Settings, "KeepLog");
                mnuOpen.Text = "Ελαχιστοποίηση";
                tabGGPSSettings.PageVisible = checkBoχSendFiles.Checked;
                tabSettings.PageVisible = true;
                tabSettings.PageEnabled = true;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }


            this.Text = Resources.FISCAL_SERVICE_NAME + " Configuration v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            this.taskBarIcon.Text = Resources.FISCAL_SERVICE_NAME;
            timer = new System.Threading.Timer(UpdateStatuses, null, 1000, 1000);
            switch (Settings.FiscalDevice)
            {
                case Retail.Platform.Enumerations.eFiscalDevice.ALGOBOX_NET:
                    comboBox1.SelectedIndex = 0;
                    break;
                case Retail.Platform.Enumerations.eFiscalDevice.RBS_NET:
                    comboBox1.SelectedIndex = 1;
                    break;
                case Retail.Platform.Enumerations.eFiscalDevice.ALGOBOX_NETV2:
                    comboBox1.SelectedIndex = 2;
                    break;
            }



#if DEBUG
            grbDebug.Visible = true;
#endif
        }

        private void simpleButtonViewLog_Click(object sender, EventArgs e)
        {
            if (Settings.KeepLog == false)
            {
                MessageBox.Show("Η δυνατότητα καταγραφής Request / Response δεν έχει ενεργοποιηθεί.Παρακαλώ ελέξτε τη ρύθμιση 'Καταγραφή Log'",
                                "H Καταγραφή Log δεν έχει ενεργοποιηθεί",
                                MessageBoxButtons.OK
                               );
                return;
            }

            using (RequestResponseLogForm requestResponseLogForm = new RequestResponseLogForm())
            {
                requestResponseLogForm.ShowDialog();
            }
        }

        private void btnSearchFolder_Click_1(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtAbcFolderPath.EditValue = this.folderBrowserDialog1.SelectedPath;
                Settings.AbcFolder = txtAbcFolderPath.Text;
            }
        }

        private void checkBoχSendFiles_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                tabGGPSSettings.PageVisible = checkBoχSendFiles.Checked;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
            }
        }
    }

}
