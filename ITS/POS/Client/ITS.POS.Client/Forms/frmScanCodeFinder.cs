using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Resources;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Forms
{
    public partial class frmScanCodeFinder : frmDialogBase
    {
        public frmScanCodeFinder(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();

            this.label1.Text = POSClientResources.PRESS_A_KEY;//"PRESS A KEY";
            this.btnClose.Text = POSClientResources.CLOSE;//"Close";
            this.label2.Text = POSClientResources.OUTPUT;//"Output";
        }

        private void frmScanCodeFinder_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmScanCodeFinder_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void frmScanCodeFinder_KeyUp(object sender, KeyEventArgs e)
        {
            string line = "KeyCode=" + Enum.GetName(typeof(Keys), e.KeyCode) + "  ScanCode=" + UtilsHelper.GetScanCode(e.KeyCode).ToString() + "  KeyData=" + ((int)e.KeyData).ToString() + "  Modifiers=" + e.Modifiers.ToString() + Environment.NewLine;
            e.Handled = true;
            txtOutput.AppendText(line);
        }
    }
}