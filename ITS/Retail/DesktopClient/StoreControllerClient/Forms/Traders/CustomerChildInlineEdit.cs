using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
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
    public partial class CustomerChildInlineEdit : XtraLocalizedForm
    {
        protected CustomerChild _CustomerChild;
        protected CustomerChild CustomerChild { get { return _CustomerChild; } }

        public CustomerChildInlineEdit(CustomerChild child)
        {
            this._CustomerChild = child;
            InitializeComponent();
        }

        private void InitializeLookupEdits()
        {
            this.lueChildSex.Properties.DataSource = Enum<eSex>.GetLocalizedDictionary();
            this.lueChildSex.Properties.Columns.Add(new LookUpColumnInfo("Value", Resources.Description));
            this.lueChildSex.Properties.ValueMember = "Key";
            this.lueChildSex.Properties.DisplayMember = "Value";
        }

        private void InitializeBindings()
        {
            this.lueChildSex.DataBindings.Add("EditValue", this.CustomerChild, "ChildSex", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dtChildBirthDate.DataBindings.Add("EditValue", this.CustomerChild, "ChildBirthDate", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void btnSaveCustomerChild_Click(object sender, EventArgs e)
        {
            string message;
            if(ObjectCanBeSaved(out message))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool ObjectCanBeSaved(out string errormessage)
        {
            errormessage = Resources.FillAllMissingFields;
            return (this.lueChildSex.EditValue != null &&
                    this.dtChildBirthDate.DateTime != DateTime.MinValue);
        }

        private void btnCancelCustomerChild_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void CustomerChildInlineEdit_Load(object sender, EventArgs e)
        {
            InitializeLookupEdits();
            InitializeBindings();
        }

        private void CustomerChildInlineEdit_Shown(object sender, EventArgs e)
        {
            this.lueChildSex.Focus();
        }
    }
}
