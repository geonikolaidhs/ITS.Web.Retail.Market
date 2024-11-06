using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Enumerations.ViewModel;
using DevExpress.Xpo;
using Newtonsoft.Json;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using ITS.POS.Model.Settings;
using ITS.POS.Client.UserControls;

namespace ITS.POS.Client.Forms
{
    public partial class frmSelectCustomer : frmInputFormBase
    {
        /// <summary>
        /// Gets the customer selected by the user
        /// </summary>
        public Customer SelectedCustomer { get; protected set; }

        /// <summary>
        /// Gets the inserted customer info
        /// </summary>
        public string CustomerLookupCode { get; protected set; }

        /// <summary>
        /// Gets the selected customer's Address
        /// </summary>
        public Address SelectedAddress { get; protected set; }


        public InsertedCustomerViewModel CustomerViewModel { get; protected set; }

        protected BindingList<InsertedCustomerViewModel> Customers;



        public frmSelectCustomer(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();
            btnOK.Text = POSClientResources.OK;
            btnCancel.Text = POSClientResources.CANCEL;
            lblTitle.Text = POSClientResources.CUSTOMER_CODE;

            //InitBackGroundOnFocus();

            InitBindings();
            edtCustomerCode.Focus();
        }

        //private void InitBackGroundOnFocus()
        //{
        //    foreach(ucTextEdit txtEdit in this.Controls)
        //    {

        //        txtEdit.Properties.AppearanceFocused.BackColor = System.Drawing.Color.LightYellow;
        //    }
        //}

        private void InitBindings()
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            txtTaxOffice.Properties.DataSource = new XPCollection<TaxOffice>(sessionManager.GetSession<TaxOffice>());
            txtTaxOffice.Properties.ValueMember = "Oid";
            txtTaxOffice.Properties.DisplayMember = "Description";
            txtTaxOffice.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Κωδικός", 50));
            txtTaxOffice.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "ΔΟY", 150));


            Customers = new BindingList<InsertedCustomerViewModel>();
            bsCustomers.DataSource = Customers;
            txtAddressCity.DataBindings.Add("EditValue", bsCustomers, "City");
            txtAddressPostalCode.DataBindings.Add("EditValue", bsCustomers, "PostalCode");
            txtAddressStreet.DataBindings.Add("EditValue", bsCustomers, "Street");
            txtAddressProfession.DataBindings.Add("EditValue", bsCustomers, "AddressProfession");
            //txtAddressProfession.DataBindings.Add("EditValue", bsCustomers, "Profession");
            txtCompanyName.DataBindings.Add("EditValue", bsCustomers, "CompanyName");
            txtCustomerCode.DataBindings.Add("EditValue", bsCustomers, "Code");
            txtName.DataBindings.Add("EditValue", bsCustomers, "FirstName");
            txtPhone.DataBindings.Add("EditValue", bsCustomers, "Phone");
            txtCardID.DataBindings.Add("EditValue", bsCustomers, "CardID");
            txtSurname.DataBindings.Add("EditValue", bsCustomers, "LastName");
            txtTaxCode.DataBindings.Add("EditValue", bsCustomers, "TaxCode");
            txtTaxOffice.DataBindings.Add("EditValue", bsCustomers, "TaxOfficeLookup");
            txtThirdPartNum.DataBindings.Add("EditValue", bsCustomers, "ThirdPartNum");

            SetMoveButtonsState();
            EnableControls(false, false);
        }

        private void edtCustomerCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.Customers.Count > 0)
            {
                btnOK.PerformClick();
            }
            else if (e.KeyCode == Keys.Enter && this.Customers.Count == 0)
            {
                btnSearch.PerformClick();
            }
            else if (e.KeyCode == Keys.Down && this.Customers.Count > 0)
            {
                btnNextAddress.PerformClick();
            }
            else if (e.KeyCode == Keys.Up && this.Customers.Count > 0)
            {
                btnPreviousAddress.PerformClick();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IAppContext applicationContext = Kernel.GetModule<IAppContext>();
            if (this.Customers.Count == 0 && string.IsNullOrWhiteSpace(edtCustomerCode.Text))
            {
                formManager.ShowCancelOnlyMessageBox(POSClientResources.INSERT_VALUES);
                return;
            }
            if (this.Customers.Count > 0)
            {
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                InsertedCustomerViewModel selectedCustomer = bsCustomers.Current as InsertedCustomerViewModel;
                if (
                    (selectedCustomer.IsNew && validationProviderCustomer.Validate() == false)
                    ||
                    (selectedCustomer.AddressIsNew && validationProviderCustomerAddress.Validate() == false)
                    )
                {
                    return;
                }

                if (selectedCustomer.IsNew && ((Guid)txtTaxOffice.EditValue == Guid.Empty))
                {
                    formManager.ShowCancelOnlyMessageBox(POSClientResources.TAX_OFFICE_MISSING);
                    return;
                }

                if (selectedCustomer.IsSupplier)
                {
                    if (DialogResult.Cancel == formManager.ShowMessageBox(POSClientResources.SUPPLIER_FOUND + Environment.NewLine + POSClientResources.CREATE_CUSTOMER_FROM_SUPPLIER, MessageBoxButtons.OKCancel))
                    {
                        return;
                    }
                    selectedCustomer.IsNew = true;
                }

                this.SelectedCustomer = sessionManager.GetObjectByKey<Customer>(selectedCustomer.CustomerOid);
                this.SelectedAddress = sessionManager.GetObjectByKey<Address>(selectedCustomer.AddressOid);
                if (applicationContext.CurrentDocument != null)
                {
                    applicationContext.CurrentDocument.DenormalisedAddress = selectedCustomer.AddressOid;
                    //applicationContext.CurrentDocument.AddressProfession = selectedCustomer.AddressProfession;
                }
                if (selectedCustomer.IsNew)
                {
                    selectedCustomer.Profession = selectedCustomer.AddressProfession;
                }
                this.CustomerViewModel = selectedCustomer;
            }
            this.CustomerLookupCode = edtCustomerCode.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ShowHideAddressNavigation(true);
            ICustomerService customerService = Kernel.GetModule<ICustomerService>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();

            this.Customers.Clear();
            btnAddNewCustomer.Enabled = false;
            btnAddNewCustomerAddress.Enabled = false;
            List<InsertedCustomerViewModel> customerList = new List<InsertedCustomerViewModel>();
            if (string.IsNullOrWhiteSpace(edtCustomerCode.Text) == false)
            {
                IEnumerable<InsertedCustomerViewModel> tempCustomerList = customerService.GetValidCustomers(edtCustomerCode.Text);

                if (tempCustomerList == null || tempCustomerList.Count() == 0)
                {
                    InsertedCustomerViewModel customerViewModel = JsonConvert.DeserializeObject<InsertedCustomerViewModel>
                        (appContext.CurrentDocument == null || appContext.CurrentDocument.DenormalizedCustomer == null ? "" :
                        appContext.CurrentDocument.DenormalizedCustomer);
                    if (customerService.SearchDenormalizedCustomer(edtCustomerCode.Text, customerViewModel))
                    {
                        customerList.Add(customerViewModel);
                    }
                }
                else
                {
                    if (appContext.CurrentDocument != null)
                    {
                        appContext.CurrentDocument.DenormalizedCustomer = "";
                    }
                    customerList = tempCustomerList.ToList();
                }
            }
            else
            {
                formManager.ShowCancelOnlyMessageBox(POSClientResources.INSERT_VALUES);
                return;
            }
            if (customerList == null || customerList.Count() == 0)
            {
                btnAddNewCustomer.Enabled = true;
                if (DialogResult.OK == formManager.ShowMessageBox(POSClientResources.CUSTOMER_NOT_FOUND + Environment.NewLine + POSClientResources.DO_YOU_WANT_TO_CREATE_NEW_CUSTOMER, MessageBoxButtons.OKCancel))
                {
                    btnAddNewCustomer.PerformClick();
                }
                return;
            }
            btnAddNewCustomerAddress.Enabled = true;
            customerList.ToList().ForEach(x => this.Customers.Add(x));
            customerList = customerList.GroupBy(x => x.TaxCode).Select(grp => grp.First()).ToList();
            bsCustomers.MoveFirst();
            if (customerList.Count > 1)
            {
                using (frmSelectCustomerGrid formGrid = new frmSelectCustomerGrid( Kernel , customerList, edtCustomerCode.Text, ""))
                {
                    switch (formGrid.ShowDialog())
                    {
                        case DialogResult.OK:
                            //SelectedCustomer = formGrid.Customers.Where(x => x.CustomerOid == formGrid.selectedCustomer.CustomerOid) as Customer;
                            //this.SelectedAddress = sessionManager.GetObjectByKey<Address>(formGrid.selectedCustomer.AddressOid);
                            customerList = this.Customers.ToList();
                            ClearControls();
                            customerList.Where(x => x.Code == formGrid.selectedCustomer.Code).ToList().ForEach(x => this.Customers.Add(x));
                            if (Customers.Count == 0)
                            {
                                Customers.Add(formGrid.selectedCustomer);
                            }
                            int index = bsCustomers.List.IndexOf(formGrid.selectedCustomer);
                            if (index >= 0)
                            {
                                bsCustomers.Position = index;
                            }
                            edtCustomerCode.Focus();
                            break;
                        case DialogResult.Retry:
                            edtCustomerCode.Text = formGrid.SearchText;
                            
                            btnSearch.PerformClick();
                            return;
                    }
                }
            }
            
            SetMoveButtonsState();
        }

        private void SetMoveButtonsState()
        {
            btnPreviousAddress.Enabled = this.Customers.Count > 1 && bsCustomers.Position > 0;
            btnNextAddress.Enabled = this.Customers.Count > 1 && bsCustomers.Position < this.Customers.Count - 1;
        }

        private void btnPreviousAddress_Click(object sender, EventArgs e)
        {
            bsCustomers.MovePrevious();
            SetMoveButtonsState();
        }

        private void btnNextAddress_Click(object sender, EventArgs e)
        {
            bsCustomers.MoveNext();
            SetMoveButtonsState();
        }

        public override void Update(KeyListenerParams parameters)
        {
            base.Update(parameters);
            if (parameters.NotificationType == eNotificationsTypes.ACTION && parameters.ActionCode == eActions.MOVE_UP && btnPreviousAddress.Enabled)
            {
                bsCustomers.MovePrevious();
                SetMoveButtonsState();
            }
            else if (parameters.NotificationType == eNotificationsTypes.ACTION && parameters.ActionCode == eActions.MOVE_DOWN && btnNextAddress.Enabled)
            {
                bsCustomers.MoveNext();
                SetMoveButtonsState();
            }
        }

        private void btnAddNewCustomer_Click(object sender, EventArgs e)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            using (frmSelectCustomerGrid formGrid = new frmSelectCustomerGrid(Kernel, Customers.ToList() , edtCustomerCode.Text, "customer"))
            {
                if (formGrid.ShowDialog() == DialogResult.OK && formGrid.selectedCustomer.IsNew)
                {
                    //this.SelectedCustomer = sessionManager.GetObjectByKey<Customer>(formGrid.selectedCustomer.CustomerOid);
                    //this.SelectedAddress = sessionManager.GetObjectByKey<Address>(formGrid.selectedCustomer.AddressOid);
                    Customers.Add(formGrid.selectedCustomer);
                    int index = bsCustomers.List.IndexOf(formGrid.selectedCustomer);
                    if (index >= 0)
                    {
                        bsCustomers.Position = index;
                    }
                    edtCustomerCode.Focus();
                    btnOK.PerformClick();
                }
            }
        }

        private void EnableControls(bool activeTrader, bool activeAddress)
        {
            txtAddressCity.Enabled = activeAddress;
            txtAddressPostalCode.Enabled = activeAddress;
            txtAddressStreet.Enabled = activeAddress;
            txtPhone.Enabled = activeAddress;
            txtAddressProfession.Enabled = activeAddress;
            txtCustomerCode.Enabled = activeTrader;
            txtTaxCode.Enabled = activeTrader;
            txtCompanyName.Enabled = activeTrader;
            txtName.Enabled = activeTrader;
            txtCardID.Enabled = activeTrader;
            txtSurname.Enabled = activeTrader;
            txtTaxOffice.Enabled = activeTrader;
        }



        private void edtCustomerCode_EditValueChanged(object sender, EventArgs e)
        {
            if (this.Customers.Count > 0)
            {
                EnableControls(false, false);
                ClearControls();
            }
            btnAddNewCustomer.Enabled = false;
        }

        private void ClearControls()
        {
            txtCustomerCode.Text =
            txtTaxCode.Text =
            txtAddressCity.Text =
            txtAddressPostalCode.Text =
            txtAddressStreet.Text =
            txtAddressProfession.Text =
            txtCompanyName.Text =
            txtName.Text =
            txtPhone.Text =
            txtCardID.Text =
            txtSurname.Text = "";
            txtTaxOffice.EditValue = null;
            this.Customers.Clear();
        }

        private void btnAddNewCustomerAddress_Click(object sender, EventArgs e)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            using (frmSelectCustomerGrid formGrid = new frmSelectCustomerGrid(Kernel, Customers.ToList(), edtCustomerCode.Text, "address"))
            {
                if (formGrid.ShowDialog() == DialogResult.OK && formGrid.selectedCustomer.AddressIsNew)
                {
                    this.SelectedCustomer = sessionManager.GetObjectByKey<Customer>(formGrid.selectedCustomer.CustomerOid);
                    this.SelectedAddress = sessionManager.GetObjectByKey<Address>(formGrid.selectedCustomer.AddressOid);
                    
                    edtCustomerCode.Focus();
                    btnOK.PerformClick();
                }
            }
        }

        private void ShowHideAddressNavigation(bool show)
        {
            this.dataNavigator1.Visible =
            this.btnPreviousAddress.Visible =
            this.btnNextAddress.Visible = show;
        }

        private void SearchTraderOnValidation(object sender, CancelEventArgs e)
        {
            TextEdit txtEdit = (TextEdit)sender;
            if (txtEdit != null && !String.IsNullOrEmpty(txtEdit.Text))
            {
                string field = txtEdit.DataBindings[0].BindingMemberInfo.BindingField;
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                ICustomerService customerService = Kernel.GetModule<ICustomerService>();
                BaseObj obj;

                obj = customerService.SearchCustomer<Customer>(txtEdit.Text);
                if (obj == null && sender != txtCardID)
                {
                    obj = customerService.SearchCustomer<SupplierNew>(txtEdit.Text);
                }

                if (obj != null)
                {

                    IFormManager formManager = Kernel.GetModule<IFormManager>();
                    if (formManager.ShowMessageBox(String.Format(POSClientResources.CUSTOMER_ALREADY_EXISTS, obj.GetType().GetProperty("CompanyName").GetValue(obj, null)) + Environment.NewLine + POSClientResources.OPEN_FOUND_CUSTOMER, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        IEnumerable<InsertedCustomerViewModel> foundAddresses = customerService.GetCustomerAddresses(obj, obj.GetType());
                        if (foundAddresses != null)
                        {
                            this.Customers.Clear();
                            foundAddresses.ToList().ForEach(x => this.Customers.Add(x));
                            btnAddNewCustomerAddress.Enabled = true;
                            bsCustomers.MoveFirst();
                            SetMoveButtonsState();
                        }
                    }
                }
            }
        }

        private void txtTaxOffice_Validating(object sender, CancelEventArgs e)
        {
            if ((Guid)(sender as LookUpEdit).EditValue == Guid.Empty)
            {
                e.Cancel = true;
            }
        }

        //private void CancelButton_Click(object sender, EventArgs e)
        //{
        //    ClearControls();
        //    edtCustomerCode.Text = string.Empty;
        //    this.panelControl6.Hide();
        //    this.panelControl1.Show();
        //    this.panelControl2.Show();
        //    this.panelControl4.Show();
        //}

        private void OKButton_Click(object sender, EventArgs e)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            InsertedCustomerViewModel selectedCustomer = bsCustomers.Current as InsertedCustomerViewModel;
            this.SelectedCustomer = sessionManager.GetObjectByKey<Customer>(selectedCustomer.CustomerOid);
            this.SelectedAddress = sessionManager.GetObjectByKey<Address>(selectedCustomer.AddressOid);
            //if (this.panelControl6.Visible)
            //{
            //    this.panelControl6.Hide();
            //    this.panelControl1.Show();
            //    this.panelControl2.Show();
            //    this.panelControl4.Show();
            //    this.panelControl6.Visible = false;
            //}
            edtCustomerCode.Focus();
        }

        private void arrows_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && this.Customers.Count > 0)
            {
                btnNextAddress.PerformClick();
            }
            else if (e.KeyCode == Keys.Up && this.Customers.Count > 0)
            {
                btnPreviousAddress.PerformClick();
            }
        }
    }
}
