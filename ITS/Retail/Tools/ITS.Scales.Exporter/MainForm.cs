using ITS.Scales.Exporter.Classes;
using ITS.Scales.Exporter.Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ITS.Scales.Exporter
{
    public partial class MainForm : Form
    {
        private LogHelper logHelper;
        private ScalesExporterSettings scalesExporterSettings;
        private Thread webserviceCallThread;

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            richTextBoxLog.Clear();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            //string logFolder = Application.StartupPath + ScalesExporterConstants.LOG_FOLDER;
            //if (Directory.Exists(logFolder) == false)
            //{
            //    Directory.CreateDirectory(logFolder);
            //}

            string logFile = GetLogFileName();
            //if (File.Exists(logFile) == false)
            //{
            //    using (File.Create(logFile))
            //    {

            //    }
            //}

            logHelper = new LogHelper(richTextBoxLog, logFile);

            this.scalesExporterSettings = ReadSettings();

            buttonStartStop.Text = String.Empty;
            buttonStartStop.Image = GetStartImage();

            webserviceCallThread = new Thread(CallWebService);

            textBoxServiceURL.Text = this.scalesExporterSettings.ServiceURL;
            numericUpDownRepeatTimeInMinutes.Value = this.scalesExporterSettings.RepeatTimeInMinutes;
            numericUpDownwebServiceCallTimeOut.Value = this.scalesExporterSettings.WebServiceCallTimeOut;
            checkBoxAutoRun.Checked = this.scalesExporterSettings.Autorun;
            if (this.scalesExporterSettings.Autorun)
            {
                buttonStartStop_Click(buttonStartStop,new EventArgs());
            }
        }

        private void CallWebService()
        {
            while (true)
            {
                try
                {
                    string message = String.Empty;
                    using (ScalesService.ScalesServiceClient scalesService = GetNewWebService()) //new ScalesService.ScalesServiceClient())
                    {
                        scalesService.Open();
                        message = scalesService.ExportChanges();
                        scalesService.Close();
                    }
                    logHelper.Success(message);
                    SetTrayIconMessage(message);
                }
                catch (ThreadAbortException threadAbortException)
                {
                    logHelper.Warning(threadAbortException.Message);
                }
                catch (Exception exception)
                {
                    logHelper.Error(exception.Message);
                }
                SetTrayIconMessage("Running.New command will be send in a while");
                Thread.Sleep(this.scalesExporterSettings.RepeatTimeInMinutes * 60000);
            }
        }

        private void SetTrayIconMessage(string message)
        {
            notifyIcon.Text = notifyIcon.BalloonTipText = message;
        }

        private ScalesService.ScalesServiceClient GetNewWebService()
        {
            string remoteURL = GetRemoteServiceURL();
            ScalesService.ScalesServiceClient scalesService = new ScalesService.ScalesServiceClient();
            scalesService.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(remoteURL), scalesService.Endpoint.Address.Identity, scalesService.Endpoint.Address.Headers);
            scalesService.Endpoint.Binding.OpenTimeout = TimeSpan.FromMilliseconds(this.scalesExporterSettings.WebServiceCallTimeOut);
            return scalesService;
        }

        private string GetRemoteServiceURL()
        {
            string remoteURL = this.scalesExporterSettings.ServiceURL;
            if (remoteURL.EndsWith("/") == false)
            {
                remoteURL += "/";
            }
            remoteURL += ScalesExporterConstants.SERVICE_NAME;
            return remoteURL;
        }

        private string GetLogFileName()
        {
            DateTime now = DateTime.Now;
            string timeStamp = String.Format("{0}_{1}_{2}_{3}_{4}_{5}_", now.Year,now.Month,now.Day,now.Hour,now.Minute,now.Second);
            return String.Format("{0}{1}{2}{3}", Application.StartupPath, ScalesExporterConstants.LOG_FOLDER,timeStamp,ScalesExporterConstants.LOG_FILE);
        }

        private Image GetStartImage()
        {
            return System.Drawing.Image.FromFile( String.Format("{0}{1}", GetImageFolder() , "start.png") );
        }

        private Image GetStopImage()
        {
            return System.Drawing.Image.FromFile(String.Format("{0}{1}", GetImageFolder(), "stop.png"));
        }

        private string GetImageFolder()
        {
            return Application.StartupPath+ "\\Images\\";
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            SaveSettingsFromUI();
        }

        private void SaveSettingsFromUI()
        {
            if (String.IsNullOrEmpty(this.textBoxServiceURL.Text))
            {
                logHelper.Warning("Please fill in Service URL");
                return;
            }
            this.scalesExporterSettings.ServiceURL = this.textBoxServiceURL.Text;
            this.scalesExporterSettings.RepeatTimeInMinutes = (int)this.numericUpDownRepeatTimeInMinutes.Value;
            this.scalesExporterSettings.WebServiceCallTimeOut = (int)this.numericUpDownwebServiceCallTimeOut.Value;
            this.scalesExporterSettings.Autorun = checkBoxAutoRun.Checked;

            SaveSettings(this.scalesExporterSettings);
        }

        private ScalesExporterSettings ReadSettings()
        {
            try
            {
                if ( File.Exists(ScalesExporterConstants.CONFIG_FILE) == false)
                {
                    logHelper.Message("Please fill in settings.");
                    return new ScalesExporterSettings()
                    {
                        ServiceURL = String.Empty,
                        RepeatTimeInMinutes = 1,
                        WebServiceCallTimeOut = 50000,
                        Autorun = false
                    };
                }

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScalesExporterSettings));

                using (FileStream fileStream = new FileStream(ScalesExporterConstants.CONFIG_FILE, FileMode.Open, FileAccess.Read))
                {
                    using (TextReader textReader = new StreamReader(fileStream))
                    {
                        return xmlSerializer.Deserialize(textReader) as ScalesExporterSettings;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SaveSettings(ScalesExporterSettings scalesExporterSettings)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScalesExporterSettings));

            using (FileStream fs = new FileStream(ScalesExporterConstants.CONFIG_FILE, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fs))
                {
                    xmlSerializer.Serialize(textWriter, scalesExporterSettings);
                }
            }

            logHelper.Success("Settings saved succesfully");
            this.scalesExporterSettings = ReadSettings();
        }

        private void buttonStartStop_Click(object sender, EventArgs e)
        {
            SaveSettingsFromUI();

            string message = String.Empty;

            switch ( webserviceCallThread.ThreadState )
            {
                case ThreadState.Aborted:
                    webserviceCallThread = new Thread(CallWebService);
                    webserviceCallThread.Start();
                    buttonStartStop.Image = GetStopImage();
                    message = "Web service scheduled calls restarted";
                    logHelper.Success(message);
                    SetTrayIconMessage(message);
                    break;
                case ThreadState.AbortRequested:
                    break;
                case ThreadState.Background:
                    break;
                case ThreadState.Running:
                    //webserviceCallThread.Abort();
                    //buttonStartStop.Image = GetStartImage();
                    //logHelper.Success("Web service scheduled calls aborted");
                    break;
                case ThreadState.Stopped:
                    break;
                case ThreadState.StopRequested:
                    break;
                case ThreadState.Suspended:
                    break;
                case ThreadState.SuspendRequested:
                    break;
                case ThreadState.Unstarted:
                    webserviceCallThread.Start();
                    buttonStartStop.Image = GetStopImage();
                    message = "Web service scheduled calls started";
                    logHelper.Success(message);
                    SetTrayIconMessage(message);
                    break;
                case ThreadState.WaitSleepJoin:
                    webserviceCallThread.Abort();
                    buttonStartStop.Image = GetStartImage();
                    message = "Web service scheduled calls aborted";
                    logHelper.Success(message);
                    SetTrayIconMessage(message);
                    break;
                default:
                    string errorMessage = String.Format("Unhandled state {0}", webserviceCallThread.ThreadState);
                    logHelper.Error(errorMessage);
                    throw new NotImplementedException( errorMessage);
            }
        }
    }
}
