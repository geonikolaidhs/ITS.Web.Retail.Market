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
    public partial class CustomerAddressInlineEdit : XtraLocalizedForm
    {
        protected Address _Address;
        protected Address Address { get { return _Address; } }

        private string OldPhoneSerialized { get; set; }

        private Phone _EditingPhone;
        protected Phone EditingPhone { get { return _EditingPhone; } set { _EditingPhone = value; } }

        public CustomerAddressInlineEdit(Address address)
        {
            this._Address = address;
            InitializeComponent();
        }

        private void InitializeLookupEdits()
        {
            this.lueAddressType.Properties.DataSource = new XPCollection<AddressType>(this.Address.Session);
            this.lueAddressType.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.lueAddressType.Properties.ValueMember = "Oid";
            this.lueAddressType.Properties.DisplayMember = "Description";

            this.lueVatLevel.Properties.DataSource = new XPCollection<VatLevel>(this.Address.Session);
            this.lueVatLevel.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.lueVatLevel.Properties.ValueMember = "Oid";
            this.lueVatLevel.Properties.DisplayMember = "Description";

            this.lueDefaultPhone.Properties.DataSource = this.Address.Phones;
            this.lueDefaultPhone.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.lueDefaultPhone.Properties.ValueMember = "Oid";
            this.lueDefaultPhone.Properties.DisplayMember = "Description";
        }

        private void InitializeBindings()
        {
            this.lueAddressType.DataBindings.Add("EditValue", this.Address, "AddressType!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueDefaultPhone.DataBindings.Add("EditValue", this.Address, "DefaultPhoneOid", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueVatLevel.DataBindings.Add("EditValue", this.Address, "VatLevel!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtPOBox.DataBindings.Add("EditValue", this.Address, "POBox", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtCity.DataBindings.Add("EditValue", this.Address, "City", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtPostCode.DataBindings.Add("EditValue", this.Address, "PostCode", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtStreet.DataBindings.Add("EditValue", this.Address, "Street", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtAddressProfession.DataBindings.Add("EditValue", this.Address, "Profession", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtThirdPartNum.DataBindings.Add("EditValue", this.Address, "ThirdPartNum", true, DataSourceUpdateMode.OnPropertyChanged);

            this.gridPhones.DataSource = this.Address.Phones;

            cmbPhoneType.DataSource = new XPCollection<PhoneType>(Program.Settings.ReadOnlyUnitOfWork);       
            cmbPhoneType.Columns.Clear();
            cmbPhoneType.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            cmbPhoneType.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
        }

        private bool ObjectCanBeSaved(out string errormessage)
        {
            errormessage = "";
            bool result = true;
            if(String.IsNullOrWhiteSpace(this.txtStreet.EditValue as string))
            {
                errormessage = Resources.StreetIsEmpty;
                result = false;
            }
            else if (String.IsNullOrWhiteSpace(this.txtAddressProfession.EditValue as string))
            {
                errormessage = Resources.AddressProfessionIsEmpty;
                result = false;
            }
            else if (lueDefaultPhone.EditValue == null)
            {
                errormessage = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
                result = false;
            }
            else
            {
                bool found = false;
                foreach (Phone item in this.Address.Phones)
                {
                    if (this.Address.DefaultPhoneOid == item.Oid) found = true;
                   
                }
                if (found == false)
                {
                    errormessage = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
                    result = false;
                }
            }
            return result;
        }

        private void btn_delete_phone_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this._EditingPhone = this.grdPhones.GetFocusedRow() as Phone;
            if (this._EditingPhone != null)
            {
                this.EditingPhone.Delete();
            }
            this.grdPhones.RefreshData();
        }

        //private void btnAddAddressPhone_Click(object sender, EventArgs e)
        //{
        //    DialogResult dialogResult = DialogResult.None;
        //    this._EditingPhone = new Phone(this.Address.Session);
        //    using (AddressPhoneInlineEdit addressPhoneForm = new AddressPhoneInlineEdit(this.EditingPhone))
        //    {
        //        dialogResult = addressPhoneForm.ShowDialog();
        //    }
        //    if (dialogResult == DialogResult.OK)
        //    {
        //        this.EditingPhone.Save();
        //        this.Address.Phones.Add(this.EditingPhone);
        //        this.grdPhones.RefreshData();
        //    }
        //    else
        //    {
        //        this.EditingPhone.Delete();
        //    }
        //}

        //private void gridPhones_DoubleClick(object sender, EventArgs e)
        //{
        //    EditPhonesGrid();
        //}

        //private void btnEdit_Click(object sender, EventArgs e)
        //{
        //    EditPhonesGrid();
        //}

        //private void EditPhonesGrid()
        //{
        //   /* DialogResult dialogResult = System.Windows.Forms.DialogResult.None;
        //    this._EditingPhone = this.grdPhones.GetFocusedRow() as Phone;
        //    if (this._EditingPhone != null)
        //    {
        //        this.OldPhoneSerialized = this.EditingPhone.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
        //        using (AddressPhoneInlineEdit addressPhoneForm = new AddressPhoneInlineEdit(this.EditingPhone))
        //        {
        //            dialogResult = addressPhoneForm.ShowDialog();
        //        }
        //        if (dialogResult == DialogResult.OK)
        //        {
        //            this.grdPhones.RefreshData();
        //        }
        //        else
        //        {
        //            string error;
        //            bool result = this.EditingPhone.FromJson(this.OldPhoneSerialized, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
        //        }
        //    }*/
        //}



        private void CustomerAddressInlineEdit_Load(object sender, EventArgs e)
        {
            InitializeLookupEdits();
            InitializeBindings();
            //if (!Program.Settings.CurrentUser.Role.GDPREnabled)
            //{
            //    lcPhones.Parent.Remove(lcPhones);
            //    lcDefaultPhone.Parent.Remove(lcDefaultPhone);
            //}
        }

        private void btnSaveCustomerAddress_Click(object sender, EventArgs e)
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

        private void btnCancelCustomerAddress_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void gridPhones_Click(object sender, EventArgs e)
        {

        }

        private void grdPhones_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {            
            if (e.Column == grdDeleteButton && grdPhones.GetRow(e.RowHandle) as Phone == null)
            {
                e.Handled = true;
            }
        }

        private void CustomerAddressInlineEdit_Shown(object sender, EventArgs e)
        {
            this.txtStreet.Focus();
        }

    }
}
