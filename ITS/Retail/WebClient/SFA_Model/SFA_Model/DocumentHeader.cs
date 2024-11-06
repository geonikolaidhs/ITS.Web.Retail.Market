
using ITS.WRM.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Xpo;
using SFA_Model.NonPersistant;

namespace SFA_Model
{
    //[ITS.WRM.SFA.Model.Attributes.CreateOrUpdaterOrder(Order = 940, Permissions = eUpdateDirection.SFA_TO_MASTER)]
    public class DocumentHeader : BaseObj, IDocumentHeader
    {
        public DocumentHeader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentHeader(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            _IsNewRecord = true;
            DocumentNumber = 0;
            POS = null;
            ExecutionDate = null;
            RefferenceDate = FinalizedDate = DateTime.Now;
            TransformationLevel = eTransformationLevel.DEFAULT;
            Remarks = string.Empty;
            //_EffectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy();
        }
        //private EffectivePriceCatalogPolicy _EffectivePriceCatalogPolicy;
        private bool _IsNewRecord;
        public string AddressProfession { get; set; }

        public Guid BillingAddress { get; set; }

        public Guid? CanceledByDocumentOid { get; set; }

        public Guid? CancelsDocumentOid { get; set; }

        public bool CheckFromStore { get; set; }

        public decimal ConsumedPointsForDiscount { get; set; }

        public bool CouponsHaveBeenUpdatedOnMaster { get; set; }

        public bool CouponsHaveBeenUpdatedOnStoreController { get; set; }
        public Guid Customer { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerLookupCode { get; set; }

        public string CustomerLookUpTaxCode { get; set; }

        public string CustomerName { get; set; }

        public bool CustomerNotFound { get; set; }

        public string DeliveryAddress { get; set; }
        public Guid DeliveryTo { get; set; }

        public string DeliveryToTraderTaxCode { get; set; }


        public Guid? DenormalisedAddress { get; set; }

        public string DenormalizedCustomer { get; set; }
        public Guid Division { get; set; }

        public decimal DocumentDiscountAmount { get; set; }

        public decimal DocumentDiscountPercentage { get; set; }

        public decimal DocumentDiscountPercentagePerLine { get; set; }
        public Guid DocumentDiscountType { get; set; }

        public bool DocumentFinished { get; set; }

        public int DocumentNumber { get; set; }

        public decimal DocumentPoints { get; set; }
        public Guid DocumentSeries { get; set; }

        public string DocumentSeriesCode { get; set; }

        public string DocumentStatusCode { get; set; }

        public Guid DocumentType { get; set; }

        public string DocumentTypeCode { get; set; }

        public DateTime? ExecutionDate { get; set; }

        public DateTime FinalizedDate { get; set; }

        public DateTime FiscalDate { get; set; }

        public decimal GrossTotal { get; set; }

        public decimal GrossTotalBeforeDiscount { get; set; }

        public decimal GrossTotalBeforeDocumentDiscount { get; set; }

        public bool HasBeenChecked { get; set; }

        public bool HasBeenExecuted { get; set; }

        public bool HasBeenOnHold { get; set; }

        public bool HasPaymentWithRatification { get; set; }

        public bool InEmulationMode { get; set; }

        public DateTime InvoicingDate { get; set; }

        public bool IsCanceled { get; set; }

        public bool IsDayStartingAmount { get; set; }

        public bool IsFiscalPrinterHandled { get; set; }

        public bool IsNewRecord { get; set; }

        public bool IsShiftStartingAmount { get; set; }

        public decimal NetTotal { get; set; }

        public decimal NetTotalBeforeDiscount { get; set; }

        public string PlaceOfLoading { get; set; }

        public bool PointsAddedToCustomer { get; set; }

        public bool PointsAddedToCustomerAtStoreController { get; set; }

        public bool PointsConsumed { get; set; }

        public bool PointsConsumedAtStoreController { get; set; }

        public decimal PointsDiscountAmount { get; set; }

        public decimal PointsDiscountPercentage { get; set; }

        public decimal PointsDiscountPercentagePerLine { get; set; }

        public IPOS POS { get; set; }

        public int POSID { get; set; }

        public Guid PriceCatalog { get; set; }


        public Guid PriceCatalogPolicy { get; set; }

        public string ProcessedDenormalizedCustomer { get; set; }

        public decimal PromotionPoints { get; set; }


        public decimal PromotionsDiscountAmount { get; set; }

        public decimal PromotionsDiscountPercentage { get; set; }

        public decimal PromotionsDiscountPercentagePerLine { get; set; }

        public Guid PromotionsDocumentDiscountType { get; set; }

        public DateTime RefferenceDate { get; set; }

        public string Remarks { get; set; }

        public Guid SecondaryStore { get; set; }

        public string Signature { get; set; }

        public Guid Status { get; set; }

        public string VehicleNumber { get; set; }

        public Guid StoreOid { get; set; }
        public string StoreCode { get; set; }

        public Guid Supplier { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal TotalPoints { get; set; }

        public decimal TotalQty { get; set; }

        public decimal TotalVatAmount { get; set; }

        public decimal TotalVatAmountBeforeDiscount { get; set; }

        public string TransferMethod { get; set; }

        public Guid TransferPurpose { get; set; }

        public eTransformationLevel TransformationLevel { get; set; }

        public string TriangularAddress { get; set; }

        public Guid TriangularCustomer { get; set; }

        [NonPersistent]
        public Guid TriangularStore { get; set; }

        public Guid TriangularSupplier { get; set; }

        public decimal VatAmount1 { get; set; }

        public decimal VatAmount2 { get; set; }

        public decimal VatAmount3 { get; set; }

        public decimal VatAmount4 { get; set; }


        public Guid Store { get; set; }
        [NonPersistent]
        IStore IDocumentHeader.SecondaryStore
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IAddress IDocumentHeader.BillingAddress
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IPriceCatalog IDocumentHeader.PriceCatalog
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ISupplierNew IDocumentHeader.Supplier
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ITransferPurpose IDocumentHeader.TransferPurpose
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IPOS IDocumentHeader.POS
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDiscountType IDocumentHeader.DocumentDiscountType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDocumentType IDocumentHeader.DocumentType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDocumentSeries IDocumentHeader.DocumentSeries
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ITrader IDocumentHeader.DeliveryTo
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDeliveryType IDocumentHeader.DeliveryType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IStore IDocumentHeader.Store
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDivision IDocumentHeader.Division
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ICustomer IDocumentHeader.Customer
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDocumentStatus IDocumentHeader.Status
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ICustomer IDocumentHeader.TriangularCustomer
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]

        ISupplierNew IDocumentHeader.TriangularSupplier
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IStore IDocumentHeader.TriangularStore
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDiscountType IDocumentHeader.PromotionsDocumentDiscountType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IPriceCatalogPolicy IDocumentHeader.PriceCatalogPolicy
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}