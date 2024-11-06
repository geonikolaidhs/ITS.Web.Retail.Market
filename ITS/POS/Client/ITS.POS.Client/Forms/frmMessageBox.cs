using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmMessageBox : frmDialogBase
    {
        public string Message
        {
            get
            {
                return this.rtbMessageArea.Text;
            }
            set
            {
                this.rtbMessageArea.Text = value;
            }
        }

        protected frmMessageBox()
        {
            InitializeComponent();
        }

        public frmMessageBox(IPosKernel Kernel)
            : base(Kernel)
        {
            InitializeComponent();
            btnOK.Text = POSClientResources.OK;
            btnCancel.Text = POSClientResources.CANCEL;
            btnRetry.Text = POSClientResources.RETRY;
            btnYes.Text = POSClientResources.YES;
            btnNo.Text = POSClientResources.NO;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.Close();
        }

        private void frmMessageBox_Shown(object sender, EventArgs e)
        {
            //Console.Beep();
        }


        public void SetTextAreaHeight(int height)
        {
            this.rtbMessageArea.Height = height;
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }
    }
}
