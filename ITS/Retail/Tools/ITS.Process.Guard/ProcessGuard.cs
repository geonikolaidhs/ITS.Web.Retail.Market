using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;

using System.Diagnostics;

namespace ITS.Process.Guard
{
    public partial class ProcessGuard : Form
    {

        private int GuardPid { get; set; }

        private int FileLocation { get; set; }

        private System.Timers.Timer _GuardProcessTimer { get; set; }


        public ProcessGuard()
        {
            InitializeComponent();
            String[] args = Program.GetArguments();
            if (args != null && args.Length > 0)
            {
                int pid;
                if (int.TryParse(args[0], out pid))
                {
                    GuardPid = pid;
                };
            }

            this._GuardProcessTimer = new System.Timers.Timer();
            this._GuardProcessTimer.Interval = 30000;
            this._GuardProcessTimer.Elapsed += new ElapsedEventHandler(this.TimeElapsed);
            this._GuardProcessTimer.Enabled = true;

            // this.Visible = false;
        }

        private void TimeElapsed(Object sender, ElapsedEventArgs eventArgs)
        {
            CheckProccess();
        }

        private void CheckProccess()
        {
            int pid = 0;
            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(GuardPid);
                pid = process.Id;
            }
            catch (Exception e)
            {
                Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error);
            }
            if (pid == 0)
            {
                StartProcess();
            }

        }

        private void StartProcess()
        {
            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "ITS.Service.Guard.exe";
                process.Start();
                GuardPid = process.Id;
            }
            catch (Exception e)
            {
                Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error);
            }
        }



        private void ProcessGuard_Shown(object sender, EventArgs e)
        {
            // this.Visible = false;
            // this.Hide();
        }
    }
}
