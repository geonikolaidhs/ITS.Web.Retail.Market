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
    public partial class frmFinalAmount : frmInputFormBase
    {
        public decimal FinalAmount { get; set; }

        public frmFinalAmount(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();
            lblTitle.Text = POSClientResources.DRAWER_AMOUNT;
            this.btnOK.Text = POSClientResources.OK;//"OK";
            this.btnCancel.Text = POSClientResources.CANCEL;//"Ακύρωση";
            this.labelControl1.Text = POSClientResources.AMOUNT;//"Ποσό:";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                FinalAmount = decimal.Parse(txtInput.Text.Replace(config.CurrencySymbol, "").Trim());
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.POSClientResources.INVALID_AMOUNT + "(" + txtInput.Text + ")", ex);
            }
        }
    }
}
