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
    public partial class frmPrintFiscalMemoryBlocks : frmInputFormBase
    {
        public frmPrintFiscalMemoryBlocks(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();

            lblTitle.Text = POSClientResources.PRINT_FISCAL_MEMORY_BLOCKS;
            lblFromFilter.Text = POSClientResources.FROM_BLOCK;
            lblToFilter.Text = POSClientResources.TO_BLOCK;
        }

        public int FromBlock
        {
            get
            {
                int value;
                int.TryParse(edtFromBlock.Text, out value);
                return value;
            }
        }

        public int ToBlock
        {
            get
            {
                int value;
                int.TryParse(edtToBlock.Text, out value);
                return value;
            }
        }

        private void edtFromBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                edtToBlock.Focus();
            }
        }

        private void frmPrintFiscalMemoryBlocks_Shown(object sender, EventArgs e)
        {
            edtFromBlock.Focus();
        }

        private void edtToBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
        }

    }
}
