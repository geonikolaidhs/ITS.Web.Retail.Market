//-----------------------------------------------------------------------
// <copyright file="DataFileReceived.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Drawing;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Model.NonPersistant;

namespace ITS.Retail.Model
{
    public class DecodedRawData : BasicObj, IRequiredOwner
    {
        public DecodedRawData()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DecodedRawData(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        // Fields...

        private string _ContentUnit;
        private string _PackingQuantity;
        private string _Item;
        private string _Fax;
        private string _JsonDataRecord;
        private string _Description2;
        private string _EndDate;
        private string _FromDate;
        private string _SplitOrder;
        private string _Firm;
        private string _BaseStore;
        private string _TaxOffice;
        private string _Phone;
        private string _ZipCode;
        private string _Region;
        private string _LoyaltyCode;
        private string _Discount;
        private string _GrossPrice;
        private string _NetPrice;
        private string _PriceCatalog;
        private string _PriceCatalogPolicy;
        private string _DefaultPriceCatalogPolicy;
        private string _SubItemQty;
        private string _SubItemCode;
        private string _FactorQty;
        private string _Barcode;
        private string _MaxItemOrderQty;
        private string _MinOrderQty;
        private string _CentralStoreCode;
        private string _InsertedDate;
        private string _MeasurmentUnit;
        private string _VatCategory;
        private string _Seasonality;
        private string _Buyer;
        private string _TaxCode;
        private string _MotherCode;
        private string _SupplierCode;
        private string _Level4;
        private string _Level3;
        private string _Level2;
        private string _Level1;
        private string _Profession;
        private string _Address;
        private string _CompanyName;
        private string _Code;
        private string _City;
        private string _Description;
        private DataFileRecordHeader _Head;
        private bool _IsLicenced;
        private CompanyNew _Owner;
        private int _Counter;
        private string _PackingMeasurmentUnit;
        private string _BarcodeTypeCode;
        private string _PluCode;
        private string _PluPrefix;
        private string _Points;
        private bool _IsDefaultAddress;
        private string _CustomPriceOptions;
        private string _RefundStore;
        private string _CardID;
        private string _BarcodeMeasurmentUnit;
        private string _TimeValue;
        private string _TimeValueValidUntilDate;
        private string _TimeValueValidFromDate;
        private string _ReferenceUnit;
        private string _RemoteReferenceId;
        private string _Root;
        private string _ParentCode;
        private string _Node;
        private string _AddressProfession;
        private string _PackedAt;
        private string _Store;
        private string _ExpiresAt;
        private string _Origin;
        private string _Lot;
        private string _ThirdPartNum;
        private string _AddressVatCategory;

        //private string _PriceCatalogDetail;

        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }

        [Association("DataFileRecordHeader-DecocedData")]
        public DataFileRecordHeader Head
        {
            get
            {
                return _Head;
            }
            set
            {
                SetPropertyValue("Head", ref _Head, value);
            }
        }


        public string jsonDataRecord
        {
            get
            {
                return _JsonDataRecord;
            }
            set
            {
                SetPropertyValue("jsonDataRecord", ref _JsonDataRecord, value);
            }
        }
        [ImportEntity(typeof(ItemExtraInfo))]
        [ImportEntity(typeof(Item))]
        [ImportEntity(typeof(ItemCategory))]
        [ImportEntity(typeof(ItemCategoryImportData))]
        [ImportEntity(typeof(PriceCatalog))]
        [ImportEntity(typeof(Store))]
        [ImportEntity(typeof(Offer))]
        [ImportEntity(typeof(BarcodeType))]
        [ImportEntity(typeof(MeasurementUnit))]
        [ImportEntity(typeof(Leaflet))]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        [ImportEntity(typeof(Store))]
        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(SupplierNew))]
        public string City
        {
            get
            {
                return _City;
            }
            set
            {
                SetPropertyValue("City", ref _City, value);
            }
        }

        [ImportEntity(typeof(SupplierNew))]
        [ImportEntity(typeof(PriceCatalog))]
        [ImportEntity(typeof(Store))]
        [ImportEntity(typeof(Item))]
        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(Offer))]
        [ImportEntity(typeof(OfferDetail))]
        [ImportEntity(typeof(BarcodeType))]
        [ImportEntity(typeof(MeasurementUnit))]
        [ImportEntity(typeof(PriceCatalogDetailTimeValue))]
        [ImportEntity(typeof(ItemCategoryImportData))]
        [ImportEntity(typeof(Leaflet))]
        [ImportEntity(typeof(LeafletDetail))]
        [ImportEntity(typeof(LeafletStore))]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }
        [ImportEntity(typeof(ItemAnalyticTree))]
        public string Root
        {
            get
            {
                return _Root;
            }
            set
            {
                SetPropertyValue("Root", ref _Root, value);
            }
        }
        [ImportEntity(typeof(ItemAnalyticTree))]
        public string Node
        {
            get
            {
                return _Node;
            }
            set
            {
                SetPropertyValue("Node", ref _Node, value);
            }
        }
        [ImportEntity(typeof(ItemCategoryImportData))]
        public string ParentCode
        {
            get
            {
                return _ParentCode;
            }
            set
            {
                SetPropertyValue("ParentCode", ref _ParentCode, value);
            }
        }

        [ImportEntity(typeof(SupplierNew))]
        [ImportEntity(typeof(Customer))]
        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }
            set
            {
                SetPropertyValue("CompanyName", ref _CompanyName, value);
            }
        }

        [ImportEntity(typeof(SupplierNew))]
        [ImportEntity(typeof(Customer))]
        public string Address
        {
            get
            {
                return _Address;
            }
            set
            {
                SetPropertyValue("Address", ref _Address, value);
            }
        }

        [ImportEntity(typeof(SupplierNew))]
        [ImportEntity(typeof(Customer))]
        public string Profession
        {
            get
            {
                return _Profession;
            }
            set
            {
                SetPropertyValue("Profession", ref _Profession, value);
            }
        }

        [ImportEntity(typeof(Item))]
        [ImportEntity(typeof(ItemCategory))]
        public string Level1
        {
            get
            {
                return _Level1;
            }
            set
            {
                SetPropertyValue("Level1", ref _Level1, value);
            }
        }

        [ImportEntity(typeof(Item))]
        [ImportEntity(typeof(ItemCategory))]
        public string Level2
        {
            get
            {
                return _Level2;
            }
            set
            {
                SetPropertyValue("Level2", ref _Level2, value);
            }
        }

        [ImportEntity(typeof(Item))]
        [ImportEntity(typeof(ItemCategory))]
        public string Level3
        {
            get
            {
                return _Level3;
            }
            set
            {
                SetPropertyValue("Level3", ref _Level3, value);
            }
        }

        [ImportEntity(typeof(Item))]
        [ImportEntity(typeof(ItemCategory))]
        public string Level4
        {
            get
            {
                return _Level4;
            }
            set
            {
                SetPropertyValue("Level4", ref _Level4, value);
            }
        }


        [ImportEntity(typeof(Item))]
        public string SupplierCode
        {
            get
            {
                return _SupplierCode;
            }
            set
            {
                SetPropertyValue("SupplierCode", ref _SupplierCode, value);
            }
        }

        [ImportEntity(typeof(Item))]
        public string MotherCode
        {
            get
            {
                return _MotherCode;
            }
            set
            {
                SetPropertyValue("MotherCode", ref _MotherCode, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(SupplierNew))]
        public string TaxCode
        {
            get
            {
                return _TaxCode;
            }
            set
            {
                SetPropertyValue("TaxCode", ref _TaxCode, value);
            }
        }

        [ImportEntity(typeof(Item))]
        public string Buyer
        {
            get
            {
                return _Buyer;
            }
            set
            {
                SetPropertyValue("Buyer", ref _Buyer, value);
            }
        }

        [ImportEntity(typeof(Item))]
        public string Seasonality
        {
            get
            {
                return _Seasonality;
            }
            set
            {
                SetPropertyValue("Seasonality", ref _Seasonality, value);
            }
        }

        [ImportEntity(typeof(Item))]
        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(SupplierNew))]
        public string VatCategory
        {
            get
            {
                return _VatCategory;
            }
            set
            {
                SetPropertyValue("VatCategory", ref _VatCategory, value);
            }
        }

        [ImportEntity(typeof(Item))]
        public string MeasurmentUnit
        {
            get
            {
                return _MeasurmentUnit;
            }
            set
            {
                SetPropertyValue("MeasurmentUnit", ref _MeasurmentUnit, value);
            }
        }

        [ImportEntity(typeof(Item))]
        public string PackingMeasurmentUnit
        {
            get
            {
                return _PackingMeasurmentUnit;
            }
            set
            {
                SetPropertyValue("PackingMeasurmentUnit", ref _PackingMeasurmentUnit, value);
            }
        }

        [ImportEntity(typeof(Item))]
        public string InsertedDate
        {
            get
            {
                return _InsertedDate;
            }
            set
            {
                SetPropertyValue("InsertedDate", ref _InsertedDate, value);
            }
        }

        [ImportEntity(typeof(Item))]
        public string CentralStoreCode
        {
            get
            {
                return _CentralStoreCode;
            }
            set
            {
                SetPropertyValue("CentralStoreCode", ref _CentralStoreCode, value);
            }
        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(Item))]
        public string MinOrderQty
        {
            get
            {
                return _MinOrderQty;
            }
            set
            {
                SetPropertyValue("MinOrderQty", ref _MinOrderQty, value);
            }
        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(Item))]
        public string MaxItemOrderQty
        {
            get
            {
                return _MaxItemOrderQty;
            }
            set
            {
                SetPropertyValue("MaxItemOrderQty", ref _MaxItemOrderQty, value);
            }
        }

        [ImportEntity(typeof(Barcode))]
        [ImportEntity(typeof(LeafletDetail))]
        public string Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                SetPropertyValue("Barcode", ref _Barcode, value);
            }
        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(Barcode))]
        public string FactorQty
        {
            get
            {
                return _FactorQty;
            }
            set
            {
                SetPropertyValue("FactorQty", ref _FactorQty, value);
            }
        }

        [ImportEntity(typeof(LinkedItem))]
        public string SubItemCode
        {
            get
            {
                return _SubItemCode;
            }
            set
            {
                SetPropertyValue("SubItemCode", ref _SubItemCode, value);
            }
        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(LinkedItem))]
        public string SubItemQty
        {
            get
            {
                return _SubItemQty;
            }
            set
            {
                SetPropertyValue("SubItemQty", ref _SubItemQty, value);
            }
        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(PriceCatalogDetail))]
        [ImportEntity(typeof(LeafletDetail))]
        public string GrossPrice
        {
            get
            {
                return _GrossPrice;
            }
            set
            {
                SetPropertyValue("GrossPrice", ref _GrossPrice, value);
            }
        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(PriceCatalogDetail))]
        public string NetPrice
        {
            get
            {
                return _NetPrice;
            }
            set
            {
                SetPropertyValue("NetPrice", ref _NetPrice, value);
            }
        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(PriceCatalogDetail))]
        public string Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                SetPropertyValue("Discount", ref _Discount, value);
            }
        }


        public string LoyaltyCode
        {
            get
            {
                return _LoyaltyCode;
            }
            set
            {
                SetPropertyValue("LoyaltyCode", ref _LoyaltyCode, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        public string Region
        {
            get
            {
                return _Region;
            }
            set
            {
                SetPropertyValue("Region", ref _Region, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(SupplierNew))]
        public string ZipCode
        {
            get
            {
                return _ZipCode;

            }
            set
            {
                SetPropertyValue("ZipCode", ref _ZipCode, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(SupplierNew))]
        public string Phone
        {
            get
            {
                return _Phone;
            }
            set
            {
                SetPropertyValue("Phone", ref _Phone, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(SupplierNew))]
        public string TaxOffice
        {
            get
            {
                return _TaxOffice;
            }
            set
            {
                SetPropertyValue("TaxOffice", ref _TaxOffice, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        public string BaseStore
        {
            get
            {
                return _BaseStore;
            }
            set
            {
                SetPropertyValue("BaseStore", ref _BaseStore, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        public string RefundStore
        {
            get
            {
                return _RefundStore;
            }
            set
            {
                SetPropertyValue("RefundStore", ref _RefundStore, value);
            }
        }
        

        [ImportEntity(typeof(Customer))]
        public string Firm
        {
            get
            {
                return _Firm;
            }
            set
            {
                SetPropertyValue("Firm", ref _Firm, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        public string SplitOrder
        {
            get
            {
                return _SplitOrder;
            }
            set
            {
                SetPropertyValue("SplitOrder", ref _SplitOrder, value);
            }
        }

        [ImportEntity(typeof(Offer))]
        [ImportEntity(typeof(PriceCatalog))]
        [ImportEntity(typeof(Leaflet))]
        public string FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                SetPropertyValue("FromDate", ref _FromDate, value);
            }
        }

        [ImportEntity(typeof(Offer))]
        [ImportEntity(typeof(PriceCatalog))]
        [ImportEntity(typeof(Leaflet))]
        public string EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                SetPropertyValue("EndDate", ref _EndDate, value);
            }
        }

        [ImportEntity(typeof(ItemExtraInfo))]
        public string PackedAt
        {
            get
            {
                return _PackedAt;
            }
            set
            {
                SetPropertyValue("PackedAt", ref _PackedAt, value);
            }
        }
        [ImportEntity(typeof(ItemExtraInfo))]
        public string ExpiresAt
        {
            get
            {
                return _ExpiresAt;
            }
            set
            {
                SetPropertyValue("ExpiresAt", ref _ExpiresAt, value);
            }
        }
        [ImportEntity(typeof(ItemExtraInfo))]
        [ImportEntity(typeof(LeafletStore))]
        public string Store
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
        [ImportEntity(typeof(ItemExtraInfo))]
        public string Origin
        {
            get
            {
                return _Origin;
            }
            set
            {
                SetPropertyValue("Origin", ref _Origin, value);
            }
        }
        [ImportEntity(typeof(ItemExtraInfo))]
        public string Lot
        {
            get
            {
                return _Lot;
            }
            set
            {
                SetPropertyValue("Lot", ref _Lot, value);
            }
        }
        [ImportEntity(typeof(PriceCatalog))]
        [ImportEntity(typeof(PriceCatalogDetail))]
        [ImportEntity(typeof(PriceCatalogDetailTimeValue))]
        //[ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(Offer))]
        [ImportEntity(typeof(PriceCatalogPolicy))]
        public string PriceCatalog
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

        [ImportEntity(typeof(PriceCatalogPolicy))]
        [ImportEntity(typeof(StorePriceCatalogPolicy))]
        [ImportEntity(typeof(PriceCatalogPolicyPromotion))]
        [ImportEntity(typeof(Customer))]
        public string PriceCatalogPolicy
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

        [ImportEntity(typeof(Store))]
        public string DefaultPriceCatalogPolicy
        {
            get
            {
                return _DefaultPriceCatalogPolicy;
            }
            set
            {
                SetPropertyValue("DefaultPriceCatalogPolicy", ref _DefaultPriceCatalogPolicy, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(SupplierNew))]
        public string Fax
        {
            get
            {
                return _Fax;
            }
            set
            {
                SetPropertyValue("Fax", ref _Fax, value);
            }
        }
        [ImportEntity(typeof(ItemExtraInfo))]
        [ImportEntity(typeof(Barcode))]
        [ImportEntity(typeof(LinkedItem))]
        [ImportEntity(typeof(PriceCatalogDetail))]
        [ImportEntity(typeof(OfferDetail))]
        [ImportEntity(typeof(ItemAnalyticTree))]
        [ImportEntity(typeof(LeafletDetail))]
        public string Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetPropertyValue("Item", ref _Item, value);
            }
        }

        [ImportEntity(typeof(Offer))]
        public string Description2
        {
            get
            {
                return _Description2;
            }
            set
            {
                SetPropertyValue("Description2", ref _Description2, value);
            }
        }

        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter)), Delayed]
        public Image Image
        {
            get
            {
                return GetDelayedPropertyValue<Image>("Image");
            }
            set
            {
                SetDelayedPropertyValue<Image>("Image", value);
            }

        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(Item))]
        public string PackingQuantity
        {
            get
            {
                return _PackingQuantity;
            }
            set
            {
                SetPropertyValue("PackingQuantity", ref _PackingQuantity, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(SupplierNew))]
        public bool IsDefaultAddress
        {
            get
            {
                return _IsDefaultAddress;
            }
            set
            {
                SetPropertyValue("IsDefaultAddress", ref _IsDefaultAddress, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        public bool IsLicenced
        {
            get
            {
                return _IsLicenced;
            }
            set
            {
                SetPropertyValue("IsLicenced", ref _IsLicenced, value);
            }
        }

        [ImportEntity(typeof(Barcode))]
        public string BarcodeTypeCode
        {
            get
            {
                return _BarcodeTypeCode;
            }
            set
            {
                SetPropertyValue("BarcodeTypeCode", ref _BarcodeTypeCode, value);
            }
        }

        [ImportEntity(typeof(Barcode))]
        public string PluCode
        {
            get
            {
                return _PluCode;
            }
            set
            {
                SetPropertyValue("PluCode", ref _PluCode, value);
            }
        }

        
        [ImportEntity(typeof(Barcode))]
        public string PluPrefix
        {
            get
            {
                return _PluPrefix;
            }
            set
            {
                SetPropertyValue("PluPrefix", ref _PluPrefix, value);
            }
        }

        public int Counter
        {
            get
            {
                return _Counter;
            }
            set
            {
                SetPropertyValue("Counter", ref _Counter, value);
            }
        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(Item))]
        [ImportEntity(typeof(ItemCategory))]
        [ImportEntity(typeof(ItemCategoryImportData))]
        [ImportEntity(typeof(Customer))]
        public string Points
        {
            get
            {
                return _Points;
            }
            set
            {
                SetPropertyValue("Discount", ref _Points, value);
            }
        }


        [RawDataNumeric(NumericType.INTEGER)]
        [ImportEntity(typeof(Item))]
        public string CustomPriceOptions
        {
            get
            {
                return _CustomPriceOptions;
            }
            set
            {
                SetPropertyValue("CustomPriceOptions", ref _CustomPriceOptions, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        public string CardID
        {
            get
            {
                return _CardID;
            }
            set
            {
                SetPropertyValue("CardID", ref _CardID, value);
            }
        }

        [ImportEntity(typeof(Barcode))]
        public string BarcodeMeasurmentUnit
        {
            get
            {
                return _BarcodeMeasurmentUnit;
            }
            set
            {
                SetPropertyValue("BarcodeMeasurmentUnit", ref _BarcodeMeasurmentUnit, value);
            }
        }

        [RawDataNumeric(NumericType.DOUBLE)]
        [ImportEntity(typeof(PriceCatalogDetailTimeValue))]
        public string TimeValue
        {
            get
            {
                return _TimeValue;
            }
            set
            {
                SetPropertyValue("TimeValue", ref _TimeValue, value);
            }
        }

        [ImportEntity(typeof(PriceCatalogDetailTimeValue))]
        public string TimeValueValidFromDate
        {
            get
            {
                return _TimeValueValidFromDate;
            }
            set
            {
                SetPropertyValue("TimeValueValidFromDate", ref _TimeValueValidFromDate, value);
            }
        }

        [ImportEntity(typeof(PriceCatalogDetailTimeValue))]
        public string TimeValueValidUntilDate
        {
            get
            {
                return _TimeValueValidUntilDate;
            }
            set
            {
                SetPropertyValue("TimeValueValidUntilDate", ref _TimeValueValidUntilDate, value);
            }
        }


        [ImportEntity(typeof(Item))]
        public string ContentUnit
        {
            get
            {
                return _ContentUnit;
            }
            set
            {
                SetPropertyValue("ContentUnit", ref _ContentUnit, value);
            }
        }

        [ImportEntity(typeof(Item))]
        public string ReferenceUnit
        {
            get
            {
                return _ReferenceUnit;
            }
            set
            {
                SetPropertyValue("ReferenceUnit", ref _ReferenceUnit, value);
            }
        }
        [ImportEntity(typeof(ItemExtraInfo))]
        [ImportEntity(typeof(PriceCatalogDetailTimeValue))]
        [ImportEntity(typeof(ItemAnalyticTree))]
        [ImportEntity(typeof(LeafletStore))]
        public string RemoteReferenceId
        {
            get
            {
                return _RemoteReferenceId;
            }
            set
            {
                SetPropertyValue("RemoteReferenceId", ref _RemoteReferenceId, value);
            }
        }

        [ImportEntity(typeof(SupplierNew))]
        [ImportEntity(typeof(Customer))]
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

        [ImportEntity(typeof(SupplierNew))]
        [ImportEntity(typeof(Customer))]
        public string ThirdPartNumber
        {
            get
            {
                return _ThirdPartNum;
            }
            set
            {
                SetPropertyValue("ThirdPartNum", ref _ThirdPartNum, value);
            }
        }

        [ImportEntity(typeof(Customer))]
        [ImportEntity(typeof(SupplierNew))]
        public string AddressVatCategory
        {
            get
            {
                return _AddressVatCategory;
            }
            set
            {
                SetPropertyValue("AddressVatCategory", ref _AddressVatCategory, value);
            }
        }
    }
}
