using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Diagnostics;
using System.Timers;
using System.IO;

namespace ITS.Service.Guard
{
    public partial class ServiceGuard : Form
    {

        private System.Timers.Timer _checkServiceTimer { get; set; }

        private ContextMenu _trayMenu { get; set; }

        public ServiceGuard()
        {
            InitializeComponent();
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Show", FormShow);
            noty.Text = "Service Guard";
            noty.ContextMenu = _trayMenu;

            this._checkServiceTimer = new System.Timers.Timer();
            this._checkServiceTimer.Interval = Settings.getInstance().Interval * 1000;
            this._checkServiceTimer.Elapsed += new ElapsedEventHandler(this.TimeElapsed);
            this._checkServiceTimer.Enabled = true;

            MinimizeForm();

            if (Settings.getInstance().CreateStartUpShortCut == 1)
            {
                CreateStartUpShortCut();
            }

            if (Settings.getInstance().Monitor == 1)
            {
                StartSelfGuard();
            }
            this.WindowState = FormWindowState.Minimized;


        }


        private void TimeElapsed(Object sender, ElapsedEventArgs eventArgs)
        {
            UpdateServiceStatus();
        }

        private void UpdateServiceStatus()
        {
            ServiceController sc = ServiceHelper.GetServiceByName(Settings.getInstance().ServiceName);
            LabelServiceName.Text = sc == null ? "Service " + Settings.getInstance().ServiceName + " Not Found" : "Service Name : " + ServiceHelper.UpdateService(sc);
        }

        private void ServiceGuard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                MinimizeForm();
            }
        }


        private void FormShow(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Visible = true;
        }

        private void ServiceGuard_Shown(object sender, EventArgs e)
        {
            UpdateServiceStatus();
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            MinimizeForm();
        }

        private void MinimizeForm()
        {
            this.Visible = false;
            noty.Visible = true;
            noty.BalloonTipTitle = "Running on Background";
            noty.BalloonTipText = LabelLastOperation.Text + "  " + ServiceStatusLabel.Text;
            noty.ShowBalloonTip(500);
        }


        private void StartSelfGuard()
        {
            try
            {
                int nProcessID = Process.GetCurrentProcess().Id;
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "ITS.Process.Guard.exe";
                process.StartInfo.Arguments = nProcessID.ToString();
                process.Start();
            }
            catch (Exception ex)
            {
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            CloseForm form = new CloseForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void CreateStartUpShortCut()
        {

            try
            {
                string startDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                if (File.Exists(startDir + "//" + "ITS.Service.Guard.url"))
                {
                    File.Delete(startDir + "//" + "ITS.Service.Guard.url");
                }

                using (StreamWriter writer = new StreamWriter(startDir + "\\" + "ITS.Service.Guard" + ".url"))
                {
                    string app = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=file:///" + app);
                    writer.WriteLine("IconIndex=0");
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error);
            }
        }
    }
}
