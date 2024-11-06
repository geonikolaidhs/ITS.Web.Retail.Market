using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;

namespace ITS.POS.Model.Settings
{
    public class DocumentType : Lookup2Fields
    {
        public DocumentType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentType(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public DocumentType(string code, string description)
            : base(code, description)
        {
        }
        public DocumentType(Session session, string code, string description)
            : base(session, code, description)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public bool SupportLoyalty
        {
            get
            {
                return _SupportLoyalty;
            }
            set
            {
                SetPropertyValue("SupportLoyalty", ref _SupportLoyalty, value);
            }
        }

        private bool _AllowItemZeroPrices;
        private Guid _MinistryDocumentType;
        private bool _MergedSameDocumentLines;
        private bool _IsForWholesale;
        private int _QuantityFactor;
        private bool _IncreaseVatAndSales;
        private bool _JoinInTotalizers;
        private bool _DisplayInCashCount;
        private bool _SupportCustomerVatLevel;


        public int QuantityFactor
        {
            get
            {
                return _QuantityFactor;
            }
            set
            {
                SetPropertyValue("QuantityFactor", ref _QuantityFactor, value);
            }
        }

        private int _ValueFactor;
        public int ValueFactor
        {
            get
            {
                return _ValueFactor;
            }
            set
            {
                SetPropertyValue("ValueFactor", ref _ValueFactor, value);
            }
        }

        public bool IncreaseVatAndSales
        {
            get
            {
                return _IncreaseVatAndSales;
            }
            set
            {
                SetPropertyValue("IncreaseVatAndSales", ref _IncreaseVatAndSales, value);
            }
        }


        public bool JoinInTotalizers
        {
            get
            {
                return _JoinInTotalizers;
            }
            set
            {
                SetPropertyValue("JoinInTotalizers", ref _JoinInTotalizers, value);
            }
        }



        public Guid MinistryDocumentType
        {
            get
            {
                return _MinistryDocumentType;
            }
            set
            {
                SetPropertyValue("MinistryDocumentType", ref _MinistryDocumentType, value);
            }
        }

        private Guid _Division;
        public Guid Division
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

        private bool _TakesDigitalSignature;
        private bool _SupportLoyalty;
        private uint _MaxCountOfLines;
        private bool _AcceptsGeneralItems;
        private bool _ReserveCoupons;

        private bool _IsPrintedOnStoreController;
        private eDocTypeCustomerCategory _DocTypeCustomerCategoryMode;
        private eDocumentTypeItemCategory _DocumentTypeItemCategoryMode;
        private bool _UsesPaymentMethods;
        private decimal _MaxDetailQty;
        private decimal _MaxDetailValue;
        private decimal _MaxDetailTotal;
        private decimal _MaxPaymentAmount;
        private Guid _ReasonCategory;
        private bool? _UsesPricesPersistent;

        public bool TakesDigitalSignature
        {
            get
            {
                return _TakesDigitalSignature;
            }
            set
            {
                SetPropertyValue("TakesDigitalSignature", ref _TakesDigitalSignature, value);
            }
        }


        public bool IsForWholesale
        {
            get
            {
                return _IsForWholesale;
            }
            set
            {
                SetPropertyValue("IsForWholesale", ref _IsForWholesale, value);
            }
        }


        public bool MergedSameDocumentLines
        {
            get
            {
                return _MergedSameDocumentLines;
            }
            set
            {
                SetPropertyValue("MergedSameDocumentLines", ref _MergedSameDocumentLines, value);
            }
        }

        protected override void OnDeleting()
        {
        }


        public bool AllowItemZeroPrices
        {
            get
            {
                return _AllowItemZeroPrices;
            }
            set
            {
                SetPropertyValue("AllowItemZeroPrices", ref _AllowItemZeroPrices, value);
            }
        }

        public bool AcceptsGeneralItems
        {
            get
            {
                return _AcceptsGeneralItems;
            }
            set
            {
                SetPropertyValue("AcceptsGeneralItems ", ref _AcceptsGeneralItems, value);
            }
        }

        public uint MaxCountOfLines
        {
            get
            {
                return _MaxCountOfLines;
            }
            set
            {
                SetPropertyValue("MaxCountOfLines", ref _MaxCountOfLines, value);
            }
        }

        public decimal MaxDetailQty
        {
            get
            {
                return _MaxDetailQty;
            }
            set
            {
                SetPropertyValue("MaxDetailQty", ref _MaxDetailQty, value);
            }
        }
        public decimal MaxDetailValue
        {
            get
            {
                return _MaxDetailValue;
            }
            set
            {
                SetPropertyValue("MaxDetailValue", ref _MaxDetailValue, value);
            }
        }
        public decimal MaxDetailTotal
        {
            get
            {
                return _MaxDetailTotal;
            }
            set
            {
                SetPropertyValue("MaxDetailTotal", ref _MaxDetailTotal, value);
            }
        }

        public decimal MaxPaymentAmount
        {
            get
            {
                return _MaxPaymentAmount;
            }
            set
            {
                SetPropertyValue("MaxPaymentAmount", ref _MaxPaymentAmount, value);
            }
        }

        public bool IsPrintedOnStoreController
        {
            get
            {
                return _IsPrintedOnStoreController;
            }
            set
            {
                SetPropertyValue("IsPrintedOnStoreController ", ref _IsPrintedOnStoreController, value);
            }
        }

        public eDocTypeCustomerCategory DocTypeCustomerCategoryMode
        {
            get
            {
                return _DocTypeCustomerCategoryMode;
            }
            set
            {
                SetPropertyValue("DocTypeCustomerCategoryMode", ref _DocTypeCustomerCategoryMode, value);
            }
        }

        public eDocumentTypeItemCategory DocumentTypeItemCategoryMode
        {
            get
            {
                return _DocumentTypeItemCategoryMode;
            }
            set
            {
                SetPropertyValue("DocumentTypeItemCategoryMode", ref _DocumentTypeItemCategoryMode, value);
            }
        }

        public bool UsesPaymentMethods
        {
            get
            {
                return _UsesPaymentMethods;
            }
            set
            {
                SetPropertyValue("UsesPaymentMethods", ref _UsesPaymentMethods, value);
            }
        }

        public bool ReserveCoupons
        {
            get
            {
                return _ReserveCoupons;
            }
            set
            {
                SetPropertyValue("ReserveCoupons", ref _ReserveCoupons, value);
            }
        }

        public Guid ReasonCategory
        {
            get
            {
                return _ReasonCategory;
            }
            set
            {
                SetPropertyValue("ReasonCategory", ref _ReasonCategory, value);
            }
        }

        [Persistent("UsesPrices")]
        public bool? UsesPricesPersistent
        {
            get
            {
                return _UsesPricesPersistent;
            }
            set
            {
                SetPropertyValue("UsesPricesPersistent", ref _UsesPricesPersistent, value);
            }
        }

        [NonPersistent]
        public bool UsesPrices
        {
            get
            {
                return UsesPricesPersistent.HasValue ? UsesPricesPersistent.Value : true;
            }
            set
            {
                UsesPricesPersistent = value;
            }
        }



        public bool DisplayInCashCount
        {
            get
            {
                return _DisplayInCashCount;
            }
            set
            {
                SetPropertyValue("DisplayInCashCount", ref _DisplayInCashCount, value);
            }
        }

        public bool SupportCustomerVatLevel
        {
            get
            {
                return _SupportCustomerVatLevel;
            }
            set
            {
                SetPropertyValue("SupportCustomerVatLevel", ref _SupportCustomerVatLevel, value);
            }
        }

        public XPCollection<StoreDocumentSeriesType> StoreDocumentSeriesTypes
        {
            get
            {
                return new XPCollection<StoreDocumentSeriesType>(this.Session, new BinaryOperator("DocumentType", Oid));
            }
        }

    }
}
