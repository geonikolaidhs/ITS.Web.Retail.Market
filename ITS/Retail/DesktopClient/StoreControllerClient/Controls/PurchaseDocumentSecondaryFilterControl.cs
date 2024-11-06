using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.Retail.Model;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class PurchaseDocumentSecondaryFilterControl : BaseSecondaryFilterControl
    {
        public PurchaseDocumentSecondaryFilterControl()
        {
            InitializeComponent();
            searchLookUpEditDocumentSupplierFilter.Properties.DataSource = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(SupplierNew),
    new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid));
            searchLookUpEditDocumentSupplierFilter.Properties.ValueMember = "Oid";

        }

        private void searchLookUpEditDocumentSupplierFilter_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }
    }
}
