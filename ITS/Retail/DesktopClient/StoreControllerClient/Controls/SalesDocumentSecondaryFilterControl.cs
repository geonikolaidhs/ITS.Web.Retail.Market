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
using DevExpress.Data.Filtering;
using DevExpress.Xpo;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class SalesDocumentSecondaryFilterControl : BaseSecondaryFilterControl
    {
        public SalesDocumentSecondaryFilterControl()
        {
            InitializeComponent();
            searchLookUpEditDocumentCustomerFilter.Properties.DataSource = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(Customer),
                  new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid));
            searchLookUpEditDocumentCustomerFilter.Properties.ValueMember = "Oid";
            
        }

        private void searchLookUpEditDocumentCustomerFilter_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }
    }
}
