using DevExpress.Xpo;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Enumerations.ViewModel;
using ITS.Retail.Platform.Kernel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.POS.Model.Transactions
{
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
        }

        private bool _DocumentOnHold;
        private DateTime _RefferenceDate;
        private string _PriceCatalogCode;
        private Guid _PriceCatalog;
        private string _DocumentStatusCode;
        private string _DeliveryToTraderTaxCode;
        private string _CustomerCode;
        private string _StoreCode;
        private string _DocumentSeriesCode;
        private string _DocumentTypeCode;
        private int _POSID;
        private bool _CheckFromStore;
        private bool _HasBeenChecked;
        private bool _HasBeenExecuted;
        private string _Signature;
        private bool _IsNewRecord;
        private decimal _GrossTotal;
        private decimal _TotalVatAmount;
        private decimal _NetTotal;
        private UserDailyTotals _UserDailyTotals;
        private bool _IsOpen;
        private Guid _BillingAddress;
        private bool _IsFiscalPrinterHandled;
        private decimal _TotalQty;
        private bool _HasPaymentWithRatification;
        private decimal _TotalVatAmountBeforeDiscount;
        private decimal _GrossTotalBeforeDiscount;
        private decimal _NetTotalBeforeDiscount;
        private Guid _DocumentDiscountType;
        private decimal _TotalPoints;
        private decimal _GrossTotalBeforeDocumentDiscount;
        private decimal _DocumentPoints;
        private int _FiscalPrinterNumber;

        private decimal _DocumentDiscountPercentage;
        private decimal _DocumentDiscountPercentagePerLine;

        private DateTime _FinalizedDate;
        private Guid _Status;
        private string _Remarks;
        private bool _IsCanceled;
        private bool _IsAddedToTotals;
        private decimal _PromotionPoints;
        private string _CustomerName;
        private Guid _PromotionsDocumentDiscountType;
        private DocumentSource _Source;
        private bool _IsDayStartingAmount;
        private bool _IsShiftStartingAmount;
        private string _CustomerLookupCode;
        private bool _CustomerNotFound;
        private bool _InEmulationMode;
        private bool _HasBeenOnHold;
        private bool _CouponsHaveBeenUpdatedOnStoreController;
        private bool _CouponsHaveBeenUpdatedOnMaster;
        private string _DenormalizedCustomer;

        private Guid _Customer;
        private Guid _DeliveryTo;
        private string _DeliveryAddress;
        private double _VatAmount1;
        private double _VatAmount2;
        private double _VatAmount3;
        private double _VatAmount4;
        private decimal _TotalDiscountAmount;
        [Persistent("Change")]
        private decimal _Change;

        private decimal _DocumentDiscountAmount;
        private decimal _ConsumedPointsForDiscount;
        private decimal _PointsDiscountPercentage;
        private decimal _PointsDiscountPercentagePerLine;
        private decimal _PointsDiscountAmount;
        private decimal _PromotionsDiscountPercentage;
        private decimal _PromotionsDiscountPercentagePerLine;
        private decimal _PromotionsDiscountAmount;
        private decimal _CustomerDiscountPercentagePerLine;
        private decimal _CustomerDiscountAmount;
        private decimal _DefaultDocumentDiscountPercentagePerLine;
        private decimal _DefaultDocumentDiscountAmount;
        private Guid _POS;
        private DateTime _FiscalDate;
        private Guid _DocumentType;
        private Guid _DocumentSeries;
        private int _DocumentNumber;
        private Guid _Store;
        private eDivision _Division;
        private string _CustomerLookUpTaxCode;
        private Guid? _DenormalisedAddress;
        private string _AddressProfession;
        private Guid _PriceCatalogPolicy;
        private Guid _ReferenceCompany;
        private Guid _MainCompany;

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

        /// <summary>
        /// The actual percentage that is applied to the lines
        /// </summary>
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

        public Guid PromotionsDocumentDiscountType
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

        public Guid DocumentDiscountType
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

        public Guid BillingAddress
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


        [NonPersistent]
        public bool IsNewRecord
        {
            get
            {
                return _IsNewRecord;
            }
        }


        [DenormalizedField("POSID", typeof(Settings.POS), "ITS.Retail.Model.POS", "ID")]
        public Guid POS
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

        [DenormalizedField("DocumentTypeCode", typeof(DocumentType), "ITS.Retail.Model.DocumentType", "Code")]
        public Guid DocumentType
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


        [DenormalizedField("DocumentSeriesCode", typeof(DocumentSeries), "ITS.Retail.Model.DocumentSeries", "Code")]
        public Guid DocumentSeries
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



        [DenormalizedField("StoreCode", typeof(Store), "ITS.Retail.Model.Store", "Code")]
        public Guid Store
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



        [DenormalizedField("CustomerCode", typeof(Customer), "ITS.Retail.Model.Customer", "Code")]
        public Guid Customer
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

        public string CustomerDisplayName
        {
            get
            {
                return this.CustomerNotFound ? this.CustomerLookupCode : this.CustomerName;
            }
        }



        [DenormalizedField("DeliveryToTraderTaxCode", typeof(Trader), "ITS.Retail.Model.Trader", "TaxCode")]
        public Guid DeliveryTo
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


        public double VatAmount1
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


        public double VatAmount2
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


        public double VatAmount3
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


        public double VatAmount4
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




        [PersistentAlias("_Change")]
        public decimal Change
        {
            get
            {
                decimal amountPaid = DocumentPayments.Where(documentPayment => documentPayment.Amount >= 0).Sum(documentPayment => documentPayment.Amount);
                decimal change = amountPaid - this.GrossTotal;
                if (change < 0)
                {
                    change = 0;
                }
                _Change = change;
                return _Change;
            }
        }


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


        [DenormalizedField("DocumentStatusCode", typeof(DocumentStatus), "ITS.Retail.Model.DocumentStatusCode", "Code")]
        public Guid Status
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



        public bool IsAddedToTotals
        {
            get
            {
                return _IsAddedToTotals;
            }
            set
            {
                SetPropertyValue("IsAddedToTotals", ref _IsAddedToTotals, value);
            }
        }



        [Indexed("GCRecord", Unique = false)]
        public bool IsOpen
        {
            get
            {
                return _IsOpen;
            }
            set
            {
                SetPropertyValue("IsOpen", ref _IsOpen, value);
            }
        }

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

        /// <summary>
        /// Gets the Gross Total without including the document discount
        /// </summary>
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


        [Aggregated, Association("DocumentHeader-DocumentDetails")]
        public XPCollection<DocumentDetail> DocumentDetails
        {
            get
            {
                return GetCollection<DocumentDetail>("DocumentDetails");
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

        public IEnumerable<DocumentDetail> DiscountableDocumentDetails()
        {
            return this.DocumentDetails.Where(x => x.IsCanceled == false && x.IsReturn == false && x.IsTax == false && x.DoesNotAllowDiscount == false);
        }

        [Aggregated, Association("DocumentHeader-Payments")]
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


        public string PriceCatalogCode
        {
            get
            {
                return _PriceCatalogCode;
            }
            set
            {
                SetPropertyValue("PriceCatalogCode", ref _PriceCatalogCode, value);
            }
        }

        [DenormalizedField("PriceCatalogCode", typeof(PriceCatalog), "ITS.Retail.Model.PriceCatalog", "Code")]
        public Guid PriceCatalog
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

        public Guid PriceCatalogPolicy
        {
            get
            {
                return _PriceCatalogPolicy;
            }
            set
            {
                SetPropertyValue("PriceCatalogPolicy", ref _PriceCatalogPolicy, value);
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

        public bool DocumentOnHold
        {
            get
            {
                return _DocumentOnHold;
            }
            set
            {
                SetPropertyValue("DocumentOnHold", ref _DocumentOnHold, value);
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

        /// <summary>
        /// Calculated. Gets all the discounts that must be shown as "Document Header Discounts".
        /// </summary>
        public decimal AllDocumentHeaderDiscounts
        {
            get
            {
                return this.PointsDiscountAmount + this.DocumentDiscountAmount + this.PromotionsDiscountAmount; //+ this.CustomerDiscountAmount todo
            }
        }


        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType);
            dictionary.Add("DocumentDetails", DocumentDetails.Select(g => g.GetDict(settings, includeType)).ToList());
            dictionary.Add("DocumentPayments", DocumentPayments.Select(g => g.GetDict(settings, includeType)).ToList());
            dictionary.Add("DocumentPromotions", DocumentPromotions.Select(g => g.GetDict(settings, includeType)).ToList());
            dictionary.Add("DocumentPaymentsEdps", DocumentPaymentsEdps.Select(g => g.GetDict(settings, includeType)).ToList());
            dictionary.Add("DocumentPaymentsCardlink", DocumentPaymentsCardlink.Select(g => g.GetDict(settings, includeType)).ToList());
            dictionary.Add("TransactionCoupons", TransactionCoupons.Select(g => g.GetDict(settings, includeType)).ToList());
            return dictionary;
        }


        IEnumerable<IDocumentPromotion> IDocumentHeader.DocumentPromotions
        {
            get { return DocumentPromotions; }
        }

        IEnumerable<IDocumentDetail> IDocumentHeader.DocumentDetails
        {
            get { return DocumentDetails; }
        }

        [NonPersistent]
        public Guid PromotionsDocumentDiscountTypeOid
        {
            get
            {
                return PromotionsDocumentDiscountType;
            }
            set
            {
                PromotionsDocumentDiscountType = value;
            }
        }

        IEnumerable<IDocumentDetail> IDocumentHeader.DiscountableDocumentDetails()
        {
            return this.DiscountableDocumentDetails();
        }

        public Guid CustomerOid
        {
            get { return Customer; }
        }

        [Association("DocumentHeader-Edps")]
        public XPCollection<DocumentPaymentEdps> DocumentPaymentsEdps
        {
            get
            {
                return GetCollection<DocumentPaymentEdps>("DocumentPaymentsEdps");
            }
        }

        [Association("DocumentHeader-Cardlink")]
        public XPCollection<DocumentPaymentCardlink> DocumentPaymentsCardlink
        {
            get
            {
                return GetCollection<DocumentPaymentCardlink>("DocumentPaymentsCardlink");
            }
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
            }
        }


        [NonPersistent]
        public InsertedCustomerViewModel CustomerViewModel
        {
            get
            {
                return String.IsNullOrEmpty(this.DenormalizedCustomer) ? null : JsonConvert.DeserializeObject<InsertedCustomerViewModel>(this.DenormalizedCustomer);
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

        public Guid ReferenceCompany
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


        public Guid MainCompany
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

        /// <summary>
        /// On Saving Get next document number
        /// </summary>
        protected override void OnSaving()
        {
            if (this.IsFiscalPrinterHandled && FiscalPrinterNumber > 0)
            {
                DocumentNumber = FiscalPrinterNumber;
            }
            base.OnSaving();
        }
    }
}
