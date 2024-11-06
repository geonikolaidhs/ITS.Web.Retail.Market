using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.Forms
{
    public partial class frmGetItemPrice : frmInputFormBase , IGetItemPriceForm
    {
        public Item Item { get; set; }
        public decimal Price { get; set; }

        public frmGetItemPrice(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();
            labelItem.Text = POSClientResources.ITEM;
            labelInsertPrice.Text = POSClientResources.PRICE;
            btnClose.Text = POSClientResources.CANCEL;
        }

        private void frmGetItemPrice_Load(object sender, EventArgs e)
        {
            this.txtItemName.Text = Item.Name;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void textEdit1_Properties_Enter(object sender, EventArgs e)
        {

        }

        private void txtItemPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                decimal price = 0;
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                if (decimal.TryParse(txtItemPrice.Text.Replace(config.CurrencySymbol, "").Trim(), out price))
                {
                    //GlobalContext.GetAction(eActions.ADD_ITEM).Execute(new ActionAddItemParams(GlobalContext.CurrentDocument,this.Item.Code,this.Quantity,price));
                    if (price < 0)
                    {
                        IFormManager formManager = Kernel.GetModule<IFormManager>();
                        formManager.ShowCancelOnlyMessageBox(POSClientResources.NEGATIVE_VALUES_NOT_PERMITTED);
                        return;
                    }

                    Price = price;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
        }

        private void frmGetItemPrice_FormClosed(object sender, FormClosedEventArgs e)
        {
            txtItemPrice.HideTouchPad();
        }


    }
}