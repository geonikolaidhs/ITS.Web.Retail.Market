using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.DesktopClient.StoreControllerClient.Forms.Documents;
using ITS.Retail.DesktopClient.StoreControllerClient.Forms.Editors;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.DesktopClient.StoreControllerClient.ITSStoreControllerDesktopService;
using ITS.Retail.Model;
using ITS.Retail.Model.Exceptions;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class DocumentEditForm : XtraLocalizedForm
    {
        protected List<DocumentDetail> notIncludedDetails = new List<DocumentDetail>();
        private PropertyInfo LayoutDocumentHeaderPropertyInfo = null;
        protected virtual string LayoutDocumentHeaderProperty
        {
            get
            {
                return string.Empty;
            }
        }

        private PropertyInfo LayoutDocumentDetailPropertyInfo = null;
        protected virtual string LayoutDocumentDetailProperty
        {
            get
            {
                return string.Empty;
            }
        }

        private PropertyInfo LayoutTraderPropertyInfo = null;
        protected virtual string LayoutTraderProperty
        {
            get
            {
                return string.Empty;
            }
        }

        private PropertyInfo LayoutTotalPropertyInfo = null;
        protected virtual string LayoutTotalProperty
        {
            get
            {
                return string.Empty;
            }
        }


        protected bool PreviewDocument { get; set; }
        protected bool DocumentIsSaved { get; set; }

        private string OldDocumentDetailSerialized { get; set; }
        private string OldDocumentPaymentSerialized { get; set; }

        private Dictionary<BaseEdit, string> documentDetailDataBindings;

        protected DocumentHeader _DocumentHeader;
        protected DocumentHeader DocumentHeader { get { return _DocumentHeader; } }

        private DocumentDetail _EditingDocumentDetail;
        protected DocumentDetail EditingDocumentDetail { get { return _EditingDocumentDetail; } set { _EditingDocumentDetail = value; ResetDocumentDetailBindings(); } }

        private DocumentDetailDiscount _EditingDocumentDetailDiscount;
        protected DocumentDetailDiscount EditingDocumentDetailDiscount { get { return _EditingDocumentDetailDiscount; } set { _EditingDocumentDetailDiscount = value; ResetDocumentDetailDiscountBindings(); } }

        private AddressViewModel _DeliveryAddress;
        protected AddressViewModel DeliveryAddress { get { return _DeliveryAddress; } set { _DeliveryAddress = value; SetDeliveryAddressBindings(); } }

        private AddressViewModel _TriangularAddress;
        protected AddressViewModel TriangularAddress { get { return _TriangularAddress; } set { _TriangularAddress = value; SetTriangularAddressBindings(); } }


        private DocumentPayment _EditingDocumentPayment;
        protected DocumentPayment EditingDocumentPayment { get { return _EditingDocumentPayment; } private set { _EditingDocumentPayment = value; RestoreDocumentPaymentBindings(); } }


        protected WeightedBarcodeInfo WeightedBarcodeInfo { get; set; }

        protected bool onInitialization;
        private string _NewValueInit;
        public string NewValewInit
        {
            get
            {
                return _NewValueInit;
            }
            set
            {
                if (value != null)
                    _NewValueInit = value;
            }
        }

        private DocumentEditForm()
        {
            InitializeComponent();
        }

        public DocumentEditForm(DocumentHeader header, bool previewDocument = false)
        {
            this.components = new Container();
            onInitialization = true;
            InitializeComponent();
            PreviewDocument = previewDocument;
            this._DocumentHeader = header;
            if (this._DocumentHeader.Session.IsNewObject(this._DocumentHeader))
            {
                this.DocumentHeader.CreatedBy = this.DocumentHeader.Session.GetObjectByKey<User>(Program.Settings.CurrentUser.Oid);
            }
            this.VatAnalysis = new BindingList<VatAnalysis>();
            layoutControlItemVehicleNumber.Text = Resources.VehicleNumber;
        }



        private void DocumentEditForm_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                onInitialization = false;
                return;
            }

            UpdateVatAnalysis();
            InitializeBindings();
            InitializeLookupEdits();

            if (PreviewDocument)
            {
                this.btnCancelDocument.Text = Resources.Close;


                IEnumerable<BaseEdit> editControls = this.EnumerateComponents().Where(x => x is BaseEdit).Cast<BaseEdit>();
                foreach (BaseEdit editControl in editControls)
                {
                    editControl.ReadOnly = true;
                }
                layoutControlItemOK.Visibility = LayoutVisibility.Never;
                layoutControlItemCancel.Visibility = LayoutVisibility.Never;

                //btnSaveDocument.Visible = false;
                //btnSavePrintDocument.Visible = false;
                layoutBtnSaveDoc.Visibility = LayoutVisibility.Never;
                layoutBtnSavePrintDoc.Visibility = LayoutVisibility.Never;

                btnCancelDocument.Enabled = true;
                btnCancelDocument.TabStop = true;
                (this.xtpDocumentDetails as Control).Enabled = true;
                gridDocumentDetails.Enabled = true;
                gridDocumentDetails.TabStop = true;
                grdDocumentDetails.OptionsBehavior.Editable = false;
                grcCommandColumn.Visible = true;
                grcCommandEdit.Visible = true;

                lueDocumentStatus.Enabled = false;
                lueDocumentStatus.TabStop = false;

                textEditDocumentHeaderTriangularProfession.ReadOnly = true;
                textEditDocumentHeaderTriangularProfession.TabStop = false;
            }
            else
            {
                CreateNewDocumentPayment();
            }
            EnableDisableDocumentHeaderFields();
            EnableDisableDetailEditFields(true, true);

            LayoutDocumentHeaderPropertyInfo = typeof(StoreControllerClientSettings).GetProperty(this.LayoutDocumentHeaderProperty);
            LayoutDocumentDetailPropertyInfo = typeof(StoreControllerClientSettings).GetProperty(this.LayoutDocumentDetailProperty);
            LayoutInitialization(LayoutDocumentHeaderPropertyInfo, lcmDocumentHeader);
            LayoutInitialization(LayoutDocumentDetailPropertyInfo, lcmDocumentDetail);
            LayoutInitialization(this.LayoutTraderPropertyInfo, lcmTrader);
            LayoutInitialization(this.LayoutTotalPropertyInfo, lcmTotal);
            this.DocumentHeader.Changed += DocumentHeader_Changed;
            onInitialization = false;
            lcs.ToList().ForEach(x => x.Value.HideToCustomization());



            this.lueDocumentType.Focus();

            ShowHideTransformationInfoTabs();
        }

        private void ShowHideTransformationInfoTabs()
        {
            if (this.DocumentHeader != null)
            {
                this.xtraTabPageReferencedDocuments.PageVisible = this.DocumentHeader.ReferencedDocuments.Count > 0;
                this.xtraTabPageDerivedDocuments.PageVisible = this.DocumentHeader.DerivedDocuments.Count > 0;
            }
        }

        private void LayoutInitialization(PropertyInfo pInfo, LayoutControl layout)
        {
            if (pInfo == null)
            {
                return;
            }
            try
            {
                string value = pInfo.GetValue(Program.Settings, null) as string;
                if (!String.IsNullOrWhiteSpace(value))
                {
                    RestoreLayout(layout, value);
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.GetFullMessage();
            }
        }

        protected virtual void DocumentHeader_Changed(object sender, ObjectChangeEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DocumentType":
                    DocumentTypeChanged(sender, e);
                    break;
                case "DocumentSeries":
                    DocumentSeriesChanged(sender, e);
                    break;
                case "ChargedToUser":
                    ChargedToChanged(sender, e);
                    break;
                case "BillingAddress":
                    if (e.OldValue != e.NewValue) // BillingAddress has changed
                    {
                        if (DocumentHeader.DocumentDetails.Count > 0)
                        {
                            bool recomputeDocumentDetails = DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT;
                            DocumentHelper.RecalculateDocumentCosts(ref this._DocumentHeader, recomputeDocumentDetails);
                        }
                    }
                    Address newAddress = (Address)(e.NewValue);
                    if (newAddress != null)
                    {
                        NewValewInit = newAddress.ToString();
                        DocumentHeader.DeliveryAddress = newAddress.ToString();
                        DocumentHeader.AddressProfession = newAddress.Profession;
                    }
                    break;
                default:
                    break;
            }
            EnableDisableDocumentHeaderFields();
            UpdateVatAnalysis();
        }

        protected virtual void EnableDisableDocumentHeaderFields()
        {
            if (PreviewDocument == false)
            {
                dtInvoicingDate.ReadOnly = !DocumentHeader.HasBeenExecuted;
                txtDocumentNumber.TabStop = txtDocumentNumber.Enabled = !(DocumentHeader.DocumentSeries == null || DocumentHeader.DocumentSeries.HasAutomaticNumbering);

                btnOkEditLine.Enabled = btnCancelEditLine.Enabled = (this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT) && this.EditingDocumentDetail != null;

                layoutControlItemCancel.Visibility =
                layoutControlItemOK.Visibility =
                lcOkPayment.Visibility =
                lcCancelPayment.Visibility =
                this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT ? LayoutVisibility.Always : LayoutVisibility.Never;

                lcOkPayment.Visibility = lcCancelPayment.Visibility = lcRemainingAmountPayment.Visibility = this.DocumentHeader.DocumentType == null || this.DocumentHeader.DocumentType.UsesPaymentMethods ? LayoutVisibility.Always : LayoutVisibility.Never;
                lcDocumentPaymentEdit.Enabled = this.DocumentHeader.DocumentType == null || this.DocumentHeader.DocumentType.UsesPaymentMethods;
                grcPaymentsCommandEdit.Visible = this.DocumentHeader.DocumentType == null || this.DocumentHeader.DocumentType.UsesPaymentMethods;
                grcPaymentsCommandColumn.Visible = this.DocumentHeader.DocumentType == null || this.DocumentHeader.DocumentType.UsesPaymentMethods;
                grdDocumentPayments.OptionsBehavior.Editable = this.DocumentHeader.DocumentType == null || this.DocumentHeader.DocumentType.UsesPaymentMethods;
                lueChargedTo.Enabled = this.DocumentHeader.DocumentType == null || this.DocumentHeader.DocumentType.ChargeToUser;
            }
            else
            {
                btnOkEditLine.Enabled = false;
                btnOkEditLine.Visible = false;

                //DocumentPaymentsGrid
                lcOkPayment.Visibility = lcCancelPayment.Visibility = lcRemainingAmountPayment.Visibility = LayoutVisibility.Never;
                lcDocumentPaymentEdit.Enabled = false;
                grcPaymentsCommandEdit.Visible = false;
                grcPaymentsCommandColumn.Visible = false;
                grdDocumentPayments.OptionsBehavior.Editable = false;

                gridViewLinkedDocumentDetails.OptionsBehavior.Editable = false;
                gridViewMainDocumentDetails.OptionsBehavior.Editable = false;
            }
            SetTabStop();
        }

        private void UpdateVatAnalysis()
        {
            foreach (VatAnalysis analysis in this.VatAnalysis)
            {
                analysis.PropertyChanged += VatAnalysisChange;
            }
            this.VatAnalysis.Clear();
            this.DocumentHeader.DocumentDetails.GroupBy(x => x.VatFactorGuid).Select(x => new VatAnalysis()
            {
                GrossTotal = x.Sum(y => y.GrossTotal),
                NetTotal = x.Sum(y => y.NetTotal),
                TotalVatAmount = x.Sum(y => y.TotalVatAmount),
                VatFactor = x.First().VatFactor,
                VatFactorGuid = x.First().VatFactorGuid,
                Qty = x.Sum(y => y.Qty)
            }).ToList().ForEach(x => { x.PropertyChanged += VatAnalysisChange; this.VatAnalysis.Add(x); });
            this.gridVatAnalysis.MainView = this.grdVatAnalysis;
            this.gridVatAnalysis.RefreshDataSource();
        }

        protected virtual void VatAnalysisChange(object sender, PropertyChangedEventArgs e)
        {

        }

        protected virtual void LookupeditInitialization()
        {

        }

        Dictionary<string, LayoutControlItem> lcs;

        private void InitializeLookupEdits()
        {

            gridVatAnalysis.DataSource = this.VatAnalysis;
            CriteriaOperator criteria = null;
            lueDocumentType.Properties.DataSource = (PreviewDocument
                                                    || (this.DocumentHeader != null && this.DocumentHeader.ReferencedDocuments.Count > 0)
                                                    )
                                                    ? new List<DocumentType>() { DocumentHeader.DocumentType } :
                                                    StoreHelper.StoreDocumentTypes(DocumentHeader.Store,
                                                                                    DocumentHeader.Division,
                                                                                    DocumentHelper.GetDocSeriesModule(Program.Settings.MasterAppInstance));

            lueDocumentType.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            lueDocumentType.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueDocumentType.Properties.ValueMember = "Oid";
            lueDocumentType.Properties.DisplayFormat.FormatString = "{0} {1}";

            if (DocumentHeader.DocumentType != null)
            {
                lueDocumentSeries.Properties.DataSource = PreviewDocument ? new List<DocumentSeries>() { this.DocumentHeader.DocumentSeries }
                                                                          : StoreHelper.StoreSeriesForDocumentType(DocumentHeader.Store,
                                                                                                                   DocumentHeader.DocumentType,
                                                                                                                   GetModule(),
                                                                                                                   PreviewDocument,
                                                                                                                   PreviewDocument
                                                                                                                   ).ToList();
            }



            criteria = new BinaryOperator("IsActive", true);
            lueDocumentStatus.Properties.DataSource = new XPCollection<DocumentStatus>(this.DocumentHeader.Session, criteria);
            lueDocumentStatus.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueDocumentStatus.Properties.ValueMember = "Oid";

            ///
            lueBillingAddress.Properties.ValueMember = "Oid";
            lueBillingAddress.Properties.Columns.Clear();
            lueBillingAddress.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            if (DocumentHeader.Customer != null)
            {
                lueBillingAddress.Properties.DataSource = DocumentHeader.Customer.Trader.Addresses;
            }
            else if (DocumentHeader.Supplier != null)
            {
                lueBillingAddress.Properties.DataSource = DocumentHeader.Supplier.Trader.Addresses;
            }

            ///
            lueDocumentSeries.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lueDocumentSeries.Properties.ValueMember = "Oid";

            luePaymentMethod.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            luePaymentMethod.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            luePaymentMethod.Properties.DataSource = new XPCollection<PaymentMethod>(this.DocumentHeader.Session);
            lcs = new Dictionary<string, LayoutControlItem>()
            {
                {"DateField1",lcDateField1}, {"DateField2",lcDateField2}, {"DateField3",lcDateField3}, {"DateField4",lcDateField4},
                {"DateField5",lcDateField5}, {"DecimalField1",lcDecimalField1}, {"DecimalField2",lcDecimalField2}, {"DecimalField3",lcDecimalField3},
                {"DecimalField4",lcDecimalField4}, {"DecimalField5",lcDecimalField5}, {"StringField1",lcStringField1}, {"StringField2",lcStringField2},
                {"StringField3",lcStringField3}, {"StringField4",lcStringField4}, {"StringField5",lcStringField5}, {"IntegerField1",lcIntegerField1},
                {"IntegerField2",lcIntegerField2}, {"IntegerField3",lcIntegerField3}, {"IntegerField4",lcIntegerField4}, {"IntegerField5",lcIntegerField5},
                {"CustomEnumerationValue1",lcCustomEnumerationValue1},
                {"CustomEnumerationValue2",lcCustomEnumerationValue2},
                {"CustomEnumerationValue3",lcCustomEnumerationValue3},
                {"CustomEnumerationValue4",lcCustomEnumerationValue4},
                {"CustomEnumerationValue5",lcCustomEnumerationValue5}
            };

            lueVatFactor.Properties.DataSource = new XPCollection<VatFactor>(Program.Settings.ReadOnlyUnitOfWork);
            lueVatFactor.Properties.ValueMember = "Factor";
            lueVatFactor.Properties.DisplayMember = "Factor";
            lueVatFactor.Properties.Columns.Clear();
            LookUpColumnInfo vatFactors = new LookUpColumnInfo("Factor", Resources.VatFactor);
            vatFactors.FormatType = DevExpress.Utils.FormatType.Numeric;
            vatFactors.FormatString = "P";
            lueVatFactor.Properties.Columns.Add(vatFactors);

            if (this.DocumentHeader.DocumentType != null)
            {
                RefreshDocumentItemsDataSource();
            }
            lueDescription.Properties.ValueMember = "Oid";
            lueDescription.Properties.DisplayMember = "Name";


            CriteriaOperator transferPurposeCriteria = RetailHelper.ApplyOwnerCriteria(null, typeof(TransferPurpose), Program.Settings.StoreControllerSettings.Owner);
            this.lookUpEditTransferPurpose.Properties.DataSource = new XPServerCollectionSource(this.DocumentHeader.Session, typeof(TransferPurpose), transferPurposeCriteria);
            this.lookUpEditTransferPurpose.Properties.ValueMember = "Oid";
            this.lookUpEditTransferPurpose.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            this.lookUpEditTransferPurpose.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));

            criteria = CriteriaOperator.And(new BinaryOperator("IsActive", true), new BinaryOperator("IsPOSUser", true));
            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("EntityType", "ITS.Retail.Model.Store", BinaryOperatorType.Equal), new BinaryOperator("EntityOid", DocumentHeader.Store.Oid));
            List<UserTypeAccess> user_stores = new XPCollection<UserTypeAccess>(DocumentHeader.Store.Session, crop).ToList();

            lueChargedTo.Properties.ValueMember = "Oid";
            lueChargedTo.Properties.Columns.Add(new LookUpColumnInfo("UserName", Resources.UserName));
            lueChargedTo.Properties.Columns.Add(new LookUpColumnInfo("FullName", Resources.FullName));
            lueChargedTo.Properties.Columns.Add(new LookUpColumnInfo("POSUserName", Resources.POSUserName));
            lueChargedTo.Properties.DataSource = new XPCollection<User>(DocumentHeader.Session, criteria).ToList().Intersect(user_stores.Select(u => u.User));
            lcChargedToUser.Text = Resources.ChargeToUser;
            LookupeditInitialization();
        }

        protected virtual void BindingInit()
        {

        }

        XPCollection mainDetails, linkedDetails;

        private void InitializeBindings()
        {
            this.lueDocumentType.DataBindings.Add("EditValue", this.DocumentHeader, "DocumentType!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueDocumentSeries.DataBindings.Add("EditValue", this.DocumentHeader, "DocumentSeries!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueBillingAddress.DataBindings.Add("EditValue", DocumentHeader, "BillingAddress!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueDocumentStatus.DataBindings.Add("EditValue", this.DocumentHeader, "Status!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lueChargedTo.DataBindings.Add("EditValue", this.DocumentHeader, "ChargedToUser!Key", true, DataSourceUpdateMode.OnPropertyChanged);

            this.txtDocumentNumber.DataBindings.Add("EditValue", this.DocumentHeader, "DocumentNumber", true, DataSourceUpdateMode.OnValidation);
            this.chkHasBeenChecked.DataBindings.Add("EditValue", this.DocumentHeader, "HasBeenChecked", true, DataSourceUpdateMode.OnPropertyChanged);
            this.chkHasBeenExecuted.DataBindings.Add("EditValue", this.DocumentHeader, "HasBeenExecuted", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dtExecutionDate.DataBindings.Add("EditValue", this.DocumentHeader, "ExecutionDate", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dtFinalizeDate.DataBindings.Add("EditValue", this.DocumentHeader, "FinalizedDate", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dtInvoicingDate.DataBindings.Add("EditValue", this.DocumentHeader, "InvoicingDate", true, DataSourceUpdateMode.OnPropertyChanged);
            this.dtReferenceDate.DataBindings.Add("EditValue", this.DocumentHeader, "RefferenceDate", true, DataSourceUpdateMode.OnPropertyChanged);


            this.txtDocumentGrossTotal.DataBindings.Add("EditValue", this.DocumentHeader, "GrossTotal", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDocumentNetTotal.DataBindings.Add("EditValue", this.DocumentHeader, "NetTotal", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDocumentNetTotalBeforeDiscount.DataBindings.Add("EditValue", this.DocumentHeader, "NetTotalBeforeDiscount", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDocumentTotalVatAmount.DataBindings.Add("EditValue", this.DocumentHeader, "TotalVatAmount", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDocumentPoints.DataBindings.Add("EditValue", this.DocumentHeader, "TotalPoints", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDocumentRemarks.DataBindings.Add("EditValue", this.DocumentHeader, "Remarks", true, DataSourceUpdateMode.OnPropertyChanged);

            this.txtTotalInPayments.DataBindings.Add("EditValue", this.DocumentHeader, "TotalAmountInPayments", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtRemainingPaymentAmount.DataBindings.Add("EditValue", this.DocumentHeader, "RemainingPayment", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtAddressProfession.DataBindings.Add("EditValue", this.DocumentHeader, "AddressProfession", true, DataSourceUpdateMode.OnPropertyChanged);
            //this.txtTriangularAddress.DataBindings.Add("EditValue", DocumentHeader, "TriangularAddress", true, DataSourceUpdateMode.OnPropertyChanged);

            this.textEditTransferMethod.DataBindings.Add("EditValue", this.DocumentHeader, "TransferMethod", true, DataSourceUpdateMode.OnPropertyChanged);
            this.textEditPlaceOfLoading.DataBindings.Add("EditValue", this.DocumentHeader, "PlaceOfLoading", true, DataSourceUpdateMode.OnPropertyChanged);
            this.lookUpEditTransferPurpose.DataBindings.Add("EditValue", this.DocumentHeader, "TransferPurpose!Key", true, DataSourceUpdateMode.OnPropertyChanged);
            this.textEditVehicleNumber.DataBindings.Add("EditValue", this.DocumentHeader, "VehicleNumber", true, DataSourceUpdateMode.OnPropertyChanged);

            mainDetails = new XPCollection(this.DocumentHeader.DocumentDetails);
            linkedDetails = new XPCollection(this.DocumentHeader.DocumentDetails);

            mainDetails.Filter = CriteriaOperator.Or(
                new BinaryOperator("LinkedLine", Guid.Empty),
                new NullOperator("LinkedLine"));

            this.gridDocumentDetails.DataSource = this.DocumentHeader.DocumentDetails;
            this.grdMainDocumentDetails.DataSource = mainDetails;
            /*this.grdMainDocumentDetails.DataSource = this.DocumentHeader.DocumentDetails;
            this.gridViewMainDocumentDetails.ActiveFilterCriteria = CriteriaOperator.Or(
                new BinaryOperator("LinkedLine", Guid.Empty),
                new NullOperator("LinkedLine"));*/


            this.grdLinkedDocumentDetails.DataSource = linkedDetails;
            /*this.gridViewLinkedDocumentDetails.ActiveFilterCriteria = CriteriaOperator.And(
                    new BinaryOperator("LinkedLine", Guid.Empty, BinaryOperatorType.NotEqual),
                    new BinaryOperator("LinkedLine", Guid.Empty)
                );*/

            documentDetailDataBindings = new Dictionary<BaseEdit, string>();
            documentDetailDataBindings.Add(txtItemCode, "ItemCode");
            documentDetailDataBindings.Add(txtBarcode, "BarcodeCode");
            documentDetailDataBindings.Add(txtDescriptionReadOnly, "CustomDescription");
            documentDetailDataBindings.Add(txtQty, "PackingQuantity");
            documentDetailDataBindings.Add(txtMeasurementUnit, "CustomMeasurementUnit");
            documentDetailDataBindings.Add(lueVatFactor, "VatFactor");
            documentDetailDataBindings.Add(txtUnitPrice, "CustomUnitPrice");//UnitPrice
            documentDetailDataBindings.Add(txtPoints, "Points");
            documentDetailDataBindings.Add(txtNetTotalBeforeDisocunt, "NetTotalBeforeDiscount");
            documentDetailDataBindings.Add(txtPriceCatalogDiscountPercentage, "PriceCatalogDiscountPercentage");
            documentDetailDataBindings.Add(txtPriceCatalogDiscountValue, "PriceCatalogDiscountAmount");
            documentDetailDataBindings.Add(txtTotalDiscountPercentage, "PriceCatalogDiscountAmount");
            documentDetailDataBindings.Add(txtTotalDiscountValue, "PriceCatalogDiscountAmount");
            documentDetailDataBindings.Add(txtNetTotal, "NetTotal");
            documentDetailDataBindings.Add(txtTotalVatAmount, "TotalVatAmount");
            documentDetailDataBindings.Add(txtGrossTotal, "GrossTotal");
            documentDetailDataBindings.Add(memoEditRemarks, "Remarks");

            gridDocumentPayments.DataSource = this.DocumentHeader.DocumentPayments;
            grdDocumentPayments.OptionsEditForm.CustomEditFormLayout = new DocumentPaymentEditor(this.DocumentHeader.Session as UnitOfWork);

            this.gridControlReferencedDocuments.DataSource = this.DocumentHeader.ReferencedDocuments;
            this.gridControlDerivedDocuments.DataSource = this.DocumentHeader.DerivedDocuments;
            BindingInit();
        }

        private void ResetDocumentDetailDiscountBindings()
        {
            txtSecondDiscountPercentage.DataBindings.Clear();
            txtSecondDiscountValue.DataBindings.Clear();

            if (EditingDocumentDetailDiscount != null)
            {
                txtSecondDiscountPercentage.DataBindings.Add("EditValue", this.EditingDocumentDetailDiscount, "Percentage", true, DataSourceUpdateMode.OnValidation);
                txtSecondDiscountValue.DataBindings.Add("EditValue", this.EditingDocumentDetailDiscount, "Value", true, DataSourceUpdateMode.OnValidation);
            }
            else
            {
                txtSecondDiscountPercentage.EditValue = txtSecondDiscountValue.EditValue = null;
            }
        }

        private void ResetDocumentDetailBindings()
        {
            foreach (var pair in documentDetailDataBindings)
            {
                pair.Key.DataBindings.Clear();
                if (EditingDocumentDetail != null)
                {
                    pair.Key.DataBindings.Add("EditValue", this.EditingDocumentDetail, pair.Value, true, DataSourceUpdateMode.OnValidation);
                }
                else
                {
                    if (pair.Key is CalcEdit || pair.Key is SpinEdit)
                    {
                        pair.Key.EditValue = 0;
                    }
                    else
                    {
                        pair.Key.EditValue = null;
                    }
                }
            }
            btnOkEditLine.Enabled = btnCancelEditLine.Enabled = (this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT) && this.EditingDocumentDetail != null;
        }

        protected virtual void SetDeliveryAddressBindings()
        {
            txtStreet.DataBindings.Clear();
            txtPOBox.DataBindings.Clear();
            txtZipCode.DataBindings.Clear();
            txtCity.DataBindings.Clear();
            txtDeliveryAddress.DataBindings.Clear();
            textEditDeliveryProfession.DataBindings.Clear();
            txtProfession.DataBindings.Clear();
            if (DeliveryAddress != null)
            {
                txtStreet.DataBindings.Add("EditValue", this.DeliveryAddress, "Street", true, DataSourceUpdateMode.OnPropertyChanged);
                txtPOBox.DataBindings.Add("EditValue", this.DeliveryAddress, "POBox", true, DataSourceUpdateMode.OnPropertyChanged);
                txtZipCode.DataBindings.Add("EditValue", this.DeliveryAddress, "PostCode", true, DataSourceUpdateMode.OnPropertyChanged);
                txtCity.DataBindings.Add("EditValue", this.DeliveryAddress, "City", true, DataSourceUpdateMode.OnPropertyChanged);
                textEditDeliveryProfession.DataBindings.Add("EditValue", this.DeliveryAddress, "Profession", true, DataSourceUpdateMode.OnPropertyChanged);
                txtDeliveryAddress.DataBindings.Add("EditValue", this.DocumentHeader, "DeliveryAddress", true, DataSourceUpdateMode.OnPropertyChanged);
                txtProfession.DataBindings.Add("EditValue", this.DocumentHeader, "DeliveryProfession", true, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        protected virtual void SetTriangularAddressBindings()
        {
            txtTrStreet.DataBindings.Clear();
            txtTrPOBOx.DataBindings.Clear();
            txtTrPostCode.DataBindings.Clear();
            txtTrCity.DataBindings.Clear();
            txtTriangularAddress.DataBindings.Clear();
            textEditTriangularProfession.DataBindings.Clear();
            textEditDocumentHeaderTriangularProfession.DataBindings.Clear();
            if (TriangularAddress != null)
            {
                txtTrStreet.DataBindings.Add("EditValue", this.TriangularAddress, "Street", true, DataSourceUpdateMode.OnPropertyChanged);
                txtTrPOBOx.DataBindings.Add("EditValue", this.TriangularAddress, "POBox", true, DataSourceUpdateMode.OnPropertyChanged);
                txtTrPostCode.DataBindings.Add("EditValue", this.TriangularAddress, "PostCode", true, DataSourceUpdateMode.OnPropertyChanged);
                txtTrCity.DataBindings.Add("EditValue", this.TriangularAddress, "City", true, DataSourceUpdateMode.OnPropertyChanged);
                textEditTriangularProfession.DataBindings.Add("EditValue", this.TriangularAddress, "Profession", true, DataSourceUpdateMode.OnPropertyChanged);
                txtTriangularAddress.DataBindings.Add("EditValue", this.DocumentHeader, "TriangularAddress", true, DataSourceUpdateMode.OnPropertyChanged);
                textEditDocumentHeaderTriangularProfession.DataBindings.Add("EditValue", this.DocumentHeader, "TriangularProfession", true, DataSourceUpdateMode.OnPropertyChanged);
            }
        }


        private void DocumentTypeChanged(object sender, ObjectChangeEventArgs e)
        {
            if (!this.InvokeRequired)
            {
                documentTypeChanged();
            }
            else
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    documentTypeChanged();
                });
            }

        }

        protected virtual void documentTypeChanged()
        {
            DocumentHeader.DocumentSeries = null;
            if (DocumentHeader.DocumentType == null)
            {
                lueDocumentSeries.Properties.DataSource = new List<DocumentSeries>();
            }
            else
            {
                lueDocumentSeries.Properties.DataSource = StoreHelper.StoreSeriesForDocumentType(DocumentHeader.Store, DocumentHeader.DocumentType, GetModule(), PreviewDocument, PreviewDocument).ToList();
                if (onInitialization == false)
                {
                    DocumentHeader.Status = DocumentHeader.DocumentType.DefaultDocumentStatus;
                    IEnumerable<DocumentSeries> docSeries = StoreHelper.StoreSeriesForDocumentType(DocumentHeader.Store, DocumentHeader.DocumentType, GetModule(), PreviewDocument, PreviewDocument);
                    if (docSeries.Count() == 1)
                    {
                        DocumentHeader.DocumentSeries = docSeries.First();
                    }
                }
                RefreshDocumentItemsDataSource();
            }
        }
        private void ChargedToChanged(object sender, ObjectChangeEventArgs e)
        {
            if (!this.InvokeRequired)
            {
                ChargedToChanged();
            }
            else
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    ChargedToChanged();
                });
            }

        }
        protected virtual void ChargedToChanged()
        {
            DialogResult dialogResult = DialogResult.None;
            using (LoginForm loginForm = new LoginForm(false, lueChargedTo.Text))
            {
                dialogResult = loginForm.ShowDialog();
                if (dialogResult != DialogResult.OK)
                {
                    lueChargedTo.Text = null;
                    this.DocumentHeader.ChargedToUser = null;
                }
            }
        }

        private void DocumentSeriesChanged(object sender, ObjectChangeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    documentSeriesChanged();
                });
            }
            else
            {
                documentSeriesChanged();
            }
        }

        private void documentSeriesChanged()
        {
            if (onInitialization == false && DocumentHeader.DocumentSeries != null)
            {
                StoreDocumentSeriesType sdst = DocumentHeader.Session.FindObject<StoreDocumentSeriesType>(
                    CriteriaOperator.And(
                        new BinaryOperator("DocumentSeries.Oid", DocumentHeader.DocumentSeries.Oid),
                        new BinaryOperator("DocumentType.Oid", DocumentHeader.DocumentType.Oid)
                    ));
                if (sdst != null && sdst.DefaultCustomer != null)
                {

                    DocumentHeader.Customer = sdst.DefaultCustomer;
                }
            }
            txtDocumentNumber.Enabled = !(DocumentHeader.DocumentSeries == null || DocumentHeader.DocumentSeries.HasAutomaticNumbering);
        }

        private void txtCodeOrBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCodeOrBarcode.Text) == false && e.KeyChar == 13)
            {
                SearchAndAddDocumentDetail();
            }
        }

        protected virtual string SearchAndAddDocumentDetail(string code, string barcode, DocumentDetail existingDetail, List<Guid> forbiddenItems)
        {
            try
            {
                this.WeightedBarcodeInfo = null;

                ItemBarcode itembarcode = this.DocumentHeader.Session.FindObject<ItemBarcode>(CriteriaOperator.And(
                   CriteriaOperator.Or(new BinaryOperator("Barcode.Code", barcode), new BinaryOperator("Item.Code", code)),
                   new BinaryOperator("Owner.Oid", this.DocumentHeader.Owner),
                   new BinaryOperator("Item.IsActive", true)
                   ));

                if (itembarcode == null)
                {
                    this.WeightedBarcodeInfo = ItemHelper.GetWeightedBarcodeInfo(barcode, this.DocumentHeader);
                    if (this.WeightedBarcodeInfo != null)
                    {
                        if (this.WeightedBarcodeInfo.ItemBarcode.HasValue)
                        {
                            itembarcode = this.DocumentHeader.Session.GetObjectByKey<ItemBarcode>(this.WeightedBarcodeInfo.ItemBarcode.Value);
                        }
                    }
                }

                if (itembarcode == null)
                {
                    txtCodeOrBarcode.EditValue = "";
                    lueDescription.EditValue = null;
                    return Resources.ItemNotFound;
                }
                else if (!DocumentHelper.DocumentTypeSupportsItem(this.DocumentHeader, itembarcode.Item))
                {
                    return Resources.ItemNotSupported;
                }
                else if (forbiddenItems != null && forbiddenItems.Contains(itembarcode.Item.Oid))
                {
                    return Resources.MainItemAndLinkedItemCannotMatch;
                }
                else if (this.DocumentHeader.DocumentDetails.Any(x => x.Item.Oid == itembarcode.Item.Oid && x.Barcode.Oid == itembarcode.Barcode.Oid))
                {
                    decimal totalQty = DocumentHeader.DocumentDetails.Where((x => x.Item.Oid == itembarcode.Item.Oid && x.Barcode.Oid == itembarcode.Barcode.Oid)).Sum(x => x.Qty);
                    if (XtraMessageBox.Show(Resources.ItemAlreadyOrderedWithQuantity + totalQty + Environment.NewLine + Resources.Continue,
                        Resources.Message, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        txtCodeOrBarcode.EditValue = "";
                        lueDescription.EditValue = null;
                        return null;
                    }
                }

                decimal customUnitPrice = DocumentHeader.Division == eDivision.Purchase ?
                    (DocumentHeader.DocumentType.PriceSuggestionType == ePriceSuggestionType.NONE) ? 0 :
                    ItemHelper.GetSupplierLastPrice(itembarcode.Item, (DocumentHeader.DocumentType.PriceSuggestionType == ePriceSuggestionType.LAST_SUPPLIER_PRICE) ? DocumentHeader.Supplier : null) : 0;

                ClearEditingDocumentDetail(true);
                this.EditingDocumentDetail = existingDetail == null ? new DocumentDetail(DocumentHeader.Session) : existingDetail;

                EditingDocumentDetail.Item = itembarcode.Item;
                EditingDocumentDetail.ItemCode = itembarcode.Item.Code;
                EditingDocumentDetail.Barcode = itembarcode.Barcode;
                EditingDocumentDetail.BarcodeCode = itembarcode.Barcode.Code;
                EditingDocumentDetail.CustomUnitPrice = customUnitPrice;
                EditingDocumentDetail.HasCustomPrice = true;


                this.EditingDocumentDetailDiscount = new DocumentDetailDiscount(DocumentHeader.Session) { DiscountSource = eDiscountSource.CUSTOM };
                this.UpdateDocumentDetail(this.EditingDocumentDetail, null);
                this.EditingDocumentDetail.Changed += DocumentDetailChanged;
                txtQty.Focus();
                EditingDocumentDetailDiscount.Changed += DocumentDetailDiscountChanged;
                EnableDisableDetailEditFields(true, itembarcode.Item.AcceptsCustomDescription);
                return null;
            }
            catch (Exception ex)
            {
                return ex.GetFullMessage();
            }
        }

        private void SearchAndAddDocumentDetail()
        {
            string codeSearch = txtCodeOrBarcode.Text, barcodeSearch = txtCodeOrBarcode.Text;
            if (Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.PadBarcodes)
            {
                barcodeSearch = txtCodeOrBarcode.Text.PadLeft(Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.BarcodeLength,
                    Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
            }
            if (Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.PadItemCodes)
            {
                codeSearch = txtCodeOrBarcode.Text.PadLeft(Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.ItemCodeLength,
                    Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
            }
            string result = SearchAndAddDocumentDetail(codeSearch, barcodeSearch, null, null);
            if (String.IsNullOrWhiteSpace(result) == false)
            {
                XtraMessageBox.Show(result, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected void DocumentDetailDiscountChanged(object sender, ObjectChangeEventArgs e)
        {
            bool updateDocumentLine = false;
            if (e.Reason == ObjectChangeReason.PropertyChanged && EditingDocumentDetail != null)
            {
                EditingDocumentDetailDiscount.Changed -= DocumentDetailDiscountChanged;
                if (e.PropertyName == "Value")
                {
                    EditingDocumentDetailDiscount.DiscountType = eDiscountType.VALUE;
                    EditingDocumentDetailDiscount.Percentage = 0;
                    updateDocumentLine = true;
                }
                else if (e.PropertyName == "Percentage")
                {
                    EditingDocumentDetailDiscount.DiscountType = eDiscountType.PERCENTAGE;
                    EditingDocumentDetailDiscount.Value = 0;
                    updateDocumentLine = true;
                }
                if (updateDocumentLine)
                {
                    UpdateDocumentDetail(this.EditingDocumentDetail, null);
                }

                EditingDocumentDetailDiscount.Changed += DocumentDetailDiscountChanged;
            }
        }

        protected void DocumentDetailChanged(object sender, ObjectChangeEventArgs e)
        {
            if (e.Reason == ObjectChangeReason.PropertyChanged && e.OldValue != null && e.OldValue.Equals(e.NewValue) == false)
            {
                DocumentDetail detail = e.Object as DocumentDetail;
                detail.Changed -= DocumentDetailChanged;
                UpdateDocumentDetail(detail, e);
                detail.Changed += DocumentDetailChanged;
                if (detail.LinkedLine == Guid.Empty)
                {
                    UpdateLinkedMasterDetail();
                }
            }
        }
        protected virtual void UpdateDocumentDetail(DocumentDetail detail, ObjectChangeEventArgs objectChangeEventArgs)
        {
            throw new NotImplementedException();
        }



        private void grdDocumentDetails_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            DocumentDetail detail = view.GetRow(view.FocusedRowHandle) as DocumentDetail;
            e.Cancel = (DocumentHeader.TransformationLevel != eTransformationLevel.DEFAULT) || (detail != null && detail.IsLinkedLine);
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                FocusOnNextField(txtQty);
            }
            else
            {
                if (this.EditingDocumentDetail != null
                 && this.EditingDocumentDetail.Barcode != null
                 && (e.KeyChar == 44 || e.KeyChar == 46)//comma and period
                   )
                {
                    if (this.EditingDocumentDetail.Barcode.MeasurementUnit(this.DocumentHeader.Owner).SupportDecimal == false)
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        if (txtQty.EditValue != null && txtQty.EditValue.ToString().Contains(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator))
                        {
                            e.Handled = true;
                            return;
                        }

                        string comma = ",";
                        string period = ".";

                        //comma 
                        if (e.KeyChar == 44
                          && Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator == period
                           )
                        {
                            if (txtQty.EditValue != null && txtQty.EditValue.ToString().Contains(period) == false)
                            {
                                txtQty.EditValue = txtQty.EditValue.ToString() + period;
                                txtQty.SelectionStart = txtQty.EditValue.ToString().Length;
                                txtQty.SelectionLength = 0;
                            }
                            e.Handled = true;
                        }

                        //period
                        if (e.KeyChar == 46
                          && Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator == comma
                           )
                        {
                            if (txtQty.EditValue != null && txtQty.EditValue.ToString().Contains(comma) == false)
                            {
                                txtQty.EditValue = txtQty.EditValue.ToString() + comma;
                                txtQty.SelectionStart = txtQty.EditValue.ToString().Length;
                                txtQty.SelectionLength = 0;
                            }
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void txtSecondDiscountPercentage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                FocusOnNextField(txtSecondDiscountPercentage);
            }
        }

        private void txtSecondDiscountValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                FocusOnNextField(txtSecondDiscountValue);
            }
        }

        private void btnOkEditLine_Click(object sender, EventArgs e)
        {
            if (EditingDocumentDetail != null)
            {
                ClearEditingDocumentDetail(false);

                if (EditingDocumentDetail.CustomUnitPrice == 0 && DocumentHeader.DocumentType != null && DocumentHeader.DocumentType.AllowItemZeroPrices == false)
                {
                    XtraMessageBox.Show(Resources.InvalidValue, Resources.InvalidValue, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (EditingDocumentDetail.GrossTotal > this.DocumentHeader.DocumentType.MaxDetailTotal && this.DocumentHeader.DocumentType.MaxDetailTotal > 0)
                {
                    XtraMessageBox.Show(Resources.InvalidDetailTotal, Resources.InvalidDetailTotal, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (EditingDocumentDetail.DocumentHeader == null)
                {
                    string errormsg;
                    if (DocumentHelper.MaxCountOfLinesExceeded(_DocumentHeader, out errormsg))
                    {
                        XtraMessageBox.Show(errormsg, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {
                        DocumentHelper.AddItem(ref _DocumentHeader, EditingDocumentDetail);
                    }
                }
                else
                {
                    DocumentHelper.UpdateLinkedItems(ref _DocumentHeader, EditingDocumentDetail);
                }
                EditingDocumentDetail = null;
                EditingDocumentDetailDiscount = null;
                txtCodeOrBarcode.EditValue = "";
                lueDescription.EditValue = null;
                txtCodeOrBarcode.Focus();
                gridDocumentDetails.RefreshDataSource();
                UpdateVatAnalysis();
            }
            EnableDisableDetailEditFields(false, false);
            grdDocumentDetails.VisibleColumns.Where(col => col == grcCommandColumn).FirstOrDefault().OptionsColumn.AllowEdit = true;
        }

        private void btnCancelEditLine_Click(object sender, EventArgs e)
        {
            CancelCurrentEditLine();
            grdDocumentDetails.VisibleColumns.Where(col => col == grcCommandColumn).FirstOrDefault().OptionsColumn.AllowEdit = true;
        }

        protected void CancelCurrentEditLine()
        {
            if (EditingDocumentDetail != null)
            {
                ClearEditingDocumentDetail(false);
                if (EditingDocumentDetail.DocumentHeader == null)
                {
                    EditingDocumentDetail.Delete();
                    EditingDocumentDetailDiscount.Delete();
                }
                else
                {
                    string error;
                    bool result = EditingDocumentDetail.FromJson(this.OldDocumentDetailSerialized,
                        PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    this.EditingDocumentDetail.DocumentHeader = this.DocumentHeader;
                }

                EditingDocumentDetail = null;
                EditingDocumentDetailDiscount = null;
                //txtQty.EditValue = 0;
                //txtUnitPrice.EditValue = (decimal)0;
                txtCodeOrBarcode.Focus();
                txtCodeOrBarcode.EditValue = "";
                lueDescription.EditValue = null;

                if (this.DocumentHeader.Division == eDivision.Financial)
                {
                    DocumentHelper.SetFinancialDocumentDetail(this.DocumentHeader);
                }
            }
            EnableDisableDetailEditFields(false, false);
        }

        private void lueDescription_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Item item = this.DocumentHeader.Session.GetObjectByKey<Item>(lueDescription.EditValue);
                if (item != null)
                {
                    txtCodeOrBarcode.EditValue = item.Code;
                    SearchAndAddDocumentDetail();
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Debug(ex, "lueDescription_EditValueChanged");
            }
        }

        private void gridDocumentDetails_DoubleClick(object sender, EventArgs e)
        {
            DocumentDetail detail = grdDocumentDetails.GetRow(grdDocumentDetails.FocusedRowHandle) as DocumentDetail;
            StartEditLine(detail);
        }

        private void StartEditLine(DocumentDetail detail)
        {
            CancelCurrentEditLine();
            if (detail != null && detail.IsLinkedLine == false)
            {
                ClearEditingDocumentDetail(true);
                EditingDocumentDetail = detail;
                EditingDocumentDetailDiscount = EditingDocumentDetail.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.CUSTOM);
                if (EditingDocumentDetailDiscount == null)
                {
                    EditingDocumentDetailDiscount = new DocumentDetailDiscount(EditingDocumentDetail.Session);
                }
                EditingDocumentDetail.Changed += DocumentDetailChanged;
                EditingDocumentDetailDiscount.Changed += DocumentDetailDiscountChanged;
                this.OldDocumentDetailSerialized = detail.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                txtQty.Focus();
                EnableDisableDetailEditFields(detail.Item.AcceptsCustomPrice || detail.HasCustomPrice, detail.Item.AcceptsCustomDescription);
            }
        }

        private void btnDeleteGridRow_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show(Resources.ConfirmDelete, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DocumentDetail detail = grdDocumentDetails.GetRow(grdDocumentDetails.FocusedRowHandle) as DocumentDetail;
                if (EditingDocumentDetail != null && detail.Oid == EditingDocumentDetail.Oid)
                {
                    CancelCurrentEditLine();
                }
                else if (detail != null && detail.IsLinkedLine == false)
                {
                    DocumentHelper.DeleteItem(ref this._DocumentHeader, detail);
                    DocumentHelper.RecalculateDocumentCosts(ref this._DocumentHeader, false, false);
                }
            }
        }

        protected void xpInstantFeedbackSource1_ResolveSession(object sender, ResolveSessionEventArgs e)
        {
            e.Session = XpoHelper.GetNewUnitOfWork();
        }

        protected void xpInstantFeedbackSource1_DismissSession(object sender, ResolveSessionEventArgs e)
        {
            if (e.Session is UnitOfWork)
            {
                ((UnitOfWork)e.Session).Dispose();
            }
        }

        private void btnCancelDocument_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected void SaveDocument(object sender, EventArgs e)
        {
            string message;
            int tab;
            string tabTag;
            ClearEditingDocumentDetail(true);
            if (DocumentHelper.IsDocumentReadyForSaving(this.DocumentHeader, out message, out tab, out tabTag))
            {
                if (DocumentHeader.DocumentType.ChargeToUser == (DocumentHeader.ChargedToUser != null))
                {
                    if (notIncludedDetails.Count > 0)
                    {
                        XtraMessageBox.Show(Resources.DocumentContainsNotIncludedItems, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.tabDocumentInfo.SelectedTabPage = xtpDocumentDetails;
                        return;
                    }
                    this.DocumentHeader.Changed -= this.DocumentHeader_Changed;

                    ClearEditingDocumentDetail(true);
                    try
                    {
                        using (ITSStoreControllerDesktopServiceClient itsService = Program.Settings.ITSStoreControllerDesktopService)
                        {
                            this.DocumentHeader.UpdatedOnTicks = itsService.GetNowTicks();
                        }
                        this.DocumentHeader.UpdatedBy = this.DocumentHeader.Session.GetObjectByKey<User>(Program.Settings.CurrentUser.Oid);
                        this.DocumentHeader.Save();
                        this.DocumentHeader.Session.CommitTransaction();
                    }
                    catch (Exception except)
                    {
                        XtraMessageBox.Show(Resources.ConnectionTimeOut + Environment.NewLine + except.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    this.DocumentHeader.UpdatedBy = this.DocumentHeader.Session.GetObjectByKey<User>(Program.Settings.CurrentUser.Oid);
                    this.DocumentHeader.Save();
                    this.DocumentHeader.Session.CommitTransaction();
                    this.DocumentIsSaved = true;

                    #region Take Digital Signature if nessecary
                    if (DocumentHeader.DocumentType.TakesDigitalSignature
                            && DocumentHeader.DocumentNumber > 0
                            && DocumentHeader.Status.TakeSequence
                            && String.IsNullOrEmpty(DocumentHeader.Signature))
                    {
                        StoreControllerSettings settings = DocumentHeader.Session.GetObjectByKey<StoreControllerSettings>(Program.Settings.StoreControllerSettings.Oid);
                        List<POSDevice> posDevices = settings.StoreControllerTerminalDeviceAssociations.
                            Where(x =>
                                    x.DocumentSeries.Any(y => y.DocumentSeries.Oid == DocumentHeader.DocumentSeries.Oid)
                                 && x.TerminalDevice is POSDevice
                                 && (x.TerminalDevice as POSDevice).DeviceSettings.DeviceType == DeviceType.DiSign
                            ).Select(x => x.TerminalDevice).Cast<POSDevice>().ToList();

                        try
                        {
                            DocumentHeader.Signature = DocumentHelper.SignDocument(DocumentHeader,
                                DocumentHeader.Session.GetObjectByKey<User>(Program.Settings.CurrentUser.Oid),
                                DocumentHeader.Owner,
                                null,
                                posDevices
                                );
                            using (ITSStoreControllerDesktopServiceClient itsService = Program.Settings.ITSStoreControllerDesktopService)
                            {
                                this.DocumentHeader.UpdatedOnTicks = itsService.GetNowTicks();
                            }
                            this.DocumentHeader.Save();
                            this.DocumentHeader.Session.CommitTransaction();
                        }
                        catch (SignatureFailureException signatureFailureException)
                        {
                            XtraMessageBox.Show(signatureFailureException.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            this.Close();
                        }
                    }
                    #endregion

                    if (sender == btnSavePrintDocument)
                    {
                        PrintDocumentHelper.PrintDocument(this.DocumentHeader, true);
                    }
                    DocumentIsSaved = true;
                    if (sender == btnSaveDocument || sender == btnSavePrintDocument)
                    {
                        this.Close();
                    }
                }
                else
                {
                    message = Resources.PleaseFillInDocumentHeaderData;
                    XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.tabDocumentInfo.SelectedTabPage = this.tabDocumentInfo.TabPages.FirstOrDefault(x => x.Tag == tabTag);
            }
        }

        private void gridDeliveryAddresses_DoubleClick(object sender, EventArgs e)
        {
            if (this.PreviewDocument == false)
            {
                GetAddressFromGrid(sender, DeliveryAddress);
            }
        }

        private void lueDocumentType_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (!onInitialization && e.OldValue != e.NewValue && (DocumentHeader.DocumentDetails.Count > 0 || EditingDocumentDetail != null))
            {
                DialogResult dialogResult = XtraMessageBox.Show(Resources.AllItemsWillBeLostAreYouSure, Resources.Error, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.OK)
                {
                    ClearEditingDocumentDetail(true);
                    DocumentHeader.Session.Delete(DocumentHeader.DocumentDetails);
                    CancelCurrentEditLine();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        protected virtual void lueDocumentType_EditValueChanged(object sender, EventArgs e)
        {
            EnableDisablePaymentMethodsTab();
        }

        protected virtual void EnableDisablePaymentMethodsTab()
        {
            this.Invalidate();
            Guid documentTypeGuid = Guid.Empty;
            if (Guid.TryParse(lueDocumentType.EditValue.ToString(), out documentTypeGuid))
            {
                DocumentType currentDocumentType = Program.Settings.ReadOnlyUnitOfWork.GetObjectByKey<DocumentType>(documentTypeGuid);
                xtpPayments.PageEnabled = (currentDocumentType != null && (currentDocumentType.UsesPaymentMethods || this.DocumentHeader.DocumentPayments.Count > 0));
            }
        }

        protected bool DocumentTypeUsesPrices
        {
            get
            {
                return DocumentHeader != null && DocumentHeader.DocumentType != null && DocumentHeader.DocumentType.UsesPrices;
            }
        }

        protected virtual void EnableDisableDetailEditFields(bool enableCustomPrice, bool enableCustomDescription)
        {
            txtUnitPrice.ReadOnly = (enableCustomPrice == false || DocumentTypeUsesPrices == false)
                                 && (this.DocumentHeader.DocumentType == null
                                        || (this.DocumentHeader.DocumentType != null
                                            && this.DocumentHeader.DocumentType.AllowItemValueEdit == false
                                           )
                                    );
            txtDescriptionReadOnly.ReadOnly = !enableCustomDescription;
            txtCodeOrBarcode.Enabled = this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT;
            lueDescription.Enabled = this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT;
            txtQty.Enabled = this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT;
            txtSecondDiscountValue.Enabled = this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT && DocumentTypeUsesPrices;
            txtSecondDiscountPercentage.Enabled = this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT && DocumentTypeUsesPrices;
            txtUnitPrice.Enabled = this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT && DocumentTypeUsesPrices
                                && this.DocumentHeader.DocumentType != null && this.DocumentHeader.DocumentType.AllowItemValueEdit;
            txtDescriptionReadOnly.Enabled = this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT && DocumentTypeUsesPrices;
            memoEditRemarks.Enabled = this.DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT;
            this.txtSecondDiscountValue.Enabled = this.txtSecondDiscountPercentage.Enabled = this.EditingDocumentDetail != null
                                                                                        && this.EditingDocumentDetail.Item != null
                                                                                        && !this.EditingDocumentDetail.IsTax;


            if (DocumentHeader.DocumentType != null && DocumentHeader.DocumentType.Division.Section == ITS.Retail.Platform.Enumerations.eDivision.Purchase && DocumentTypeUsesPrices)
            {
                lueVatFactor.Enabled = true;
                lueVatFactor.Properties.Buttons[0].Visible = true;
            }
            SetTabStop();
            //UpdateLinkedMasterDetail();
        }

        protected void SetTabStop()
        {
            foreach (Component cmp in this.EnumerateComponents())
            {
                if (cmp is BaseEdit)
                {
                    BaseEdit baseEdit = (BaseEdit)cmp;
                    baseEdit.TabStop = baseEdit.Enabled && !baseEdit.ReadOnly;
                }

            }

            this.Invalidate();
        }

        private void grdDocumentPayments_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            txtTotalInPayments.Refresh();
            txtRemainingPaymentAmount.Refresh();
        }

        private bool CancelDocument()
        {
            if (this.PreviewDocument ||
                XtraMessageBox.Show(Resources.Cancel, Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) ==
                DialogResult.Yes)
            {
                this.DocumentHeader.Changed -= this.DocumentHeader_Changed;
                if (this.PreviewDocument == false)
                {
                    this.DocumentHeader.Session.RollbackTransaction();
                }
                return true;
            }
            return false;
        }

        private void DocumentEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DocumentIsSaved == false)
            {
                e.Cancel = !this.CancelDocument();
            }
        }

        private void btnDeletePayment_Click(object sender, EventArgs e)
        {
            DocumentPayment payment = grdDocumentPayments.GetRow(grdDocumentPayments.FocusedRowHandle) as DocumentPayment;
            if (payment != null && !PreviewDocument && DocumentHeader.TransformationLevel == eTransformationLevel.DEFAULT)
            {
                if (payment == this.EditingDocumentPayment)
                {
                    CancelCurrentPayment();
                }
                payment.Delete();
                if (this.DocumentHeader.Division == eDivision.Financial)
                {
                    DocumentHelper.SetFinancialDocumentDetail(this.DocumentHeader);
                }
            }
        }

        private void grdDocumentPayments_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Delete)
            {
                btnDeletePayment_Click(sender, e);
            }
        }

        private void gridDocumentPayments_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Delete)
            {
                btnDeletePayment_Click(sender, e);
            }
        }

        private void gridDocumentPayments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Delete)
            {
                btnDeletePayment_Click(sender, e);
            }
        }

        private void grdDocumentPayments_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs evtarg)
        {
            DocumentPayment payment = evtarg.Row as DocumentPayment;

            if (payment != null)
            {
                string error = "";
                evtarg.Valid = ValidatePayment(payment, ref error);
                evtarg.ErrorText = error;
            }
        }

        private bool ValidatePayment(DocumentPayment payment, ref string error)
        {
            bool valid = true;
            if (payment.Amount <= 0)
            {
                valid = false;
                error = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS + ":" + Resources.Amount;
            }
            else if (payment.PaymentMethod == null)
            {
                valid = false;
                error = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS + ":" + Resources.PaymentMethod;
            }
            else
            {

                foreach (PaymentMethodField field in payment.PaymentMethod.PaymentMethodFields)
                {
                    PropertyInfo pInfo = typeof(DocumentPayment).GetProperty(field.FieldName);
                    object value = pInfo.GetValue(payment, null);
                    if (value == null)
                    {
                        valid = false;
                        error = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
                    }
                    else
                    {
                        if ((value is Guid && (Guid)value == Guid.Empty) || (value is decimal && (decimal)value <= 0)
                            || (value is string && String.IsNullOrEmpty(value as string)) || (value is int && (int)value <= 0)
                            || (value is DateTime && (DateTime)value == DateTime.MinValue)
                            )
                        {
                            valid = false;
                            error = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
                        }
                    }
                }
            }
            return valid;
        }

        private void grdDocumentDetails_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            DocumentDetail detail = grdDocumentDetails.GetRow(e.RowHandle) as DocumentDetail;
            if ((e.Column == grcCommandColumn || e.Column == grcCommandEdit) && detail != null && detail.IsLinkedLine)
            {
                e.Handled = true;
            }
        }

        private void RestoreDocumentPaymentBindings()
        {
            //Clear Bindings
            this.luePaymentMethod.DataBindings.Clear();
            this.txtPaymentAmount.DataBindings.Clear();

            this.txtStringField1.DataBindings.Clear();
            this.txtStringField2.DataBindings.Clear();
            this.txtStringField3.DataBindings.Clear();
            this.txtStringField4.DataBindings.Clear();
            this.txtStringField5.DataBindings.Clear();

            this.dateField1.DataBindings.Clear();
            this.dateField2.DataBindings.Clear();
            this.dateField3.DataBindings.Clear();
            this.dateField4.DataBindings.Clear();
            this.dateField5.DataBindings.Clear();

            this.txtIntegerField1.DataBindings.Clear();
            this.txtIntegerField2.DataBindings.Clear();
            this.txtIntegerField3.DataBindings.Clear();
            this.txtIntegerField4.DataBindings.Clear();
            this.txtIntegerField5.DataBindings.Clear();

            this.txtDecimalField1.DataBindings.Clear();
            this.txtDecimalField2.DataBindings.Clear();
            this.txtDecimalField3.DataBindings.Clear();
            this.txtDecimalField4.DataBindings.Clear();
            this.txtDecimalField5.DataBindings.Clear();

            this.lueCustomEnumerationValue1.DataBindings.Clear();
            this.lueCustomEnumerationValue2.DataBindings.Clear();
            this.lueCustomEnumerationValue3.DataBindings.Clear();
            this.lueCustomEnumerationValue4.DataBindings.Clear();
            this.lueCustomEnumerationValue5.DataBindings.Clear();

            if (EditingDocumentPayment != null)
            {

                //Payment Method
                this.luePaymentMethod.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "PaymentMethod!Key", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtPaymentAmount.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "Amount", true, DataSourceUpdateMode.OnPropertyChanged));

                //String Fields
                this.txtStringField1.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "StringField1", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtStringField2.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "StringField2", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtStringField3.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "StringField3", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtStringField4.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "StringField4", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtStringField5.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "StringField5", true, DataSourceUpdateMode.OnPropertyChanged));

                //Date Fields
                this.dateField1.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DateField1", true, DataSourceUpdateMode.OnPropertyChanged));
                this.dateField2.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DateField2", true, DataSourceUpdateMode.OnPropertyChanged));
                this.dateField3.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DateField3", true, DataSourceUpdateMode.OnPropertyChanged));
                this.dateField4.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DateField4", true, DataSourceUpdateMode.OnPropertyChanged));
                this.dateField5.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DateField5", true, DataSourceUpdateMode.OnPropertyChanged));

                //Integer Fields
                this.txtIntegerField1.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "IntegerField1", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtIntegerField2.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "IntegerField2", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtIntegerField3.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "IntegerField3", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtIntegerField4.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "IntegerField4", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtIntegerField5.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "IntegerField5", true, DataSourceUpdateMode.OnPropertyChanged));

                //Decimal Fields
                this.txtDecimalField1.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DecimalField1", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtDecimalField2.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DecimalField2", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtDecimalField3.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DecimalField3", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtDecimalField4.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DecimalField4", true, DataSourceUpdateMode.OnPropertyChanged));
                this.txtDecimalField5.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "DecimalField5", true, DataSourceUpdateMode.OnPropertyChanged));

                //CustomEnum Fields
                this.lueCustomEnumerationValue1.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "CustomEnumerationValue1!Key", true, DataSourceUpdateMode.OnPropertyChanged));
                this.lueCustomEnumerationValue2.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "CustomEnumerationValue2!Key", true, DataSourceUpdateMode.OnPropertyChanged));
                this.lueCustomEnumerationValue3.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "CustomEnumerationValue3!Key", true, DataSourceUpdateMode.OnPropertyChanged));
                this.lueCustomEnumerationValue4.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "CustomEnumerationValue4!Key", true, DataSourceUpdateMode.OnPropertyChanged));
                this.lueCustomEnumerationValue5.DataBindings.Add(new Binding("EditValue", this.EditingDocumentPayment, "CustomEnumerationValue5!Key", true, DataSourceUpdateMode.OnPropertyChanged));

            }
        }

        private void grdDocumentPayments_DoubleClick(object sender, EventArgs e)
        {
            StartPaymentEditLine();
        }



        private void CancelCurrentPayment()
        {
            if (this.PreviewDocument == false)
            {
                this.EditingDocumentPayment.Changed -= EditingDocumentPayment_Changed;
                if (this.EditingDocumentPayment.DocumentHeader == null)
                {
                    this.EditingDocumentPayment.Delete();
                }
                else
                {
                    string error;
                    this.EditingDocumentPayment.FromJson(this.OldDocumentPaymentSerialized, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);
                    DocumentHeader.DocumentPayments.Add(EditingDocumentPayment);
                }
                CreateNewDocumentPayment();
            }
        }

        protected void EditingDocumentPayment_Changed(object sender, ObjectChangeEventArgs e)
        {
            if (e != null && e.PropertyName == "PaymentMethod" && this.EditingDocumentPayment != null && this.EditingDocumentPayment.PaymentMethod != null)
            {
                RedrawPaymentPanel();
            }
            else if (e != null && e.PropertyName == "Amount")
            {
                txtTotalInPayments.Refresh();
                txtRemainingPaymentAmount.Refresh();
            }
        }

        private void RedrawPaymentPanel()
        {
            lcs.ToList().ForEach(x => x.Value.HideToCustomization());
            this.EditingDocumentPayment.PaymentMethod.PaymentMethodFields.ToList().ForEach(x =>
            {
                if (lcs.ContainsKey(x.FieldName))
                {
                    lcs[x.FieldName].RestoreFromCustomization(this.Root);
                    lcs[x.FieldName].Text = x.Label;
                    if (x.CustomEnumeration != null && lcs[x.FieldName].Control is LookUpEditBase)
                    {
                        ((LookUpEditBase)lcs[x.FieldName].Control).Properties.DataSource = x.CustomEnumeration.CustomEnumerationValues.OrderBy(value => value.Ordering);
                        ((LookUpEditBase)lcs[x.FieldName].Control).Properties.ValueMember = "Oid";
                        ((LookUpEditBase)lcs[x.FieldName].Control).Properties.DisplayMember = "Description";
                        if (lcs[x.FieldName].Control is LookUpEdit)
                        {
                            ((LookUpEdit)lcs[x.FieldName].Control).Properties.Columns.Clear();
                            ((LookUpEdit)lcs[x.FieldName].Control).Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
                        }

                    }
                }
            });
        }

        private void btnCancelPayment_Click(object sender, EventArgs e)
        {
            this.CancelCurrentPayment();
        }

        private void btnOkPayment_Click(object sender, EventArgs e)
        {
            string error = "";
            if (this.ValidatePayment(this.EditingDocumentPayment, ref error))
            {
                this.EditingDocumentPayment.DocumentHeader = this.DocumentHeader;
                this.EditingDocumentPayment.Changed -= EditingDocumentPayment_Changed;
                CreateNewDocumentPayment();
                gridDocumentPayments.RefreshDataSource();
            }
            else
            {
                XtraMessageBox.Show(error, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (this.DocumentHeader.Division == eDivision.Financial)
            {
                DocumentHelper.SetFinancialDocumentDetail(this.DocumentHeader);
            }
        }

        private void CreateNewDocumentPayment()
        {
            this.EditingDocumentPayment = new DocumentPayment(this.DocumentHeader.Session);
            this.EditingDocumentPayment.Changed += EditingDocumentPayment_Changed;
            lcs.ToList().ForEach(x => x.Value.HideToCustomization());
        }


        public static DocumentEditForm CreateForm(DocumentHeader documentHeader, bool previewDocument = false)
        {
            switch (documentHeader.Division)
            {
                case eDivision.Financial:
                    return new FinancialDocumentEditForm(documentHeader, previewDocument);
                case eDivision.Sales:
                    return new SalesDocumentEditForm(documentHeader, previewDocument);
                case eDivision.Purchase:
                    return new PurchaseDocumentEditForm(documentHeader, previewDocument);
                case eDivision.Store:
                    return new StoreDocumentForm(documentHeader, previewDocument);
                default:
                    throw new NotImplementedException();
            }
        }

        private void txtUnitPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                FocusOnNextField(txtUnitPrice);
            }
        }

        private void txtQty_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (this.EditingDocumentDetail != null)
                {
                    string message = CheckedDetailLimits();
                    if (!String.IsNullOrWhiteSpace(message))
                    {
                        e.Cancel = true;
                        BindingManagerBase manager = BindingContext[txtQty];
                        manager.CancelCurrentEdit();
                        XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.GetFullMessage();
                e.Cancel = true;
                XtraMessageBox.Show(Resources.InvalidItemQty, Resources.InvalidItemQty, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        private void txtUnitPrice_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (this.EditingDocumentDetail != null)
                {
                    string message = CheckedDetailLimits();
                    if (!String.IsNullOrWhiteSpace(message))
                    {
                        e.Cancel = true;
                        BindingManagerBase manager = BindingContext[txtUnitPrice];
                        manager.CancelCurrentEdit();
                        XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    //else
                    //{
                    //    this.EditingDocumentDetail.CustomUnitPrice = (decimal)(txtUnitPrice.EditValue);
                    //}
                }
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.GetFullMessage();
                e.Cancel = true;
                XtraMessageBox.Show(Resources.InvalidValue, Resources.InvalidValue, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        private string CheckedDetailLimits()
        {
            string message = "";
            if (this.EditingDocumentDetail != null)
            {
                decimal quantity = (DocumentHeader.UsesPackingQuantities && this.EditingDocumentDetail.Item.PackingQty > 0) ?
                               (decimal)txtQty.EditValue * (decimal)this.EditingDocumentDetail.PackingMeasurementUnitRelationFactor : (decimal)txtQty.EditValue;

                decimal value = (decimal)txtUnitPrice.EditValue;

                if (quantity > DocumentHeader.DocumentType.MaxDetailQty && DocumentHeader.DocumentType.MaxDetailQty > 0)
                {
                    message = Resources.InvalidDetailQty;
                }
                else if (value > DocumentHeader.DocumentType.MaxDetailValue && DocumentHeader.DocumentType.MaxDetailValue > 0)
                {
                    message = Resources.InvalidDetailValue;
                }
                else if (Math.Abs(quantity * value) > DocumentHeader.DocumentType.MaxDetailTotal && DocumentHeader.DocumentType.MaxDetailTotal > 0)
                {
                    message = Resources.InvalidDetailTotal;
                }
            }
            return message;
        }

        private void RestoreLayout(LayoutControl view, string setting)
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(setting);
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    view.RestoreLayoutFromStream(stream);
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Info(ex, "Error Restoring Layout");
            }
        }

        private string GetLayout(LayoutControl view)
        {
            using (Stream str = new MemoryStream())
            {
                view.SaveLayoutToStream(str);
                str.Seek(0, System.IO.SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(str))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void lcmDocumentHeader_HideCustomization(object sender, EventArgs e)
        {
            if (LayoutDocumentHeaderPropertyInfo != null)
            {
                try
                {
                    string value = GetLayout(this.lcmDocumentHeader);
                    LayoutDocumentHeaderPropertyInfo.SetValue(Program.Settings, value, null);
                }
                catch (Exception ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                }
            }
        }

        private void lcmDocumentDetail_HideCustomization(object sender, EventArgs e)
        {
            if (LayoutDocumentDetailPropertyInfo != null)
            {
                try
                {
                    string value = GetLayout(this.lcmDocumentDetail);
                    LayoutDocumentDetailPropertyInfo.SetValue(Program.Settings, value, null);
                }
                catch (Exception ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                }
            }
        }

        private void lcmTrader_HideCustomization(object sender, EventArgs e)
        {
            if (LayoutTraderPropertyInfo != null)
            {
                try
                {
                    string value = GetLayout(this.lcmTrader);
                    LayoutTraderPropertyInfo.SetValue(Program.Settings, value, null);
                }
                catch (Exception ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                }
            }
        }

        private void lcmTotal_HideCustomization(object sender, EventArgs e)
        {
            if (LayoutTotalPropertyInfo != null)
            {
                try
                {
                    string value = GetLayout(this.lcmTotal);
                    LayoutTotalPropertyInfo.SetValue(Program.Settings, value, null);
                }
                catch (Exception ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                }
            }
        }

        protected virtual List<Control> fields { get; set; }

        protected virtual void FocusOnNextField(Control currentField)
        {
            int position = fields.IndexOf(currentField);
            position = (position + 1) % fields.Count;
            Control control = fields[position];
            while (control.Enabled == false || (control is BaseEdit && ((BaseEdit)control).ReadOnly == true))
            {
                position = (position + 1) % fields.Count;
                control = fields[position];
            }


            control.Focus();
        }

        protected virtual void txtSecondDiscountPercentage_Validating(object sender, CancelEventArgs e)
        {
            if (this.EditingDocumentDetail != null && ((decimal)txtSecondDiscountPercentage.EditValue) >= 1)
            {
                e.Cancel = true;
                XtraMessageBox.Show(Resources.InvalidValue, Resources.InvalidValue, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        private void txtSecondDiscountValue_Validating(object sender, CancelEventArgs e)
        {
            if (this.EditingDocumentDetail != null && ((decimal)txtSecondDiscountValue.EditValue) > this.EditingDocumentDetail.GrossTotalBeforeDiscount)
            {
                e.Cancel = true;
                XtraMessageBox.Show(Resources.InvalidValue, Resources.InvalidValue, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        private void simpleButtonAddRemainAmountToPayment_Click(object sender, EventArgs e)
        {
            decimal remainingPayment = this.DocumentHeader.RemainingPayment;
            if (this.EditingDocumentPayment.Session.IsNewObject(this.EditingDocumentPayment) == false)
            {
                remainingPayment += this.EditingDocumentPayment.Amount;
            }
            if (remainingPayment <= 0)
            {
                XtraMessageBox.Show(Resources.ThereIsNoRemainingPayment, Resources.ThereIsNoRemainingPayment, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.EditingDocumentPayment.PaymentMethod == null)
            {
                XtraMessageBox.Show(Resources.PleaseDefinePaymentMethod, Resources.PleaseDefinePaymentMethod, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            txtPaymentAmount.EditValue = remainingPayment;
            if (this.DocumentHeader.Division == eDivision.Financial)
            {
                DocumentHelper.SetFinancialDocumentDetail(this.DocumentHeader);
            }
            btnOkPayment.Focus();
        }

        private void btnEditGridRow_Click(object sender, EventArgs e)
        {
            DocumentDetail detail = grdDocumentDetails.GetRow(grdDocumentDetails.FocusedRowHandle) as DocumentDetail;
            grdDocumentDetails.VisibleColumns.Where(col => col == grcCommandColumn).FirstOrDefault().OptionsColumn.AllowEdit = false;
            StartEditLine(detail);
        }

        private void btnEditPaymentsGridRow_Click(object sender, EventArgs e)
        {
            StartPaymentEditLine();
        }

        private void StartPaymentEditLine()
        {
            DocumentPayment payment = grdDocumentPayments.GetFocusedRow() as DocumentPayment;
            if (payment != null && payment != EditingDocumentPayment)
            {
                CancelCurrentPayment();
                this.EditingDocumentPayment = payment;
                this.OldDocumentPaymentSerialized = payment.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
                this.EditingDocumentPayment.Changed += EditingDocumentPayment_Changed;
                RedrawPaymentPanel();
            }
        }

        private void DocumentEditForm_Shown(object sender, EventArgs e)
        {
            this.lueDocumentType.Focus();
            if (this.DesignMode)
            {
                return;
            }
            this.txtUnitPrice.Properties.DisplayFormat.FormatString = this.DocumentHeader.Owner.OwnerApplicationSettings.formatItemValueString;
            this.txtNetTotalBeforeDisocunt.Properties.DisplayFormat.FormatString = this.DocumentHeader.Owner.OwnerApplicationSettings.formatCurrencyString;
            this.txtPriceCatalogDiscountValue.Properties.DisplayFormat.FormatString = this.DocumentHeader.Owner.OwnerApplicationSettings.formatCurrencyString;
            this.txtSecondDiscountValue.Properties.DisplayFormat.FormatString = this.DocumentHeader.Owner.OwnerApplicationSettings.formatCurrencyString;
            this.txtTotalDiscountValue.Properties.DisplayFormat.FormatString = this.DocumentHeader.Owner.OwnerApplicationSettings.formatCurrencyString;
            this.txtNetTotal.Properties.DisplayFormat.FormatString = this.DocumentHeader.Owner.OwnerApplicationSettings.formatCurrencyString;
            this.txtTotalVatAmount.Properties.DisplayFormat.FormatString = this.DocumentHeader.Owner.OwnerApplicationSettings.formatCurrencyString;
            this.txtGrossTotal.Properties.DisplayFormat.FormatString = this.DocumentHeader.Owner.OwnerApplicationSettings.formatCurrencyString;
        }

        protected virtual void grdDocumentDetails_RowStyle(object sender, RowStyleEventArgs e)
        {
            DocumentDetail detail = grdDocumentDetails.GetRow(e.RowHandle) as DocumentDetail;

            if (detail != null && detail.IsCanceled)
            {
                e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Strikeout);
                e.Appearance.ForeColor = Color.Red;
            }
            if (detail != null && notIncludedDetails.Contains(detail))
            {
                e.Appearance.BackColor = Color.Red;
            }
        }

        protected void RefreshDocumentItemsDataSource()
        {
            lueDescription.Properties.DataSource = null;
            if (xpInstantFeedbackSource1 != null)
            {
                xpInstantFeedbackSource1.Dispose();
            }
            xpInstantFeedbackSource1 = new XPInstantFeedbackSource(this.components);
            xpInstantFeedbackSource1.ObjectType = typeof(Item);
            xpInstantFeedbackSource1.ResolveSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_ResolveSession);
            xpInstantFeedbackSource1.DismissSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_DismissSession);
            xpInstantFeedbackSource1.FixedFilterCriteria = DocumentHelper.DocumentTypeSupportedItemsCriteria(DocumentHeader);


            if (xpInstantFeedbackSource2 != null)
            {
                xpInstantFeedbackSource2.Dispose();
            }

            xpInstantFeedbackSource2 = new XPInstantFeedbackSource(this.components);
            xpInstantFeedbackSource2.ObjectType = typeof(Item);
            xpInstantFeedbackSource2.ResolveSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_ResolveSession);
            xpInstantFeedbackSource2.DismissSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_DismissSession);
            xpInstantFeedbackSource2.FixedFilterCriteria = DocumentHelper.DocumentTypeSupportedItemsCriteria(DocumentHeader);

            if (xpInstantFeedbackSource3 != null)
            {
                xpInstantFeedbackSource3.Dispose();
            }

            xpInstantFeedbackSource3 = new XPInstantFeedbackSource(this.components);
            xpInstantFeedbackSource3.ObjectType = typeof(Item);
            xpInstantFeedbackSource3.ResolveSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_ResolveSession);
            xpInstantFeedbackSource3.DismissSession += new EventHandler<ResolveSessionEventArgs>(xpInstantFeedbackSource1_DismissSession);
            xpInstantFeedbackSource3.FixedFilterCriteria = DocumentHelper.DocumentTypeSupportedItemsCriteria(DocumentHeader);


            lueDescription.Properties.DataSource = xpInstantFeedbackSource1;
            repositoryItemSearchDescription.DataSource = xpInstantFeedbackSource2;
            repositoryItemDetailSearchDescription.DataSource = xpInstantFeedbackSource3;


            xtpDocumentDetails.PageVisible = !(xtpDocumentDetailsDouble.PageVisible = this.DocumentHeader.DocumentType != null && this.DocumentHeader.DocumentType.ManualLinkedLineInsertion);
        }

        private void grdDocumentDetails_RowClick(object sender, RowClickEventArgs e)
        {
            DocumentDetail detail = grdDocumentDetails.GetRow(e.RowHandle) as DocumentDetail;
            if (detail != null)
            {
                notIncludedDetails.Remove(detail);
            }
        }

        private void gridTriangularAddresses_DoubleClick(object sender, EventArgs e)
        {
            if (this.PreviewDocument == false)
            {
                GetAddressFromGrid(sender, TriangularAddress);
            }
        }

        private void GetAddressFromGrid(object sender, AddressViewModel address)
        {
            GridView grd = (GridView)((GridControl)sender).MainView;
            if (grd != null)
            {
                MayChangeProorismos = true;
                Address adr = grd.GetRow(grd.FocusedRowHandle) as Address;
                address.City = adr.City;
                address.POBox = adr.POBox;
                address.PostCode = adr.PostCode;
                address.Street = adr.Street;
                address.Profession = adr.Profession;
            }
        }

        protected void TriangularAddress_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (TriangularAddress != null)
            {
                switch (e.PropertyName)
                {
                    case "Description":
                        this.DocumentHeader.TriangularAddress = TriangularAddress.Description;
                        break;
                    case "Profession":
                        this.DocumentHeader.TriangularProfession = TriangularAddress.Profession;
                        textEditDocumentHeaderTriangularProfession.Text = TriangularAddress.Profession;
                        break;
                    default:
                        break;
                }
            }
            if (e.PropertyName == "Description" && TriangularAddress != null)
            {

            }
        }

        protected void DeliveryAddress_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (DeliveryAddress != null)
            {
                switch (e.PropertyName)
                {
                    case "Description":
                        this.DocumentHeader.DeliveryAddress = DeliveryAddress.Description;
                        break;
                    case "Profession":
                        this.DocumentHeader.DeliveryProfession = DeliveryAddress.Profession;
                        txtProfession.Text = DeliveryAddress.Profession;
                        break;
                    default:
                        break;
                }
            }
        }

        protected eModule GetModule()
        {
            switch (Program.Settings.MasterAppInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    return eModule.DUAL;
                case eApplicationInstance.RETAIL:
                    throw new NotSupportedException(String.Format("DocumentEditForm.InitializeLookupEdits() {0}", Program.Settings.MasterAppInstance));
                case eApplicationInstance.STORE_CONTROLER:
                    return eModule.STORECONTROLLER;
                default:
                    throw new NotImplementedException(String.Format("DocumentEditForm.InitializeLookupEdits() {0}", Program.Settings.MasterAppInstance));
            }
        }

        private void gridViewMainDocumentDetails_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateLinkedMasterDetail();
        }

        private void UpdateLinkedMasterDetail()
        {
            DocumentDetail detail = gridViewMainDocumentDetails.GetFocusedRow() as DocumentDetail;
            Guid oidToShow = (detail != null) ? detail.Oid : Guid.Empty;
            //this.gridViewLinkedDocumentDetails.ActiveFilterCriteria =
            linkedDetails.Filter =
                CriteriaOperator.And(
                    new BinaryOperator("LinkedLine", Guid.Empty, BinaryOperatorType.NotEqual),
                    new BinaryOperator("LinkedLine", oidToShow)
                );
        }

        private void repositoryTextCodeEdit_Validating(object sender, CancelEventArgs e)
        {
            ClearEditingDocumentDetail(true);
            GridView view = ((sender as TextEdit).Parent as GridControl).MainView as GridView;
            DocumentDetail detail = view.GetFocusedRow() as DocumentDetail;
            List<Guid> forbiddenItems = new List<Guid>();
            if (detail.Item == null)
            {
                DocumentDetail masterDetail = null;
                string inp = (sender as TextEdit).EditValue as string;
                if (string.IsNullOrWhiteSpace(inp))
                {
                    e.Cancel = true;
                    return;
                }
                if (view == gridViewLinkedDocumentDetails)
                {
                    masterDetail = gridViewMainDocumentDetails.GetFocusedRow() as DocumentDetail;
                    if (masterDetail != null)
                    {
                        forbiddenItems.Add(masterDetail.Item.Oid);
                    }
                }
                string codeSearch = inp, barcodeSearch = inp;

                if (Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.PadBarcodes)
                {
                    barcodeSearch = txtCodeOrBarcode.Text.PadLeft(Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.BarcodeLength,
                        Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                }
                if (Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.PadItemCodes)
                {
                    codeSearch = txtCodeOrBarcode.Text.PadLeft(Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.ItemCodeLength,
                        Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.ItemCodePaddingCharacter[0]);
                }
                detail.DocumentHeader = null;
                string result = SearchAndAddDocumentDetail(codeSearch, barcodeSearch, detail, forbiddenItems);
                if (detail.Item != null && masterDetail != null && masterDetail.Item != null && detail.Item.Oid == masterDetail.Item.Oid)
                {
                    XtraMessageBox.Show(Resources.MainItemAndLinkedItemCannotMatch, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }
                DocumentHelper.AddItem(ref _DocumentHeader, detail);
                if (String.IsNullOrWhiteSpace(result) == false)
                {
                    XtraMessageBox.Show(result, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }

                if (masterDetail != null)
                {
                    detail.LinkedLine = masterDetail.Oid;
                }
                else
                {
                    UpdateLinkedMasterDetail();
                }
            }

        }

        private void gridViewMainDocumentDetails_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedRowHandle == GridControl.NewItemRowHandle)
            {
                if (view.FocusedColumn == grcMainQty &&
                        (view.GetFocusedRow() == null ||
                            (view.GetFocusedRow() is DocumentDetail && ((DocumentDetail)view.GetFocusedRow()).Item == null)))
                {
                    e.Cancel = true;
                    return;
                }
                e.Cancel = false;
                return;
            }
            if (view.FocusedColumn == grcMainItemCode || view.FocusedColumn == grcMainItemDescription)
            {
                e.Cancel = true;
            }
            ClearEditingDocumentDetail(true);
            EditingDocumentDetail = view.GetFocusedRow() as DocumentDetail;

            if (EditingDocumentDetail != null)
            {
                EditingDocumentDetail.Changed += DocumentDetailChanged;
                EditingDocumentDetailDiscount = EditingDocumentDetail.DocumentDetailDiscounts.FirstOrDefault();
                if (EditingDocumentDetailDiscount != null)
                {
                    EditingDocumentDetailDiscount.Changed += DocumentDetailDiscountChanged;
                }
            }
        }

        private void gridViewMainDocumentDetails_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column == grcMainItemCode)
            {
                DocumentDetail detail = e.Row as DocumentDetail;
                e.Value = (detail == null || detail.Item == null) ? "" : detail.Item.Code;
            }
        }



        protected void ClearEditingDocumentDetail(bool setNull)
        {
            if (EditingDocumentDetail != null)
            {
                EditingDocumentDetail.Changed -= DocumentDetailChanged;
                ClearEditingDocumentDetailDiscount(setNull);
                if (setNull)
                {
                    EditingDocumentDetail = null;
                }
            }
        }

        private void ClearEditingDocumentDetailDiscount(bool setNull)
        {
            if (EditingDocumentDetailDiscount != null)
            {
                EditingDocumentDetailDiscount.Changed -= DocumentDetailDiscountChanged;
                if (setNull)
                {
                    EditingDocumentDetailDiscount = null;
                }
            }
        }


        private void repositoryItemSearchDescription_Validating(object sender, CancelEventArgs e)
        {
            SearchLookUpEdit lookupEdit = sender as SearchLookUpEdit;
            GridControl grid = lookupEdit.Parent as GridControl;
            GridView view = grid.MainView as GridView;
            DocumentDetail detail = view.GetFocusedRow() as DocumentDetail;
            if (detail.Item == null && lookupEdit.EditValue != null && lookupEdit.EditValue is Guid)
            {
                Item item = DocumentHeader.Session.GetObjectByKey<Item>((Guid)lookupEdit.EditValue);
                DocumentDetail masterDetail = null;

                if (item == null)
                {
                    return;
                }
                if (view == gridViewLinkedDocumentDetails)
                {
                    masterDetail = gridViewMainDocumentDetails.GetFocusedRow() as DocumentDetail;
                    if (masterDetail.Item.Oid == item.Oid)
                    {
                        XtraMessageBox.Show(Resources.MainItemAndLinkedItemCannotMatch, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                        return;
                    }
                }
                string inp = item.Code;
                string codeSearch = inp, barcodeSearch = inp;

                if (Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.PadBarcodes)
                {
                    barcodeSearch = txtCodeOrBarcode.Text.PadLeft(Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.BarcodeLength,
                        Program.Settings.StoreControllerSettings.Owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
                }
                detail.DocumentHeader = null;
                string result = SearchAndAddDocumentDetail(codeSearch, barcodeSearch, detail, null);
                DocumentHelper.AddItem(ref _DocumentHeader, detail);
                if (detail.Item != null && masterDetail != null && masterDetail.Item != null && detail.Item.Oid == masterDetail.Item.Oid)
                {
                    XtraMessageBox.Show(Resources.MainItemAndLinkedItemCannotMatch, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }
                if (String.IsNullOrWhiteSpace(result) == false)
                {
                    XtraMessageBox.Show(result, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }
                if (masterDetail != null)
                {
                    detail.LinkedLine = masterDetail.Oid;
                }
                else
                {
                    UpdateLinkedMasterDetail();
                }
            }
        }

        private void gridViewLinkedDocumentDetails_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedRowHandle == GridControl.NewItemRowHandle)
            {
                if (view.FocusedColumn == grcDetailQty &&
                        (view.GetFocusedRow() == null ||
                            (view.GetFocusedRow() is DocumentDetail && ((DocumentDetail)view.GetFocusedRow()).Item == null)))
                {
                    e.Cancel = true;
                    return;
                }

                DocumentDetail masterDetail = gridViewMainDocumentDetails.GetFocusedRow() as DocumentDetail;
                e.Cancel = masterDetail == null;
                return;
            }
            if (view.FocusedColumn == grcDetailItemCode || view.FocusedColumn == grcDetailItemDescription)
            {
                e.Cancel = true;
            }
            ClearEditingDocumentDetail(true);
            EditingDocumentDetail = view.GetFocusedRow() as DocumentDetail;
            if (EditingDocumentDetail != null)
            {
                EditingDocumentDetail.Changed += DocumentDetailChanged;
                EditingDocumentDetailDiscount = EditingDocumentDetail.DocumentDetailDiscounts.FirstOrDefault();
                if (EditingDocumentDetailDiscount != null)
                {
                    EditingDocumentDetailDiscount.Changed += DocumentDetailDiscountChanged;
                }
            }
        }

        private void gridViewLinkedDocumentDetails_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column == grcDetailItemCode)
            {
                DocumentDetail detail = e.Row as DocumentDetail;
                e.Value = (detail == null || detail.Item == null) ? "" : detail.Item.Code;
            }
        }

        private void btnLinkedLineDelete_Click(object sender, EventArgs e)
        {
            ClearEditingDocumentDetail(true);
            DocumentDetail detail = gridViewLinkedDocumentDetails.GetFocusedRow() as DocumentDetail;
            if (detail != null)
            {
                detail.Delete();
            }
        }

        private void tabDocumentInfo_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {

            ClearEditingDocumentDetail(true);
        }

        private void btnMainLineDelete_Click(object sender, EventArgs e)
        {
            ClearEditingDocumentDetail(true);
            DocumentDetail detail = gridViewMainDocumentDetails.GetFocusedRow() as DocumentDetail;
            if (detail != null)
            {
                foreach (DocumentDetail linked in detail.LinkedLines)
                {
                    linked.Delete();
                }
                detail.Delete();
            }
        }

        private void gridViewLinkedDocumentDetails_RowUpdated(object sender, RowObjectEventArgs e)
        {
            if (e.Row is DocumentDetail)
            {
                DocumentDetail detail = (DocumentDetail)e.Row;
                if (detail.Item == null)
                {
                    detail.Delete();
                }
            }
        }

        private void repositoryItemCalcEditQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            CalcEdit calcEdit = sender as CalcEdit;
            if (calcEdit.EditValue == null)
            {
                if (e.KeyChar == 44 || e.KeyChar == 46)
                {
                    e.Handled = true;
                }
                return;
            }

            string quantity = calcEdit.EditValue.ToString();

            if (this.EditingDocumentDetail != null
             && this.EditingDocumentDetail.Barcode != null
               )
            {
                if (e.KeyChar == 44 || e.KeyChar == 46)//comma and period
                {
                    if (this.EditingDocumentDetail.Barcode.MeasurementUnit(this.DocumentHeader.Owner).SupportDecimal == false)
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        if (quantity.Contains(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator))
                        {
                            e.Handled = true;
                            return;
                        }

                        string comma = ",";
                        string period = ".";

                        //comma 
                        if (e.KeyChar == 44
                          && Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator == period
                           )
                        {
                            if (quantity.Contains(period) == false)
                            {
                                calcEdit.EditValue = calcEdit.EditValue.ToString() + period;
                                calcEdit.SelectionStart = calcEdit.EditValue.ToString().Length;
                                calcEdit.SelectionLength = 0;
                            }
                            e.Handled = true;
                        }

                        //period
                        if (e.KeyChar == 46
                          && Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator == comma
                           )
                        {
                            if (quantity.ToString().Contains(comma) == false)
                            {
                                calcEdit.EditValue = calcEdit.EditValue.ToString() + comma;
                                calcEdit.SelectionStart = calcEdit.EditValue.ToString().Length;
                                calcEdit.SelectionLength = 0;
                            }
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void repositoryItemCalcEditQty_EditValueChanged(object sender, EventArgs e)
        {
            CalcEdit calcEdit = sender as CalcEdit;
            if (calcEdit.EditValue == null)
            {
                return;
            }

            if (this.EditingDocumentDetail != null
            && this.EditingDocumentDetail.Barcode != null
            && this.EditingDocumentDetail.Barcode.MeasurementUnit(this.DocumentHeader.Owner).SupportDecimal == false
            && calcEdit.EditValue.ToString().Contains(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator)
               )
            {
                calcEdit.EditValue = calcEdit.EditValue.ToString().Replace(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator, String.Empty);
            }
        }

        private void repositoryItemCalcEditLinkedLineQuantity_EditValueChanged(object sender, EventArgs e)
        {
            CalcEdit calcEdit = sender as CalcEdit;
            if (calcEdit.EditValue == null)
            {
                return;
            }

            if (this.EditingDocumentDetail != null
            && this.EditingDocumentDetail.Barcode != null
            && this.EditingDocumentDetail.Barcode.MeasurementUnit(this.DocumentHeader.Owner).SupportDecimal == false
            && calcEdit.EditValue.ToString().Contains(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator)
               )
            {
                calcEdit.EditValue = calcEdit.EditValue.ToString().Replace(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator, String.Empty);
            }
        }

        private void repositoryItemCalcEditLinkedLineQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            CalcEdit calcEdit = sender as CalcEdit;
            if (calcEdit.EditValue == null)
            {
                if (e.KeyChar == 44 || e.KeyChar == 46)
                {
                    e.Handled = true;
                }
                return;
            }

            string quantity = calcEdit.EditValue.ToString();

            if (this.EditingDocumentDetail != null
             && this.EditingDocumentDetail.Barcode != null
               )
            {
                if (e.KeyChar == 44 || e.KeyChar == 46)//comma and period
                {
                    if (this.EditingDocumentDetail.Barcode.MeasurementUnit(this.DocumentHeader.Owner).SupportDecimal == false)
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        if (quantity.Contains(Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator))
                        {
                            e.Handled = true;
                            return;
                        }

                        string comma = ",";
                        string period = ".";

                        //comma 
                        if (e.KeyChar == 44
                          && Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator == period
                           )
                        {
                            if (quantity.Contains(period) == false)
                            {
                                calcEdit.EditValue = calcEdit.EditValue.ToString() + period;
                                calcEdit.SelectionStart = calcEdit.EditValue.ToString().Length;
                                calcEdit.SelectionLength = 0;
                            }
                            e.Handled = true;
                        }

                        //period
                        if (e.KeyChar == 46
                          && Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator == comma
                           )
                        {
                            if (quantity.ToString().Contains(comma) == false)
                            {
                                calcEdit.EditValue = calcEdit.EditValue.ToString() + comma;
                                calcEdit.SelectionStart = calcEdit.EditValue.ToString().Length;
                                calcEdit.SelectionLength = 0;
                            }
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void repositoryItemDetailSearchDescription_EditValueChanged(object sender, EventArgs e)
        {
            string s = ";";
        }

        private void tabDocumentInfo_TabStopChanged(object sender, EventArgs e)
        {

        }

        private void tabDocumentInfo_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            if (EditingDocumentDetail != null)
            {
                e.Cancel = true;
                XtraMessageBox.Show(Resources.YouHaveUnsavedChanges, Resources.Explanation, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        public bool MayChangeProorismos = false;
        private void txtStreet_Enter(object sender, EventArgs e)
        {
            MayChangeProorismos = true;
        }

        private void txtStreet_EditValueChanged(object sender, EventArgs e)
        {
            ChangeProorismos();
        }
        private void ChangeProorismos()
        {
            if (MayChangeProorismos == true)
            {
                string address = "";
                if (String.IsNullOrEmpty(txtStreet.Text) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ", ";
                    }
                    address += txtStreet.Text;
                }


                if (String.IsNullOrEmpty(txtCity.Text) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ", ";
                    }
                    address += txtCity.Text;
                }

                if (String.IsNullOrEmpty(txtZipCode.Text) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ", ";
                    }
                    address += txtZipCode.Text;
                }

                if (String.IsNullOrEmpty(txtPOBox.Text) == false)
                {
                    if (String.IsNullOrEmpty(address) == false)
                    {
                        address += ", ";
                    }
                    address += txtPOBox.Text;
                }

                DocumentHeader.DeliveryAddress = address;
            }
        }

        private void txtPOBox_EditValueChanged(object sender, EventArgs e)
        {
            ChangeProorismos();
        }

        private void txtZipCode_EditValueChanged(object sender, EventArgs e)
        {
            ChangeProorismos();
        }

        private void txtCity_EditValueChanged(object sender, EventArgs e)
        {
            ChangeProorismos();
        }

        private void txtPOBox_Enter(object sender, EventArgs e)
        {
            MayChangeProorismos = true;
        }

        private void gridViewLinkedDocumentDetails_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            DocumentDetail detail = (sender as GridView).GetRow(e.RowHandle) as DocumentDetail;
            DocumentDetail masterDetail = gridViewMainDocumentDetails.GetFocusedRow() as DocumentDetail;
            detail.LinkedLine = masterDetail.Oid;
        }
    }
}
