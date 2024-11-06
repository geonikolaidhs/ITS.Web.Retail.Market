using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.Common;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.MigrationTool
{
    public partial class OwnerInfoForm : Form
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TaxCode { get; set; }
        public TaxOffice TaxOffice { get; set; }
        public bool RequestStoreInfo { get; set; }
        public string StoreName { get; set; }
        public string StoreCity { get; set; }
        public string OwnerCompanyName { get; set; }
        public string StoreCode { get; set; }
        public string StoreStreet { get; set; }
        public UnitOfWork UOW { get; set; }
        
        public OwnerInfoForm(bool requestStoreInfo, UnitOfWork uow)
        {
            InitializeComponent();
            this.RequestStoreInfo = requestStoreInfo;
            this.panelStoreInfo.Enabled= requestStoreInfo;
            this.UOW = uow;
            this.PrepareTaxOfficeComboBox();
        }

        private void PrepareTaxOfficeComboBox()
        {
            //Step 1: Get Data Source
            XPCollection<TaxOffice> taxOffices;
            taxOffices = new XPCollection<TaxOffice>(PersistentCriteriaEvaluationBehavior.InTransaction,this.UOW,null,false);

            //step 2: Assign Datasorce to combo box
            this.cmbTaxOffice.Properties.DataSource = taxOffices;
            this.cmbTaxOffice.Properties.DisplayMember = "Description";
            this.cmbTaxOffice.Properties.ValueMember = "Description";
            this.cmbTaxOffice.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description", 20));
            this.cmbTaxOffice.Properties.Columns.Add(new LookUpColumnInfo("Code", "Code",20));
            this.cmbTaxOffice.Properties.Columns["Code"].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
            this.cmbTaxOffice.EditValue = taxOffices[0].Description;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if ((String.IsNullOrWhiteSpace(txtFirstName.Text)
                || String.IsNullOrWhiteSpace(txtLastName.Text)
                || String.IsNullOrWhiteSpace(txtTaxCode.Text))
                || (this.RequestStoreInfo == true && String.IsNullOrWhiteSpace(txtStoreName.Text))
                || (this.RequestStoreInfo == true && String.IsNullOrWhiteSpace(txtStoreCode.Text))
                || cmbTaxOffice.EditValue == null)
            {
                MessageBox.Show("The fields marked with (*) are required");
                return;
            }
            this.FirstName = txtFirstName.Text;
            this.LastName = txtLastName.Text;
            this.TaxCode = txtTaxCode.Text;
            this.TaxOffice = cmbTaxOffice.Properties.GetDataSourceRowByKeyValue(cmbTaxOffice.EditValue) as TaxOffice;
            this.StoreName = txtStoreName.Text;
            this.StoreCity = txtCity.Text;
            this.StoreStreet = txtStreet.Text;
            this.OwnerCompanyName = txtCompanyName.Text;
            this.StoreCode = txtStoreCode.Text;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

    }
}
