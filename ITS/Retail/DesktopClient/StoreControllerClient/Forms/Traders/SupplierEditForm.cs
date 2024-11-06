using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class SupplierEditForm : XtraLocalizedForm
    {
        protected SupplierNew _Supplier;
        protected SupplierNew Supplier { get { return _Supplier; } }

        private string OldSupplierAddressSerialized { get; set; }

        private Address _EditingAddress;
        protected Address EditingAddress { get { return _EditingAddress; } set { _EditingAddress = value; } }

        protected bool onInitialization;

        private bool PreviewMode { get; set; }
        protected bool ObjectIsSaved { get; set; }

        public delegate void MyEvent(SupplierNew supplier);
        public event MyEvent DuplicateTraderFound;

        public SupplierEditForm(SupplierNew supplier, bool previewMode) : base()
        {
            this._Supplier = supplier;
            PreviewMode = previewMode;
            if (PreviewMode)
            {
                this.Text = Resources.PreviewDocument;
            }
            InitializeComponent();
        }

        private void InitializeBindings()
        {
            this.lueTaxOffice.DataBindings.Add("EditValue", this.Supplier.Trader, "TaxOfficeLookUp!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueVatLevel.DataBindings.Add("EditValue", this.Supplier, "VatLevel!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueDefaultAddress.DataBindings.Add("EditValue", this.Supplier, "DefaultAddress!Key", true, DataSourceUpdateMode.OnPropertyChanged);


            this.txtCode.DataBindings.Add("EditValue", this.Supplier, "Code", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtTaxCode.DataBindings.Add("EditValue", this.Supplier.Trader, "TaxCode", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtCompanyName.DataBindings.Add("EditValue", this.Supplier, "CompanyName", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtProfession.DataBindings.Add("EditValue", this.Supplier, "Profession", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtFirstName.DataBindings.Add("EditValue", this.Supplier.Trader, "FirstName", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtLastName.DataBindings.Add("EditValue", this.Supplier.Trader, "LastName", true, DataSourceUpdateMode.OnPropertyChanged);

            this.txtGDPRAnonymizationDate.DataBindings.Add("EditValue", this.Supplier, "GDPRAnonymizationDate", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtGDPRAnonymizationProtocolNumber.DataBindings.Add("EditValue", this.Supplier, "GDPRAnonymizationProtocolNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            try
            {
                this.txtGDPRAnonymizationUser.DataBindings.Add("EditValue", this.Supplier.GDPRAnonymizationUser, "FullName", true, DataSourceUpdateMode.OnPropertyChanged);
            }
            catch { }
            this.txtGDPRComments.DataBindings.Add("EditValue", this.Supplier, "GDPRComments", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtGDPRExportDate.DataBindings.Add("EditValue", this.Supplier, "GDPRExportDate", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtGDPRExportProtocolNumber.DataBindings.Add("EditValue", this.Supplier, "GDPRExportProtocolNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            try
            {
                this.txtGDPRExportUser.DataBindings.Add("EditValue", this.Supplier.GDPRExportUser, "FullName", true, DataSourceUpdateMode.OnPropertyChanged);
            }
            catch { }

            this.txtGDPRRegistrationDate.DataBindings.Add("EditValue", this.Supplier.Trader, "GDPRRegistrationDate", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtGDPRRegistrationsProtocolNumber.DataBindings.Add("EditValue", this.Supplier.Trader, "GDPRProtocolNumber", true, DataSourceUpdateMode.OnPropertyChanged);

            this.chkIsActive.DataBindings.Add("EditValue", this.Supplier, "IsActive", true, DataSourceUpdateMode.OnPropertyChanged);

            this.gridSupplierAddresses.DataSource = this.Supplier.Trader.Addresses;
        }

        private void InitializeLookupEdits()
        {
            CriteriaOperator criteria = null;
            lueTaxOffice.Properties.View.Columns.Add(new GridColumn() { FieldName = "Code", Caption = Resources.Code, Visible = true, VisibleIndex = 0 });
            lueTaxOffice.Properties.View.Columns.Add(new GridColumn() { FieldName = "Description", Caption = Resources.Description, Visible = true, VisibleIndex = 1 });
            lueTaxOffice.Properties.DataSource = new XPCollection<TaxOffice>(this.Supplier.Session, criteria);

            lueTaxOffice.Properties.ValueMember = "Oid";
            lueTaxOffice.Properties.DisplayFormat.FormatString = "{0} {1}";

            lueVatLevel.Properties.DataSource = new XPCollection<VatLevel>(this.Supplier.Session, criteria);
            lueVatLevel.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueVatLevel.Properties.ValueMember = "Oid";

            lueDefaultAddress.Properties.DataSource = this.Supplier.Trader.Addresses;
            lueDefaultAddress.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueDefaultAddress.Properties.ValueMember = "Oid";
        }

        private void SupplierEditForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                onInitialization = false;
                return;
            }
            if (PreviewMode)
            {
                this.btnCancelSupplier.Text = Resources.Close;
            }

            InitializeLookupEdits();
            InitializeBindings();

            if (PreviewMode)
            {
                IEnumerable<BaseEdit> editControls = this.EnumerateComponents().Where(x => x is BaseEdit).Cast<BaseEdit>();
                foreach (BaseEdit editControl in editControls)
                {
                    editControl.ReadOnly = true;
                    if (editControl is DateEdit)
                    {
                        (editControl as DateEdit).Properties.AllowDropDownWhenReadOnly = DefaultBoolean.False;
                    }
                }
                IEnumerable<SimpleButton> buttons = this.EnumerateComponents().Where(x => x is SimpleButton).Cast<SimpleButton>();
                foreach (SimpleButton btn in buttons)
                {
                    btn.Visible = false;
                }
                IEnumerable<GridView> gridViews = this.EnumerateComponents().Where(x => x is GridView).Cast<GridView>();
                foreach (GridView gridView in gridViews)
                {
                    gridView.OptionsBehavior.Editable = false;
                }
                grdSupplierAddressEditBtn.Visible = grdSupplierAddressDeleteBtn.Visible = false;
                layoutControlItem4.HideToCustomization();
                lueTaxOffice.Properties.Buttons.Clear();
            }
            this.Supplier.Changed += Supplier_Changed;
            onInitialization = false;
            if (!Program.Settings.CurrentUser.Role.GDPREnabled)
            {
                //grdSupplierAddresses.Columns.Remove(grdSupplierAddressPhone);
                this.lcFirstName.HideToCustomization();
                this.lcLastName.HideToCustomization();
            }
        }

        private void Supplier_Changed(object sender, ObjectChangeEventArgs e)
        {

        }

        private void btn_delete_adrs_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this._EditingAddress = this.grdSupplierAddresses.GetFocusedRow() as Address;
            if (this.Supplier.DefaultAddress != null && this.Supplier.DefaultAddress.Oid == this.EditingAddress.Oid)
            {
                this.Supplier.DefaultAddress = null;
            }
            this.EditingAddress.Delete();
            this.grdSupplierAddresses.RefreshData();
        }

        private void btnAddSupplierAddress_Click(object sender, EventArgs e)
        {

            this._EditingAddress = new Address(this.Supplier.Session);
            using (CustomerAddressInlineEdit customerAddressForm = new CustomerAddressInlineEdit(this.EditingAddress))
            {
                if (customerAddressForm.ShowDialog() == DialogResult.OK)
                {
                    this.EditingAddress.Save();
                    this.Supplier.Trader.Addresses.Add(this.EditingAddress);
                    this.grdSupplierAddresses.RefreshData();
                }
                else
                {
                    this.EditingAddress.Delete();
                }
            }
        }

        private void gridSupplierAddresses_DoubleClick(object sender, EventArgs e)
        {
            EditSupplierAddress();
        }

        private void btnEditSupplierAddress_Click(object sender, EventArgs e)
        {
            EditSupplierAddress();
        }

        private void EditSupplierAddress()
        {
            if (!PreviewMode)
            {
                DialogResult dialogResult = DialogResult.None;
                this._EditingAddress = this.grdSupplierAddresses.GetFocusedRow() as Address;
                this.OldSupplierAddressSerialized = this.EditingAddress.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                using (CustomerAddressInlineEdit customerAddressForm = new CustomerAddressInlineEdit(this.EditingAddress))
                {
                    dialogResult = customerAddressForm.ShowDialog();
                }
                if (dialogResult == DialogResult.OK)
                {
                    this.grdSupplierAddresses.RefreshData();
                }
                else
                {
                    string error;
                    bool result = this.EditingAddress.FromJson(this.OldSupplierAddressSerialized, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    this.EditingAddress.Trader = this.Supplier.Trader;
                }
            }
        }

        private void btnSaveSupplier_Click(object sender, EventArgs e)
        {
            string message;
            bool taxOfficeMissing;
            if (ObjectCanBeSaved(out message, out taxOfficeMissing))
            {
                this.SaveSupplier();
            }
            else if (taxOfficeMissing)
            {
                DialogResult result = XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    this.SaveSupplier();
                }
            }
            else
            {
                XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool ObjectCanBeSaved(out string errormessage, out bool taxOfficeMissing)
        {
            taxOfficeMissing = false;
            bool result = (this.txtCode.EditValue != null &&
                        this.txtTaxCode.EditValue != null &&
                        this.txtCompanyName.EditValue != null &&
                        !String.IsNullOrEmpty(this.lueVatLevel.EditValue.ToString()));
            if (!result)
            {
                errormessage = Resources.FillAllMissingFields;
            }
            else //Checks for Tax Office
            {
                result = !String.IsNullOrEmpty(this.lueTaxOffice.EditValue.ToString());
                errormessage = Resources.TaxofficeIsNotSelected;
                taxOfficeMissing = true;
            }
            return result;
        }

        private void btnCancelSupplier_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SupplierEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ObjectIsSaved == false)
            {
                e.Cancel = !this.CancelSupplier();
            }
        }

        private bool CancelSupplier()
        {
            if (this.PreviewMode || XtraMessageBox.Show(Resources.Cancel, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.Supplier.Changed -= this.Supplier_Changed;
                if (this.PreviewMode == false)
                {
                    this.Supplier.Session.RollbackTransaction();
                }
                return true;
            }
            return false;
        }

        protected void SaveSupplier()
        {
            try
            {
                if (String.IsNullOrEmpty(this.Supplier.Code))
                {
                    XtraMessageBox.Show(Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (this.Supplier.Trader.Addresses.Count == 0)
                {
                    XtraMessageBox.Show(Resources.NoAddressInserted, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    this.Supplier.Save();
                    this.Supplier.Session.CommitTransaction();
                    this.ObjectIsSaved = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void lueTaxOffice_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this.lueTaxOffice.EditValue = null;
        }

        private void SupplierEditForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            this.BringToFront();
            this.txtCode.Focus();
        }

        public void CheckIfTraderExists(object sender, EventArgs e)
        {
            if (this.PreviewMode)
            {
                return;
            }
            string senderName = (sender as BaseEdit).Name;
            string code = ((BaseEdit)sender).EditValue == null || ((BaseEdit)sender).EditValue.GetType() == typeof(System.DBNull)
                         ? String.Empty
                         : (string)(sender as BaseEdit).EditValue;
            SupplierNew supplier = null;
            Customer customer = null;
            Trader trader = null;

            if (senderName == "txtTaxCode")
            {
                trader = this.Supplier.Session.FindObject<Trader>(CriteriaOperator.And(new BinaryOperator("TaxCode", code),
                                                                new BinaryOperator("Oid", this.Supplier.Trader.Oid, BinaryOperatorType.NotEqual)));
                if (trader != null)
                {
                    supplier = trader.Suppliers.FirstOrDefault();
                    if (supplier == null)
                    {
                        customer = trader.Customers.FirstOrDefault();
                    }
                }
            }
            else if (senderName == "txtCode")
            {
                supplier = this.Supplier.Session.FindObject<SupplierNew>(CriteriaOperator.And(new BinaryOperator("Code", code),
                                                                        new BinaryOperator("Oid", this.Supplier.Oid, BinaryOperatorType.NotEqual)));
                if (supplier != null)
                {
                    trader = supplier.Trader;

                }
                else
                {
                    string taxCode = txtTaxCode.EditValue == null || txtTaxCode.EditValue.GetType() == typeof(System.DBNull)
                                     ? String.Empty
                                     : (string)txtTaxCode.EditValue;

                    customer = this.Supplier.Session.FindObject<Customer>(CriteriaOperator.And(new BinaryOperator("Code", code), new BinaryOperator("Trader.TaxCode", taxCode, BinaryOperatorType.NotEqual)));
                    if (customer != null)
                    {
                        trader = customer.Trader;
                    }
                }
            }

            if (trader != null)
            {
                string confirmMessage = Resources.TraderFoundWithSameCode + Environment.NewLine + trader.Code + Environment.NewLine
                                               + String.Format(Resources.WouldYouLikeToOpenTheCurrentTrader, Resources.Trader);

                if (XtraMessageBox.Show(confirmMessage, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.FormClosing -= this.SupplierEditForm_FormClosing;
                    this.Hide();
                    this.Supplier.Session.RollbackTransaction();
                    this.Close();

                    if (supplier == null)
                    {
                        supplier = new SupplierNew(this.Supplier.Session);
                        supplier.Trader = trader;
                        supplier.CompanyName = customer == null ? "" : customer.CompanyName;
                        supplier.Owner = this.Supplier.Session.GetObjectByKey<CompanyNew>(Program.Settings.StoreControllerSettings.Owner.Oid);
                        if (senderName == "txtCode" && txtCode.EditValue != null)
                        {
                            supplier.Code = txtCode.EditValue.ToString();
                        }
                    }
                    this.DuplicateTraderFound(supplier);
                }
                else
                {
                    (sender as BaseEdit).EditValue = null;
                    (sender as BaseEdit).Focus();
                }
            }
        }
    }
}
