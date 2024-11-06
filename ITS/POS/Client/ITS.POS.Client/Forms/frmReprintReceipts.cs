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
    public partial class frmReprintReceipts : frmInputFormBase
    {
        public frmReprintReceipts(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();

            lblTitle.Text = POSClientResources.REPRINT_RECEIPTS;
            lblFromFilter.Text = POSClientResources.FROM_RECEIPT;
            lblToFilter.Text = POSClientResources.TO_RECEIPT;
        }

        public int FromReceiptNumber
        {
            get
            {
                int value;
                int.TryParse(edtFromReceipt.Text, out value);
                return value;
            }
        }

        public int ToReceiptNumber
        {
            get
            {
                int value;
                int.TryParse(edtToReceipt.Text, out value);
                return value;
            }
        }

        private void edtFromReceipt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                edtToReceipt.Focus();
            }
        }

        private void frmReprintReceipts_Shown(object sender, EventArgs e)
        {
            edtFromReceipt.Focus();
        }

        private void edtToReceipt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
        }

    }
}
