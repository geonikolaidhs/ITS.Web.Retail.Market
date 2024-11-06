using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class ITSSplashScreen : WaitForm
    {
        public enum Command
        {
            UPDATE_PROGRESS,
            SET_MAX_PROGRESS
        }
        public ITSSplashScreen()
        {
            InitializeComponent();
            this.labelControl2.Text = ResourcesLib.Resources.StartingApp;
        }

        public override void SetDescription(string description)
        {
            //base.SetDescription(description);
            this.labelControl2.Text = description;
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            if (cmd is Command)
            {

            }
            else
            {
                base.ProcessCommand(cmd, arg);
            }

        }

        #endregion

        public enum SplashScreenCommand
        {
        }
    }
}