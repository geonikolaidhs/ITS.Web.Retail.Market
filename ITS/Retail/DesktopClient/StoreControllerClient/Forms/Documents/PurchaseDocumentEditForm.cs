using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class PurchaseDocumentEditForm : DocumentEditForm
    {
        DevExpress.Xpo.XPInstantFeedbackSource CustomerFeedbackSource;
        protected override string LayoutDocumentHeaderProperty
        {
            get { return "PurchaseDocumentHeader"; }
        }

        protected override string LayoutDocumentDetailProperty
        {
            get { return "PurchaseDocumentDetail"; }
        }

        protected override string LayoutTraderProperty
        {
            get { return "SupplierLayout"; }
        }

        protected override string LayoutTotalProperty
        {
            get { return "LayoutPurchaseTotal"; }
        }


        public PurchaseDocumentEditForm(DocumentHeader header, bool previewDocument = false)
            : base(header, previewDocument)
        {
            InitializeComponent();
            if (previewDocument)
            {
                this.Text = Resources.PreviewDocument;
                this.simpleButtonSaveOrderAndRecalculateCosts.Visible = false;
                lueSupplier.Enabled = false;
            }
            else
            {
                this.Text = header.Session.IsNewObject(header) ? Resources.NewPurchaseDocument : Resources.EditDocument;
                this.lueVatFactor.ReadOnly = false;
                this.simpleButtonSaveOrderAndRecalculateCosts.Visible = header != null && header.DocumentType != null
                                                                        ? header.DocumentType.UsesMarkUp && header.DocumentType.UsesMarkUpForm
                                                                        : false;
            }
            if (this.components == null)
            {
                this.components = new System.ComponentModel.Container();
            }
            this.fields = new List<Control>()
            {
                txtQty,
                lueVatFactor,
                txtUnitPrice,
                txtSecondDiscountPercentage,
                txtSecondDiscountValue,
                //"memoEditRemarks",
                btnOkEditLine
            };

            this.grdVatAnalysis.OptionsBehavior.Editable = true;
            this.lueDocumentType.Focus();
            if (!Program.Settings.CurrentUser.Role.GDPREnabled)
            {
                lcTelephone.Parent.Remove(lcTelephone);
            }
        }

        protected override void DocumentHeader_Changed(object sender, ObjectChangeEventArgs e)
        {
            base.DocumentHeader_Changed(sender, e);
            lueSupplier.Enabled = DocumentHeader.DocumentType != null && DocumentHeader.DocumentSeries != null;
            switch (e.PropertyName)
            {
                case "Supplier":
                    SupplierChanged(sender, e);
                    break;
                case "DocumentType":
                    this.simpleButtonSaveOrderAndRecalculateCosts.Visible = this.DocumentHeader.DocumentType.UsesMarkUp && this.DocumentHeader.DocumentType.UsesMarkUpForm;
                    RefreshDocumentItemsDataSource();
                    break;
            }
        }

        private void SupplierChanged(object sender, ObjectChangeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    supplierChanged(e);
                });
            }
            else
            {
                supplierChanged(e);
            }
        }

        private void supplierChanged(ObjectChangeEventArgs e)
        {
            if (DocumentHeader.Supplier != null)
            {
                lueBillingAddress.Properties.DataSource = DocumentHeader.Supplier.Trader.Addresses;
                gridAddresses.DataSource = DocumentHeader.Supplier.Trader.Addresses;
                if (onInitialization == false)
                {
                    if (DocumentHeader.BillingAddress == null || DocumentHeader.Supplier.Trader.Addresses.Any(x => x.Oid == DocumentHeader.BillingAddress.Oid) == false)
                    {
                        DocumentHeader.BillingAddress = DocumentHeader.Supplier.DefaultAddress;
                    }
                }
                if (DeliveryAddress != null)
                {
                    DeliveryAddress.PropertyChanged -= DeliveryAddress_PropertyChanged;
                }
                DeliveryAddress = new AddressViewModel();
                DeliveryAddress.PropertyChanged += DeliveryAddress_PropertyChanged;
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
            if (e != null && e.OldValue != e.NewValue) //Supplier has changed
            {
                if (DocumentHeader.DocumentDetails.Count > 0)
                {
                    bool recomputeDocumentDetails = DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT;
                    DocumentHelper.RecalculateDocumentCosts(ref this._DocumentHeader, recomputeDocumentDetails);
                }
            }
            this.txtCodeName.DataBindings.Clear();
            this.txtProfession.DataBindings.Clear();
            if (this.DocumentHeader.Supplier != null)
            {
                this.txtCodeName.DataBindings.Add("EditValue", this.DocumentHeader.Supplier, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
                this.txtProfession.DataBindings.Add("EditValue", this.DocumentHeader.Supplier, "Profession", true, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        protected override void BindingInit()
        {
            base.BindingInit();
            this.lueSupplier.DataBindings.Add("EditValue", this.DocumentHeader, "Supplier!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueTriangularSupplier.DataBindings.Add("EditValue", this.DocumentHeader, "TriangularSupplier!Key", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        protected override void LookupeditInitialization()
        {
            base.LookupeditInitialization();
            CriteriaOperator criteria = new BinaryOperator("IsActive", true);
           
            CustomerFeedbackSource = new XPInstantFeedbackSource(this.components);
            CustomerFeedbackSource.ObjectType = typeof(SupplierNew);
            CustomerFeedbackSource.ResolveSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_ResolveSession);
            CustomerFeedbackSource.DismissSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_DismissSession);
            CustomerFeedbackSource.FixedFilterCriteria = criteria;
            lueSupplier.Properties.ValueMember = "Oid";
            lueSupplier.Properties.DisplayMember = "CompanyName";
            lueSupplier.Properties.DataSource = CustomerFeedbackSource;

            this.lueTriangularSupplier.Properties.DataSource = new XPServerCollectionSource(this.DocumentHeader.Session, typeof(SupplierNew), criteria);
            this.lueTriangularSupplier.Properties.ValueMember = "Oid";

            lueSupplier.Enabled = DocumentHeader.DocumentType != null && DocumentHeader.DocumentSeries != null;
            lueSupplier.Refresh();
        }
        protected override void EnableDisableDocumentHeaderFields()
        {
            base.EnableDisableDocumentHeaderFields();
            tabDocumentInfo.Enabled =
                DocumentHeader.DocumentType != null && DocumentHeader.DocumentSeries != null &&
                DocumentHeader.Supplier != null;
        }

        protected override void UpdateDocumentDetail(DocumentDetail detail, ObjectChangeEventArgs objectChangeEventArgs)
        {
            try
            {
                if (objectChangeEventArgs != null)
                    UpdateDocumentDetail(detail, true, false);
                else
                    UpdateDocumentDetail(detail, true, true);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.GetFullMessage(), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected void UpdateDocumentDetail(DocumentDetail detail, bool IsOnInit, bool VatIsOnInit)
        {
            List<DocumentDetailDiscount> discounts = new List<DocumentDetailDiscount>();
            if (EditingDocumentDetailDiscount != null
                && (EditingDocumentDetailDiscount.Value != 0 || EditingDocumentDetailDiscount.Percentage != 0)
                )
            {
                discounts.Add(EditingDocumentDetailDiscount);
            }
            VatFactor vatFactor = VatIsOnInit ? null : this.lueVatFactor.GetSelectedDataRow() as VatFactor;
            decimal quantity = this.WeightedBarcodeInfo != null ? this.WeightedBarcodeInfo.Quantity : detail.PackingQuantity;

            DocumentHelper.ComputeDocumentLine(ref this._DocumentHeader, EditingDocumentDetail.Item, EditingDocumentDetail.Barcode, quantity, false,
                EditingDocumentDetail.CustomUnitPrice, true, EditingDocumentDetail.Item.Name, discounts, oldDocumentLine: detail, customVatFactor: vatFactor);
            DocumentHelper.UpdateLinkedItems(ref _DocumentHeader, detail);
            if (detail.DocumentHeader != null)
            {
                gridDocumentDetails.Refresh();
            }
        }

        private void lueVatFactor_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.FocusOnNextField(lueVatFactor);
        }

        private void simpleButtonSaveOrderAndRecalculateCosts_Click(object sender, EventArgs e)
        {
            SaveDocument(sender, e);
            if (DocumentIsSaved)
            {
                //Display the new form
                using (MarkUpForm markUpForm = new MarkUpForm(this.DocumentHeader))
                {
                    markUpForm.ShowDialog();
                    this.Close();
                }
            }
        }
        protected override void VatAnalysisChange(object sender, PropertyChangedEventArgs e)
        {
            base.VatAnalysisChange(sender, e);
            VatAnalysis analysis = sender as VatAnalysis;

            if (analysis != null)
            {
                DocumentHelper.FixDocumentVatDeviations(ref this._DocumentHeader
                    , this._DocumentHeader.Session.GetObjectByKey<VatFactor>(analysis.VatFactorGuid), analysis.NetTotal, analysis.TotalVatAmount);

            }
        }

        private void PurchaseDocumentEditForm_Load(object sender, EventArgs e)
        {
            tabDocumentInfo.Enabled = DocumentHeader.DocumentType != null && DocumentHeader.DocumentSeries != null && DocumentHeader.Supplier != null;
            tabDocumentInfo.TabStop = tabDocumentInfo.Enabled;
            if (this.PreviewDocument)
            {
                this.supplierChanged(null);
            }
            else // Edit Purchase Document
            {
                DeliveryAddress = new AddressViewModel();
                SetDeliveryAddressBindings();
                if (DocumentHeader.Supplier != null)
                {
                    gridAddresses.DataSource = DocumentHeader.Supplier.Trader.Addresses;
                }
            }
        }

        private void lueTriangularSupplier_EditValueChanged(object sender, EventArgs e)
        {
            if (((SearchLookUpEdit)sender).EditValue != null)
            {
                if (((SearchLookUpEdit)sender).EditValue != DBNull.Value)
                {
                    SupplierNew suppTriangular = this.DocumentHeader.Session.GetObjectByKey<SupplierNew>((Guid)((SearchLookUpEdit)sender).EditValue);
                    this.DocumentHeader.TriangularSupplier = suppTriangular;
                    if (DocumentHeader.TriangularSupplier != null)
                    {
                        if (suppTriangular.DefaultAddress != null)
                        {
                            this.DocumentHeader.TriangularAddress = suppTriangular.DefaultAddress.Description;
                        }
                        gridTriangularAddresses.DataSource = DocumentHeader.TriangularSupplier.Trader.Addresses;

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
                this.DocumentHeader.TriangularSupplier = null;
                this.DocumentHeader.TriangularAddress = "";
                if (TriangularAddress != null)
                {
                    TriangularAddress.PropertyChanged -= TriangularAddress_PropertyChanged;
                    TriangularAddress = null;
                }
            }
        }
    }
}
