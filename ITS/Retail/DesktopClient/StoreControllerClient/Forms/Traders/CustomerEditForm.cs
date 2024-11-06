using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using ITS.Retail.DesktopClient.StoreControllerClient.ITSStoreControllerDesktopPOSUpdateService;
using DevExpress.XtraGrid.Columns;
using System.Threading;
using System.Globalization;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.DesktopClient.StoreControllerClient.Forms.Traders;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class CustomerEditForm : XtraLocalizedForm
    {
        protected Customer _Customer;
        protected Customer Customer { get { return _Customer; } }

        private bool result { get; set; } = false;

        private String CompanyNameForm { get; set; } = String.Empty;
        private String AddressForm { get; set; } = String.Empty;
        private String CityForm { get; set; } = String.Empty;
        private String NumberForm { get; set; } = String.Empty;
        private String PostCodeForm { get; set; } = String.Empty;

        private string OldCustomerChildSerialized { get; set; }
        private string OldCustomerStorePriceListSerialized { get; set; }
        private string OldCustomerAddressSerialized { get; set; }
        private string OldCustomerAnalyticTreeSerialized { get; set; }

        private Address _EditingAddress;
        protected Address EditingAddress { get { return _EditingAddress; } set { _EditingAddress = value; } }

        private CustomerStorePriceList _EditingCustomerStorePriceList;
        protected CustomerStorePriceList EditingCustomerStorePriceList { get { return _EditingCustomerStorePriceList; } set { _EditingCustomerStorePriceList = value; } }

        private CustomerChild _EditingCustomerChild;
        protected CustomerChild EditingCustomerChild { get { return _EditingCustomerChild; } set { _EditingCustomerChild = value; } }


        private CustomerAnalyticTree _EditingCustomerAnalyticTree;
        protected CustomerAnalyticTree EditingCustomerAnalyticTree { get { return _EditingCustomerAnalyticTree; } set { _EditingCustomerAnalyticTree = value; } }

        public delegate void MyEvent(Customer customer);
        public event MyEvent DuplicateTraderFound;

        protected bool onInitialization;

        private bool PreviewCustomer { get; set; }
        protected bool CustomerIsSaved { get; set; }

        public CustomerEditForm(Customer customer, bool previewMode) : base()
        {
            this._Customer = customer;
            PreviewCustomer = previewMode;
            if (PreviewCustomer)
            {
                this.Text = Resources.PreviewDocument;
            }
            InitializeComponent();
            this.checkVatnumberBtn.Height = 20;
        }

        private void InitializeBindings()
        {
            this.lueTaxOfficeLookup.DataBindings.Add("EditValue", this.Customer.Trader, "TaxOfficeLookUp!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueVatLevel.DataBindings.Add("EditValue", this.Customer, "VatLevel!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueDefaultAddress.DataBindings.Add("EditValue", this.Customer, "DefaultAddress!", true, DataSourceUpdateMode.OnPropertyChanged);
            this.luePaymentMethod.DataBindings.Add("EditValue", this.Customer, "PaymentMethod!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueRefundStore.DataBindings.Add("EditValue", this.Customer, "RefundStore!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueSex.DataBindings.Add("EditValue", this.Customer, "Sex", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueMaritalStatus.DataBindings.Add("EditValue", this.Customer, "MaritalStatus", true, DataSourceUpdateMode.OnPropertyChanged);

            this.txtCode.DataBindings.Add("EditValue", this.Customer, "Code", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtTaxCode.DataBindings.Add("EditValue", this.Customer.Trader, "TaxCode", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtCompanyName.DataBindings.Add("EditValue", this.Customer, "CompanyName", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtProfession.DataBindings.Add("EditValue", this.Customer, "Profession", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtFirstName.DataBindings.Add("EditValue", this.Customer.Trader, "FirstName", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtLastName.DataBindings.Add("EditValue", this.Customer.Trader, "LastName", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtCardID.DataBindings.Add("EditValue", this.Customer, "CardID", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtEmail.DataBindings.Add("EditValue", this.Customer, "Email", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtFatherName.DataBindings.Add("EditValue", this.Customer, "FatherName", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDogs.DataBindings.Add("EditValue", this.Customer, "Dogs", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtCats.DataBindings.Add("EditValue", this.Customer, "Cats", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtOtherPets.DataBindings.Add("EditValue", this.Customer, "OtherPets", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDiscount.DataBindings.Add("EditValue", this.Customer, "Discount", true, DataSourceUpdateMode.OnPropertyChanged);

            this.chkIsActive.DataBindings.Add("EditValue", this.Customer, "IsActive", true, DataSourceUpdateMode.OnPropertyChanged);
            this.chkBreakOrderToCentral.DataBindings.Add("EditValue", this.Customer, "BreakOrderToCentral", true, DataSourceUpdateMode.OnPropertyChanged);

            this.dtBirthDate.DataBindings.Add("EditValue", this.Customer, "BirthDate", true, DataSourceUpdateMode.OnPropertyChanged);

            this.txtGDPRAnonymizationDate.DataBindings.Add("EditValue", this.Customer, "GDPRAnonymizationDate", true, DataSourceUpdateMode.OnPropertyChanged, true);
            this.txtGDPRAnonymizationProtocolNumber.DataBindings.Add("EditValue", this.Customer, "GDPRAnonymizationProtocolNumber", true, DataSourceUpdateMode.OnPropertyChanged, true);
            try
            {
                this.txtGDPRAnonymizationUser.DataBindings.Add("EditValue", this.Customer.GDPRAnonymizationUser, "FullName", true, DataSourceUpdateMode.OnPropertyChanged, true);
            }
            catch { }
            this.txtGDPRComments.DataBindings.Add("EditValue", this.Customer, "GDPRComments", true, DataSourceUpdateMode.OnPropertyChanged, true);
            this.txtGDPRExportDate.DataBindings.Add("EditValue", this.Customer, "GDPRExportDate", true, DataSourceUpdateMode.OnPropertyChanged, true);
            this.txtGDPRExportProtocolNumber.DataBindings.Add("EditValue", this.Customer, "GDPRExportProtocolNumber", true, DataSourceUpdateMode.OnPropertyChanged, true);
            try
            {
                this.txtGDPRExportUser.DataBindings.Add("EditValue", this.Customer.GDPRExportUser, "FullName", true, DataSourceUpdateMode.OnPropertyChanged, true);
            }
            catch { }
            this.txtGDPRRegistrationDate.DataBindings.Add("EditValue", this.Customer.Trader, "GDPRRegistrationDate", true, DataSourceUpdateMode.OnPropertyChanged, true);
            this.txtGDPRRegistrationsProtocolNumber.DataBindings.Add("EditValue", this.Customer.Trader, "GDPRProtocolNumber", true, DataSourceUpdateMode.OnPropertyChanged, true);



            this.gridCustomerAddresses.DataSource = this.Customer.Trader.Addresses;
            this.gridCustomerStorePriceLists.DataSource = this.Customer.CustomerStorePriceLists;
            this.gridCustomerChild.DataSource = this.Customer.CustomerChilds;
            this.gridCustomerCategories.DataSource = this.Customer.CustomerAnalyticTrees;

            this.textEditCustomerBalance.EditValue = this.Customer.Balance;

            this.txtDiscount.Enabled = true;

        }

        private void InitializeLookupEdits()
        {
            CriteriaOperator criteria = null;

            lueTaxOfficeLookup.Properties.View.Columns.Add(new GridColumn() { FieldName = "Code", Caption = Resources.Code, Visible = true, VisibleIndex = 0 });
            lueTaxOfficeLookup.Properties.View.Columns.Add(new GridColumn() { FieldName = "Description", Caption = Resources.Description, Visible = true, VisibleIndex = 1 });
            lueTaxOfficeLookup.Properties.DataSource = new XPCollection<TaxOffice>(this.Customer.Session, criteria);

            lueTaxOfficeLookup.Properties.ValueMember = "Oid";
            lueTaxOfficeLookup.Properties.DisplayFormat.FormatString = "{0} {1}";

            lueVatLevel.Properties.DataSource = new XPCollection<VatLevel>(this.Customer.Session, criteria);
            lueVatLevel.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueVatLevel.Properties.ValueMember = "Oid";

            lueDefaultAddress.Properties.DataSource = this.Customer.Trader.Addresses;
            lueDefaultAddress.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueDefaultAddress.Properties.ValueMember = "This";

            luePaymentMethod.Properties.DataSource = new XPCollection<PaymentMethod>(this.Customer.Session, criteria);
            luePaymentMethod.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            luePaymentMethod.Properties.ValueMember = "Oid";

            lueRefundStore.Properties.DataSource = new XPCollection<Store>(this.Customer.Session, criteria);
            lueRefundStore.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            lueRefundStore.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueRefundStore.Properties.ValueMember = "Oid";
            lueRefundStore.Properties.DisplayFormat.FormatString = "{0} {1}";

            lueSex.Properties.DataSource = Enum<eSex>.GetLocalizedDictionary();
            lueSex.Properties.Columns.Add(new LookUpColumnInfo("Value", Resources.Description));
            lueSex.Properties.ValueMember = "Key";
            lueSex.Properties.DisplayMember = "Value";

            lueMaritalStatus.Properties.DataSource = Enum<eMaritalStatus>.GetLocalizedDictionary();
            lueMaritalStatus.Properties.Columns.Add(new LookUpColumnInfo("Value", Resources.Description));
            lueMaritalStatus.Properties.ValueMember = "Key";
            lueMaritalStatus.Properties.DisplayMember = "Value";

            searchItemLookUpEditSex.DataSource = Enum<eSex>.GetLocalizedDictionary();
            searchItemLookUpEditSex.Columns.Add(new LookUpColumnInfo("Value", Resources.Description));

            itemLookUpEditStorePriceCatalog.DataSource = new XPCollection<StorePriceList>(this.Customer.Session,
                new BinaryOperator("Store.Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid)).ToList();

            itemLookUpEditStorePriceCatalog.Columns.Add(new LookUpColumnInfo("Store", Resources.Store));
            itemLookUpEditStorePriceCatalog.Columns.Add(new LookUpColumnInfo("PriceList", Resources.PriceCatalog));

        }

        private void CustomerEditForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                onInitialization = false;
                return;
            }
            if (PreviewCustomer)
            {
                this.btnCancelCustomer.Text = Resources.Close;
            }

            InitializeLookupEdits();
            InitializeBindings();

            if (PreviewCustomer)
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

                grdColumnEdit.Visible = false;
                grdDeleteBtn.Visible = false;
                grdEditCustomerStorePriceLists.Visible = false;
                grdDeleteCustomerStorePriceLists.Visible = false;

                layoutControlItem9.HideToCustomization();
                layoutControlItem3.HideToCustomization();
                lueTaxOfficeLookup.Properties.Buttons.Clear();
                lueRefundStore.Properties.Buttons.Clear();
            }

            this.tabCustomerTabs.SelectedTabPageIndex = 0;
            this.Customer.Changed += Customer_Changed;
            onInitialization = false;
            xtpCustomerStorePriceList.PageVisible = false;
            if (!Program.Settings.CurrentUser.Role.GDPREnabled)
            {
                //grdCustomerAddresses.Columns.Remove(grdPhone);
                lcEmail.HideToCustomization();
                lcFatherName.HideToCustomization();
                lcDogs.HideToCustomization();
                lcCats.HideToCustomization();
                lcOtherPets.HideToCustomization();
                lcSex.HideToCustomization();
                lcMaritalStatus.HideToCustomization();
                lcBirthDate.HideToCustomization();
                lcFirstName.HideToCustomization();
                lcLastName.HideToCustomization();
                xtpCustomerChild.PageVisible = false;
            }
        }

        protected virtual void Customer_Changed(object sender, ObjectChangeEventArgs e)
        {

        }

        //Delete Actions

        private void btn_del_CustomerStorePriceList_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this._EditingCustomerStorePriceList = this.grdCustomerStorePriceLists.GetFocusedRow() as CustomerStorePriceList;
            this.EditingCustomerStorePriceList.Delete();
            this.grdCustomerStorePriceLists.RefreshData();
        }

        private void btn_del_CustomerChild_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this._EditingCustomerChild = this.grdChildInfo.GetFocusedRow() as CustomerChild;
            this.EditingCustomerChild.Delete();
            this.grdChildInfo.RefreshData();
        }

        private void btn_delete_adrs_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this._EditingAddress = this.grdCustomerAddresses.GetFocusedRow() as Address;
            if (this.Customer.DefaultAddress != null && this.Customer.DefaultAddress.Oid == this.EditingAddress.Oid)
            {
                this.Customer.DefaultAddress = null;
            }
            this.EditingAddress.Delete();
            this.grdCustomerAddresses.RefreshData();
        }

        private void btn_delete_category_Click(object sender, EventArgs e)
        {
            this._EditingCustomerAnalyticTree = this.grdCustomerCategories.GetFocusedRow() as CustomerAnalyticTree;
            this.EditingCustomerAnalyticTree.Delete();
            this.grdCustomerCategories.RefreshData();
        }

        //Add Actions

        private void btnAddCustomerStorePriceList_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.None;
            this._EditingCustomerStorePriceList = new CustomerStorePriceList(this.Customer.Session);
            using (CustomerStorePriceListInlineEdit customerStorePriceListForm = new CustomerStorePriceListInlineEdit(this.EditingCustomerStorePriceList))
            {
                dialogResult = customerStorePriceListForm.ShowDialog();
            }
            if (dialogResult == DialogResult.OK)
            {
                this.EditingCustomerStorePriceList.Save();
                this.Customer.CustomerStorePriceLists.Add(this.EditingCustomerStorePriceList);
                this.grdCustomerStorePriceLists.RefreshData();
            }
            else
            {
                this.EditingCustomerStorePriceList.Delete();
            }
        }


        private void btnAddCustomerChild_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.None;
            this._EditingCustomerChild = new CustomerChild(this.Customer.Session);
            using (CustomerChildInlineEdit customerChildForm = new CustomerChildInlineEdit(this.EditingCustomerChild))
            {
                dialogResult = customerChildForm.ShowDialog();
            }
            if (dialogResult == DialogResult.OK)
            {
                this.EditingCustomerChild.Save();
                this.Customer.CustomerChilds.Add(this.EditingCustomerChild);
                this.grdChildInfo.RefreshData();
            }
            else
            {
                this.EditingCustomerChild.Delete();
            }
        }

        private void btnAddCustomerAddress_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.None;
            this._EditingAddress = new Address(this.Customer.Session);
            if (result)
            {
                this._EditingAddress.Street = this.AddressForm + " " + this.NumberForm;
                this._EditingAddress.City = this.CityForm;
                this._EditingAddress.PostCode = this.PostCodeForm;
                this._EditingAddress.Profession = this.txtProfession.Text;

            }
            using (CustomerAddressInlineEdit customerAddressForm = new CustomerAddressInlineEdit(this.EditingAddress))
            {
                dialogResult = customerAddressForm.ShowDialog();
            }
            if (dialogResult == DialogResult.OK)
            {
                this.EditingAddress.Save();
                this.Customer.Trader.Addresses.Add(this.EditingAddress);
                this.grdCustomerAddresses.RefreshData();
            }
            else
            {
                this.EditingAddress.Delete();
            }
        }

        private void btnAddCustomerCategory_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.None;
            this._EditingCustomerAnalyticTree = new CustomerAnalyticTree(this.Customer.Session);
            this.EditingCustomerAnalyticTree.Object = this.Customer;
            using (CustomerCategoryInlineEdit customerCategoryForm = new CustomerCategoryInlineEdit(this.EditingCustomerAnalyticTree))
            {
                dialogResult = customerCategoryForm.ShowDialog();
            }
            if (dialogResult == DialogResult.OK)
            {
                this.EditingCustomerAnalyticTree.Save();
                this.EditingCustomerAnalyticTree.Root = this.EditingCustomerAnalyticTree.Node.GetRoot(this.Customer.Session);
                this.Customer.CustomerAnalyticTrees.Add(this.EditingCustomerAnalyticTree);
                this.grdCustomerCategories.RefreshData();
            }
            else
            {
                this.EditingCustomerAnalyticTree.Delete();
            }
        }

        //Edit Actions

        private void btnEditCustomerStorePriceLists_Click(object sender, EventArgs e)
        {
            EditCustomerStorePriceList();
        }

        private void EditCustomerStorePriceList()
        {
            if (!PreviewCustomer)
            {
                DialogResult dialogResult = DialogResult.None;
                this._EditingCustomerStorePriceList = this.grdCustomerStorePriceLists.GetFocusedRow() as CustomerStorePriceList;
                this.OldCustomerStorePriceListSerialized = this.EditingCustomerStorePriceList.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                using (CustomerStorePriceListInlineEdit customerChildForm = new CustomerStorePriceListInlineEdit(this.EditingCustomerStorePriceList))
                {
                    dialogResult = customerChildForm.ShowDialog();
                }
                if (dialogResult == DialogResult.OK)
                {
                    this.grdCustomerStorePriceLists.RefreshData();
                }
                else
                {
                    string error;
                    bool result = this.EditingCustomerStorePriceList.FromJson(this.OldCustomerStorePriceListSerialized, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    this.EditingCustomerStorePriceList.Customer = this.Customer;
                }
            }
        }

        private void EditCustomerChild()
        {
            if (!PreviewCustomer)
            {
                DialogResult dialogResult = DialogResult.None;
                this._EditingCustomerChild = this.grdChildInfo.GetFocusedRow() as CustomerChild;
                this.OldCustomerChildSerialized = this.EditingCustomerChild.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                using (CustomerChildInlineEdit customerChildForm = new CustomerChildInlineEdit(this.EditingCustomerChild))
                {
                    dialogResult = customerChildForm.ShowDialog();
                }
                if (dialogResult == DialogResult.OK)
                {
                    this.grdChildInfo.RefreshData();
                }
                else
                {
                    string error;
                    bool result = this.EditingCustomerChild.FromJson(this.OldCustomerChildSerialized, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    this.EditingCustomerChild.Customer = this.Customer;
                }
            }
        }

        private void gridCustomerAddresses_DoubleClick(object sender, EventArgs e)
        {
            EditCustomerAddress();
        }

        private void btnEditGridRow_Click(object sender, EventArgs e)
        {
            EditCustomerAddress();
        }

        private void EditCustomerAddress()
        {
            if (!PreviewCustomer)
            {
                DialogResult dialogResult = DialogResult.None;
                this._EditingAddress = this.grdCustomerAddresses.GetFocusedRow() as Address;
                this.OldCustomerAddressSerialized = this.EditingAddress.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                using (CustomerAddressInlineEdit customerAddressForm = new CustomerAddressInlineEdit(this.EditingAddress))
                {
                    dialogResult = customerAddressForm.ShowDialog();
                }
                if (dialogResult == DialogResult.OK)
                {
                    this.grdCustomerAddresses.RefreshData();
                }
                else
                {
                    string error;
                    bool result = this.EditingAddress.FromJson(this.OldCustomerAddressSerialized, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    this.EditingAddress.Trader = this.Customer.Trader;
                }
            }
        }

        private void btnSaveCustomer_Click(object sender, EventArgs e)
        {
            string message;
            bool taxOfficeMissing;

            if (ObjectCanBeSaved(out message, out taxOfficeMissing))
            {

                this.SaveCustomer();
            }
            else if (taxOfficeMissing)
            {
                DialogResult result = XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {

                    this.SaveCustomer();
                }
                this.tabCustomerTabs.SelectedTabPageIndex = 0;
            }
            else
            {
                XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.tabCustomerTabs.SelectedTabPageIndex = 0;
            }
        }

        private bool ObjectCanBeSaved(out string errormessage, out bool taxOfficeMissing)
        {
            taxOfficeMissing = false;
            bool result = (this.txtCode.EditValue != null &&
                            this.txtTaxCode.EditValue != null &&
                            this.txtCompanyName.EditValue != null &&
                            !String.IsNullOrEmpty(this.txtProfession.EditValue.ToString()) &&
                            !String.IsNullOrEmpty(this.lueVatLevel.EditValue.ToString()));
            if (!result)
            {
                if (this.txtCode.EditValue == null || String.IsNullOrWhiteSpace(this.txtCode.EditValue.ToString()))
                {
                    this.txtCode.Focus();
                }
                else if (this.txtTaxCode.EditValue == null || String.IsNullOrWhiteSpace(this.txtTaxCode.EditValue.ToString()))
                {
                    this.txtTaxCode.Focus();
                }
                else if (this.txtCompanyName.EditValue == null || String.IsNullOrWhiteSpace(this.txtCompanyName.EditValue.ToString()))
                {
                    this.txtCompanyName.Focus();
                }
                else if (this.txtProfession.EditValue == null || String.IsNullOrWhiteSpace(this.txtProfession.EditValue.ToString()))
                {
                    this.txtProfession.Focus();
                }
                else if (this.lueVatLevel.EditValue == null || String.IsNullOrWhiteSpace(this.lueVatLevel.EditValue.ToString()))
                {
                    this.lueVatLevel.Focus();
                }

                errormessage = Resources.FillAllMissingFields;
            }
            else //Checks for Tax Office
            {
                result = !String.IsNullOrEmpty(this.lueTaxOfficeLookup.EditValue.ToString());
                errormessage = Resources.TaxofficeIsNotSelected;
                taxOfficeMissing = true;
            }
            return result;
        }

        private void btnCancelCustomer_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CustomerEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CustomerIsSaved == false)
            {
                e.Cancel = !this.CancelCustomer();
            }
        }

        private bool CancelCustomer()
        {
            if (this.PreviewCustomer || XtraMessageBox.Show(Resources.Cancel, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.Customer.Changed -= this.Customer_Changed;
                if (this.PreviewCustomer == false)
                {
                    this.Customer.Session.RollbackTransaction();
                }
                return true;
            }
            return false;
        }

        protected void SaveCustomer()
        {
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            try
            {
                if (String.IsNullOrEmpty(this.Customer.Code))
                {
                    XtraMessageBox.Show(Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (this.Customer.Trader.Addresses.Count == 0)
                {
                    XtraMessageBox.Show(Resources.NoAddressInserted, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    if (Program.Settings.MasterAppInstance == eApplicationInstance.STORE_CONTROLER)
                    {

                        Thread.CurrentThread.CurrentCulture = PlatformConstants.DefaultCulture;
                        using (POSUpdateService webservice = new POSUpdateService())
                        {
                            webservice.Timeout = 300000;
                            webservice.Url = Program.Settings.MasterURLService;
                            string toSend = this.Customer.FullJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, false), message;
                            if (webservice.InsertOrUpdateRecord(Program.Settings.StoreControllerSettings.Oid, "Customer", toSend, out message) == false)
                            {
                                XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            var objectsToBeSaved = this.Customer.Session.GetObjectsToSave().Cast<object>().Where(G => G is BaseObj).ToList();
                            foreach (BaseObj baseObject in objectsToBeSaved)
                            {
                                if (baseObject.IsDeleted)
                                {
                                    webservice.DeleteRecord(Program.Settings.StoreControllerSettings.Oid,
                                        baseObject.GetType().Name,
                                        baseObject.Oid,
                                        out message
                                        );
                                }
                            }
                        }
                    }

                    if (this.txtDiscount.Value > 100)
                    {
                        this.txtDiscount.Value = 100;
                    }

                    if (this.txtDiscount.Value < 0)
                    {
                        this.txtDiscount.Value = 0;
                    }



                    this.Customer.Save();
                    this.Customer.Session.CommitTransaction();
                    this.CustomerIsSaved = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
            }
        }

        private void grdChildInfo_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "ChildSex")
                e.DisplayText = ((eSex)e.Value).ToLocalizedString();
        }

        private void lueRefundStore_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this.lueRefundStore.EditValue = null;
        }

        private void lueTaxOfficeLookup_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this.lueTaxOfficeLookup.EditValue = null;
        }

        private void btnEditChildInfo_Click(object sender, EventArgs e)
        {
            EditCustomerChild();
        }


        private void grdChildInfo_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column == grdDeleteButton && grdChildInfo.GetRow(e.RowHandle) as CustomerChild == null)
            {
                e.Handled = true;
            }
        }

        private void grdCustomerStorePriceLists_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column == grdDeleteCustomerStorePriceLists && grdChildInfo.GetRow(e.RowHandle) as StorePriceList == null)
            {
                e.Handled = true;
            }
        }

        private void grdCustomerStorePriceLists_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            CustomerStorePriceList cspl = e.Row as CustomerStorePriceList;
            if (cspl != null)
            {
                if (cspl.IsDefault && this.Customer.CustomerStorePriceLists.Any(x => x.Oid != cspl.Oid && x.IsDefault))
                {
                    e.ErrorText = Resources.IsDefaultUniqueFieldsAreNotCorrectlyDefined;
                    e.Valid = false;
                    return;
                }
                if (this.Customer.CustomerStorePriceLists.Any(x => x.Oid != cspl.Oid && x.StorePriceList.Store.Oid == cspl.StorePriceList.Store.Oid))
                {
                    e.ErrorText = Resources.CustomerCanNotHaveMoreThanOnePricatalogInOneStore;
                    e.Valid = false;
                    return;
                }
            }
        }

        private void CustomerEditForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            this.BringToFront();
            this.txtCode.Focus();
        }

        public void CheckIfTraderExists(object sender, EventArgs e)
        {
            if (this.PreviewCustomer)
            {
                return;
            }
            string senderName = ((BaseEdit)sender).Name;
            string code = ((BaseEdit)sender).EditValue == null || ((BaseEdit)sender).EditValue.GetType() == typeof(System.DBNull)
                         ? String.Empty
                         : (string)((BaseEdit)sender).EditValue;

            Customer customer = null;
            SupplierNew supplier = null;
            Trader trader = null;
            if (senderName == "txtTaxCode")
            {
                trader = this.Customer.Session.FindObject<Trader>(CriteriaOperator.And(new BinaryOperator("TaxCode", code),
                                                                                              new BinaryOperator("Oid", this.Customer.Trader.Oid, BinaryOperatorType.NotEqual)));
                if (trader != null)
                {
                    customer = trader.Customers.FirstOrDefault();
                    if (customer == null)
                    {
                        supplier = trader.Suppliers.FirstOrDefault();
                    }
                }
            }
            else if (senderName == "txtCode")
            {
                customer = this.Customer.Session.FindObject<Customer>(CriteriaOperator.And(new BinaryOperator("Code", code),
                                                                                           new BinaryOperator("Oid", this.Customer.Oid, BinaryOperatorType.NotEqual)));
                if (customer != null)
                {
                    trader = customer.Trader;
                }
                else
                {
                    string taxCode = txtTaxCode.EditValue == null || txtTaxCode.EditValue.GetType() == typeof(System.DBNull)
                                     ? String.Empty
                                     : (string)txtTaxCode.EditValue;

                    supplier = this.Customer.Session.FindObject<SupplierNew>(CriteriaOperator.And(new BinaryOperator("Code", code), new BinaryOperator("Trader.TaxCode", taxCode, BinaryOperatorType.NotEqual)));
                    if (supplier != null)
                    {
                        trader = supplier.Trader;
                    }
                }
            }
            if (trader != null)
            {
                string confirmMessage = Resources.TraderFoundWithSameCode + Environment.NewLine + trader.Code + Environment.NewLine
                                              + String.Format(Resources.WouldYouLikeToOpenTheCurrentTrader, Resources.Trader);
                if (XtraMessageBox.Show(confirmMessage, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.FormClosing -= this.CustomerEditForm_FormClosing;
                    this.Hide();
                    this.Customer.Session.RollbackTransaction();
                    this.Close();

                    if (customer == null)
                    {
                        customer = new Customer(this.Customer.Session);
                        customer.Trader = trader;
                        customer.CompanyName = supplier == null ? "" : supplier.CompanyName;
                        customer.Owner = this.Customer.Session.GetObjectByKey<CompanyNew>(Program.Settings.StoreControllerSettings.Owner.Oid);
                        if (senderName == "txtCode" && txtCode.EditValue != null)
                        {
                            customer.Code = txtCode.EditValue.ToString();
                        }
                    }

                    this.DuplicateTraderFound(customer);
                }
                else
                {
                    ((BaseEdit)sender).EditValue = null;
                    ((BaseEdit)sender).Focus();
                }
            }
        }

        private void checkVatnumberBtn_Click(object sender, EventArgs e)
        {

            String name = String.Empty;
            String address = String.Empty;
            bool isValid = false;
            String TaxCode = this.txtTaxCode.Text;
            String errorMessage = ValidateTaxcode(TaxCode);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            String CompanyName = String.Empty;
            String CompanyAddress = String.Empty;
            String PostCode = String.Empty;
            String AddressNo = String.Empty;
            String City = String.Empty;

            this.result = false;
            this.CompanyNameForm = String.Empty;
            this.AddressForm = String.Empty;
            this.NumberForm = String.Empty;
            this.AddressForm = String.Empty;
            this.CityForm = String.Empty;
            this.PostCodeForm = String.Empty;


            String CountryCode = "EL";

            if (Char.IsDigit(TaxCode[0]) == false && Char.IsDigit(TaxCode[1]) == false)
                CountryCode = TaxCode[0].ToString() + TaxCode[1].ToString();


            String NumericCode = GetNumeric(TaxCode);
            CustomerHelper.CheckTaxCodeOnViesApi(NumericCode, CountryCode, out name, out address, out isValid);


            if (isValid == true)
            {
                CompanyName = name.Replace("|", "");
                String tempAddress = address.Replace("|", "");

                string[] tempStringArray = tempAddress.Split(new char[0]);

                int t = 0;
                for (int i = 0; i < tempStringArray.Length; i++)
                    if (tempStringArray[i].Length > 1)
                        t++;

                string[] stringArray = new string[t];
                int postCodeArrayIndex = 0;

                try
                {
                    int k = 0;
                    for (int i = 0; i < tempStringArray.Length; i++)
                        if (tempStringArray[i].Length > 1)
                        {
                            stringArray[k] = tempStringArray[i].Replace(" ", "");
                            k++;
                        }




                    int postCodeStartIndex = 0;
                    for (int i = 0; i < stringArray.Length; i++)
                    {
                        string part = stringArray[i];
                        for (int j = 0; j < part.Length; j++)
                        {
                            if (Char.IsDigit(part[j]) == true && part.Length > 3)
                            {
                                PostCode = part;
                                postCodeArrayIndex = i;
                                postCodeStartIndex = tempAddress.IndexOf(PostCode);
                                i = 100;
                                break;
                            }
                        }
                    }
                    String tempCity = tempAddress.Substring((postCodeStartIndex + 5), (tempAddress.Length - (postCodeStartIndex + 5)));
                    City = tempCity.Replace("-", "");
                    AddressNo = stringArray[postCodeArrayIndex - 1];
                    int addressNoStartIndex = tempAddress.IndexOf(AddressNo);
                    CompanyAddress = (tempAddress.Substring(0, addressNoStartIndex));

                    this.CompanyNameForm = CompanyName;
                    this.AddressForm = AddressForm;
                    this.NumberForm = AddressNo;
                    this.AddressForm = CompanyAddress;
                    this.CityForm = City;
                    this.PostCodeForm = PostCode;
                }
                catch (Exception ex) { }

                try
                {
                    TaxCodeValidated taxcodeForm = new TaxCodeValidated(CompanyName, CompanyAddress, City, AddressNo, PostCode, this);
                    taxcodeForm.Owner = this;
                    taxcodeForm.Show();
                    taxcodeForm.Dock = DockStyle.Fill;
                    taxcodeForm.BringToFront();
                    taxcodeForm.TopMost = true;
                }
                catch (Exception ex) { }
            }

            else
            {
                MessageBox.Show(Resources.CouldNotValidateTaxcode);
            }
        }

        private static String ValidateTaxcode(String TaxCode)
        {
            String errorMessage = "";

            if (string.IsNullOrEmpty(TaxCode))
            {
                errorMessage = @ITS.Retail.ResourcesLib.Resources.TaxCodeCannotBeNull;
                return errorMessage;
            }

            String numericCode = GetNumeric(TaxCode);

            if (numericCode.Length < 9)
            {
                errorMessage = @ITS.Retail.ResourcesLib.Resources.TaxCodeShouldbe9digits;
                return errorMessage;
            }

            for (var i = 0; i < TaxCode.Length; i++)
            {
                if (Char.IsDigit(TaxCode[i]) == false && i > 1)
                {
                    errorMessage = @ITS.Retail.ResourcesLib.Resources.OnlyDigitsAllowed;
                    return errorMessage;
                }
            }
            return errorMessage;
        }

        private static String GetNumeric(String text)
        {
            List<Char> numeric = new List<Char>();
            String result = String.Empty;

            for (int i = 0; i < text.Length; i++)
                if (Char.IsDigit(text[i]) == true)
                    numeric.Add(text[i]);

            foreach (Char c in numeric)
                result += c;

            return result;
        }


        public void SetValuesFromCheckTaxCode(String Name, String Address, String City, String Number, String PostCode)
        {
            this.txtCompanyName.Text = Name;
            this.txtCode.Text = this.txtTaxCode.Text;
            this.lueVatLevel.EditValue = CustomerHelper.GetDefaultVatLevel();
        }

        public void SetResult(bool res)
        {
            this.result = res;
        }



    }
}
