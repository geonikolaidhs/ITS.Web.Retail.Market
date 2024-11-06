using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace ITS.Service.Guard
{
    public partial class CloseForm : Form
    {
        public CloseForm()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            CloseApp();
        }


        private void CloseApp()
        {
            String pass = TextPassword.Text;
            if (pass == "1t$ervices")
            {
                try
                {
                    foreach (Process proc in System.Diagnostics.Process.GetProcesses())
                    {
                        if (proc.ProcessName.ToUpper().Contains("ITS.PROCESS.GUARD"))
                        {
                            proc.Kill();
                        }
                    }
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }
                catch (Exception ex)
                {
                    Program.WriteToWindowsEventLog(ex.Message, EventLogEntryType.Error);
                    TextPassword.Text = String.Empty;
                }
            }
            else
            {
                TextPassword.Text = String.Empty;
            }
        }

        private void TextPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                CloseApp();
            }
        }
    }
}
