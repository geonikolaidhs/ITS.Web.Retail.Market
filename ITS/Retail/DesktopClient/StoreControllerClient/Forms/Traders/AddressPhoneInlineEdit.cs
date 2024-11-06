using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class AddressPhoneInlineEdit : XtraLocalizedForm
    {
        protected Phone _Phone;
        protected Phone Phone { get { return _Phone; } }

        public AddressPhoneInlineEdit(Phone phone)
        {
            this._Phone = phone;
            InitializeComponent();
        }

        private void InitializeLookupEdits()
        {
            this.luePhoneType.Properties.DataSource = new XPCollection<PhoneType>(this.Phone.Session);
            this.luePhoneType.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.luePhoneType.Properties.ValueMember = "Oid";
            this.luePhoneType.Properties.DisplayMember = "Description";
        }

        private void InitializeBindings()
        {
            this.luePhoneType.DataBindings.Add("EditValue", this.Phone, "PhoneType!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtPhoneNumber.DataBindings.Add("EditValue", this.Phone, "Number", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private bool ObjectCanBeSaved(out string errormessage)
        {
            errormessage = Resources.FillAllMissingFields;
            return (this.luePhoneType.EditValue != null &&
                    this.txtPhoneNumber.EditValue != null);
        }

        private void btnSavePhone_Click(object sender, EventArgs e)
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

        private void btnCancelPhone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void AddressPhoneInlineEdit_Load(object sender, EventArgs e)
        {
            InitializeLookupEdits();
            InitializeBindings();
        }

        private void AddressPhoneInlineEdit_Shown(object sender, EventArgs e)
        {
            this.txtPhoneNumber.Focus();
        }
    }
}
