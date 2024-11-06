using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class SalesDocumentEditForm : DocumentEditForm
    {
        DevExpress.Xpo.XPInstantFeedbackSource CustomerFeedbackSource;
        protected override string LayoutDocumentHeaderProperty
        {
            get { return "SalesDocumentHeader"; }
        }

        protected override string LayoutDocumentDetailProperty
        {
            get { return "SalesDocumentDetail"; }
        }

        protected override string LayoutTraderProperty
        {
            get { return "CustomerLayout"; }
        }

        protected override string LayoutTotalProperty
        {
            get { return "LayoutSalesTotal"; }
        }

        protected bool CustomerNotSupportedByDocType = false;

        public SalesDocumentEditForm(DocumentHeader header, bool previewDocument = false) : base(header, previewDocument)
        {
            InitializeComponent();
            this.components = new Container();
            DocumentHelper.fromDesktopClient = true;
            if (!Program.Settings.CurrentUser.Role.GDPREnabled)
            {
                lcTelephone.Parent.Remove(lcTelephone);
            }
            if (DocumentHeader != null && DocumentHeader.Tablet != null)
            {
                lueTablet.EditValue = DocumentHeader.Tablet.Oid;
            }
        }

        private void InitializeCustomerFeedback()
        {
            lueCustomer.Properties.DataSource = null;
            if (CustomerFeedbackSource != null)
            {
                CustomerFeedbackSource.Dispose();
            }
            CriteriaOperator customerCriteria = DocumentHelper.CustomerCriteria("%", DocumentHeader, Program.Settings.StoreControllerSettings.Owner);
            CustomerFeedbackSource = new XPInstantFeedbackSource(this.components);
            CustomerFeedbackSource.ObjectType = typeof(Customer);
            CustomerFeedbackSource.ResolveSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_ResolveSession);
            CustomerFeedbackSource.DismissSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_DismissSession);
            CustomerFeedbackSource.FixedFilterCriteria = customerCriteria;
            lueCustomer.Properties.ValueMember = "Oid";
            lueCustomer.Properties.DisplayMember = "CompanyName";
            lueCustomer.Properties.DataSource = CustomerFeedbackSource;

        }
        protected override void LookupeditInitialization()
        {
            base.LookupeditInitialization();

            CriteriaOperator customerCriteria = DocumentHelper.CustomerCriteria("%", DocumentHeader, Program.Settings.StoreControllerSettings.Owner);
            if (PreviewDocument)
            {
                lueCustomer.Properties.DataSource = new List<Customer>() { this.DocumentHeader.Customer };
            }
            else
            {
                InitializeCustomerFeedback();
            }
            lueCustomer.Properties.ValueMember = "Oid";
            lueCustomer.Enabled = DocumentHeader.DocumentType != null && DocumentHeader.DocumentSeries != null;
            lueCustomer.TabStop = lueCustomer.Enabled;
            lueCustomer.Refresh();
            lookUpEditTriangularCustomer.EditValue = null;
            if (PreviewDocument)
            {
                lookUpEditTriangularCustomer.Properties.DataSource = new List<Customer>() { this.DocumentHeader.TriangularCustomer };
            }
            else
            {
                lookUpEditTriangularCustomer.EditValue = this.DocumentHeader.TriangularCustomer;
                lookUpEditTriangularCustomer.Properties.DataSource = new XPServerCollectionSource(this.DocumentHeader.Session, typeof(Customer), customerCriteria);
            }

            List<PriceCatalogPolicy> priceCatalogPolicies = this.DocumentHeader.Store.StorePriceCatalogPolicies.Select(stprcatpol => stprcatpol.PriceCatalogPolicy).ToList();

            PriceCatalogHelper.IncludeCustomerPolicyToPoliciesList(this.DocumentHeader, priceCatalogPolicies);

            luePriceCatalogPolicy.Properties.DataSource = priceCatalogPolicies.Distinct();
            luePriceCatalogPolicy.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            luePriceCatalogPolicy.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            luePriceCatalogPolicy.Properties.ValueMember = "Oid";
            this.txtDefaultDocumentDiscount.Enabled = true;
            this.txtCustomerDiscount.Enabled = true;

            lueTablet.Properties.DataSource = new XPCollection<SFA>(this.DocumentHeader.Session, CriteriaOperator.And(new BinaryOperator("IsActive", true, BinaryOperatorType.Equal)));
            lueTablet.Properties.Columns.Add(new LookUpColumnInfo("Name", Resources.Description));
            lueTablet.Properties.ValueMember = "Oid";
            lueTablet.Properties.DisplayMember = "Name";
        }

        protected override void EnableDisableDocumentHeaderFields()
        {
            base.EnableDisableDocumentHeaderFields();

            lueCustomer.ReadOnly = lueDocumentStatus.ReadOnly = luePriceCatalogPolicy.ReadOnly =
                (DocumentHeader.DocumentSeries != null && DocumentHeader.DocumentSeries.HasAutomaticNumbering && DocumentHeader.DocumentNumber > 0);
            luePriceCatalogPolicy.ReadOnly |= (DocumentHeader.TransformationLevel != eTransformationLevel.DEFAULT);


            grcCommandColumn.Visible = grcCommandEdit.Visible = tabDocumentInfo.Enabled =
                DocumentHeader.DocumentType != null && DocumentHeader.DocumentSeries != null &&
                DocumentHeader.Customer != null && DocumentHeader.PriceCatalogPolicy != null;

            grcCommandColumn.Visible = grcCommandEdit.Visible = tabDocumentInfo.Enabled && this.PreviewDocument == false;
        }

        protected override string SearchAndAddDocumentDetail(string codeSearch, string barcodeSearch, DocumentDetail existingdetail, List<Guid> forbiddenItems)
        {
            try
            {
                this.WeightedBarcodeInfo = null;
                PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(DocumentHeader.Session as UnitOfWork,
                                                                                                           DocumentHeader.EffectivePriceCatalogPolicy,
                                                                                                           barcodeSearch);
                PriceCatalogDetail pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                if (pricecatalogdetail == null)
                {
                    priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(DocumentHeader.Session as UnitOfWork,
                                                                                            DocumentHeader.EffectivePriceCatalogPolicy,
                                                                                            codeSearch);
                    pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                }

                if (pricecatalogdetail == null)
                {
                    this.WeightedBarcodeInfo = ItemHelper.GetWeightedBarcodeInfo(codeSearch, this.DocumentHeader);
                    if (this.WeightedBarcodeInfo != null)
                    {
                        if (this.WeightedBarcodeInfo.PriceCatalogDetail.HasValue)
                        {
                            pricecatalogdetail = DocumentHeader.Session.GetObjectByKey<PriceCatalogDetail>(this.WeightedBarcodeInfo.PriceCatalogDetail.Value);
                        }
                    }
                }

                if (pricecatalogdetail == null)
                {
                    txtCodeOrBarcode.EditValue = "";
                    lueDescription.EditValue = null;
                    return Resources.ItemNotFound;
                }

                if (!DocumentHelper.DocumentTypeSupportsItem(this.DocumentHeader, pricecatalogdetail.Item))
                {
                    return Resources.ItemNotSupported;
                }

                if (forbiddenItems != null && forbiddenItems.Contains(pricecatalogdetail.Item.Oid))
                {
                    return Resources.MainItemAndLinkedItemCannotMatch;
                }

                if (this.DocumentHeader.DocumentDetails.Any(x => x.Item.Oid == pricecatalogdetail.Item.Oid && x.Barcode.Oid == pricecatalogdetail.Barcode.Oid))
                {
                    decimal totalQty = DocumentHeader.DocumentDetails.Where((x => x.Item.Oid == pricecatalogdetail.Item.Oid
                        && x.Barcode.Oid == pricecatalogdetail.Barcode.Oid)).Sum(x => x.Qty);
                    if (XtraMessageBox.Show(Resources.ItemAlreadyOrderedWithQuantity + totalQty + Environment.NewLine + Resources.Continue,
                        Resources.Message, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        txtCodeOrBarcode.EditValue = "";
                        lueDescription.EditValue = null;
                        return null;
                    }
                }
                Barcode searchBarcode = priceCatalogPolicyPriceResult != null && priceCatalogPolicyPriceResult.SearchBarcode != null
                                       ? priceCatalogPolicyPriceResult.SearchBarcode
                                       : pricecatalogdetail.Barcode;
                this.EditingDocumentDetail = existingdetail == null ? new DocumentDetail(DocumentHeader.Session) { Item = pricecatalogdetail.Item, Barcode = searchBarcode } : existingdetail;
                this.EditingDocumentDetailDiscount = new DocumentDetailDiscount(DocumentHeader.Session) { DiscountSource = eDiscountSource.CUSTOM };
                this.UpdateDocumentDetail(this.EditingDocumentDetail, pricecatalogdetail, null);
                this.EditingDocumentDetail.Changed += DocumentDetailChanged;
                txtQty.Focus();
                EditingDocumentDetailDiscount.Changed += DocumentDetailDiscountChanged;
                EnableDisableDetailEditFields(pricecatalogdetail.Item.AcceptsCustomPrice, pricecatalogdetail.Item.AcceptsCustomDescription);
                return null;
            }
            catch (Exception ex)
            {
                return ex.GetFullMessage();
            }
        }

        protected override void UpdateDocumentDetail(DocumentDetail detail, ObjectChangeEventArgs objectChangeEventArgs)
        {
            try
            {
                UpdateDocumentDetail(detail, null, objectChangeEventArgs);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.GetFullMessage(), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected void UpdateDocumentDetail(DocumentDetail detail, PriceCatalogDetail pricecatalogdetail, ObjectChangeEventArgs objectChangeEventArgs)
        {
            PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult;
            if (pricecatalogdetail == null)
            {
                priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(DocumentHeader.Session as UnitOfWork,
                                                                                        DocumentHeader.EffectivePriceCatalogPolicy,
                                                                                        detail.Item,
                                                                                        detail.Barcode);
                pricecatalogdetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
            }

            List<DocumentDetailDiscount> discounts = new List<DocumentDetailDiscount>();
            if (EditingDocumentDetailDiscount != null && (EditingDocumentDetailDiscount.Value != 0 || EditingDocumentDetailDiscount.Percentage != 0))
            {
                discounts.Add(EditingDocumentDetailDiscount);
            }

            bool hasCustomPrice = (objectChangeEventArgs != null && objectChangeEventArgs.PropertyName == "CustomUnitPrice")
                                || detail.HasCustomPrice;

            decimal unitPrice = hasCustomPrice ? detail.CustomUnitPrice
                                               : -1;

            decimal quantity = this.WeightedBarcodeInfo != null ? this.WeightedBarcodeInfo.Quantity : detail.PackingQuantity;

            DocumentHelper.ComputeDocumentLine(ref this._DocumentHeader, pricecatalogdetail.Item, detail.Barcode, quantity, false, unitPrice,
                hasCustomPrice, pricecatalogdetail.Item.Name, discounts, oldDocumentLine: detail);
            DocumentHelper.UpdateLinkedItems(ref _DocumentHeader, detail);

            if (detail.DocumentHeader != null)
            {
                gridDocumentDetails.Refresh();
            }
        }
        protected override void DocumentHeader_Changed(object sender, ObjectChangeEventArgs e)
        {
            base.DocumentHeader_Changed(sender, e);
            lueCustomer.Enabled = DocumentHeader.DocumentType != null && DocumentHeader.DocumentSeries != null;
            lueCustomer.TabStop = lueCustomer.Enabled;
            switch (e.PropertyName)
            {
                case "Customer":
                    CustomerChanged(sender, e);
                    break;
                case "PriceCatalogPolicy":
                    if (e.OldValue != e.NewValue)
                    {
                        OnPriceCatalogChanged(sender, e);
                    }
                    break;
                case "DocumentType":
                    IDisposable customerDS = lueCustomer.Properties.DataSource as IDisposable;
                    if (customerDS != null)
                    {
                        customerDS.Dispose();
                    }
                    InitializeCustomerFeedback();

                    //lueCustomer.Properties.PopupFindMode = FindMode.FindClick;
                    DocumentType_Changed();
                    break;

            }
        }

        private void OnPriceCatalogChanged(object sender, ObjectChangeEventArgs e)
        {
            try
            {
                if (!this.onInitialization)
                {
                    if (DocumentHeader.DocumentDetails.Count > 0)
                    {
                        bool recomputeDocumentDetails = DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT;
                        DocumentHelper.RecalculateDocumentCosts(ref this._DocumentHeader, recomputeDocumentDetails);
                    }
                    RefreshDocumentItemsDataSource();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.GetFullMessage(), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DocumentType_Changed()
        {
            try
            {
                if (DocumentHeader.DocumentType != null)
                {

                    this.txtDefaultDocumentDiscount.EditValue = DocumentHeader.StoreDocumentSeriesType.DefaultDiscountPercentage;
                    DocumentHeader.DefaultCustomerDiscount = this.txtDefaultDocumentDiscount.Value;
                    if (DocumentHeader.Customer != null)
                    {
                        DocumentHeader.DefaultCustomerDiscount = this.txtCustomerDiscount.Value;
                    }
                    if (DocumentHeader.DocumentDetails.Count > 0)
                    {
                        DocumentHeader document = DocumentHeader as DocumentHeader;
                        DocumentHelper.ApplyDiscountsOnDocumentTotal(ref document);
                    }
                    else
                    {
                        this.txtDocumentNetTotalBeforeDiscount.Text = 0.ToString();
                        this.txtTotalDiscount.Text = 0.ToString();
                        this.txtDocumentNetTotal.Text = 0.ToString();
                        this.txtDocumentTotalVatAmount.Text = 0.ToString();
                        this.txtPoints.Value = 0;
                        this.txtDocumentGrossTotal.Text = 0.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.GetFullMessage();
            }
            if (DocumentHeader.Customer == null)
            {
                return;
            }
            try
            {
                if (DocumentHeader.DocumentType == null || !DocumentHelper.DocTypeSupportsCustomer(DocumentHeader, DocumentHeader.Customer))
                {
                    DocumentHeader.Customer = null;

                }

            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.GetFullMessage();
            }
        }

        private void CustomerChanged(object sender, ObjectChangeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    customerChanged(e);
                });
            }
            else
            {
                customerChanged(e);
            }
        }

        private void customerChanged(ObjectChangeEventArgs e)
        {
            if (DocumentHeader.Customer != null)
            {
                lueBillingAddress.Properties.DataSource = DocumentHeader.Customer.Trader.Addresses;
                gridAddresses.DataSource = DocumentHeader.Customer.Trader.Addresses;
                List<PriceCatalogPolicy> priceCatalogPolicies = this.DocumentHeader.Store.StorePriceCatalogPolicies.Select(stprcatpol => stprcatpol.PriceCatalogPolicy).ToList();

                PriceCatalogHelper.IncludeCustomerPolicyToPoliciesList(DocumentHeader, priceCatalogPolicies);
                luePriceCatalogPolicy.Properties.DataSource = priceCatalogPolicies.Distinct();
                if (onInitialization == false && PreviewDocument == false)
                {
                    if (DocumentHeader.BillingAddress == null || DocumentHeader.Customer.Trader.Addresses.Any(address => address.Oid == DocumentHeader.BillingAddress.Oid) == false)
                    {
                        DocumentHeader.BillingAddress = DocumentHeader.Customer.DefaultAddress;
                    }
                    DocumentHeader.PriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(DocumentHeader.Store, DocumentHeader.Customer);
                }
                if (this.PreviewDocument == false
                    || (this.PreviewDocument && this.DocumentHeader.DocumentNumber <= 0)
                   )
                {
                    if (DeliveryAddress != null)
                    {
                        DeliveryAddress.PropertyChanged -= DeliveryAddress_PropertyChanged;
                    }
                    DeliveryAddress = new AddressViewModel();
                }
                lueBillingAddress.Refresh();
            }
            else
            {
                lueBillingAddress.Properties.DataSource = new List<Address>();
                DocumentHeader.BillingAddress = null;
                if (DeliveryAddress != null)
                {
                    DeliveryAddress.PropertyChanged -= DeliveryAddress_PropertyChanged;
                    DeliveryAddress = null;
                }

            }
            if (e != null && e.OldValue != e.NewValue) //Customer has changed
            {
                try
                {
                    if (DocumentHeader.Customer != null)
                    {
                        this.txtCustomerDiscount.EditValue = (decimal)DocumentHeader.Customer.Discount;
                        DocumentHeader.DefaultCustomerDiscount = this.txtCustomerDiscount.Value;
                        DocumentHeader.DefaultDocumentDiscount = this.txtDefaultDocumentDiscount.Value;
                        if (DocumentHeader.DocumentDetails.Count > 0)
                        {
                            DocumentHeader document = DocumentHeader as DocumentHeader;
                            DocumentHelper.ApplyDiscountsOnDocumentTotal(ref document);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                }

                if (DocumentHeader.Customer != null && DocumentHeader.DocumentDetails.Count > 0)
                {
                    bool recomputeDocumentDetails = DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT;
                    DocumentHelper.RecalculateDocumentCosts(ref this._DocumentHeader, recomputeDocumentDetails);
                }
            }

            BindCustomerData();
            lueCustomer.Refresh();
            //lueCustomer.RefreshEditValue();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (DocumentHeader.DocumentType == null || !DocumentHelper.DocTypeSupportsCustomer(DocumentHeader, DocumentHeader.Customer))
            {
                DocumentHeader.Customer = null;
                CustomerNotSupportedByDocType = true;
            }

            if (this.PreviewDocument)
            {
                this.Text = Resources.PreviewDocument;
                this.luePriceCatalogPolicy.Enabled = false;
                lueCustomer.Enabled = false;
                lueCustomer.TabStop = lueCustomer.Enabled;
            }
            else
            {
                this.Text = this.DocumentHeader.Session.IsNewObject(this.DocumentHeader) ? Resources.NewSalesDocument : Resources.EditDocument;
                this.luePriceCatalogPolicy.Enabled = Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.AllowPriceCatalogSelection;
                this.luePriceCatalogPolicy.ReadOnly = !Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.AllowPriceCatalogSelection;
            }
            this.fields = new List<Control>()
            {
                txtQty,
                txtUnitPrice,
                txtSecondDiscountPercentage,
                txtSecondDiscountValue,
                btnOkEditLine
            };
        }
        private void SalesDocumentEditForm_Load(object sender, EventArgs e)
        {
            tabDocumentInfo.Enabled = DocumentHeader.DocumentType != null && DocumentHeader.DocumentSeries != null && DocumentHeader.Customer != null && DocumentHeader.PriceCatalogPolicy != null;
            tabDocumentInfo.TabStop = tabDocumentInfo.Enabled;
            if (this.PreviewDocument)
            {
                this.customerChanged(null);
            }
            else // Edit Purchase Document
            {
                if (this.DocumentHeader.DocumentNumber <= 0 && this.DocumentHeader.ReferencedDocuments.Count <= 0)
                {
                    this.customerChanged(null);
                }

                DeliveryAddress = new AddressViewModel();
                SetDeliveryAddressBindings();
                if (DocumentHeader.Customer != null)
                {
                    gridAddresses.DataSource = DocumentHeader.Customer.Trader.Addresses;
                }
            }
        }

        protected override void BindingInit()
        {
            base.BindingInit();
            this.luePriceCatalogPolicy.DataBindings.Add("EditValue", this.DocumentHeader, "PriceCatalogPolicy!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueCustomer.DataBindings.Add("EditValue", this.DocumentHeader, "Customer!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lookUpEditTriangularCustomer.DataBindings.Add("EditValue", this.DocumentHeader, "TriangularCustomer!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            BindCustomerData();
        }

        private void BindCustomerData()
        {

            this.txtDeliveryAddress.DataBindings.Clear();
            this.textEditTaxCode.DataBindings.Clear();
            this.txtTaxOffice.DataBindings.Clear();
            this.txtCodeName.DataBindings.Clear();
            this.txtTelephone.DataBindings.Clear();

            if (this.DocumentHeader != null)
            {
                this.txtDeliveryAddress.DataBindings.Add("EditValue", this.DocumentHeader, "DeliveryAddress", true, DataSourceUpdateMode.OnPropertyChanged);
            }

            if (this.DocumentHeader.Customer != null && this.DocumentHeader.Customer.Trader != null)
            {
                this.textEditTaxCode.DataBindings.Add("EditValue", this.DocumentHeader.Customer.Trader, "TaxCode", true, DataSourceUpdateMode.OnPropertyChanged);
                if (this.DocumentHeader.Customer.Trader.TaxOfficeLookUp != null)
                {
                    this.txtTaxOffice.DataBindings.Add("EditValue", this.DocumentHeader.Customer.Trader.TaxOfficeLookUp, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
                }
                this.txtCodeName.DataBindings.Add("EditValue", this.DocumentHeader.Customer, "Code", true, DataSourceUpdateMode.OnPropertyChanged);

                if (this.DocumentHeader.Customer.DefaultAddress != null)
                {
                    this.txtStreet.EditValue = this.DocumentHeader.Customer.DefaultAddress.Street;
                    this.txtZipCode.EditValue = this.DocumentHeader.Customer.DefaultAddress.PostCode;
                    this.txtCity.EditValue = this.DocumentHeader.Customer.DefaultAddress.City;
                    this.textEditDeliveryProfession.EditValue = this.DocumentHeader.Customer.DefaultAddress.PostCode;
                    if (this.DocumentHeader.Customer.DefaultAddress.DefaultPhone != null)
                    {
                        this.txtTelephone.DataBindings.Add("EditValue", this.DocumentHeader.Customer.DefaultAddress.DefaultPhone, "Number", true, DataSourceUpdateMode.OnPropertyChanged);
                    }
                }

            }
        }

        private void luePriceCatalogPolicy_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (!onInitialization && e.OldValue != e.NewValue)
            {
                PriceCatalogPolicy priceCatalogPolicy = this.DocumentHeader.Session.GetObjectByKey<PriceCatalogPolicy>((Guid)e.NewValue);
                EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(this.DocumentHeader.Store, priceCatalogPolicy);
                notIncludedDetails = DocumentHelper.PriceCatalogNotIncludedItems(this.DocumentHeader, effectivePriceCatalogPolicy);

                if (notIncludedDetails.Count > 0)
                {
                    XtraMessageBox.Show(Resources.SelectedPriceCatalogPolicyNotContainsAllItems, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    grdDocumentDetails.RefreshData();
                }
            }
        }

        private void lueCustomer_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (!onInitialization && e.OldValue != e.NewValue && (DocumentHeader.DocumentDetails.Count > 0 || EditingDocumentDetail != null))
            {
                if (e.NewValue != null)
                {
                    Customer selectedCustomer = this.DocumentHeader.Session.GetObjectByKey<Customer>((Guid)e.NewValue);
                    EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(this.DocumentHeader.Store, selectedCustomer);
                    notIncludedDetails = DocumentHelper.PriceCatalogNotIncludedItems(this.DocumentHeader, effectivePriceCatalogPolicy);

                    if (notIncludedDetails.Count > 0)
                    {
                        XtraMessageBox.Show(Resources.SelectedCustomerPriceCatalogPolicyNotContainsAllItems, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        this.grdDocumentDetails.RefreshData();
                    }
                }

            }
        }


        private void lookUpEditTriangularCustomer_EditValueChanged(object sender, EventArgs e)
        {
            if (((SearchLookUpEdit)sender).EditValue != null)
            {
                if (((SearchLookUpEdit)sender).EditValue != DBNull.Value)
                {
                    Customer custTriangular = this.DocumentHeader.Session.GetObjectByKey<Customer>((Guid)((SearchLookUpEdit)sender).EditValue);
                    this.DocumentHeader.TriangularCustomer = custTriangular;
                    if (custTriangular != null)
                    {
                        if (custTriangular.DefaultAddress != null)
                        {
                            this.DocumentHeader.TriangularAddress = custTriangular.DefaultAddress.Description;
                        }
                        gridTriangularAddresses.DataSource = DocumentHeader.TriangularCustomer.Trader.Addresses;

                        if (TriangularAddress != null)
                        {
                            TriangularAddress.PropertyChanged -= TriangularAddress_PropertyChanged;
                        }
                        TriangularAddress = new AddressViewModel();
                        TriangularAddress.PropertyChanged += TriangularAddress_PropertyChanged;
                    }
                    else
                    {
                        this.DocumentHeader.TriangularAddress = "";
                        if (TriangularAddress != null)
                        {
                            TriangularAddress.PropertyChanged -= TriangularAddress_PropertyChanged;
                            TriangularAddress = null;
                        }
                    }
                }
            }
            else
            {
                this.DocumentHeader.TriangularCustomer = null;
                this.DocumentHeader.TriangularAddress = "";
                if (TriangularAddress != null)
                {
                    TriangularAddress.PropertyChanged -= TriangularAddress_PropertyChanged;
                    TriangularAddress = null;
                }
            }
        }

        private void txtDefaultDocumentDiscount_EditValueChanged(object sender, EventArgs e)
        {
            if (DocumentHeader.DocumentType != null)
            {
                DocumentHeader.DefaultDocumentDiscount = this.txtDefaultDocumentDiscount.Value;
                DocumentHeader.DefaultCustomerDiscount = this.txtCustomerDiscount.Value;

                if (DocumentHeader.DocumentDetails.Count > 0)
                {
                    DocumentHeader document = DocumentHeader as DocumentHeader;
                    DocumentHelper.ApplyDiscountsOnDocumentTotal(ref document);
                }
            }
        }

        private void txtCustomerDiscount_EditValueChanged(object sender, EventArgs e)
        {
            if (DocumentHeader.DocumentType != null)
            {
                DocumentHeader.DefaultCustomerDiscount = this.txtCustomerDiscount.Value;
                DocumentHeader.DefaultDocumentDiscount = this.txtDefaultDocumentDiscount.Value;

                if (DocumentHeader.DocumentDetails.Count > 0)
                {
                    DocumentHeader document = DocumentHeader as DocumentHeader;
                    DocumentHelper.ApplyDiscountsOnDocumentTotal(ref document);
                }
            }
        }

        private void lueTablet_EditValueChanged(object sender, EventArgs e)
        {
            if (((LookUpEdit)sender).EditValue != null)
            {
                if (((LookUpEdit)sender).EditValue != DBNull.Value)
                {
                    if (this.DocumentHeader != null)
                    {
                        DocumentHeader.Tablet = this.DocumentHeader.Session.GetObjectByKey<SFA>((Guid)((LookUpEdit)sender).EditValue);
                    }
                }
            }
        }
    }
}


