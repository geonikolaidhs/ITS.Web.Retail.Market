using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common.ViewModel;

namespace ITS.Retail.WebClient.ViewModel
{
    public class DocumentHeaderViewModel : IPersistableViewModel
    {

        public Guid Oid { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }


        public Type PersistedType
        {
            get { return typeof(DocumentHeader); }
        }

        public bool IsDeleted { get; set; }

        [UpdateViewModel(typeof(Address), "Description", "BillingAddress")]
        public string BillingAddressDescription { get; set; }
        public Guid? BillingAddress { get; set; }

        [UpdateViewModel(typeof(PriceCatalog), "Description", "PriceCatalog")]
        public string PriceCatalogDescription { get; set; }

        [UpdateViewModel(typeof(PriceCatalog), "Code", "PriceCatalog")]
        public string PriceCatalogCode { get; set; }

        public Guid? PriceCatalog { get; set; }

        [UpdateViewModel(typeof(SupplierNew), "CompanyName", "Supplier")]
        public string SupplierDescription { get; set; }
        public Guid? Supplier { get; set; }

        [UpdateViewModel(typeof(TransferPurpose), "Description", "TransferPurpose")]
        public string TransferPurposeDescription { get; set; }
        public Guid? TransferPurpose { get; set; }

        /*
                public string POSDescription { get; set; }
                public Guid POS { get; set; }

                public string UserDailyTotalsDescription { get; set; }
                public Guid UserDailyTotals { get; set; }

                public string DocumentDiscountTypeDescription { get; set; }
                public Guid DocumentDiscountType { get; set; }

                public string DeliveryToDescription { get; set; }
                public Guid DeliveryTo { get; set; }
        */
        [UpdateViewModel(typeof(DeliveryType), "Description", "DeliveryType")]
        public string DeliveryTypeDescription { get; set; }
        public Guid DeliveryType { get; set; }

        [UpdateViewModel(typeof(Store), "Description", "Store")]
        public string StoreDescription { get; set; }
        public Guid? Store { get; set; }

        [UpdateViewModel(typeof(Customer), "CompanyName", "Customer")]
        public string CustomerDescription { get; set; }
        public Guid? Customer { get; set; }

        [UpdateViewModel(typeof(DocumentType), "Description", "DocumentType")]
        public string DocumentTypeDescription { get; set; }
        public Guid? DocumentType { get; set; }

        [UpdateViewModel(typeof(DocumentSeries), "Description", "DocumentSeries")]
        public string DocumentSeriesDescription { get; set; }
        public Guid? DocumentSeries { get; set; }

        [UpdateViewModel(typeof(DocumentStatus), "Description", "Status")]
        public string StatusDescription { get; set; }
        public Guid? Status { get; set; }

        public eTransformationLevel TransformationLevel { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public decimal GrossTotalBeforeDocumentDiscount { get; set; }
        public DateTime RefferenceDate { get; set; }
        public string DocumentStatusCode { get; set; }
        public string DeliveryToTraderTaxCode { get; set; }
        public string CustomerCode { get; set; }
        public string StoreCode { get; set; }
        public string DocumentSeriesCode { get; set; }
        public string DocumentTypeCode { get; set; }
        public int POSID { get; set; }
        public string PlaceOfLoading { get; set; }
        public string VehicleNumber { get; set; }
        public string TransferMethod { get; set; }
        public string Signature { get; set; }
        public bool IsCanceled { get; set; }
        public bool HasBeenExecuted { get; set; }
        public bool HasBeenChecked { get; set; }
        public bool DocumentFinished { get; set; }
        public bool CheckFromStore { get; set; }
        public bool IsNewRecord { get; set; }
        public Guid? CancelsDocumentOid { get; set; }
        public bool IsFiscalPrinterHandled { get; set; }
        public decimal TotalQty { get; set; }
        public bool HasPaymentWithRatification { get; set; }
        public decimal TotalPoints { get; set; }
        public decimal TotalVatAmountBeforeDiscount { get; set; }
        public decimal GrossTotalBeforeDiscount { get; set; }
        public decimal NetTotalBeforeDiscount { get; set; }
        public bool PointsConsumed { get; set; }
        public bool PointsAddedToCustomer { get; set; }
        public bool PointsConsumedAtStoreController { get; set; }
        public bool PointsAddedToCustomerAtStoreController { get; set; }
        public decimal DocumentPoints { get; set; }
        public DateTime FiscalDate { get; set; }
        public DateTime InvoicingDate { get; set; }
        public Guid? CanceledByDocumentOid { get; set; }
        public decimal PromotionPoints { get; set; }
        public string DeliveryAddress { get; set; }
        public eDivision Division { get; set; }
        public Int32 DocumentNumber { get; set; }
        public decimal DocumentDiscountPercentage { get; set; }
        public decimal DocumentDiscountPercentagePerLine { get; set; }
        public decimal DocumentDiscountAmount { get; set; }
        public decimal PromotionsDiscountAmount { get; set; }
        public decimal PromotionsDiscountPercentagePerLine { get; set; }
        public decimal PromotionsDiscountPercentage { get; set; }
        public decimal PointsDiscountAmount { get; set; }
        public decimal PointsDiscountPercentagePerLine { get; set; }
        public decimal ConsumedPointsForDiscount { get; set; }
        public decimal PointsDiscountPercentage { get; set; }
        public decimal VatAmount1 { get; set; }
        public decimal VatAmount2 { get; set; }
        public decimal GrossTotal { get; set; }
        public DateTime FinalizedDate { get; set; }
        public decimal VatAmount3 { get; set; }
        public decimal VatAmount4 { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public string Remarks { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal NetTotal { get; set; }
        public Guid? Owner { get; set; }


        public List<DocumentPaymentViewModel> DocumentPayments { get; set; }
        public List<DocumentPromotionViewModel> DocumentPromotions { get; set; }
        public List<DocumentDetailViewModel> DocumentDetails { get; set; }

        public DocumentHeaderViewModel()
        {
            this.DocumentPayments = new List<DocumentPaymentViewModel>();
            this.DocumentPromotions = new List<DocumentPromotionViewModel>();
            this.DocumentDetails = new List<DocumentDetailViewModel>();
        }

        public void UpdateModel(Session uow)
        {
            this.UpdateProperties(uow);
            this.DocumentPayments.ForEach(x => { x.UpdateModel(uow); x.DocumentHeader = this.Oid; });
            this.DocumentPromotions.ForEach(x => { x.UpdateModel(uow); x.DocumentHeader = this.Oid; });
            this.DocumentDetails.ForEach(x => { x.UpdateModel(uow); x.DocumentHeader = this.Oid; });
            Store store = uow.GetObjectByKey<Store>(this.Store);

        }

        public bool Validate(out string message)
        {
            throw new NotImplementedException();
        }


        public bool UsesPackingQuantities(Session session)
        {
            if (this.DocumentType.HasValue)
            {
                DocumentType type = session.GetObjectByKey<DocumentType>(this.DocumentType.Value);
                return type == null ? false : type.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING;
            }
            return false;
        }

        public TemporaryObject GetTemporaryObject(Session session)
        {
            return session.FindObject<TemporaryObject>(
                CriteriaOperator.And(
                        new BinaryOperator("EntityOid", this.Oid),
                        new BinaryOperator("EntityType", PersistedType.FullName)
                    ));
        }
    }
}