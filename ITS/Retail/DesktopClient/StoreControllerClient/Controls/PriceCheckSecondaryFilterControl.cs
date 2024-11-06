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
using ITS.Retail.ResourcesLib;
using DevExpress.XtraGrid.Columns;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class PriceCheckSecondaryFilterControl : BaseSecondaryFilterControl
    {
        public PriceCheckSecondaryFilterControl()
        {
            InitializeComponent();
            SetLookups();

        }
        private void SetLookups()
        {
            BinaryOperator ownerCriteria = new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
            this.lueCustomer.Properties.DataSource = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(Customer), ownerCriteria);
            this.lueCustomer.Properties.View.Columns.Add(new GridColumn() { FieldName = "Code", Caption = Resources.Code, Visible = true });
            this.lueCustomer.Properties.View.Columns.Add(new GridColumn() { FieldName = "Trader.TaxCode", Caption = Resources.TaxCode, Visible = true });
            this.lueCustomer.Properties.View.Columns.Add(new GridColumn() { FieldName = "CompanyName", Caption = Resources.CompanyName, Visible = true });
            this.lueCustomer.Properties.ValueMember = "Oid";


            ownerCriteria = new BinaryOperator("Item.Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
            this.lueItemBarcode.Properties.DataSource = new XPServerCollectionSource(Program.Settings.ReadOnlyUnitOfWork, typeof(ItemBarcode), ownerCriteria);
            this.lueItemBarcode.Properties.View.Columns.Add(new GridColumn() { FieldName = "Barcode.Code", Caption = Resources.Barcode, Visible = true });
            this.lueItemBarcode.Properties.View.Columns.Add(new GridColumn() { FieldName = "Item.Code", Caption = Resources.Code, Visible = true });
            this.lueItemBarcode.Properties.View.Columns.Add(new GridColumn() { FieldName = "Item.Name", Caption = Resources.Description, Visible = true });
            this.lueItemBarcode.Properties.ValueMember = "Barcode.Code";
        }
    }
}
