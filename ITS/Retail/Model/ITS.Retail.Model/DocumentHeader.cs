//-----------------------------------------------------------------------
// <copyright file="DocumentHeader.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Enumerations.ViewModel;
using ITS.Retail.Platform.Kernel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ITS.Retail.Model
{


    [DataViewParameter]
    [Updater(Order = 940, Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.POS_TO_STORECONTROLLER)]
    [Indices("DocumentNumber;IsCanceled;FinalizedDate", "DocumentNumber;IsCanceled;FinalizedDate;Oid", "DocumentNumber;IsCanceled;FinalizedDate;Oid;Store;Customer")]
    [TriggerActionType]
    public class DocumentHeader : BaseObj, IOwner, IDocumentHeader
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
            _EffectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy();
            _CustomerBalanceComputedAtHeadquarters = false;
            _CustomerBalanceComputedAtStoreController = false;
            _Balance = 0;
            _PreviousBalance = 0;
        }

        [DescriptionField]
        public string Description
        {
            get
            {
                return String.Format("{0} {1} {2}", (DocumentType == null ? "" : DocumentType.Description), (DocumentSeries == null ? "" : DocumentSeries.Description), DocumentNumber);
            }
        }

        private string _DenormalizedCustomer;
        private bool _HasBeenOnHold;
        private Store _SecondaryStore;
        private DocumentSource _Source;
        private eTransformationLevel _TransformationLevel;
        private DateTime? _ExecutionDate;
        private Address _BillingAddress;
        private decimal _GrossTotalBeforeDocumentDiscount;
        private DateTime _RefferenceDate;
        private PriceCatalog _PriceCatalog;
        private SupplierNew _Supplier;
        private TransferPurpose _TransferPurpose;
        private string _DocumentStatusCode;
        private string _DeliveryToTraderTaxCode;
        private string _CustomerCode;
        private string _StoreCode;
        private string _DocumentSeriesCode;
        private string _DocumentTypeCode;
        private int _POSID;
        private string _PlaceOfLoading;
        private string _TransferMethod;
        private string _Signature;
        private POS _POS;
        private bool _IsCanceled;
        private bool _HasBeenExecuted;
        private bool _HasBeenChecked;
        private bool _DocumentFinished;
        private bool _CheckFromStore;
        private bool _IsNewRecord;
        private UserDailyTotals _UserDailyTotals;
        private Guid? _CancelsDocumentOid;
        private bool _IsFiscalPrinterHandled;
        private decimal _TotalQty;
        private bool _HasPaymentWithRatification;
        private decimal _TotalPoints;
        private decimal _TotalVatAmountBeforeDiscount;
        private decimal _GrossTotalBeforeDiscount;
        private decimal _NetTotalBeforeDiscount;
        private DiscountType _DocumentDiscountType;
        private bool _PointsConsumed;
        private bool _PointsAddedToCustomer;
        private bool _PointsConsumedAtStoreController;
        private bool _PointsAddedToCustomerAtStoreController;
        private decimal _DocumentPoints;
        private DateTime _FiscalDate;
        private DateTime _InvoicingDate;
        private DocumentType _DocumentType;
        private DocumentSeries _DocumentSeries;
        private Guid? _CanceledByDocumentOid;
        private decimal _PromotionPoints;
        private string _DeliveryAddress;
        private Trader _DeliveryTo;
        private DeliveryType _DeliveryType;
        private int _DocumentNumber;
        private Store _Store;
        private eDivision _Division;
        private Customer _Customer;
        private decimal _DocumentDiscountPercentage;
        private decimal _DocumentDiscountPercentagePerLine;
        private decimal _DocumentDiscountAmount;
        private decimal _PromotionsDiscountAmount;
        private decimal _PromotionsDiscountPercentagePerLine;
        private decimal _PromotionsDiscountPercentage;
        private decimal _PointsDiscountAmount;
        private decimal _PointsDiscountPercentagePerLine;
        private decimal _ConsumedPointsForDiscount;
        private decimal _PointsDiscountPercentage;
        private decimal _CustomerDiscountPercentagePerLine;
        private decimal _CustomerDiscountAmount;
        private decimal _DefaultDocumentDiscountPercentagePerLine;
        private decimal _DefaultDocumentDiscountAmount;
        private decimal _VatAmount1;
        private decimal _VatAmount2;
        private DocumentStatus _Status;
        private decimal _GrossTotal;
        private DateTime _FinalizedDate;
        private decimal _VatAmount3;
        private decimal _VatAmount4;
        private decimal _TotalDiscountAmount;
        private string _Remarks;
        private decimal _TotalVatAmount;
        private decimal _NetTotal;
        private bool _IsShiftStartingAmount;
        private bool _IsDayStartingAmount;
        private bool _CustomerNotFound;
        private string _CustomerLookupCode;
        private string _CustomerName;
        private Customer _TriangularCustomer;
        private SupplierNew _TriangularSupplier;
        private Store _TriangularStore;
        private bool _InEmulationMode;
        private bool _CouponsHaveBeenUpdatedOnStoreController;
        private bool _CouponsHaveBeenUpdatedOnMaster;
        private DiscountType _PromotionsDocumentDiscountType;
        private InsertedCustomerViewModel _CustomerViewModel;
        private string _CustomerLookUpTaxCode;
        private string _ProcessedDenormalizedCustomer;
        private Guid? _DenormalisedAddress;
        private string _AddressProfession;
        private string _TriangularAddress;
        private PriceCatalogPolicy _PriceCatalogPolicy;
        private bool _CustomerBalanceComputedAtStoreController;
        private bool _CustomerBalanceComputedAtHeadquarters;
        private decimal _DefaultDocumentDiscount;
        private decimal _DefaultCustomerDiscount;
        private User _ChargedToUser;
        private SFA _Tablet;
        private double _Latitude;
        private double _Longitude;
        private string _VehicleNumber;
        private int _FiscalPrinterNumber;



        public CompanyNew Owner
        {
            get
            {
                if (Store == null)
                {
                    return null;
                }
                return Store.Owner;
            }
        }

        public CompanyNew ReferenceCompany
        {
            get
            {
                return _ReferenceCompany;
            }
            set
            {
                SetPropertyValue("ReferenceCompany", ref _ReferenceCompany, value);
            }
        }


        public CompanyNew MainCompany
        {
            get
            {
                return _MainCompany;
            }
            set
            {
                SetPropertyValue("MainCompany", ref _MainCompany, value);
            }
        }

        public CompanyNew Company
        {
            get
            {
                return ReferenceCompany ?? MainCompany;
            }
        }

        [Association("Supplier-DocumentHeaders")]
        public SupplierNew Supplier
        {
            get
            {
                return _Supplier;
            }
            set
            {
                SetPropertyValue("Supplier", ref _Supplier, value);
            }
        }


        [Indexed(Unique = false)]
        public bool IsFiscalPrinterHandled
        {
            get
            {
                return _IsFiscalPrinterHandled;
            }
            set
            {
                SetPropertyValue("IsFiscalPrinterHandled", ref _IsFiscalPrinterHandled, value);
            }
        }


        [Size(200)]
        public string Signature
        {
            get
            {
                return _Signature;
            }
            set
            {
                SetPropertyValue("Signature", ref _Signature, value);
            }
        }



        public string VehicleNumber
        {
            get
            {
                return _VehicleNumber;
            }
            set
            {
                SetPropertyValue("VehicleNumber", ref _VehicleNumber, value);
            }
        }

        public double Latitude
        {
            get
            {
                return _Latitude;
            }
            set
            {
                SetPropertyValue("Latitude", ref _Latitude, value);
            }
        }

        public double Longitude
        {
            get
            {
                return _Longitude;
            }
            set
            {
                SetPropertyValue("Longitude", ref _Longitude, value);
            }
        }

        [NonPersistent]
        public bool IsNewRecord
        {
            get
            {
                return _IsNewRecord;
            }
        }


        [Indexed(Unique = false)]
        public bool IsCanceled
        {
            get
            {
                return _IsCanceled;
            }
            set
            {
                SetPropertyValue("IsCanceled", ref _IsCanceled, value);
            }
        }


        [Association("POS-DocumentHeaders"), Indexed(Unique = false)]
        public POS POS
        {
            get
            {
                return _POS;
            }
            set
            {
                SetPropertyValue("POS", ref _POS, value);
            }
        }


        public int POSID
        {
            get
            {
                return _POSID;
            }
            set
            {
                SetPropertyValue("POSID", ref _POSID, value);
            }
        }

        public DateTime FiscalDate
        {
            get
            {
                return _FiscalDate;
            }
            set
            {
                SetPropertyValue("FiscalDate", ref _FiscalDate, value);
            }
        }

        public bool InEmulationMode
        {
            get
            {
                return _InEmulationMode;
            }
            set
            {
                SetPropertyValue("InEmulationMode", ref _InEmulationMode, value);
            }
        }

        public DateTime InvoicingDate
        {
            get
            {
                return _InvoicingDate;
            }
            set
            {
                SetPropertyValue("InvoicingDate", ref _InvoicingDate, value);
            }

        }


        public DocumentType DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }


        public string DocumentTypeCode
        {
            get
            {
                return _DocumentTypeCode;
            }
            set
            {
                SetPropertyValue("DocumentTypeCode", ref _DocumentTypeCode, value);
            }
        }


        public DocumentSeries DocumentSeries
        {
            get
            {
                return _DocumentSeries;
            }
            set
            {
                SetPropertyValue("DocumentSeries", ref _DocumentSeries, value);
            }
        }


        public string DocumentSeriesCode
        {
            get
            {
                return _DocumentSeriesCode;
            }
            set
            {
                SetPropertyValue("DocumentSeriesCode", ref _DocumentSeriesCode, value);
            }
        }


        public int DocumentNumber

        {
            get
            {
                return _DocumentNumber;
            }
            set
            {
                SetPropertyValue("DocumentNumber", ref _DocumentNumber, value);
            }
        }

        [Association("Store-DocumentHeaders"), Indexed(Unique = false)]
        public Store Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }

        [UpdaterIgnoreField]
        [Association("Tablet-DocumentHeaders"), Indexed(Unique = false)]
        public SFA Tablet
        {
            get
            {
                return _Tablet;
            }
            set
            {
                SetPropertyValue("Tablet", ref _Tablet, value);
            }
        }


        public string StoreCode
        {
            get
            {
                return _StoreCode;
            }
            set
            {
                SetPropertyValue("StoreCode", ref _StoreCode, value);
            }
        }


        public eDivision Division
        {
            get
            {
                return _Division;
            }
            set
            {
                SetPropertyValue("Division", ref _Division, value);
            }
        }

        [Association("Customer-DocumentHeaders")]
        public Customer Customer
        {
            get
            {
                return _Customer;
            }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
            }
        }

        public bool CustomerNotFound
        {
            get
            {
                return _CustomerNotFound;
            }
            set
            {
                SetPropertyValue("CustomerNotFound", ref _CustomerNotFound, value);
            }
        }

        public decimal DefaultCustomerDiscount
        {
            get
            {
                return _DefaultCustomerDiscount;
            }
            set
            {
                SetPropertyValue("DefaultCustomerDiscount", ref _DefaultCustomerDiscount, value);
            }
        }


        public decimal DefaultDocumentDiscount
        {
            get
            {
                return _DefaultDocumentDiscount;
            }
            set
            {
                SetPropertyValue("DefaultDocumentDiscount", ref _DefaultDocumentDiscount, value);
            }
        }



        //[GDPR]
        public string CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                SetPropertyValue("CustomerName", ref _CustomerName, value);
            }
        }

        public string CustomerLookupCode
        {
            get
            {
                return _CustomerLookupCode;
            }
            set
            {
                SetPropertyValue("CustomerLookupCode", ref _CustomerLookupCode, value);
            }
        }
        public string CustomerLookUpTaxCode
        {
            get
            {
                return _CustomerLookUpTaxCode;
            }
            set
            {
                SetPropertyValue("CustomerLookUpTaxCode", ref _CustomerLookUpTaxCode, value);
            }
        }

        public string CustomerCode
        {
            get
            {
                return _CustomerCode;
            }
            set
            {
                SetPropertyValue("CustomerCode", ref _CustomerCode, value);
            }
        }

        public DeliveryType DeliveryType
        {
            get
            {
                return _DeliveryType;
            }
            set
            {
                SetPropertyValue("DeliveryType", ref _DeliveryType, value);
            }
        }

        public Trader DeliveryTo
        {
            get
            {
                return _DeliveryTo;
            }
            set
            {
                SetPropertyValue("DeliveryTo", ref _DeliveryTo, value);
            }
        }


        public string DeliveryToTraderTaxCode
        {
            get
            {
                return _DeliveryToTraderTaxCode;
            }
            set
            {
                SetPropertyValue("DeliveryToTraderTaxCode", ref _DeliveryToTraderTaxCode, value);
            }
        }


        public string DeliveryAddress
        {
            get
            {
                return _DeliveryAddress;
            }
            set
            {
                SetPropertyValue("DeliveryAddress", ref _DeliveryAddress, value);
            }
        }


        public decimal DocumentDiscountPercentage
        {
            get
            {
                return _DocumentDiscountPercentage;
            }
            set
            {
                SetPropertyValue("DocumentDiscountPercentage", ref _DocumentDiscountPercentage, value);
            }
        }


        public decimal DocumentDiscountPercentagePerLine
        {
            get
            {
                return _DocumentDiscountPercentagePerLine;
            }
            set
            {
                SetPropertyValue("DocumentDiscountPercentagePerLine", ref _DocumentDiscountPercentagePerLine, value);
            }
        }

        public int FiscalPrinterNumber
        {
            get
            {
                return _FiscalPrinterNumber;
            }
            set
            {
                SetPropertyValue("FiscalPrinterNumber", ref _FiscalPrinterNumber, value);
            }
        }


        public decimal DocumentDiscountAmount
        {
            get
            {
                return _DocumentDiscountAmount;
            }
            set
            {
                SetPropertyValue("DocumentDiscountAmount", ref _DocumentDiscountAmount, value);
            }
        }

        /// <summary>
        /// Calculated. Gets all the discounts that must be shown as "Document Header Discounts".
        /// </summary>
        public decimal AllDocumentHeaderDiscounts
        {
            get
            {
                return this.PointsDiscountAmount + this.DocumentDiscountAmount; //+ CustomerDiscount todo
            }
        }

        public decimal ConsumedPointsForDiscount
        {
            get
            {
                return _ConsumedPointsForDiscount;
            }
            set
            {
                SetPropertyValue("ConsumedPointsForDiscount", ref _ConsumedPointsForDiscount, value);
            }
        }

        public decimal PointsDiscountPercentage
        {
            get
            {
                return _PointsDiscountPercentage;
            }
            set
            {
                SetPropertyValue("PointsDiscountPercentage", ref _PointsDiscountPercentage, value);
            }
        }

        /// <summary>
        /// The actual percentage that is applied to the lines
        /// </summary>
        public decimal PointsDiscountPercentagePerLine
        {
            get
            {
                return _PointsDiscountPercentagePerLine;
            }
            set
            {
                SetPropertyValue("PointsDiscountPercentagePerLine", ref _PointsDiscountPercentagePerLine, value);
            }
        }

        public decimal PointsDiscountAmount
        {
            get
            {
                return _PointsDiscountAmount;
            }
            set
            {
                SetPropertyValue("PointsDiscountAmount", ref _PointsDiscountAmount, value);
            }
        }

        public decimal PromotionsDiscountPercentage
        {
            get
            {
                return _PromotionsDiscountPercentage;
            }
            set
            {
                SetPropertyValue("PromotionsDiscountPercentage", ref _PromotionsDiscountPercentage, value);
            }
        }
        /// <summary>
        /// The actual percentage that is applied to the lines
        /// </summary>
        public decimal PromotionsDiscountPercentagePerLine
        {
            get
            {
                return _PromotionsDiscountPercentagePerLine;
            }
            set
            {
                SetPropertyValue("PromotionsDiscountPercentagePerLine", ref _PromotionsDiscountPercentagePerLine, value);
            }
        }

        public decimal PromotionsDiscountAmount
        {
            get
            {
                return _PromotionsDiscountAmount;
            }
            set
            {
                SetPropertyValue("PromotionsDiscountAmount", ref _PromotionsDiscountAmount, value);
            }
        }

        public decimal PromotionPoints
        {
            get
            {
                return _PromotionPoints;
            }
            set
            {
                SetPropertyValue("PromotionPoints", ref _PromotionPoints, value);
            }
        }

        public DiscountType DocumentDiscountType
        {
            get
            {
                return _DocumentDiscountType;
            }
            set
            {
                SetPropertyValue("DocumentDiscountType", ref _DocumentDiscountType, value);
            }
        }

        public decimal VatAmount1
        {
            get
            {
                return _VatAmount1;
            }
            set
            {
                SetPropertyValue("VatAmount1", ref _VatAmount1, value);
            }
        }

        public decimal VatAmount2
        {
            get
            {
                return _VatAmount2;
            }
            set
            {
                SetPropertyValue("VatAmount2", ref _VatAmount2, value);
            }
        }

        public decimal VatAmount3
        {
            get
            {
                return _VatAmount3;
            }
            set
            {
                SetPropertyValue("VatAmount3", ref _VatAmount3, value);
            }
        }

        public decimal VatAmount4
        {
            get
            {
                return _VatAmount4;
            }
            set
            {
                SetPropertyValue("VatAmount4", ref _VatAmount4, value);
            }
        }

        public decimal TotalDiscountAmount
        {
            get
            {
                return _TotalDiscountAmount;
            }
            set
            {
                SetPropertyValue("TotalDiscountAmount", ref _TotalDiscountAmount, value);
            }
        }

        public decimal TotalVatAmount
        {
            get
            {
                return _TotalVatAmount;
            }
            set
            {
                SetPropertyValue("TotalVatAmount", ref _TotalVatAmount, value);
            }
        }

        public decimal NetTotal
        {
            get
            {
                return _NetTotal;
            }
            set
            {
                SetPropertyValue("NetTotal", ref _NetTotal, value);
            }
        }

        public decimal GrossTotal
        {
            get
            {
                return _GrossTotal;
            }
            set
            {
                SetPropertyValue("GrossTotal", ref _GrossTotal, value);
            }
        }

        public decimal NetTotalBeforeDocumentDiscount
        {
            get
            {
                return this.DocumentDetails.Sum(x => x.NetTotalBeforeDocumentDiscount);
            }
        }

        [Indexed(Unique = false)]
        public DateTime FinalizedDate
        {
            get
            {
                return _FinalizedDate;
            }
            set
            {
                SetPropertyValue("FinalizedDate", ref _FinalizedDate, value);
            }
        }

        /// <summary>
        /// The actual percentage that is applied to the lines
        /// </summary>
        public decimal CustomerDiscountPercentagePerLine
        {
            get
            {
                return _CustomerDiscountPercentagePerLine;
            }
            set
            {
                SetPropertyValue("CustomerDiscountPercentagePerLine", ref _CustomerDiscountPercentagePerLine, value);
            }
        }

        public decimal CustomerDiscountAmount
        {
            get
            {
                return _CustomerDiscountAmount;
            }
            set
            {
                SetPropertyValue("CustomerDiscountAmount", ref _CustomerDiscountAmount, value);
            }
        }

        /// <summary>
        /// The actual percentage that is applied to the lines
        /// </summary>
        public decimal DefaultDocumentDiscountPercentagePerLine
        {
            get
            {
                return _DefaultDocumentDiscountPercentagePerLine;
            }
            set
            {
                SetPropertyValue("DefaultDocumentDiscountPercentagePerLine", ref _DefaultDocumentDiscountPercentagePerLine, value);
            }
        }

        public decimal DefaultDocumentDiscountAmount
        {
            get
            {
                return _DefaultDocumentDiscountAmount;
            }
            set
            {
                SetPropertyValue("DefaultDocumentDiscountAmount", ref _DefaultDocumentDiscountAmount, value);
            }
        }

        [Association("DocumentStatus-DocumentHeaders"), Indexed(Unique = false)]
        public DocumentStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
            }
        }

        public string DocumentStatusCode
        {
            get
            {
                return _DocumentStatusCode;
            }
            set
            {
                SetPropertyValue("DocumentStatusCode", ref _DocumentStatusCode, value);
            }
        }

        public bool HasBeenChecked
        {
            get
            {
                return _HasBeenChecked;
            }
            set
            {
                SetPropertyValue("HasBeenChecked", ref _HasBeenChecked, value);
            }
        }

        public bool HasBeenExecuted
        {
            get
            {
                return _HasBeenExecuted;
            }
            set
            {
                SetPropertyValue("HasBeenExecuted", ref _HasBeenExecuted, value);
            }
        }

        public bool CheckFromStore
        {
            get
            {
                return _CheckFromStore;
            }
            set
            {
                SetPropertyValue("CheckFromStore", ref _CheckFromStore, value);
            }
        }

        public bool DocumentFinished
        {
            get
            {
                return _DocumentFinished;
            }
            set
            {
                SetPropertyValue("DocumentFinished", ref _DocumentFinished, value);
            }
        }

        [Aggregated, Association("DocumentHeader-DocumentPayments")]
        public XPCollection<DocumentPayment> DocumentPayments
        {
            get
            {
                return GetCollection<DocumentPayment>("DocumentPayments");
            }
        }

        [Aggregated, Association("DocumentHeader-DocumentPromotions")]
        public XPCollection<DocumentPromotion> DocumentPromotions
        {
            get
            {
                return GetCollection<DocumentPromotion>("DocumentPromotions");
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Remarks
        {
            get
            {
                return _Remarks;
            }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        [Aggregated, Association("DocumentHeader-DocumentDetails")]
        public XPCollection<DocumentDetail> DocumentDetails
        {
            get
            {
                return GetCollection<DocumentDetail>("DocumentDetails");
            }
        }

        [NonPersistent]
        [JsonIgnore]
        [UpdaterIgnoreField]
        public decimal CalculatedTotalQty
        {
            get
            {
                try
                {
                    if (this.DocumentDetails != null && this.DocumentDetails.Count > 0)
                    {
                        CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("DocumentHeader.Oid", Oid));
                        return (decimal)this.Session.Evaluate<DocumentDetail>(CriteriaOperator.Parse("Sum(Qty)"), crop);
                    }
                    return 0;
                }
                catch (Exception exception)
                {
                    string errorMessage = exception.Message;
                    if (this.DocumentDetails == null || this.DocumentDetails.Count <= 0)
                    {
                        return 0;
                    }
                    return this.DocumentDetails.Sum(g => g.Qty);
                }
            }
        }

        public decimal CalcTotalQty()
        {
            try
            {
                if (this.DocumentDetails != null && this.DocumentDetails.Count > 0)
                {
                    return this.DocumentDetails.Sum(g => g.Qty);
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        [Aggregated, Association("RelativeDocuments-InitialDocumentHeader")]
        public XPCollection<RelativeDocument> DerivedDocuments
        {
            get
            {
                return GetCollection<RelativeDocument>("DerivedDocuments");
            }
        }

        [Aggregated, Association("RelativeDocuments-DerivedDocumentHeader")]
        public XPCollection<RelativeDocument> ReferencedDocuments
        {
            get
            {
                return GetCollection<RelativeDocument>("ReferencedDocuments");
            }
        }

        /// <summary>
        /// A List of UnreferencedDocumentDetail with the necessary data. Check class if you want more data to be retrieved
        /// </summary>
        public IEnumerable<UnreferencedDocumentDetail> UnreferencedDetails
        {
            get
            {

                var groupedUnique = from detail in TransformableDocumentDetails
                                    group detail by new { detail.Barcode, detail.FinalUnitPrice, detail.TotalDiscount, detail.LinkedLine };
                IEnumerable<UnreferencedDocumentDetail> unreferencedDocumentDetails = groupedUnique
                    .Where(detail => detail.Sum(x => x.UnreferencedQuantity) > 0)
                    .Select(detail => new UnreferencedDocumentDetail(
                                      detail.Key.Barcode,
                                      detail.Key.FinalUnitPrice,
                                      detail.Sum(x => x.UnreferencedQuantity),
                                      detail.Key.LinkedLine,
                                                           detail.Key.TotalDiscount
                                                   )).ToList();
                return unreferencedDocumentDetails;
            }
        }

        public string TransferMethod
        {
            get
            {
                return _TransferMethod;
            }
            set
            {
                SetPropertyValue("TransferMethod", ref _TransferMethod, value);
            }
        }

        public bool HasPaymentWithRatification
        {
            get
            {
                return _HasPaymentWithRatification;
            }
            set
            {
                SetPropertyValue("HasPaymentWithRatification", ref _HasPaymentWithRatification, value);
            }
        }


        public string PlaceOfLoading
        {
            get
            {
                return _PlaceOfLoading;
            }
            set
            {
                SetPropertyValue("PlaceOfLoading", ref _PlaceOfLoading, value);
            }
        }


        public TransferPurpose TransferPurpose
        {
            get
            {
                return _TransferPurpose;
            }
            set
            {
                SetPropertyValue("TransferPurpose", ref _TransferPurpose, value);
            }
        }

        /// <summary>
        /// Gets or sets a flag that indicates that the points where consumed from the customer table at the StoreController level
        /// </summary>
        public bool PointsConsumedAtStoreController
        {
            get
            {
                return _PointsConsumedAtStoreController;
            }
            set
            {
                SetPropertyValue("PointsConsumedAtStoreController", ref _PointsConsumedAtStoreController, value);
            }
        }

        /// <summary>
        /// Gets or sets a flag that indicates that the points where consumed from the customer table at the Master/Dual level
        /// </summary>
        [UpdaterIgnoreField]
        public bool PointsConsumed
        {
            get
            {
                return _PointsConsumed;
            }
            set
            {
                SetPropertyValue("PointsConsumed", ref _PointsConsumed, value);
            }
        }

        /// <summary>
        /// Gets or sets a flag that indicates that the points where added to the customer table at the StoreController level
        /// </summary>
        public bool PointsAddedToCustomerAtStoreController
        {
            get
            {
                return _PointsAddedToCustomerAtStoreController;
            }
            set
            {
                SetPropertyValue("PointsAddedToCustomerAtStoreController", ref _PointsAddedToCustomerAtStoreController, value);
            }
        }

        /// <summary>
        /// Gets or sets a flag that indicates that the points where added to the customer table at the Master/Dual level
        /// </summary>
        [UpdaterIgnoreField]
        public bool PointsAddedToCustomer
        {
            get
            {
                return _PointsAddedToCustomer;
            }
            set
            {
                SetPropertyValue("PointsAddedToCustomer", ref _PointsAddedToCustomer, value);
            }
        }

        /// <summary>
        /// On Saving Get next document number
        /// </summary>
        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }

            ArrangePreviousBaseQuantity();
            ArrangeItemStockPropertiesForPOSDocuments();
            ArrangeCompany();
            ArrangeItemStock();
            ArrangeDocumentNumberSequenceAndCustomerPoints();
            ArrangeCustomerBalance();
            base.OnSaving();
        }

        private void ArrangePreviousBaseQuantity()
        {
            if (this.Session.GetObjectsToSave().Count == 1)
            {
                foreach (DocumentDetail currentLine in this.DocumentDetails)
                {
                    currentLine.PreviousBaseQuantity = currentLine.BaseQuantity;
                    currentLine.Save();
                }
            }

            if (DocumentWillTakeNumber() == false && DocumentNumber <= 0)
            {
                foreach (DocumentDetail currentLine in this.DocumentDetails)
                {
                    currentLine.PreviousBaseQuantity = 0;
                    currentLine.Save();
                }
            }
        }

        private void ArrangeItemStockPropertiesForPOSDocuments()
        {
            #region Calculate BaseQuantity and BaseMeasurementUnit for ItemStock for POS Documents
            if (this.POS != null)
            {
                foreach (DocumentDetail currentLine in this.DocumentDetails)
                {
                    if (currentLine.Item == null
                        || currentLine.Item.DefaultBarcode == null
                        || currentLine.IsCanceled
                        || currentLine.IsReturn
                       )
                    {
                        currentLine.BaseQuantity = 0;
                        continue;
                    }
                    if (currentLine.Item.DefaultBarcode.Oid == currentLine.Barcode.Oid)
                    {
                        currentLine.BaseQuantity = (double)currentLine.Qty;
                        currentLine.BaseMeasurementUnit = currentLine.MeasurementUnit;
                    }
                    else
                    {
                        ItemBarcode itemBarcode = currentLine.Barcode.ItemBarcode(this.Owner);
                        decimal relationFactor = itemBarcode == null ? 1 : (decimal)itemBarcode.RelationFactor;
                        currentLine.BaseQuantity = (double)(relationFactor * currentLine.Qty);
                        currentLine.BaseMeasurementUnit = currentLine.Item.DefaultBarcode.MeasurementUnit(this.Owner) ?? currentLine.MeasurementUnit;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Arranges Item Stock
        /// It Needs to check current DocumentHeader number so call this before assigning DocumentNumber - ArrangeDocumentNumberSequenceAndCustomerPoints()
        /// </summary>
        private void ArrangeItemStock()
        {
            WRMApplicationSettings wrmApplicationSettings = this.Session.FindObject<WRMApplicationSettings>(null);
            if (wrmApplicationSettings.ApplicationInstance != eApplicationInstance.DUAL_MODE
                && wrmApplicationSettings.ApplicationInstance != eApplicationInstance.RETAIL
               )
            {
                return;
            }
            if (IsTablet())
                return;
            if (DocumentAffectsItemStock())
            {
                if (this.IsCancelingAnotherDocument)
                {
                    ArrangeItemStockOnDeletingOrCanceling();
                }
                else
                {
                    switch (this.DocumentType.ItemStockAffectionOptions)
                    {
                        case ItemStockAffectionOptions.NO_AFFECTION:
                            throw new ArgumentException(String.Format("DocumentHeader.ArrangeItemStock() : cannot handle ItemStockAffectionOptions ({0})", this.DocumentType.ItemStockAffectionOptions));
                        case ItemStockAffectionOptions.AFFECTS:
                            ArrangeItemStockForAffectsOption();
                            break;
                        case ItemStockAffectionOptions.INITIALISES:
                            ArrangeItemStockForInitialisesOption();
                            break;
                        default:
                            throw new ArgumentException(String.Format("DocumentHeader.ArrangeItemStock() : cannot handle ItemStockAffectionOptions ({0})", this.DocumentType.ItemStockAffectionOptions));
                    }
                }
            }
        }
        private bool IsTablet()
        {
            Guid id;
            SFA tablet = null;
            if (Guid.TryParse(this.CreatedByDevice, out id))
            {
                tablet = this.Session.GetObjectByKey<SFA>(id);
            }
            return tablet != null ? true : false;

        }

        private void ArrangeItemStockForInitialisesOption()
        {
            if (IsTablet())
                return;
            foreach (Item item in this.DocumentDetails.Select(document => document.Item).Distinct())
            {
                ItemStock itemStock = GetExistingOrNewItemStock(item);
                itemStock.LastDocumentHeaderInventory = this;

                DocumentHeader lastInventoryDocumentHeader = item.LastInventoryDocumentHeader(this);
                if (lastInventoryDocumentHeader == null)
                {
                    lastInventoryDocumentHeader = this;
                }
                itemStock.Stock = item.RecalculateItemStockAfterInventory(lastInventoryDocumentHeader);
                itemStock.Save();
            }
            this.ItemStockHasBeenCalculated = true;
        }

        private void ArrangeItemStockForAffectsOption()
        {
            if (IsTablet())
                return;
            bool isNewObject = this.Session.IsNewObject(this);

            foreach (Item item in this.DocumentDetails.Select(document => document.Item).Distinct())
            {
                ItemStock itemStock = GetExistingOrNewItemStock(item);

                if (isNewObject == false)
                {
                    itemStock.Stock -= this.DocumentType.QuantityFactor * this.DocumentDetails
                                                                          .Where(documentLine => documentLine.Item.Oid == item.Oid)
                                                                          .Sum(documentLine => documentLine.PreviousBaseQuantity);
                }

                itemStock.Stock += this.DocumentType.QuantityFactor * this.DocumentDetails
                                                                          .Where(documentLine => documentLine.Item.Oid == item.Oid)
                                                                          .Sum(documentLine => documentLine.BaseQuantity);
                itemStock.Save();
            }
            this.ItemStockHasBeenCalculated = true;
        }

        private ItemStock GetExistingOrNewItemStock(Item item)
        {

            return item.GetItemStockForStore(this.Store) ?? new ItemStock(this.Session)
            {
                Store = this.Store,
                Item = item,
                Stock = 0,
                LastDocumentHeaderFinalizedDate = this.FinalizedDate
            };
        }

        private bool DocumentAffectsItemStock()
        {
            WRMApplicationSettings wrmApplicationSettings = this.Session.FindObject<WRMApplicationSettings>(null);
            if (wrmApplicationSettings.ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
            {
                return false;
            }
            if (this.POS != null && this.IsCanceled && this.CanceledByDocument == null)
            {
                return false;
            }
            if (IsTablet())
                return false;
            bool isNewObject = this.Session.IsNewObject(this);
            if (this.IsCanceled
                && isNewObject == false
                && this.ChangedMembers.Contains(this.ClassInfo.GetMember("ItemStockHasBeenCalculated"))
                && this.ItemStockHasBeenCalculated == false
               )
            {
                return false;
            }

            return (DocumentWillTakeNumber() ||
                        (this.DocumentNumber > 0
                        && this.DocumentSeries.eModule != eModule.HEADQUARTERS
                        && wrmApplicationSettings.ApplicationInstance == eApplicationInstance.RETAIL
                        && this.ItemStockHasBeenCalculated == false
                        )
                   )
                    && this.DocumentType != null
                    && (this.DocumentType.ItemStockAffectionOptions == ItemStockAffectionOptions.AFFECTS
                        || this.DocumentType.ItemStockAffectionOptions == ItemStockAffectionOptions.INITIALISES
                        );
        }

        private void ArrangeCustomerBalance()
        {
            WRMApplicationSettings wrmApplicationSettings = this.Session.FindObject<WRMApplicationSettings>(null);
            if (wrmApplicationSettings.ApplicationInstance != eApplicationInstance.RETAIL
            || (wrmApplicationSettings.ApplicationInstance == eApplicationInstance.RETAIL
                  && this.DocumentSeries != null
                  && (this.DocumentSeries.eModule == eModule.HEADQUARTERS || this.DocumentSeries.eModule == eModule.ALL)
                 )
               )
            {
                this.Balance = this.DocumentTotalBalance;
            }

            if (this.Customer != null
                && this.DocumentNumber > 0
                && !this.IsCanceled
                && this.DocumentType.AffectsCustomerBalance
                && this.DocumentPaymentsAffectingCustomerBalance.Count() > 0
               )
            {
                switch (wrmApplicationSettings.ApplicationInstance)
                {
                    case eApplicationInstance.DUAL_MODE:
                        if (this.Session.IsNewObject(this) == false)
                        {
                            this.Customer.Balance += -1 * this.PreviousBalance;
                        }
                        this.Customer.Balance += this.DocumentTotalBalance;
                        this.CustomerBalanceComputedAtStoreController = true;
                        this.CustomerBalanceComputedAtHeadquarters = true;
                        break;
                    case eApplicationInstance.RETAIL:
                        if (this.Session.IsNewObject(this) == false
                            && this.ChangedMembers.Contains(this.ClassInfo.GetMember("CustomerBalanceComputedAtStoreController")) == false
                           )
                        {
                            this.Customer.Balance += -1 * this.PreviousBalance;
                        }
                        this.Customer.Balance += this.DocumentTotalBalance;
                        this.CustomerBalanceComputedAtHeadquarters = true;
                        break;
                    case eApplicationInstance.STORE_CONTROLER:
                        if (this.Session.IsNewObject(this) == false)
                        {
                            this.Customer.Balance += -1 * this.PreviousBalance;
                        }
                        this.Customer.Balance += this.DocumentTotalBalance;
                        this.CustomerBalanceComputedAtStoreController = true;
                        break;
                    default:
                        throw new NotSupportedException(String.Format("DocumentHeader.OnSaving() : Unknown Application Instance {0}", wrmApplicationSettings.ApplicationInstance));
                }
            }
        }

        private void ArrangeDocumentNumberSequenceAndCustomerPoints()
        {
            if (DocumentWillTakeNumber())
            {
                StoreDocumentSeriesType sdst = null;
                sdst = DocumentSeries.StoreDocumentSeriesTypes.Where(g => g.DocumentType.Oid == this.DocumentType.Oid).FirstOrDefault();

                if (!IsCancelingAnotherDocument && (sdst == null || DocumentSeries == null))
                {
                    throw new Exception("There is a problem with your data concerning Store's Document Type, Document Series and Document Sequence!");
                }

                if (DocumentSeries.HasAutomaticNumbering && (IsCancelingAnotherDocument || DocumentSeries.eModule != eModule.POS))
                {

                    CriteriaOperator coper = new BinaryOperator("DocumentSeries", DocumentSeries.Oid, BinaryOperatorType.Equal);
                    DocumentSequence documentSequence = DocumentSeries.DocumentSequence;

                    if (documentSequence == null)
                    {
                        documentSequence = this.Session.FindObject<DocumentSequence>(PersistentCriteriaEvaluationBehavior.InTransaction, coper);
                    }

                    if (documentSequence != null)
                    {
                        DocumentNumber = ++documentSequence.DocumentNumber;
                    }
                    else
                    {
                        documentSequence = new DocumentSequence(this.Session);
                        documentSequence.DocumentSeries = DocumentSeries;
                        DocumentNumber = documentSequence.DocumentNumber = 1;
                    }
                    FiscalDate = DateTime.Now;
                    if (this.Customer != null)
                    {
                        this.Customer.CollectedPoints += this.TotalPoints;
                        this.Customer.TotalConsumedPoints += this.ConsumedPointsForDiscount;
                        this.Customer.TotalEarnedPoints += this.TotalPoints;
                    }
                }
            }
        }

        private bool DocumentWillTakeNumber()
        {
            return Status != null && Status.TakeSequence && IsCanceled == false && DocumentNumber < 1;
        }

        private void ArrangeCompany()
        {
            if (Store != null)
            {
                this.ReferenceCompany = Store.ReferenceCompany;
                this.MainCompany = Store.Owner;
            }
            else
            {
                this.ReferenceCompany = null;
                this.MainCompany = null;
            }
        }

        /// <summary>
        /// A calculated filed returng the active document payments conserning Customer Balance 
        /// </summary>
        private IEnumerable<DocumentPayment> DocumentPaymentsAffectingCustomerBalance
        {
            get
            {
                return this.Division == eDivision.Financial
                                        ? this.DocumentPayments
                                        : this.DocumentPayments.Where(payment => payment.PaymentMethod.AffectsCustomerBalance);
            }
        }

        /// <summary>
        /// A calculated field returning the active customer balance to add or subtract from customer.
        /// ± is included in the value
        /// </summary>
        private decimal DocumentTotalBalance
        {
            get
            {
                if (this.DocumentType == null || this.DocumentType.ValueFactor == 0)
                {
                    return 0;
                }
                decimal finalBalance = 0;
                this.DocumentPaymentsAffectingCustomerBalance.ToList().ForEach(payment =>
                {
                    finalBalance += this.DocumentType.ValueFactor * payment.Amount;
                });
                return finalBalance;
            }
        }

        protected override void OnDeleting()
        {
            if (CancelsDocumentOid != null)
            {
                CancelsDocument.IsCanceled = false;
                if (CancelsDocument != null)
                {
                    foreach (TransactionCoupon transactionCoupon in CancelsDocument.TransactionCoupons)
                    {
                        transactionCoupon.IsCanceled = false;
                        if (transactionCoupon.Coupon != null)
                        {
                            transactionCoupon.Coupon.NumberOfTimesUsed++;
                        }
                        transactionCoupon.Save();
                    }
                }
                CancelsDocument.CanceledByDocumentOid = null;
            }
            if (this.Customer != null && this.IsCanceled == false && this.TotalPoints > 0)
            {
                this.Customer.CollectedPoints -= this.TotalPoints;
                this.Customer.TotalConsumedPoints -= this.ConsumedPointsForDiscount;
                this.Customer.TotalEarnedPoints -= this.TotalPoints;
            }
            ArrangeItemStockOnDeletingOrCanceling();
            base.OnDeleting();
        }

        private void ArrangeItemStockOnDeletingOrCanceling()
        {
            WRMApplicationSettings wrmApplicationSettings = this.Session.FindObject<WRMApplicationSettings>(null);
            if (wrmApplicationSettings.ApplicationInstance != eApplicationInstance.DUAL_MODE
                && wrmApplicationSettings.ApplicationInstance != eApplicationInstance.RETAIL
               )
            {
                return;
            }
            if (IsTablet())
                return;
            if (DocumentAffectsItemStock())
            {
                switch (this.DocumentType.ItemStockAffectionOptions)
                {
                    case ItemStockAffectionOptions.NO_AFFECTION:
                        throw new ArgumentException(String.Format("DocumentHeader.ArrangeItemStockOnDeleting() : cannot handle ItemStockAffectionOptions ({0})", this.DocumentType.ItemStockAffectionOptions));
                    case ItemStockAffectionOptions.AFFECTS:
                        ArrangeItemStockForAffectsOptionOnDeletingOrCanceling();
                        break;
                    case ItemStockAffectionOptions.INITIALISES:
                        ArrangeItemStockForInitialisesOptionOnDeletingOrCanceling();
                        break;
                    default:
                        throw new ArgumentException(String.Format("DocumentHeader.ArrangeItemStockOnDeleting() : cannot handle ItemStockAffectionOptions ({0})", this.DocumentType.ItemStockAffectionOptions));
                }
            }
        }

        private void ArrangeItemStockForInitialisesOptionOnDeletingOrCanceling()
        {
            if (IsTablet())
                return;
            foreach (Item item in this.DocumentDetails.Select(document => document.Item).Distinct())
            {
                ItemStock itemStock = GetExistingOrNewItemStock(item);

                if (itemStock.LastDocumentHeaderInventory == null)
                {
                    itemStock.Stock = item.RecalculateItemStockForItemWithoutInventory(this);
                }
                else
                {
                    DocumentHeader lastDocumentHeaderInventory = itemStock.LastDocumentHeaderInventory;
                    if (itemStock.LastDocumentHeaderInventory.Oid == this.Oid)
                    {
                        DocumentHeader itemLastDocumentHeaderInventoryDocumentDetail = item.LastInventoryDocumentHeader(this);
                        if (itemLastDocumentHeaderInventoryDocumentDetail != null)
                        {
                            lastDocumentHeaderInventory = itemLastDocumentHeaderInventoryDocumentDetail;
                            itemStock.LastDocumentHeaderInventory = lastDocumentHeaderInventory;
                        }
                    }
                    if (lastDocumentHeaderInventory == null || itemStock.LastDocumentHeaderInventory.Oid == this.Oid)
                    {
                        itemStock.Stock = item.RecalculateItemStockForItemWithoutInventory(this);
                    }
                    else
                    {
                        itemStock.Stock = item.RecalculateItemStockAfterInventory(itemStock.LastDocumentHeaderInventory);
                    }
                }
                itemStock.Save();
            }
            this.ItemStockHasBeenCalculated = true;
        }

        private void ArrangeItemStockForAffectsOptionOnDeletingOrCanceling()
        {
            if (IsTablet())
                return;
            foreach (Item item in this.DocumentDetails.Select(document => document.Item).Distinct())
            {
                ItemStock itemStock = GetExistingOrNewItemStock(item);

                if (itemStock.LastDocumentHeaderInventory == null)
                {
                    itemStock.Stock += -1 * this.DocumentType.QuantityFactor * this.DocumentDetails
                                                                                   .Where(documentLine => documentLine.Item.Oid == item.Oid)
                                                                                   .Sum(documentLine => documentLine.BaseQuantity);
                }
                else
                {
                    itemStock.Stock = item.RecalculateItemStockAfterInventory(itemStock.LastDocumentHeaderInventory);
                }
                itemStock.Save();
            }
            this.ItemStockHasBeenCalculated = true;
        }

        public StoreDocumentSeriesType StoreDocumentSeriesType
        {
            get
            {
                if (DocumentType == null || DocumentSeries == null)
                {
                    return null;
                }
                CriteriaOperator crop = CriteriaOperator.And(
                                                             new BinaryOperator("DocumentType", DocumentType.Oid),
                                                             new BinaryOperator("DocumentSeries", DocumentSeries.Oid)
                                                         );
                return Session.FindObject<StoreDocumentSeriesType>(crop);
            }
        }


        [Persistent("CanceledByDocument")]
        public Guid? CanceledByDocumentOid
        {
            get
            {
                return _CanceledByDocumentOid;
            }
            set
            {
                SetPropertyValue("CanceledByDocumentOid", ref _CanceledByDocumentOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public DocumentHeader CanceledByDocument
        {
            get
            {
                return this.Session.FindObject<DocumentHeader>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.CanceledByDocumentOid));
            }
            set
            {
                if (value == null)
                {
                    CanceledByDocumentOid = null;
                }
                else
                {
                    CanceledByDocumentOid = value.Oid;
                }
            }
        }




        [Persistent("CancelsDocument")]
        public Guid? CancelsDocumentOid
        {
            get
            {
                return _CancelsDocumentOid;
            }
            set
            {
                SetPropertyValue("CancelsDocumentOid", ref _CancelsDocumentOid, value);
            }
        }

        public bool IsCancelingAnotherDocument
        {
            get
            {
                return this.CancelsDocumentOid != null && this.CancelsDocumentOid.Value != Guid.Empty;
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public DocumentHeader CancelsDocument
        {
            get
            {
                return this.Session.FindObject<DocumentHeader>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.CancelsDocumentOid));
            }
            set
            {
                if (value == null)
                {
                    CancelsDocumentOid = null;
                }
                else
                {
                    CancelsDocumentOid = value.Oid;
                }
            }
        }


        public PriceCatalog PriceCatalog
        {
            get
            {
                return _PriceCatalog;
            }
            set
            {
                SetPropertyValue("PriceCatalog", ref _PriceCatalog, value);
            }
        }

        [Association("PriceCatalogPolicy-DocumentHeaders")]
        public PriceCatalogPolicy PriceCatalogPolicy
        {
            get
            {
                return _PriceCatalogPolicy;
            }
            set
            {
                this._EffectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy();
                SetPropertyValue("PriceCatalogPolicy", ref _PriceCatalogPolicy, value);
            }
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);

            if (includeDetails)
            {
                dictionary.Add("DocumentDetails", DocumentDetails.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("DocumentPayments", DocumentPayments.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("ReferencedDocuments", ReferencedDocuments.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("DocumentPromotions", DocumentPromotions.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("DocumentPaymentsEdps", DocumentPaymentsEdps.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("TransactionCoupons", TransactionCoupons.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }
            return dictionary;
        }

        public decimal TotalVatAmountBeforeDiscount
        {
            get
            {
                return _TotalVatAmountBeforeDiscount;
            }
            set
            {
                SetPropertyValue("TotalVatAmountBeforeDiscount", ref _TotalVatAmountBeforeDiscount, value);
            }
        }

        public bool IsShiftStartingAmount
        {
            get
            {
                return _IsShiftStartingAmount;
            }
            set
            {
                SetPropertyValue("IsShiftStartingAmount", ref _IsShiftStartingAmount, value);
            }
        }

        public bool IsDayStartingAmount
        {
            get
            {
                return _IsDayStartingAmount;
            }
            set
            {
                SetPropertyValue("IsDayStartingAmount", ref _IsDayStartingAmount, value);
            }
        }

        public decimal GrossTotalBeforeDiscount
        {
            get
            {
                return _GrossTotalBeforeDiscount;
            }
            set
            {
                SetPropertyValue("GrossTotalBeforeDiscount", ref _GrossTotalBeforeDiscount, value);
            }
        }

        public decimal GrossTotalBeforeDocumentDiscount
        {
            get
            {
                return _GrossTotalBeforeDocumentDiscount;
            }
            set
            {
                SetPropertyValue("GrossTotalBeforeDocumentDiscount", ref _GrossTotalBeforeDocumentDiscount, value);
            }
        }

        public decimal NetTotalBeforeDiscount
        {
            get
            {
                return _NetTotalBeforeDiscount;
            }
            set
            {
                SetPropertyValue("NetTotalBeforeDiscount", ref _NetTotalBeforeDiscount, value);
            }
        }


        public DateTime RefferenceDate
        {
            get
            {
                return _RefferenceDate;
            }
            set
            {
                SetPropertyValue("RefferenceDate", ref _RefferenceDate, value);
            }
        }


        public Address BillingAddress
        {
            get
            {
                return _BillingAddress;
            }
            set
            {
                SetPropertyValue("BillingAddress", ref _BillingAddress, value);
            }
        }

        public bool HasBeenTransformed
        {
            get
            {
                return DerivedDocuments.Count > 0;
            }
        }

        public UserDailyTotals UserDailyTotals
        {
            get
            {
                return _UserDailyTotals;
            }
            set
            {
                SetPropertyValue("UserDailyTotals", ref _UserDailyTotals, value);
            }
        }

        public eTransformationStatus TransformationStatus
        {
            get
            {
                if (this.IsCanceled || this.IsCancelingAnotherDocument)
                {
                    return eTransformationStatus.CANNOT_BE_TRANSFORMED;
                }

                if (DerivedDocuments.FirstOrDefault(derDoc => !derDoc.DerivedDocument.IsCanceled) == null)
                {
                    return eTransformationStatus.NOT_TRANSFORMED;
                }

                if (UnreferencedDetails.FirstOrDefault() != null)
                {
                    return eTransformationStatus.PARTIALLY_TRANSFORMED;
                }

                return eTransformationStatus.FULLY_TRANSFORMED;
            }
        }

        public decimal TotalQty
        {
            get
            {
                return _TotalQty;
            }
            set
            {
                SetPropertyValue("TotalQty", ref _TotalQty, value);
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

        }

        public string DocumentShortDescription
        {
            get
            {
                string descr = "";

                if (DocumentType != null)
                {
                    descr += DocumentType.Code + " ";
                }

                if (DocumentSeries != null)
                {
                    descr += DocumentSeries.Description + " ";
                }

                descr += FinalizedDate.ToShortDateString();

                return descr;
            }
        }

        public bool DisplayMarkUpForm
        {
            get
            {
                return DocumentType != null && DocumentType.Division.Section == eDivision.Purchase && DocumentType.DisplaysMarkUpForm;
            }
        }

        public decimal TotalPoints
        {
            get
            {
                return _TotalPoints;
            }
            set
            {
                SetPropertyValue("TotalPoints", ref _TotalPoints, value);
            }
        }

        public decimal DocumentPoints
        {
            get
            {
                return _DocumentPoints;
            }
            set
            {
                SetPropertyValue("DocumentPoints", ref _DocumentPoints, value);
            }
        }


        public DateTime? ExecutionDate
        {
            get
            {
                return _ExecutionDate;
            }
            set
            {
                SetPropertyValue("ExecutionDate", ref _ExecutionDate, value);
            }
        }

        public bool UsesPackingQuantities
        {
            get
            {
                if (this.DocumentType != null && this.DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING)
                {
                    return true;
                }
                return false;
            }
        }

        public eTransformationLevel TransformationLevel
        {
            get
            {
                return _TransformationLevel;
            }
            set
            {
                SetPropertyValue("TransformationLevel", ref _TransformationLevel, value);
            }
        }

        public IEnumerable<DocumentDetail> SumarisableDocumentDetails
        {
            get
            {
                return this.DocumentDetails == null ? null : this.DocumentDetails.Where(documentDetail => documentDetail.ShouldBeSummarised);
            }
        }

        public IEnumerable<DocumentDetail> TransformableDocumentDetails
        {
            get
            {
                return this.DocumentDetails == null ? null : this.DocumentDetails.Where(documentDetail => documentDetail.CanBeTransformed);
            }
        }

        public DocumentSource Source
        {
            get
            {
                return _Source;
            }
            set
            {
                SetPropertyValue("Source", ref _Source, value);
            }
        }

        public int AproximateDocumentNumber
        {
            get
            {
                if (DocumentNumber > 0)
                {
                    return DocumentNumber;
                }

                if (DocumentType != null && DocumentSeries != null && Status != null)
                {
                    CriteriaOperator coper = new BinaryOperator("DocumentSeries", DocumentSeries.Oid, BinaryOperatorType.Equal);
                    DocumentSequence documentSequence = this.Session.FindObject<DocumentSequence>(coper);
                    if (documentSequence != null)
                    {
                        return documentSequence.DocumentNumber + 1;
                    }
                }
                return 0;
            }
        }

        public Customer TriangularCustomer
        {
            get
            {
                return _TriangularCustomer;
            }
            set
            {
                SetPropertyValue("TriangularCustomer", ref _TriangularCustomer, value);
            }
        }

        public SupplierNew TriangularSupplier
        {
            get
            {
                return _TriangularSupplier;
            }
            set
            {
                SetPropertyValue("TriangularSupplier", ref _TriangularSupplier, value);
            }
        }

        public Store TriangularStore
        {
            get
            {
                return _TriangularStore;
            }
            set
            {
                SetPropertyValue("TriangularStore", ref _TriangularStore, value);
            }
        }

        public string TriangularAddress
        {
            get
            {
                return _TriangularAddress;
            }
            set
            {
                SetPropertyValue("TriangularAddress", ref _TriangularAddress, value);
            }
        }


        public Store SecondaryStore
        {
            get
            {
                return _SecondaryStore;
            }
            set
            {
                SetPropertyValue("SecondaryStore", ref _SecondaryStore, value);
            }
        }


        public bool HasBeenOnHold
        {
            get
            {
                return _HasBeenOnHold;
            }
            set
            {
                SetPropertyValue("HasBeenOnHold", ref _HasBeenOnHold, value);
            }
        }

        public decimal TotalAmountInPayments
        {
            get
            {
                return this.DocumentPayments.Sum(x => x.Amount);
            }
        }



        public decimal RemainingPayment
        {
            get
            {
                return this.GrossTotal - TotalAmountInPayments;
            }
        }

        [Association("DocumentHeader-Edps")]
        public XPCollection<DocumentPaymentEdps> DocumentPaymentsEdps
        {
            get
            {
                return GetCollection<DocumentPaymentEdps>("DocumentPaymentsEdps");
            }
        }

        [Aggregated, Association("DocumentHeader-TransactionCoupons")]
        public XPCollection<TransactionCoupon> TransactionCoupons
        {
            get
            {
                return GetCollection<TransactionCoupon>("TransactionCoupons");
            }
        }

        public void RefreshNonPersistant()
        {
            this.OnChanged("TotalAmountInPayments");
            this.OnChanged("RemainingPayment");
            this.OnChanged("PrintedVat");
            this.OnChanged("PrintedPayments");
            this.OnChanged("PrintedPhone");
            this.OnChanged("PrintedTaxOffice");
            this.OnChanged("PrintedBalance");
            this.OnChanged("PrintedPreviousBalance");
        }
        public bool CouponsHaveBeenUpdatedOnStoreController
        {
            get
            {
                return _CouponsHaveBeenUpdatedOnStoreController;
            }
            set
            {
                SetPropertyValue("CouponsHaveBeenUpdatedOnStoreController", ref _CouponsHaveBeenUpdatedOnStoreController, value);
            }
        }

        public bool CouponsHaveBeenUpdatedOnMaster
        {
            get
            {
                return _CouponsHaveBeenUpdatedOnMaster;
            }
            set
            {
                SetPropertyValue("CouponsHaveBeenUpdatedOnMaster", ref _CouponsHaveBeenUpdatedOnMaster, value);
            }
        }

        public Guid CustomerOid
        {
            get
            {
                if (this.Customer == null)
                {
                    return Guid.Empty;
                }
                return this.Customer.Oid;
            }
        }

        public IEnumerable<IDocumentDetail> DiscountableDocumentDetails()
        {
            return this.DocumentDetails.Where(documentDetail => documentDetail.IsCanceled == false && documentDetail.IsReturn == false && documentDetail.IsTax == false && documentDetail.DoesNotAllowDiscount == false);
        }

        public DiscountType PromotionsDocumentDiscountType
        {
            get
            {
                return _PromotionsDocumentDiscountType;
            }
            set
            {
                SetPropertyValue("PromotionsDocumentDiscountType", ref _PromotionsDocumentDiscountType, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string ProcessedDenormalizedCustomer
        {
            get
            {
                return _ProcessedDenormalizedCustomer;
            }
            set
            {
                SetPropertyValue("ProcessedDenormalizedCustomer", ref _ProcessedDenormalizedCustomer, value);
            }
        }


        [Size(SizeAttribute.Unlimited)]
        public string DenormalizedCustomer
        {
            get
            {
                return _DenormalizedCustomer;
            }
            set
            {
                SetPropertyValue("DenormalizedCustomer", ref _DenormalizedCustomer, value);
                if (string.IsNullOrWhiteSpace(value) == false)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            this.CustomerViewModel = null;
                        }
                        else
                        {
                            InsertedCustomerViewModel insertedCustomerViewModel = JsonConvert.DeserializeObject<InsertedCustomerViewModel>(value);
                            this.DenormalisedAddress = insertedCustomerViewModel.AddressOid;
                            this.CustomerViewModel = insertedCustomerViewModel;
                        }
                    }
                    catch (Exception exception)
                    {
                        string error = exception.GetFullMessage();
                    }
                }
            }
        }



        [NonPersistent]
        public string PrintedPhone { get; set; }
        [NonPersistent]
        public string PrintedTaxOffice { get; set; }


        [NonPersistent]
        public string PrintedVat
        {
            get
            {
                return this.DocumentDetails.Where(x => x.IsCanceled == false).GroupBy(d => d.VatFactor).OrderBy(x => x.Key)
                  .ToDictionary(x => x.Key, x => new
                  {
                      Vat = x.Sum(y => y.TotalVatAmount),
                      Net = x.Sum(y => y.NetTotal)
                  }).Select(x => String.Format("{0,-8:p2}{2,12:n2}{1,12:d2}", x.Key, (x.Value.Vat + "€"), (x.Value.Net + "€")))
                            .Aggregate((a, b) => String.Format("{0}\n{1}", a, b));
            }
        }

        [NonPersistent]
        [JsonIgnore]
        [UpdaterIgnoreField]
        public string PrintedPayments
        {
            get
            {
                try
                {
                    string payments = string.Empty;
                    var dict = this.DocumentPayments.GroupBy(d => d.PaymentMethod.Code).OrderBy(x => x.Key)
                           .ToDictionary(x => x.Key, x => new
                           {
                               Desc = x.FirstOrDefault().PaymentMethod.Description,
                               Amount = x.Sum(y => y.Amount)
                           });

                    foreach (var payment in dict)
                    {
                        int pad = 40 - payment.Value.Desc.Length - (8 - payment.Value.Amount.ToString().Length);
                        payments += payment.Value.Desc.PadRight(pad) + payment.Value.Amount.ToString("F") + "€" + Environment.NewLine;
                    }
                    return payments;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        [NonPersistent]
        public string PrintedPreviousBalance { get; set; }


        [NonPersistent]
        public string PrintedBalance { get; set; }


        [NonPersistent]
        public InsertedCustomerViewModel CustomerViewModel
        {
            get
            {
                return _CustomerViewModel;
            }
            set
            {
                SetPropertyValue("CustomerViewModel", ref _CustomerViewModel, value);
            }
        }

        [NonPersistent]
        public Guid PromotionsDocumentDiscountTypeOid
        {
            get
            {
                if (this.PromotionsDocumentDiscountType == null)
                {
                    return Guid.Empty;
                }
                return this.PromotionsDocumentDiscountType.Oid;
            }
            set
            {
                if (value == null || value == Guid.Empty)
                {
                    this.PromotionsDocumentDiscountType = null;
                }
                Guid? guid = value as Guid?;
                if (!guid.HasValue)
                {
                    this.PromotionsDocumentDiscountType = null;
                }
                this.PromotionsDocumentDiscountType = this.Session.GetObjectByKey<DiscountType>(guid.Value);
            }
        }

        IEnumerable<IDocumentDetail> IDocumentHeader.DocumentDetails
        {
            get { return DocumentDetails; }
        }

        IEnumerable<IDocumentDetail> IDocumentHeader.DiscountableDocumentDetails()
        {
            return DiscountableDocumentDetails();
        }

        IEnumerable<IDocumentPromotion> IDocumentHeader.DocumentPromotions
        {
            get { return DocumentPromotions; }
        }

        /// <summary>
        /// Checks if the relevant ActionTypes should be executed. Returns true if so, otherwise else.
        /// </summary>
        public static bool ShouldExecuteActionTypes(BasicObj obj, ActionTypeEntity actionTypeEntity)
        {
            PropertyInfo property = obj.GetType().GetProperty("Status");
            if (property != null)
            {
                DocumentStatus docStatus = property.GetValue(obj, null) as DocumentStatus;
                return actionTypeEntity.ActionTypeDocStatuses.Select(x => x.DocumentStatus).Contains(docStatus);
            }

            return false;
        }

        public static CriteriaOperator RecalculateActionCriteria()
        {
            return new BinaryOperator("DocNumber", 0, BinaryOperatorType.Greater);
        }

        public XPCollection<ActionTypeEntity> ActionTypeEntities
        {
            get
            {
                if (DocumentType == null)
                {
                    return null;
                }
                return DocumentType.ActionTypeEntities;
            }
        }


        public InsertedCustomerViewModel PrintedCustomer
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(this.DenormalizedCustomer) && Customer != null)
                    {
                        return Customer.GetInsertedCustomerViewModel(this.BillingAddress);
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<InsertedCustomerViewModel>(this.DenormalizedCustomer);
                    }
                }
                catch (Exception exception)
                {
                    string exceptionMessage = exception.GetFullMessage();
                    return null;
                }
            }
        }

        public Guid? DenormalisedAddress
        {
            get
            {
                return _DenormalisedAddress;
            }
            set
            {
                SetPropertyValue("DenormalisedAddress", ref _DenormalisedAddress, value);
            }
        }



        public string AddressProfession
        {
            get
            {
                return _AddressProfession;
            }
            set
            {
                SetPropertyValue("AddressProfession", ref _AddressProfession, value);
            }
        }

        public string CreatedByDeviceName
        {
            get
            {
                string posName = string.Empty;
                if (!string.IsNullOrEmpty(this.CreatedByDevice))
                {
                    try
                    {
                        POS pos = this.Session.GetObjectByKey<POS>(Guid.Parse(this.CreatedByDevice));
                        posName = pos?.Name ?? "";

                    }
                    catch (Exception exception)
                    {
                        string error = exception.Message;
                    }

                }
                return posName;
            }
        }

        public string UpdatedByDeviceName
        {
            get
            {
                string posName = string.Empty;
                if (!string.IsNullOrEmpty(this.UpdateByDevice))
                {
                    try
                    {
                        POS pos = this.Session.GetObjectByKey<POS>(Guid.Parse(this.UpdateByDevice));
                        posName = pos.Name;

                    }
                    catch (Exception exception)
                    {
                        string error = exception.Message;
                    }

                }
                return posName;
            }
        }

        public string DerivedFrom
        {
            get
            {
                string derivedFrom = string.Empty;
                foreach (RelativeDocument relativeDocument in this.ReferencedDocuments)
                {
                    derivedFrom += relativeDocument.InitialDocument.ToString() + Environment.NewLine;
                }
                return derivedFrom;
            }
        }

        public string TransformedTo
        {
            get
            {
                string transfromedTo = string.Empty;
                foreach (RelativeDocument relativeDocument in this.DerivedDocuments)
                {
                    transfromedTo += relativeDocument.DerivedDocument.ToString() + Environment.NewLine;
                }
                return transfromedTo;
            }
        }

        private EffectivePriceCatalogPolicy _EffectivePriceCatalogPolicy;
        private CompanyNew _ReferenceCompany;
        private CompanyNew _MainCompany;
        private string _DeliveryProfession;
        private string _TriangularProfession;
        private decimal _Balance;
        private decimal _PreviousBalance;
        private bool _ItemStockHasBeenCalculated;

        /// <summary>
        /// It indicates whether current PriceCatalogPolicyDetail is applicable or not depending on Customer and Store common PriceCatalogs
        /// </summary>
        public EffectivePriceCatalogPolicy EffectivePriceCatalogPolicy
        {
            get
            {
                if (this._EffectivePriceCatalogPolicy.HasPolicyDetails() == false && this.Store != null && this.PriceCatalogPolicy != null)
                {
                    this._EffectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(this.Store, this.Customer);
                }
                return _EffectivePriceCatalogPolicy;
            }
        }


        public eDocumentTraderType TraderType
        {
            get
            {
                switch (this.Division)
                {
                    case eDivision.Financial:
                        return this.DocumentType.TraderType;
                    case eDivision.Other:
                        throw new NotSupportedException(String.Format("TraderType for Division {0}", this.Division));
                    case eDivision.Purchase:
                        return eDocumentTraderType.CUSTOMER;
                    case eDivision.Sales:
                        return eDocumentTraderType.SUPPLIER;
                    case eDivision.Store:
                        return eDocumentTraderType.STORE;
                    default:
                        throw new NotSupportedException(String.Format("TraderType for Division {0}", this.Division));
                }
            }
        }

        public List<DocumentDetail> MainLines
        {
            get
            {
                return this.DocumentDetails.Where<DocumentDetail>(detail => detail.IsLinkedLine == false).ToList();
            }
        }

        public List<DocumentDetail> LinkedLines
        {
            get
            {
                return this.DocumentDetails.Where<DocumentDetail>(detail => detail.IsLinkedLine).ToList();
            }
        }

        public bool CustomerBalanceComputedAtStoreController
        {
            get
            {
                return _CustomerBalanceComputedAtStoreController;
            }
            set
            {
                SetPropertyValue("CustomerBalanceComputedAtStoreController", ref _CustomerBalanceComputedAtStoreController, value);
            }
        }

        public bool CustomerBalanceComputedAtHeadquarters
        {
            get
            {
                return _CustomerBalanceComputedAtHeadquarters;
            }
            set
            {
                SetPropertyValue("CustomerBalanceComputedAtHeadquarters", ref _CustomerBalanceComputedAtHeadquarters, value);
            }
        }

        public string DeliveryProfession
        {
            get
            {
                return _DeliveryProfession;
            }
            set
            {
                SetPropertyValue("DeliveryProfession", ref _DeliveryProfession, value);
            }
        }

        public string TriangularProfession
        {
            get
            {
                return _TriangularProfession;
            }
            set
            {
                SetPropertyValue("TriangularProfession", ref _TriangularProfession, value);
            }
        }

        public decimal Balance
        {
            get
            {
                return _Balance;
            }
            set
            {
                this.PreviousBalance = this._Balance;
                SetPropertyValue("Balance", ref _Balance, value);
            }
        }

        public decimal PreviousBalance
        {
            get
            {
                return _PreviousBalance;
            }
            set
            {
                SetPropertyValue("PreviousBalance", ref _PreviousBalance, value);
            }
        }


        public bool ItemStockHasBeenCalculated
        {
            get
            {
                return _ItemStockHasBeenCalculated;
            }
            set
            {
                SetPropertyValue("ItemStockHasBeenCalculated", ref _ItemStockHasBeenCalculated, value);
            }
        }

        public User ChargedToUser
        {
            get
            {
                return _ChargedToUser;
            }
            set
            {
                SetPropertyValue("ChargedToUser", ref _ChargedToUser, value);
            }
        }
    }
}
