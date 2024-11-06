using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraGrid.Columns;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms.Documents
{
    public partial class FinancialDocumentEditForm : DocumentEditForm
    {

        private GridColumn companyNameColumn;

        private GridColumn codeColumn;

        private GridColumn taxCodeColumn;

        private GridColumn storeNameColumn;

        public FinancialDocumentEditForm(DocumentHeader documentHeader, bool previewDocument = false) : base(documentHeader, previewDocument)
        {
            InitializeComponent();

            this.xtpDocumentDetails.PageVisible = false;
            this.xtpDocumentDetailsDouble.PageVisible = false;
            this.lcHasBeenExecuted.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            this.lcHasBeenChecked.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            this.layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            
            List<string> alwayshiddenTabs = new List<string>()
            {
                "TriangularOrderInfo"
            };

            this.tabDocumentInfo.TabPages.Where(tabpage => alwayshiddenTabs.Contains(tabpage.Tag)).ToList().ForEach(tabpage =>
            {
                tabpage.PageVisible = false;
            });

            this.lcgVatAnalysis.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            this.lookUpEditTrader.Properties.View.OptionsBehavior.AutoPopulateColumns = false;
            this.lookUpEditTrader.Properties.ValueMember = "Oid";
            this.lookUpEditTrader.Properties.DisplayMember = "Description";

            storeNameColumn = new GridColumn()
            {
                FieldName = "Name",
                Caption = Resources.Name
            };
            taxCodeColumn = new GridColumn()
            {
                FieldName = "Trader.TaxCode",
                Caption = Resources.TaxCode
            };
            codeColumn = new GridColumn()
            {
                FieldName = "Code",
                Caption = Resources.Code
            };
            companyNameColumn = new GridColumn()
            {
                FieldName = "CompanyName",
                Caption = Resources.CompanyName
            };
        }

        private void FinancialDocumentEditForm_Load(object sender, EventArgs e)
        {
            this.layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            this.simpleButtonAddRemainAmountToPayment.Visible = false;

            EnableDisableFinancialComponents();

            if (this.PreviewDocument)
            {
                PrepareFormForPreview();
            }
            else if (this.DocumentHeader.Session.IsNewObject(this.DocumentHeader) == false)
            {
                PrepareFormForEdit();
            }
        }

        private void EnableDisableFinancialComponents()
        {
            List<string> visibleTabs = new List<string>() {
                "Customer",
                "PaymentMethods"
            };
            this.tabDocumentInfo.TabPages.ToList().ForEach(tabPage =>
            {
                tabPage.PageVisible = visibleTabs.Contains(tabPage.Tag);
            });
            this.lcRemainingAmountPayment.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;


            if (this.DocumentHeader.DocumentType == null)
            {
                this.tabDocumentInfo.Enabled = false;
            }
            else
            {
                switch (this.DocumentHeader.DocumentType.TraderType)
                {
                    case eDocumentTraderType.CUSTOMER:
                        this.tabDocumentInfo.Enabled = this.DocumentHeader.Customer != null;
                        break;
                    case eDocumentTraderType.NONE:
                        throw new NotSupportedException(String.Format("FinancialDocuemntEditForm.EnableDisableFinancialComponents().Invalid TraderType: {0} ", this.DocumentHeader.DocumentType.TraderType));
                    case eDocumentTraderType.STORE:
                        this.tabDocumentInfo.Enabled = this.DocumentHeader.SecondaryStore != null;
                        break;
                    case eDocumentTraderType.SUPPLIER:
                        this.tabDocumentInfo.Enabled = this.DocumentHeader.Supplier != null;
                        break;
                    default:
                        throw new NotSupportedException(String.Format("FinancialDocuemntEditForm.EnableDisableFinancialComponents().Invalid TraderType: {0} ", this.DocumentHeader.DocumentType.TraderType));
                }
            }
        }

        protected override void documentTypeChanged()
        {
            base.documentTypeChanged();
            this.lookUpEditTrader.DataBindings.Clear();

            if (this.DocumentHeader.DocumentType != null)
            {                
                switch(this.DocumentHeader.TraderType)
                {
                    case eDocumentTraderType.CUSTOMER:
                        this.DocumentHeader.Supplier = null;
                        this.DocumentHeader.SecondaryStore = null;
                        this.xtpCustomer.Text = this.layoutControlItemTrader.Text = Resources.Customer;
                        this.lookUpEditTrader.DataBindings.Add("EditValue", this.DocumentHeader, "Customer!Key", true, DataSourceUpdateMode.OnPropertyChanged);
                        CriteriaOperator customerCriteria = DocumentHelper.CustomerCriteria("%", DocumentHeader, Program.Settings.StoreControllerSettings.Owner);
                        InitialiseCustomerComponent(customerCriteria);
                        SetCustomerData();
                        break;
                    case eDocumentTraderType.NONE:
                        throw new NotSupportedException(String.Format("FinancialDocumentEditForm.documentTypeChanged() {0}", this.DocumentHeader.TraderType));
                    case eDocumentTraderType.STORE:
                        this.DocumentHeader.Supplier = null;
                        this.DocumentHeader.Customer = null;
                        this.xtpCustomer.Text = this.layoutControlItemTrader.Text = Resources.SecondaryStore;
                        this.lookUpEditTrader.DataBindings.Add("EditValue", this.DocumentHeader, "SecondaryStore!Key", true, DataSourceUpdateMode.OnPropertyChanged);
                        CriteriaOperator storeCriteria = new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
                        InitialiseStoreComponent(storeCriteria);
                        SetSecondaryStoreData();
                        break;
                    case eDocumentTraderType.SUPPLIER:
                        this.DocumentHeader.Customer = null;
                        this.DocumentHeader.SecondaryStore = null;
                        this.xtpCustomer.Text = this.layoutControlItemTrader.Text = Resources.Supplier;
                        this.lookUpEditTrader.DataBindings.Add("EditValue", this.DocumentHeader, "Supplier!Key", true, DataSourceUpdateMode.OnPropertyChanged);
                        CriteriaOperator supplierCriteria = new BinaryOperator("Owner.Oid", Program.Settings.StoreControllerSettings.Owner.Oid);
                        InitialiseSupplierComponent(supplierCriteria);
                        SetSupplierData();
                        break;
                    default:
                        throw new NotImplementedException(String.Format("FinancialDocumentEditForm.documentTypeChanged() {0}", this.DocumentHeader.TraderType));
                }
            }
        }

        private void InitialiseSupplierComponent(CriteriaOperator supplierCriteria)
        {
            this.lookUpEditTrader.Properties.DataSource = new XPServerCollectionSource(this.DocumentHeader.Session,
                                                                                                               typeof(SupplierNew),
                                                                                                               supplierCriteria
                                                                                                               );
            this.lookUpEditTrader.Properties.View.Columns.Clear();
            companyNameColumn.Visible = true;
            companyNameColumn.VisibleIndex = 0;
            this.lookUpEditTrader.Properties.View.Columns.Add(companyNameColumn);
            codeColumn.Visible = true;
            codeColumn.VisibleIndex = 1;
            this.lookUpEditTrader.Properties.View.Columns.Add(codeColumn);
            taxCodeColumn.Visible = true;
            taxCodeColumn.VisibleIndex = 2;
            this.lookUpEditTrader.Properties.View.Columns.Add(taxCodeColumn);
        }

        private void InitialiseStoreComponent(CriteriaOperator storeCriteria)
        {
            this.lookUpEditTrader.Properties.DataSource = new XPServerCollectionSource(this.DocumentHeader.Session,
                                                                                                               typeof(Store),
                                                                                                               storeCriteria
                                                                                                               );
            this.lookUpEditTrader.Properties.View.Columns.Clear();
            codeColumn.Visible = true;
            codeColumn.VisibleIndex = 0;
            this.lookUpEditTrader.Properties.View.Columns.Add(codeColumn);
            storeNameColumn.Visible = true;
            storeNameColumn.VisibleIndex = 1;
            this.lookUpEditTrader.Properties.View.Columns.Add(storeNameColumn);
        }

        private void InitialiseCustomerComponent(CriteriaOperator customerCriteria)
        {
            this.lookUpEditTrader.Properties.DataSource = new XPServerCollectionSource(this.DocumentHeader.Session, typeof(Customer), customerCriteria);
            this.lookUpEditTrader.Properties.View.Columns.Clear();
            companyNameColumn.Visible = true;
            companyNameColumn.VisibleIndex = 0;
            this.lookUpEditTrader.Properties.View.Columns.Add(companyNameColumn);
            codeColumn.Visible = true;
            codeColumn.VisibleIndex = 1;
            this.lookUpEditTrader.Properties.View.Columns.Add(codeColumn);
            taxCodeColumn.Visible = true;
            taxCodeColumn.VisibleIndex = 2;
            this.lookUpEditTrader.Properties.View.Columns.Add(taxCodeColumn);
        }

        protected override void DocumentHeader_Changed(object sender, ObjectChangeEventArgs e)
        {
            base.DocumentHeader_Changed(sender, e);
            EnableDisableFinancialComponents();
            switch ( e.PropertyName )
            {
                case "Customer":
                    if (this.DocumentHeader.TraderType == eDocumentTraderType.CUSTOMER)
                    {
                        SetCustomerData();
                    }
                    break;
                case "Supplier":
                    if (this.DocumentHeader.TraderType == eDocumentTraderType.SUPPLIER)
                    {
                        SetSupplierData();
                    }
                    break;
                case "SecondaryStore":
                    if (this.DocumentHeader.TraderType == eDocumentTraderType.STORE)
                    {
                        SetSecondaryStoreData();
                    }
                    break;
                default:
                    break;
            }
        }


        private void SetCustomerData()
        {
            this.lueBillingAddress.Properties.DataSource = this.DocumentHeader.Customer == null ? null : this.DocumentHeader.Customer.Trader.Addresses;
            this.DocumentHeader.BillingAddress = this.DocumentHeader.Customer == null || this.DocumentHeader.Customer.DefaultAddress == null
                                                 ? null
                                                 : this.DocumentHeader.Customer.DefaultAddress;

            if ( this.DocumentHeader.Customer != null )
            {
                this.tabDocumentInfo.Enabled = true;
                this.tabDocumentInfo.SelectedTabPage = this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "PaymentMethods");
                this.txtCodeName.Text = this.DocumentHeader.Customer.Code == null ? "" : this.DocumentHeader.Customer.Code;
                this.txtProfession.Text = this.DocumentHeader.Customer.Profession == null ? "" : this.DocumentHeader.Customer.Profession;
                this.txtTelephone.Text = this.DocumentHeader.Customer.DefaultAddress == null || this.DocumentHeader.Customer.DefaultAddress.DefaultPhone == null
                                         ? ""
                                         : this.DocumentHeader.Customer.DefaultAddress.DefaultPhone.Number;
          
                this.txtDeliveryAddress.Text = this.DocumentHeader.DeliveryAddress == null ? "" : this.DocumentHeader.DeliveryAddress;
                this.txtTaxOffice.Text = this.DocumentHeader.Customer.Trader.TaxOffice == null ? "" : this.DocumentHeader.Customer.Trader.TaxOffice;
            }
            this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "Customer").Text = Resources.Customer;
           
        }        

        private void SetSupplierData()
        {
            this.lueBillingAddress.Properties.DataSource = this.DocumentHeader.Supplier == null ? null : this.DocumentHeader.Supplier.Trader.Addresses;
            this.DocumentHeader.BillingAddress = this.DocumentHeader.Supplier == null || this.DocumentHeader.Supplier.DefaultAddress == null
                                                 ? null
                                                 : this.DocumentHeader.Supplier.DefaultAddress;
            if (this.DocumentHeader.Supplier != null)
            {
                this.tabDocumentInfo.Enabled = true;
                this.tabDocumentInfo.SelectedTabPage = this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "PaymentMethods");

                this.txtCodeName.Text = this.DocumentHeader.Supplier.Code == null ? "" : this.DocumentHeader.Supplier.Code;
                this.txtProfession.Text = this.DocumentHeader.Supplier.Profession == null ? "" : this.DocumentHeader.Supplier.Profession;
                this.txtTelephone.Text = this.DocumentHeader.Supplier.DefaultAddress == null || this.DocumentHeader.Supplier.DefaultAddress.DefaultPhone == null
                                         ? ""
                                         : this.DocumentHeader.Supplier.DefaultAddress.DefaultPhone.Number;
                this.txtDeliveryAddress.Text = this.DocumentHeader.DeliveryAddress == null ? "" : this.DocumentHeader.DeliveryAddress;
                this.txtTaxOffice.Text = this.DocumentHeader.Supplier.Trader.TaxOffice == null ? "" : this.DocumentHeader.Supplier.Trader.TaxOffice;                
            }
            this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "Customer").Text = Resources.Supplier;
        }

        private void SetSecondaryStoreData()
        {
            this.layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;//hide Billing Address
            this.DocumentHeader.BillingAddress = null;
            this.txtAddressProfession.Text = this.DocumentHeader.BillingAddress == null
                                             ? ""
                                             : this.DocumentHeader.BillingAddress.Profession;

            if (this.DocumentHeader.SecondaryStore != null)
            {
                this.tabDocumentInfo.SelectedTabPage = this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "PaymentMethods");
            }

            this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "Customer").PageVisible = false;
        }

        private void PrepareFormForPreview()
        {
            this.tabDocumentInfo.Enabled = true;

            switch (this.DocumentHeader.TraderType)
            {
                case eDocumentTraderType.CUSTOMER:
                    this.xtpCustomer.Text = this.layoutControlItemTrader.Text = Resources.Customer;
                    this.lookUpEditTrader.DataBindings.Add("EditValue", this.DocumentHeader, "Customer!Key", true, DataSourceUpdateMode.OnPropertyChanged);
                    this.lookUpEditTrader.Properties.DataSource = new List<Customer>()
                                                                  {
                                                                    this.DocumentHeader.Session.GetObjectByKey<Customer>(this.DocumentHeader.Customer.Oid)
                                                                  };

                    this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "Customer").Text = Resources.Customer;
                    SetCustomerData();
                    break;
                case eDocumentTraderType.NONE:
                    throw new NotSupportedException(String.Format("FinancialDocumentEditForm.PrepareFormForPreview() {0}", this.DocumentHeader.TraderType));
                case eDocumentTraderType.STORE:
                    this.xtpCustomer.Text = this.layoutControlItemTrader.Text = Resources.SecondaryStore;
                    this.lookUpEditTrader.DataBindings.Add("EditValue", this.DocumentHeader, "SecondaryStore!Key", true, DataSourceUpdateMode.OnPropertyChanged);
                    this.lookUpEditTrader.Properties.DataSource = new List<Store>()
                                                                  {
                                                                    this.DocumentHeader.Session.GetObjectByKey<Store>(this.DocumentHeader.SecondaryStore.Oid)
                                                                  };

                    this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "Customer").PageVisible = false;
                    SetSecondaryStoreData();
                    break;
                case eDocumentTraderType.SUPPLIER:
                    this.xtpCustomer.Text = this.layoutControlItemTrader.Text = Resources.Supplier;
                    this.lookUpEditTrader.DataBindings.Add("EditValue", this.DocumentHeader, "Supplier!Key", true, DataSourceUpdateMode.OnPropertyChanged);
                    this.lookUpEditTrader.Properties.DataSource = new List<SupplierNew>()
                                                                  {
                                                                    this.DocumentHeader.Session.GetObjectByKey<SupplierNew>(this.DocumentHeader.Supplier.Oid)
                                                                  };
                    this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "Customer").Text = Resources.Supplier;
                    SetSupplierData();
                    break;
                default:
                    throw new NotImplementedException(String.Format("FinancialDocumentEditForm.PrepareFormForPreview() {0}", this.DocumentHeader.TraderType));
            }
        }

        private void PrepareFormForEdit()
        {
            bool documentIsClosed = this.DocumentHeader != null
                                            ? this.DocumentHeader.DocumentNumber > 0
                                            : false;
            this.tabDocumentInfo.Enabled = true;
            this.lookUpEditTrader.Enabled = !documentIsClosed;
            this.lueDocumentType.Enabled = false;
            this.lueDocumentSeries.Enabled = false;
            this.lookUpEditTrader.DataBindings.Clear();
            switch (this.DocumentHeader.TraderType)
            {
                case eDocumentTraderType.CUSTOMER:
                    this.xtpCustomer.Text = this.layoutControlItemTrader.Text = Resources.Customer;
                    this.lookUpEditTrader.DataBindings.Add("EditValue", this.DocumentHeader, "Customer!Key", true, DataSourceUpdateMode.OnPropertyChanged);
                    CriteriaOperator customerCriteria = documentIsClosed
                                                        ? new BinaryOperator("Oid", this.DocumentHeader.Customer.Oid)
                                                        : DocumentHelper.CustomerCriteria("%", DocumentHeader, Program.Settings.StoreControllerSettings.Owner);
                    InitialiseCustomerComponent(customerCriteria);
                    this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "Customer").Text = Resources.Customer;
                    SetCustomerData();
                    break;
                case eDocumentTraderType.NONE:
                    throw new NotSupportedException(String.Format("FinancialDocumentEditForm.PrepareFormForPreview() {0}", this.DocumentHeader.TraderType));
                case eDocumentTraderType.STORE:
                    this.xtpCustomer.Text = this.layoutControlItemTrader.Text = Resources.SecondaryStore;
                    this.lookUpEditTrader.DataBindings.Add("EditValue", this.DocumentHeader, "SecondaryStore!Key", true, DataSourceUpdateMode.OnPropertyChanged);
                    CriteriaOperator storeCriteria = documentIsClosed
                                                ? new BinaryOperator("SecondaryStore", this.DocumentHeader.SecondaryStore.Oid)
                                                : new BinaryOperator("Owner.Oid",Program.Settings.StoreControllerSettings.Owner.Oid);
                    InitialiseStoreComponent(storeCriteria);
                    this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "Customer").PageVisible = false;
                    SetSecondaryStoreData();
                    break;
                case eDocumentTraderType.SUPPLIER:
                    this.xtpCustomer.Text = this.layoutControlItemTrader.Text = Resources.Supplier;
                    this.lookUpEditTrader.DataBindings.Add("EditValue", this.DocumentHeader, "Supplier!Key", true, DataSourceUpdateMode.OnPropertyChanged);
                    CriteriaOperator supplierCriteria = documentIsClosed
                                                       ? new BinaryOperator("Supplier", this.DocumentHeader.Supplier.Oid)
                                                       : new BinaryOperator("Owner.Oid",Program.Settings.StoreControllerSettings.Owner.Oid);
                    InitialiseSupplierComponent(supplierCriteria);
                    this.tabDocumentInfo.TabPages.FirstOrDefault(tabpage => (string)tabpage.Tag == "Customer").Text = Resources.Supplier;
                    SetSupplierData();
                    break;
                default:
                    throw new NotImplementedException(String.Format("FinancialDocumentEditForm.PrepareFormForPreview() {0}", this.DocumentHeader.TraderType));
            }
        }

        private void lookUpEditTrader_EditValueChanged(object sender, EventArgs e)
        {
            EnableDisableFinancialComponents();
        }
    }
}
