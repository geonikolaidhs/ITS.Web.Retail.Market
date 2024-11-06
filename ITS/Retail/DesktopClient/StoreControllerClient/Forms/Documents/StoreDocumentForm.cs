using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class StoreDocumentForm : DocumentEditForm
    {

        protected override string LayoutDocumentHeaderProperty
        {
            get { return "StoreDocumentHeader"; }
        }

        protected override string LayoutDocumentDetailProperty
        {
            get { return "StoreDocumentDetail"; }
        }

        protected override string LayoutTraderProperty
        {
            get { return "StoreLayout"; }
        }

        protected override string LayoutTotalProperty
        {
            get { return "LayoutStoreTotal"; }
        }


        public StoreDocumentForm(DocumentHeader header, bool previewDocument = false)
            : base(header, previewDocument)
        {
            InitializeComponent();
            if (this.components == null)
            {
                this.components = new System.ComponentModel.Container();
            }
            //this.lueVatFactor.ReadOnly = false;
            this.fields = new List<Control>()
            {
                txtQty,
                //lueVatFactor,
                txtUnitPrice,
                txtSecondDiscountPercentage,
                txtSecondDiscountValue,
                //"memoEditRemarks",
                btnOkEditLine
            };
            layoutControlItem10.HideToCustomization();
            lcAddressProfession.HideToCustomization();
            this.lueDocumentType.Focus();
            if (!Program.Settings.CurrentUser.Role.GDPREnabled)
            {
                lcTelephone.Parent.Remove(lcTelephone);
            }
            if (DocumentHeader != null && DocumentHeader.Tablet != null)
            {
                lueStoreSfa.EditValue = DocumentHeader.Tablet.Oid;

            }
        }


        protected override void UpdateDocumentDetail(DocumentDetail detail, ObjectChangeEventArgs objectChangeEventArgs)
        {
            try
            {
                List<DocumentDetailDiscount> discounts = new List<DocumentDetailDiscount>();
                if (EditingDocumentDetailDiscount != null
                    && (EditingDocumentDetailDiscount.Value != 0 || EditingDocumentDetailDiscount.Percentage != 0)
                    )
                {
                    discounts.Add(EditingDocumentDetailDiscount);
                }

                decimal quantity = this.WeightedBarcodeInfo != null ? this.WeightedBarcodeInfo.Quantity : detail.PackingQuantity;
                DocumentHelper.ComputeDocumentLine(ref this._DocumentHeader, EditingDocumentDetail.Item, EditingDocumentDetail.Barcode, quantity, false,
                    EditingDocumentDetail.CustomUnitPrice, true, EditingDocumentDetail.Item.Name, discounts, oldDocumentLine: detail);
                DocumentHelper.UpdateLinkedItems(ref _DocumentHeader, detail);
                if (detail.DocumentHeader != null)
                {
                    gridDocumentDetails.Refresh();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.GetFullMessage(), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void DocumentHeader_Changed(object sender, ObjectChangeEventArgs e)
        {
            base.DocumentHeader_Changed(sender, e);
            //this.DocumentHeader.SecondaryStore
            switch (e.PropertyName)
            {
                case "SecondaryStore":
                    SecondaryStoreChanged(sender, e);
                    break;
                case "DocumentType":
                    RefreshDocumentItemsDataSource();
                    break;
            }

        }

        private void SecondaryStoreChanged(object sender, ObjectChangeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override void EnableDisableDetailEditFields(bool enableCustomPrice, bool enableCustomDescription)
        {
            base.EnableDisableDetailEditFields(enableCustomPrice, enableCustomDescription);
        }

        protected override void LookupeditInitialization()
        {
            base.LookupeditInitialization();
            lueStore.Properties.DataSource = StoreHelper.GetStoresByReferenceCompany(Program.Settings.ReadOnlyUnitOfWork, DocumentHeader.Store);// new XPCollection<Store>(Program.Settings.ReadOnlyUnitOfWork);
            lueStore.Properties.ValueMember = "Oid";
            lueStore.Properties.DisplayMember = "Name";

            lueStore.Properties.Columns.Add(new LookUpColumnInfo("Code", Resources.Code));
            lueStore.Properties.Columns.Add(new LookUpColumnInfo("Name", Resources.Name));

            lueStoreSfa.Properties.DataSource = new XPCollection<SFA>(this.DocumentHeader.Session, CriteriaOperator.And(new BinaryOperator("IsActive", true, BinaryOperatorType.Equal)));
            lueStoreSfa.Properties.Columns.Add(new LookUpColumnInfo("Name", Resources.Description));
            lueStoreSfa.Properties.ValueMember = "Oid";
            lueStoreSfa.Properties.DisplayMember = "Name";
        }

        protected override void BindingInit()
        {
            base.BindingInit();
            this.lueStore.DataBindings.Add("EditValue", this.DocumentHeader, "SecondaryStore!Key", true, DataSourceUpdateMode.OnPropertyChanged);
        }


        protected override void EnableDisableDocumentHeaderFields()
        {
            base.EnableDisableDocumentHeaderFields();
            tabDocumentInfo.Enabled =
               DocumentHeader.DocumentType != null && DocumentHeader.DocumentSeries != null &&
               DocumentHeader.SecondaryStore != null;
        }

        private void lueStoreSfa_EditValueChanged(object sender, EventArgs e)
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
