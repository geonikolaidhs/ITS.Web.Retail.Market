//-----------------------------------------------------------------------
// <copyright file="DocumentType.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Model
{
    [Updater(Order = 190, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("DocumentType", typeof(ResourcesLib.Resources))]
    [IsDefaultUniqueFields(UniqueFields = new string[] { })]
    public class DocumentType : Lookup2Fields, IRequiredOwner
    {
        public DocumentType() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentType(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentType(string code, string description) : base(code, description)
        {

        }

        public DocumentType(Session session, string code, string description)
            : base(session, code, description)
        {

        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    Type thisType = typeof(DocumentType);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    crop = CriteriaOperator.And(new BinaryOperator("Owner.Oid", owner.Oid),
                        CriteriaOperator.Or(
                            new ContainsOperator("StoreDocumentSeriesTypes", new BinaryOperator("DocumentSeries.eModule", eModule.POS)),
                            new ContainsOperator("StoreDocumentSeriesTypes", new ContainsOperator("POSStoreDocumentSeriesTypes", new BinaryOperator("POS.Oid", deviceID)))));
                    break;
            }
            return crop;
        }

        private ePriceSuggestionType _PriceSuggestionType;
        private bool _DocumentHeaderCanBeCopied;
        private eDocumentType _Type;
        private DeficiencySettings _DeficiencySettings;
        private DocumentTypeMapping _DocumentTypeMapping;
        private bool _UsesMarkUpForm;
        private bool _UsesMarkUp;
        private bool _IsOfValues;
        private bool _IsQuantitative;
        private bool _AllowItemZeroPrices;
        private MinistryDocumentType _MinistryDocumentType;
        private bool _IsForWholesale;
        private int _ValueFactor;
        private int _QuantityFactor;
        private bool _UsesPrices;
        private bool _TakesDigitalSignature;
        private bool _MergedSameDocumentLines;
        private Division _Division;
        private bool _UsesPaymentMethods;
        private bool _SupportLoyalty;
        private eDocumentTypeMeasurementUnit _DocumentTypeProposedMeasurementUnit;
        private DocumentStatus _DefaultDocumentStatus;
        private PaymentMethod _DefaultPaymentMethod;
        private string _FormDescription;
        private bool _RecalculatePricesOnTraderChange;
        private eDocTypeCustomerCategory _DocTypeCustomerCategoryMode;
        private eDocumentTypeItemCategory _DocumentTypeItemCategoryMode;
        private bool _ShouldResetMenu;
        private uint _MaxCountOfLines;
        private bool _AcceptsGeneralItems;
        private bool _ReserveCoupons;
        private bool _IsPrintedOnStoreController;
        private decimal _MaxDetailQty;
        private decimal _MaxDetailValue;
        private decimal _MaxDetailTotal;
        private decimal _MaxPaymentAmount;
        private bool _IncreaseVatAndSales;
        private ReasonCategory _ReasonCategory;
        private eDocumentTraderType _TraderType;
        private SpecialItem _SpecialItem;
        private bool _ManualLinkedLineInsertion;
        private int _LinkedLineValueFactor;
        private int _LinkedLineQuantityFactor;
        private bool _InitialisesQuantities;
        private bool _InitialisesValues;
        private bool _AffectsCustomerBalance;
        private ItemStockAffectionOptions _ItemStockAffectionOptions;
        private bool _AllowItemValueEdit;
        private bool _JoinInTotalizers;
        private bool _DisplayInCashCount;
        private bool _ChargeToUser;
        private bool _SupportCustomerVatLevel;
        private bool _UpdateSalesRecords;

        [DisplayOrder(Order = 24)]
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

        [DisplayOrder(Order = 17)]
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

        [DisplayOrder(Order = 21)]
        public bool UsesPrices
        {
            get
            {
                return _UsesPrices;
            }
            set
            {
                SetPropertyValue("UsesPrices", ref _UsesPrices, value);
            }
        }

        [DisplayOrder(Order = 22)]
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

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        [Association("Division-DocumentTypes")]
        [DisplayOrder(Order = 3)]
        public Division Division
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

        [DisplayOrder(Order = 4)]
        public MinistryDocumentType MinistryDocumentType
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

        [DisplayOrder(Order = 14)]
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

        [DisplayOrder(Order = 15)]
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

        [DisplayOrder(Order = 23)]
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

        public List<Store> Stores
        {
            get
            {
                return this.StoreDocumentSeriesTypes == null ? new List<Store>() : this.StoreDocumentSeriesTypes.Where(stdst => stdst.DocumentSeries != null).Select(sdst => sdst.DocumentSeries.Store).ToList();
            }
        }

        [DisplayOrder(Order = 18)]
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

        [DisplayOrder(Order = 25)]
        public bool IsQuantitative
        {
            get
            {
                return _IsQuantitative;
            }
            set
            {
                SetPropertyValue("IsQuantitative", ref _IsQuantitative, value);
            }
        }

        [DisplayOrder(Order = 26)]
        public bool IsOfValues
        {
            get
            {
                return _IsOfValues;
            }
            set
            {
                SetPropertyValue("IsOfValues", ref _IsOfValues, value);
            }
        }

        [DisplayOrder(Order = 26)]
        public bool UsesMarkUp
        {
            get
            {
                return _UsesMarkUp;
            }
            set
            {
                SetPropertyValue("UsesMarkUp", ref _UsesMarkUp, value);
            }
        }

        [DisplayOrder(Order = 27)]
        public bool UsesMarkUpForm
        {
            get
            {
                return _UsesMarkUpForm;
            }
            set
            {
                SetPropertyValue("UsesMarkUpForm", ref _UsesMarkUpForm, value);
            }
        }

        public bool DisplaysMarkUpForm
        {
            get
            {
                return UsesMarkUp && UsesMarkUpForm;
            }
        }

        [Indexed("GCRecord", Unique = false)]
        public DocumentTypeMapping DocumentTypeMapping
        {
            get
            {
                return _DocumentTypeMapping;
            }
            set
            {
                if (_DocumentTypeMapping == value)
                {
                    return;
                }

                DocumentTypeMapping prevDocumentTypeMapping = _DocumentTypeMapping;
                _DocumentTypeMapping = value;

                if (IsLoading)
                {
                    return;
                }

                if (prevDocumentTypeMapping != null && prevDocumentTypeMapping.DocumentType == this)
                {
                    prevDocumentTypeMapping.DocumentType = null;
                }

                if (_DocumentTypeMapping != null)
                {
                    _DocumentTypeMapping.DocumentType = this;
                }

                OnChanged("DocumentTypeMapping");
            }
        }

        //[Association("DeficiencySettings-DocumentType")]
        public DeficiencySettings DeficiencySettings
        {
            get
            {
                return _DeficiencySettings;
            }
            set
            {
                if (_DeficiencySettings == value)
                {
                    return;
                }

                DeficiencySettings previousValue = _DeficiencySettings;
                _DeficiencySettings = value;

                if (IsLoading)
                {
                    return;
                }

                if (previousValue != null && previousValue.DeficiencyDocumentType == this)
                {
                    previousValue.DeficiencyDocumentType = null;
                }

                if (_DeficiencySettings != null)
                {
                    _DeficiencySettings.DeficiencyDocumentType = this;
                }
                OnChanged("DeficiencySettings");
            }
        }

        [DisplayOrder(Order = 7)]
        public eDocumentType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
            }
        }

        [DisplayOrder(Order = 20)]
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

        [DisplayOrder(Order = 19)]
        public bool RecalculatePricesOnTraderChange
        {
            get
            {
                return _RecalculatePricesOnTraderChange;
            }
            set
            {
                SetPropertyValue("RecalculatePricesOnTraderChange", ref _RecalculatePricesOnTraderChange, value);
            }
        }

        [NonPersistent]
        public bool ShouldResetMenu
        {
            get
            {
                return _ShouldResetMenu;
            }
            set
            {
                SetPropertyValue("ShouldResetMenu", ref _ShouldResetMenu, value);
            }
        }

        [DisplayOrder(Order = 8)]
        public eDocumentTypeMeasurementUnit MeasurementUnitMode
        {
            get
            {
                return _DocumentTypeProposedMeasurementUnit;
            }
            set
            {
                SetPropertyValue("DocumentTypeProposedMeasurementUnit", ref _DocumentTypeProposedMeasurementUnit, value);
            }
        }

        [DisplayOrder(Order = 10)]
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

        [DisplayOrder(Order = 11)]
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

        [DisplayOrder(Order = 28)]
        public bool DocumentHeaderCanBeCopied
        {
            get
            {
                return _DocumentHeaderCanBeCopied;
            }
            set
            {
                SetPropertyValue("DocumentHeaderCanBeCopied", ref _DocumentHeaderCanBeCopied, value);
            }
        }

        [DisplayOrder(Order = 5)]
        public DocumentStatus DefaultDocumentStatus
        {
            get
            {
                return _DefaultDocumentStatus;
            }
            set
            {
                SetPropertyValue("DefaultDocumentStatus", ref _DefaultDocumentStatus, value);
            }
        }

        [DisplayOrder(Order = 6)]
        public PaymentMethod DefaultPaymentMethod
        {
            get
            {
                return _DefaultPaymentMethod;
            }
            set
            {
                SetPropertyValue("DefaultPaymentMethod", ref _DefaultPaymentMethod, value);
            }
        }

        [DisplayOrder(Order = 12)]
        public string FormDescription
        {
            get
            {
                return _FormDescription;
            }
            set
            {
                SetPropertyValue("FormDescription", ref _FormDescription, value);
            }
        }

        [DisplayOrder(Order = 9)]
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

        [DisplayOrder(Order = 13)]
        public ePriceSuggestionType PriceSuggestionType
        {
            get
            {
                return _PriceSuggestionType;
            }
            set
            {
                SetPropertyValue("PriceSuggestionType", ref _PriceSuggestionType, value);
            }
        }

        [DisplayOrder(Order = 29)]
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



        public ReasonCategory ReasonCategory
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

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "Division")
            {
                ShouldResetMenu = true;
            }
        }

        protected override void OnDeleting()
        {
            if (this.DeficiencySettings != null && this.DeficiencySettings.IsActive == false)
            {
                throw new Exception(String.Format("Cannot delete {0}! {1} has reference to it", this.Description, this.DeficiencySettings.Description));
            }

            this.Session.Delete(this.ActionTypeEntities);

            base.OnDeleting();
        }

        [DisplayOrder(Order = 1)]
        [Association("DocumentType-StoreDocumentSeriesTypes"), Aggregated]
        public XPCollection<StoreDocumentSeriesType> StoreDocumentSeriesTypes
        {
            get
            {
                return GetCollection<StoreDocumentSeriesType>("StoreDocumentSeriesTypes");
            }
        }

        [DisplayOrder(Order = 5)]
        [Association("DocumentType-DocTypeCustomerCategories"), Aggregated]
        public XPCollection<DocTypeCustomerCategory> DocTypeCustomerCategories
        {
            get
            {
                return GetCollection<DocTypeCustomerCategory>("DocTypeCustomerCategories");
            }
        }

        [DisplayOrder(Order = 6)]
        [Association("DocumentType-DocumentTypeItemCategories"), Aggregated]
        public XPCollection<DocumentTypeItemCategory> DocumentTypeItemCategories
        {
            get
            {
                return GetCollection<DocumentTypeItemCategory>("DocumentTypeItemCategories");
            }
        }

        [DisplayOrder(Order = 7)]
        public XPCollection<ActionTypeEntity> ActionTypeEntities
        {
            get
            {
                CriteriaOperator criteria = new BinaryOperator("EntityOid", this.Oid);
                return new XPCollection<ActionTypeEntity>(PersistentCriteriaEvaluationBehavior.InTransaction, this.Session, criteria);
            }
        }

        [DisplayOrder(Order = 4)]
        [Association("DocumentType-DocumentTypeRoles"), Aggregated]
        public XPCollection<DocumentTypeRole> DocumentTypeRoles
        {
            get
            {
                return GetCollection<DocumentTypeRole>("DocumentTypeRoles");
            }
        }

        [DisplayOrder(Order = 2)]
        [Aggregated, Association("DocumentType-TransformationRulesTo")]
        public XPCollection<TransformationRule> TransformsTo
        {
            get
            {
                return GetCollection<TransformationRule>("TransformsTo");
            }
        }

        [Aggregated, Association("DocumentType-TransformationRulesFrom")]
        public XPCollection<TransformationRule> TransformsFrom
        {
            get
            {
                return GetCollection<TransformationRule>("TransformsFrom");
            }
        }

        [DisplayOrder(Order = 3)]
        [Aggregated, Association("DocumentType-DocumentTypeCustomReports")]
        public XPCollection<DocumentTypeCustomReport> DocumentTypeCustomReports
        {
            get
            {
                return GetCollection<DocumentTypeCustomReport>("DocumentTypeCustomReports");
            }
        }

        public List<DocumentSeries> DocumentSeries
        {
            get
            {
                return StoreDocumentSeriesTypes.Count == 0 ? new List<DocumentSeries>() : StoreDocumentSeriesTypes.Select(storeDocumentSeriesType => storeDocumentSeriesType.DocumentSeries).Where(x => x != null).ToList();
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
                SetPropertyValue("ReserveCoupons ", ref _ReserveCoupons, value);
            }
        }

        [DisplayOrder(Order = 8)]
        [Association("DocumentType-DocumentTypeBarcodeTypes"), Aggregated]
        public XPCollection<DocumentTypeBarcodeType> DocumentTypeBarcodeTypes
        {
            get
            {
                return GetCollection<DocumentTypeBarcodeType>("DocumentTypeBarcodeTypes");
            }
        }

        [Association("DocumentType-UserDailyTotalsCashCountDetails")]
        public XPCollection<UserDailyTotalsCashCountDetail> UserDailyTotalsCashCountDetails
        {
            get
            {
                return GetCollection<UserDailyTotalsCashCountDetail>("UserDailyTotalsCashCountDetails");
            }
        }

        public eDocumentTraderType TraderType
        {
            get
            {
                return _TraderType;
            }
            set
            {
                SetPropertyValue("TraderType ", ref _TraderType, value);
            }
        }

        public SpecialItem SpecialItem
        {
            get
            {
                return _SpecialItem;
            }
            set
            {
                SetPropertyValue("SpecialItem ", ref _SpecialItem, value);
            }
        }

        public bool ManualLinkedLineInsertion
        {
            get
            {
                return _ManualLinkedLineInsertion;
            }
            set
            {
                SetPropertyValue("ManualLinkedLineInsertion ", ref _ManualLinkedLineInsertion, value);
            }
        }

        public int LinkedLineValueFactor
        {
            get
            {
                return _LinkedLineValueFactor;
            }
            set
            {
                SetPropertyValue("LinkedLineValueFactor", ref _LinkedLineValueFactor, value);
            }
        }

        public int LinkedLineQuantityFactor
        {
            get
            {
                return _LinkedLineQuantityFactor;
            }
            set
            {
                SetPropertyValue("LinkedLineQuantityFactor", ref _LinkedLineQuantityFactor, value);
            }
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);

            if (includeDetails)
            {
                dictionary.Add("DocumentTypeRoles", DocumentTypeRoles.Select(documentTypeRole => documentTypeRole.GetDict(settings, includeType, includeDetails)).ToList());
            }
            return dictionary;
        }

        public bool InitialisesQuantities
        {
            get
            {
                return _InitialisesQuantities;
            }
            set
            {
                SetPropertyValue("InitialisesQuantities", ref _InitialisesQuantities, value);
            }
        }

        public bool InitialisesValues
        {
            get
            {
                return _InitialisesValues;
            }
            set
            {
                SetPropertyValue("InitialisesValues", ref _InitialisesValues, value);
            }
        }

        public bool AffectsCustomerBalance
        {
            get
            {
                return _AffectsCustomerBalance;
            }
            set
            {
                SetPropertyValue("AffectsCustomerBalance", ref _AffectsCustomerBalance, value);
            }
        }

        public ItemStockAffectionOptions ItemStockAffectionOptions
        {
            get
            {
                return _ItemStockAffectionOptions;
            }
            set
            {
                SetPropertyValue("ItemStockAffectionOptions", ref _ItemStockAffectionOptions, value);
            }
        }

        public bool AllowItemValueEdit
        {
            get
            {
                return _AllowItemValueEdit;
            }
            set
            {
                SetPropertyValue("AllowItemValueEdit", ref _AllowItemValueEdit, value);
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

        public bool ChargeToUser
        {
            get
            {
                return _ChargeToUser;
            }
            set
            {
                SetPropertyValue("ChargeToUser", ref _ChargeToUser, value);
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

        public bool UpdateSalesRecords
        {
            get
            {
                return _UpdateSalesRecords;
            }
            set
            {
                SetPropertyValue("UpdateSalesRecords", ref _UpdateSalesRecords, value);
            }
        }

    }
}
