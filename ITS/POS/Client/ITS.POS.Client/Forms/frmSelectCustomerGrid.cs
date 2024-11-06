using DevExpress.Xpo;
using DevExpress.XtraEditors;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmSelectCustomerGrid : XtraForm, IPOSForm
    {
        public InsertedCustomerViewModel selectedCustomer { get; protected set; }
        public IPosKernel Kernel { get; set; }
        public BindingList<InsertedCustomerViewModel> Customers;
        public string SearchText { get; protected set; }
        public void Initialize(IPosKernel kernel)
        {
            Kernel = kernel;
        }
        public frmSelectCustomerGrid(IPosKernel kernel, List<InsertedCustomerViewModel> customerList, string searchText, string add)
        {
            Initialize(kernel);
            InitializeComponent();
            InitBindings();
            textEdit1.Text = SearchText = searchText;
            customerList.ToList().ForEach(x => this.Customers.Add(x));

            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if (add == "customer")
            {
                HideGridControl();
                AddNewCustomer();
                panelControl2.Focus();
            }
            else if (add == "address")
            {
                HideGridControl();
                AddNewAddress();
                panelControl4.Focus();
            }
            else
            {
                ShowGridControl();
                panelControl7.Focus();
            }
            
        }
        private void InitBindings()
        {
            Customers = new BindingList<InsertedCustomerViewModel>();
            bsCustomers.DataSource = Customers;

            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            txtTaxOffice.Properties.DataSource = new XPCollection<TaxOffice>(sessionManager.GetSession<TaxOffice>());
            txtTaxOffice.Properties.ValueMember = "Oid";
            txtTaxOffice.Properties.DisplayMember = "Description";
            txtTaxOffice.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Κωδικός", 50));
            txtTaxOffice.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "ΔΟY", 150));
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
            txtThirdPartNum.DataBindings.Add("EditValue",bsCustomers,"ThirdPartNum");
        }

        private void HideGridControl()
        {
            panelControl7.SendToBack();
            gridControl1.Enabled = false;
            panelControl7.Hide();
            panelControl4.Show();
            panelControl2.Show();
        }

        private void ShowGridControl()
        {
            panelControl7.BringToFront();
            panelControl7.BringToFront();
            gridControl1.Enabled = true;
            gridControl1.Focus();
            panelControl7.Show();
            panelControl4.Hide();
            panelControl2.Hide();
        }
        private void OKButton_Click(object sender, EventArgs e)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            if (this.panelControl7.Visible || this.validationProviderCustomer.Validate() || (bsCustomers.Current as InsertedCustomerViewModel).AddressIsNew)
            {
                selectedCustomer = bsCustomers.Current as InsertedCustomerViewModel;
            }
            else if (!this.validationProviderCustomer.Validate())
            {
                HideGridControl();
                panelControl2.Focus();
            }
            if (!this.validationProviderCustomerAddress.Validate())
            {
                panelControl4.Focus();
            }
            if (selectedCustomer != null)
            {
                
                bool openCustomersGrid = Application.OpenForms.Cast<Form>().Where(x => x.Name == "frmSelectCustomer") !=null ? true : false;
                if ((this.validationProviderCustomerAddress.Validate() && txtTaxOffice.Text != "" && selectedCustomer.IsNew) || (openCustomersGrid && !selectedCustomer.IsNew) )
                this.DialogResult = DialogResult.OK;
            }
        }

        private void TextEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch.PerformClick();
            }

        }
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.Customers.Count > 0)
            {
                OKButton.PerformClick();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textEdit1.Text) == false && textEdit1.Text != SearchText)
            {
                SearchText = textEdit1.Text;
                this.DialogResult = DialogResult.Retry;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {

        }

        private void btnPreviousAddress_Click(object sender, EventArgs e)
        {
            try
            {
                if (panelControl7.Visible)
                {
                    bsCustomers.MovePrevious();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btnNextAddress_Click(object sender, EventArgs e)
        {
            try
            {
                if (panelControl7.Visible)
                {
                    bsCustomers.MoveNext();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btnAddNewCustomer_Click(object sender, EventArgs e)
        {
            AddNewCustomer();
            panelControl2.Focus();
        }
        private void AddNewCustomer()
        {
            HideGridControl();
            this.Customers.Add(new InsertedCustomerViewModel()
            {
                Code = SearchText,
                TaxCode = SearchText,
                AddressOid = Guid.NewGuid(),
                CustomerOid = Guid.Empty,
                IsNew = true,
                AddressIsNew = true
            });
            EnableControls(true, true);
            panelControl2.Focus();
            //bsCustomers.MoveLast();
        }

        private void EnableControls(bool activeTrader, bool activeAddress)
        {
            txtAddressCity.Enabled = activeAddress;
            txtAddressPostalCode.Enabled = activeAddress;
            txtAddressStreet.Enabled = activeAddress;
            txtPhone.Enabled = activeAddress;
            txtAddressProfession.Enabled = activeAddress;
            txtThirdPartNum.Enabled = activeAddress;
            txtCustomerCode.Enabled = activeTrader;
            txtTaxCode.Enabled = activeTrader;
            txtCompanyName.Enabled = activeTrader;
            txtName.Enabled = activeTrader;
            txtCardID.Enabled = activeTrader;
            txtSurname.Enabled = activeTrader;
            txtTaxOffice.Enabled = activeTrader;
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
            else
            {
                e.Cancel = false;
            }
        }

        private void btnAddNewCustomerAddress_Click(object sender, EventArgs e)
        {
            AddNewAddress();
            panelControl4.Focus();
        }

        private void AddNewAddress()
        {
            HideGridControl();
            InsertedCustomerViewModel customerViewModel = bsCustomers.Current as InsertedCustomerViewModel;
            if (customerViewModel == null)
            {
                return;
            }
            customerViewModel.AddressOid = Guid.NewGuid();
            customerViewModel.AddressIsNew = true;
            this.Customers.Clear();

            this.Customers.Add(customerViewModel);
            EnableControls(false, true);
            btnAddNewCustomerAddress.Enabled = false;
            txtAddressStreet.Focus();
            panelControl2.Enabled = panelControl3.Enabled = false;
            this.DialogResult = DialogResult.OK;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
           
            return base.ProcessDialogKey(keyData);
        }

        
    }
}
