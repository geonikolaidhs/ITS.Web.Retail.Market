using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class CustomerStorePriceListInlineEdit : XtraLocalizedForm
    {
        protected CustomerStorePriceList _CustomerStorePriceList;
        protected CustomerStorePriceList CustomerStorePriceList { get { return _CustomerStorePriceList; } }

        bool firstCall = true;

        public CustomerStorePriceListInlineEdit(CustomerStorePriceList storePriceList)
        {
            this._CustomerStorePriceList = storePriceList;
            InitializeComponent();
        }

        private void InitializeLookupEdits()
        {
            this.lueOwner.Properties.DataSource = new XPCollection<CompanyNew>(this.CustomerStorePriceList.Session).
                                                Where(comp => comp.Stores.Count > 0).
                                                OrderBy(comp => comp.CompanyName);
            this.lueOwner.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.lueOwner.Properties.ValueMember = "Oid";
            this.lueOwner.Properties.DisplayFormat.FormatString = "{0} {1}";
        }

        private void InitializeBindings()
        {
            this.lueOwner.DataBindings.Add("EditValue", this.CustomerStorePriceList, "Owner!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.chkIsDefault.DataBindings.Add("EditValue", this.CustomerStorePriceList, "IsDefault", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private bool ObjectCanBeSaved(out string errormessage)
        {
            errormessage = Resources.FillAllMissingFields;
            return (this.lueOwner.EditValue != null &&
                    this.lueStore.EditValue != null &&
                    this.luePriceCatalog.EditValue != null);
        }

        private void lueOwner_EditValueChanged(object sender, EventArgs e)
        {
            CriteriaOperator criteria = new BinaryOperator("Owner.Oid", this.lueOwner.EditValue);
            this.lueStore.Properties.DataSource = new XPCollection<Store>(this.CustomerStorePriceList.Session, criteria);
            this.lueStore.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.lueStore.Properties.ValueMember = "Oid";
            this.lueStore.Properties.DisplayFormat.FormatString = "{0} {1}";
        }

        private void lueStore_EditValueChanged(object sender, EventArgs e)
        {
            this.luePriceCatalog.Properties.DataSource = new XPCollection<PriceCatalog>(this.CustomerStorePriceList.Session)
                .Where(catalog => catalog.StorePriceLists.Any(pricelist => pricelist.Store.Oid == (Guid)this.lueStore.EditValue));
            this.luePriceCatalog.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.luePriceCatalog.Properties.ValueMember = "Oid";
            this.luePriceCatalog.Properties.DisplayFormat.FormatString = "{0} {1}";        
        }

        private void btnSaveCustomerStorePriceList_Click(object sender, EventArgs e)
        {
            string message;
            if (ObjectCanBeSaved(out message))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnCancelCustomerStorePriceList_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void CustomerStorePriceListInlineEdit_Load(object sender, EventArgs e)
        {
            InitializeLookupEdits();
            InitializeBindings();
        }

        private void luePriceCatalog_EditValueChanged(object sender, EventArgs e)
        {
            if(firstCall)
            {
                this.CustomerStorePriceList.StorePriceList = this.CustomerStorePriceList.Session.FindObject<StorePriceList>(CriteriaOperator.And(new BinaryOperator("Store.Oid", ((Guid)this.lueStore.EditValue)), new BinaryOperator("PriceList.Oid", ((Guid)this.luePriceCatalog.EditValue))));
                this.lueStore.DataBindings.Add("EditValue", this.CustomerStorePriceList.StorePriceList, "Store!Key", true, DataSourceUpdateMode.OnValidation);
                this.luePriceCatalog.DataBindings.Add("EditValue", this.CustomerStorePriceList.StorePriceList, "PriceList!Key", true, DataSourceUpdateMode.OnValidation);
                firstCall = false;
            }

        }

        private void CustomerStorePriceListInlineEdit_Shown(object sender, EventArgs e)
        {
            this.lueOwner.Focus();
        }
    }
}
