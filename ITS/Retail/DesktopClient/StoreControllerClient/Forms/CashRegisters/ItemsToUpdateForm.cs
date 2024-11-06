using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.Retail.Platform.Enumerations;
using ITS.Hardware.RBSPOSEliot.CashRegister;
using System.Globalization;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Model.SupportingClasses;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms.CashRegisters
{
    public partial class ItemsToUpdateForm : XtraLocalizedForm 
    {
        public ItemsToUpdateForm(List<CashDeviceItem> Items)
        {
            InitializeComponent();
            grdMain.DataSource = Items;
            grdMain.Refresh();
        }
        private void CashRegister_Load(object sender, EventArgs e)
        {

        }
        private void btnAddItemToDevice_Click(object sender, EventArgs e)
        {
            
        }
    }
}