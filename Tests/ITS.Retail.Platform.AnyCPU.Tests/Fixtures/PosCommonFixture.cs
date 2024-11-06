using DevExpress.Xpo;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Tests.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Tests.Fixtures
{
    public class PosCommonFixture : IDisposable
    {
        public ISessionManager MemorySessionManager { get; protected set; }
        public CompanyNew Owner { get; protected set; }
        public OwnerApplicationSettings AppSettings { get; protected set; }
        public Customer DefaultCustomer { get; protected set; }
        public Customer LoyaltyCustomer { get; protected set; }
        public DocumentType ReceiptDocumentType { get; protected set; }
        public DocumentSeries ReceiptDocumentSeries { get; protected set; }
        public VatCategory VatCat23 { get; protected set; }
        public VatCategory VatCat13 { get; protected set; }
        public MeasurementUnit MeasurementUnitNonDecimal { get; protected set; }
        public MeasurementUnit MeasurementUnitDecimal { get; protected set; }
        public PriceCatalogPolicy PriceCatalogPolicy { get; protected set; }
        public PriceCatalogPolicy SubPriceCatalogPolicy { get; protected set; }
        public PriceCatalog RootPriceCatalog { get; protected set; }
        public PriceCatalog SubCatalog { get; protected set; }
        public Store DefaultStore { get; protected set; }
        public Store SecondaryStore { get; protected set; }
        public DocumentStatus DocumentStatusThatTakesSequence { get; protected set; }
        public ITS.POS.Model.Settings.POS POS1 { get; protected set; }
        public DailyTotals DailyTotalsPOS1 { get; protected set; }
        public UserDailyTotals UserDailyTotalsPOS1 { get; protected set; }
        public User CurrentUserPOS1 { get; protected set; }
        public DiscountType LineDiscountPercentage { get; protected set; }
        public DiscountType LineDiscountValue { get; protected set; }
        public DiscountType HeaderDiscountPercentage { get; protected set; }
        public DiscountType HeaderDiscountValue { get; protected set; }
        public PaymentMethod DefaultPaymentMethod { get; protected set; }
        public PaymentMethod LoyaltyPaymentMethod { get; protected set; }
        public StorePriceList StorePriceListDefaultStoreRootCatalog { get; protected set; }
        public StorePriceList StorePriceListDefaultStoreSubCatalog { get; protected set; }
        public StorePriceList StorePriceListSecondaryStoreRootCatalog { get; protected set; }
        public StorePriceList StorePriceListSecondaryStoreSubCatalog { get; protected set; }
        public ItemCategory RootItemCategory { get; protected set; }
        public ItemCategory SubItemCategory1 { get; protected set; }
        public ItemCategory SubItemCategory2 { get; protected set; }
        public ItemCategory SubSubItemCategory1_1 { get; protected set; }
        public ItemCategory SubSubItemCategory1_2 { get; protected set; }
        public ItemCategory SubSubItemCategory2_1 { get; protected set; }
        public ItemCategory SubSubItemCategory2_2 { get; protected set; }

        public CustomerCategory RootCustomerCategory { get; protected set; }
        public CustomerCategory SubCustomerCategory1 { get; protected set; }
        public CustomerCategory SubCustomerCategory2 { get; protected set; }
        public CustomerCategory SubSubCustomerCategory1_1 { get; protected set; }
        public CustomerCategory SubSubCustomerCategory1_2 { get; protected set; }
        public CustomerCategory SubSubCustomerCategory2_1 { get; protected set; }
        public CustomerCategory SubSubCustomerCategory2_2 { get; protected set; }

        public DocumentHeader SampleReceiptOpenWith1LinePOS1 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWith1LinePOS1Detail_SampleItemVat23 { get; protected set; }

        public DocumentHeader SampleReceiptOpenWithMultipleLinesPOS1 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithMultipleLinesPOS1Detail1_SampleItemVat23 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithMultipleLinesPOS1Detail2_SampleItemVat23 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithMultipleLinesPOS1Detail3_SampleItemVat23 { get; protected set; }

        public DocumentHeader SampleReceiptOpenWithDifferentItemsPOS1 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithDifferentItemsPOS1Detail1_SampleItemVat23 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithDifferentItemsPOS1Detail2_SampleItemVat23 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithDifferentItemsPOS1Detail3_SampleItem2Vat23 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithDifferentItemsPOS1Detail4_SampleItem2Vat23 { get; protected set; }

        public DocumentHeader SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail1_SampleItem3Vat23 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail2_SampleItem4Vat23 { get; protected set; }
        public DocumentDetail SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail3_SampleItem5Vat13 { get; protected set; }

        public Item SampleItemVat23 { get; protected set; }
        public Item SampleItem2Vat23 { get; protected set; }
        public Item SampleItem3Vat23 { get; protected set; }
        public Item SampleItem4Vat23 { get; protected set; }
        public Item SampleItem5Vat13 { get; protected set; }
        public PriceCatalogDetail SampleItemPriceCatalogDetail { get; protected set; }
        public PriceCatalogDetail SampleItem2PriceCatalogDetail { get; protected set; }
        public PriceCatalogDetail SampleItem3PriceCatalogDetail { get; protected set; }
        public PriceCatalogDetail SampleItem4PriceCatalogDetail { get; protected set; }
        public PriceCatalogDetail SampleItem5PriceCatalogDetail { get; protected set; }
        public Barcode SampleItemCodeBarcode { get; protected set; }
        public Barcode SampleItemBarcode { get; protected set; }
        public Barcode SampleItemWeightedBarcodeValue { get; protected set; }
        public Barcode SampleItemWeightedBarcodeQty { get; protected set; }
        public Barcode SampleItem2CodeBarcode { get; protected set; }
        public Barcode SampleItem2Barcode { get; protected set; }
        public Barcode SampleItem3CodeBarcode { get; protected set; }
        public Barcode SampleItem3Barcode { get; protected set; }
        public Barcode SampleItem4CodeBarcode { get; protected set; }
        public Barcode SampleItem4Barcode { get; protected set; }
        public Barcode SampleItem5CodeBarcode { get; protected set; }
        public Barcode SampleItem5Barcode { get; protected set; }

        public IPlatformRoundingHandler PlatformRoundingHandler { get; protected set; }

        public const string SAMPLE_ITEM_VAT23_CODE = "18";
        public const string SAMPLE_ITEM_VAT23_BARCODE = "5201277250418";
        public const decimal SAMPLE_ITEM_VAT23_PRICE_WITH_VAT = 2.5m;
        public const decimal SAMPLE_ITEM_VAT23_DATE_PRICE_WITH_VAT = 1.5m;
        public const decimal SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT / 1.23m;
        public const string DEFAULT_CUSTOMER_TAXCODE = "999999999";
        public const string LOYALTY_CUSTOMER_TAXCODE = "999717110";
        public const string WEIGHTED_BARCODE_VALUE_PREFIX = "21";
        public const string WEIGHTED_BARCODE_QTY_PREFIX = "26";
        public const decimal SAMPLE_ITEM_VAT23_WEIGHTED_VALUE = 3.5m;
        public const decimal SAMPLE_ITEM_VAT23_WEIGHTED_QTY = 1.540m;
        public const string WEIGHTED_BARCODE_VALUE_PLU = "04350";
        public const string SAMPLE_ITEM_VAT23_WEIGHTED_BARCODE_VALUE = "2104350000000";//"IIIIIVVVvvX";
        public const string WEIGHTED_BARCODE_QTY_PLU = "04351";
        public const string SAMPLE_ITEM_VAT23_WEIGHTED_BARCODE_QTY = "2604351000000"; //IIIIIQQqqqX
        public const string SAMPLE_ITEM_VAT23_SCANNED_WEIGHTED_BARCODE_VALUE = "2104350003500";//"IIIIIVVVvvX";
        public const string SAMPLE_ITEM_VAT23_SCANNED_WEIGHTED_BARCODE_QTY = "2604351015400"; //IIIIIQQqqqX
        public const string LOYALTY_CUSTOMER_CARD = "1031001002";
        public const string LOYALTY_CUSTOMER_SCANNED_CARD = "103-100-100-2"; //XCCCXCCCXC
        public const string CUSTOMER_CARD_PREFIX = "103";


        public const string SAMPLE_ITEM_2_VAT23_BARCODE = "5231376263678";
        public const string SAMPLE_ITEM_2_VAT23_CODE = "67";
        public const decimal SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT = 4.5m;
        public const decimal SAMPLE_ITEM_2_VAT23_DATE_PRICE_WITH_VAT = 3.2m;
        public const decimal SAMPLE_ITEM_2_VAT23_PRICE_WITHOUT_VAT = SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT / 1.23m;

        public const string SAMPLE_ITEM_3_VAT23_BARCODE = "5231376263333";
        public const string SAMPLE_ITEM_3_VAT23_CODE = "133";
        public const decimal SAMPLE_ITEM_3_VAT23_PRICE_WITH_VAT = 3m;
        public const decimal SAMPLE_ITEM_3_VAT23_PRICE_WITHOUT_VAT = SAMPLE_ITEM_3_VAT23_PRICE_WITH_VAT / 1.23m;

        public const string SAMPLE_ITEM_4_VAT23_BARCODE = "5231376264444";
        public const string SAMPLE_ITEM_4_VAT23_CODE = "134";
        public const decimal SAMPLE_ITEM_4_VAT23_PRICE_WITH_VAT = 8m;
        public const decimal SAMPLE_ITEM_4_VAT23_PRICE_WITHOUT_VAT = SAMPLE_ITEM_4_VAT23_PRICE_WITH_VAT / 1.23m;

        public const string SAMPLE_ITEM_5_VAT13_BARCODE = "5231376265555";
        public const string SAMPLE_ITEM_5_VAT13_CODE = "135";
        public const decimal SAMPLE_ITEM_5_VAT13_PRICE_WITH_VAT = 6m;
        public const decimal SAMPLE_ITEM_5_VAT13_PRICE_WITHOUT_VAT = SAMPLE_ITEM_5_VAT13_PRICE_WITH_VAT/ 1.13m;

        /// <summary>
        /// Clears and rebuilds the fixture
        /// </summary>
        public void Setup()
        {
            if (MemorySessionManager != null)
            {
                MemorySessionManager.ClearAllSessions();
            }

           

            MemorySessionManager = new MockSessionManager();

            Owner = new CompanyNew(MemorySessionManager.GetSession<CompanyNew>());
            Owner.CompanyName = "Test Company";

            AppSettings = new OwnerApplicationSettings(MemorySessionManager.GetSession<OwnerApplicationSettings>());
            AppSettings.PadItemCodes = false;
            AppSettings.PadBarcodes = false;
            AppSettings.DisplayDigits = 2;
            AppSettings.ComputeDigits = 2;
            AppSettings.ComputeValueDigits = 4;
            AppSettings.SupportLoyalty = true;

            PlatformRoundingHandler = new PlatformRoundingHandler();
            PlatformRoundingHandler.SetOwnerApplicationSettings(AppSettings);

            Trader defaultCustomerTrader = new Trader(MemorySessionManager.GetSession<Trader>());
            defaultCustomerTrader.TaxCode = DEFAULT_CUSTOMER_TAXCODE;

            DefaultCustomer = new Customer(MemorySessionManager.GetSession<Customer>());
            DefaultCustomer.Code = "1";
            DefaultCustomer.CompanyName = "Default Customer";
            DefaultCustomer.Trader = defaultCustomerTrader.Oid;

            Trader loyaltyCustomerTrader = new Trader(MemorySessionManager.GetSession<Trader>());
            loyaltyCustomerTrader.TaxCode = LOYALTY_CUSTOMER_TAXCODE;

            this.LoyaltyCustomer = new Customer(MemorySessionManager.GetSession<Customer>());
            LoyaltyCustomer.Code = "2";
            LoyaltyCustomer.CompanyName = "Loyalty Customer";
            LoyaltyCustomer.Trader = loyaltyCustomerTrader.Oid;
            LoyaltyCustomer.CardID = LOYALTY_CUSTOMER_CARD;

            MinistryDocumentType ministryDocumentType = new MinistryDocumentType(MemorySessionManager.GetSession<MinistryDocumentType>());
            ministryDocumentType.Code = "1";
            ministryDocumentType.DocumentValueFactor = Enumerations.eDocumentValueFactor.PLUS;

            ReceiptDocumentType = new DocumentType(MemorySessionManager.GetSession<DocumentType>());
            ReceiptDocumentType.IsForWholesale = false;
            ReceiptDocumentType.SupportLoyalty = true;
            ReceiptDocumentType.QuantityFactor = 1;
            ReceiptDocumentType.ValueFactor = 1;

            VatCat23 = new VatCategory(MemorySessionManager.GetSession<VatCategory>());
            VatCat23.Code = "23";
            VatCat23.MinistryVatCategoryCode = Enumerations.eMinistryVatCategoryCode.C;

            VatCat13 = new VatCategory(MemorySessionManager.GetSession<VatCategory>());
            VatCat13.Code = "13";
            VatCat13.MinistryVatCategoryCode = Enumerations.eMinistryVatCategoryCode.B;

            SampleItemCodeBarcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItemCodeBarcode.Code = SAMPLE_ITEM_VAT23_CODE;

            SampleItemBarcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItemBarcode.Code = SAMPLE_ITEM_VAT23_BARCODE;

            SampleItemWeightedBarcodeValue = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItemWeightedBarcodeValue.Code = SAMPLE_ITEM_VAT23_WEIGHTED_BARCODE_VALUE;

            SampleItemWeightedBarcodeQty = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItemWeightedBarcodeQty.Code = SAMPLE_ITEM_VAT23_WEIGHTED_BARCODE_QTY;

            SampleItem2CodeBarcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItem2CodeBarcode.Code = SAMPLE_ITEM_2_VAT23_CODE;

            SampleItem2Barcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItem2Barcode.Code = SAMPLE_ITEM_2_VAT23_BARCODE;


            SampleItem3CodeBarcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItem3CodeBarcode.Code = SAMPLE_ITEM_3_VAT23_CODE;
                      
            SampleItem3Barcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItem3Barcode.Code = SAMPLE_ITEM_3_VAT23_BARCODE;

            SampleItem4CodeBarcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItem4CodeBarcode.Code = SAMPLE_ITEM_4_VAT23_CODE;
                      
            SampleItem4Barcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItem4Barcode.Code = SAMPLE_ITEM_4_VAT23_BARCODE;

            SampleItem5CodeBarcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItem5CodeBarcode.Code = SAMPLE_ITEM_5_VAT13_CODE;
                      
            SampleItem5Barcode = new Barcode(MemorySessionManager.GetSession<Barcode>());
            SampleItem5Barcode.Code = SAMPLE_ITEM_5_VAT13_BARCODE;

            this.SampleItemVat23 = new Item(MemorySessionManager.GetSession<Item>());
            this.SampleItemVat23.Code = SAMPLE_ITEM_VAT23_CODE;
            this.SampleItemVat23.VatCategory = VatCat23.Oid;
            this.SampleItemVat23.Name = "Test item";
            this.SampleItemVat23.Points = 0;
            this.SampleItemVat23.Owner = Owner.Oid;

            this.SampleItem2Vat23 = new Item(MemorySessionManager.GetSession<Item>());
            this.SampleItem2Vat23.Code = SAMPLE_ITEM_2_VAT23_CODE;
            this.SampleItem2Vat23.VatCategory = VatCat23.Oid;
            this.SampleItem2Vat23.Name = "Test item 2";
            this.SampleItem2Vat23.Points = 0;
            this.SampleItem2Vat23.Owner = Owner.Oid;

            this.SampleItem3Vat23 = new Item(MemorySessionManager.GetSession<Item>());
            this.SampleItem3Vat23.Code = SAMPLE_ITEM_3_VAT23_CODE;
            this.SampleItem3Vat23.VatCategory = VatCat23.Oid;
            this.SampleItem3Vat23.Name = "Test item 3";
            this.SampleItem3Vat23.Points = 0;
            this.SampleItem3Vat23.Owner = Owner.Oid;

            this.SampleItem4Vat23 = new Item(MemorySessionManager.GetSession<Item>());
            this.SampleItem4Vat23.Code = SAMPLE_ITEM_4_VAT23_CODE;
            this.SampleItem4Vat23.VatCategory = VatCat23.Oid;
            this.SampleItem4Vat23.Name = "Test item 4";
            this.SampleItem4Vat23.Points = 0;
            this.SampleItem4Vat23.Owner = Owner.Oid;

            this.SampleItem5Vat13 = new Item(MemorySessionManager.GetSession<Item>());
            this.SampleItem5Vat13.Code = SAMPLE_ITEM_5_VAT13_CODE;
            this.SampleItem5Vat13.VatCategory = VatCat13.Oid;
            this.SampleItem5Vat13.Name = "Test item 4";
            this.SampleItem5Vat13.Points = 0;
            this.SampleItem5Vat13.Owner = Owner.Oid;

            this.MeasurementUnitNonDecimal = new MeasurementUnit(MemorySessionManager.GetSession<MeasurementUnit>());
            this.MeasurementUnitNonDecimal.Code = "1";
            this.MeasurementUnitNonDecimal.Description = "Units";
            this.MeasurementUnitNonDecimal.SupportDecimal = false;

            this.MeasurementUnitDecimal = new MeasurementUnit(MemorySessionManager.GetSession<MeasurementUnit>());
            this.MeasurementUnitDecimal.Code = "2";
            this.MeasurementUnitDecimal.Description = "Kilos";
            this.MeasurementUnitDecimal.SupportDecimal = true;

            BarcodeType bct = new BarcodeType(MemorySessionManager.GetSession<BarcodeType>());
            bct.IsWeighed = false;

            BarcodeType bctWeightedValue = new BarcodeType(MemorySessionManager.GetSession<BarcodeType>());
            bctWeightedValue.IsWeighed = true;
            bctWeightedValue.Prefix = WEIGHTED_BARCODE_VALUE_PREFIX;
            bctWeightedValue.Mask = "IIIIIVVVvvX";

            BarcodeType bctWeightedQty = new BarcodeType(MemorySessionManager.GetSession<BarcodeType>());
            bctWeightedQty.IsWeighed = true;
            bctWeightedQty.Prefix = WEIGHTED_BARCODE_QTY_PREFIX;
            bctWeightedQty.Mask = "IIIIIQQqqqX";

            BarcodeType bctCustomerCard = new BarcodeType(MemorySessionManager.GetSession<BarcodeType>());
            bctCustomerCard.IsWeighed = false;
            bctCustomerCard.Prefix = CUSTOMER_CARD_PREFIX;
            bctCustomerCard.PrefixIncluded = true;
            bctCustomerCard.Mask = "XCCCXCCCXC";

            ItemBarcode ibcOfSampleItemCode = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItemCode.Barcode = SampleItemCodeBarcode.Oid;
            ibcOfSampleItemCode.Item = this.SampleItemVat23.Oid;
            ibcOfSampleItemCode.MeasurementUnit = MeasurementUnitNonDecimal.Oid;
            ibcOfSampleItemCode.RelationFactor = 1;
            ibcOfSampleItemCode.Type = bct.Oid;

            ItemBarcode ibcOfSampleItemBarcode = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItemBarcode.Barcode = SampleItemBarcode.Oid;
            ibcOfSampleItemBarcode.Item = this.SampleItemVat23.Oid;
            ibcOfSampleItemBarcode.MeasurementUnit = MeasurementUnitNonDecimal.Oid;
            ibcOfSampleItemBarcode.RelationFactor = 1;
            ibcOfSampleItemBarcode.Type = bct.Oid;

            ItemBarcode ibcOfSampleItemWeightedValueBarcode = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItemWeightedValueBarcode.Barcode = SampleItemWeightedBarcodeValue.Oid;
            ibcOfSampleItemWeightedValueBarcode.Item = this.SampleItemVat23.Oid;
            ibcOfSampleItemWeightedValueBarcode.MeasurementUnit = MeasurementUnitDecimal.Oid;
            ibcOfSampleItemWeightedValueBarcode.RelationFactor = 1;
            ibcOfSampleItemWeightedValueBarcode.Type = bctWeightedValue.Oid;
            ibcOfSampleItemWeightedValueBarcode.PluCode = WEIGHTED_BARCODE_VALUE_PLU;
            ibcOfSampleItemWeightedValueBarcode.PluPrefix = WEIGHTED_BARCODE_VALUE_PREFIX;

            ItemBarcode ibcOfSampleItemWeightedQtyBarcode = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItemWeightedQtyBarcode.Barcode = SampleItemWeightedBarcodeQty.Oid;
            ibcOfSampleItemWeightedQtyBarcode.Item = this.SampleItemVat23.Oid;
            ibcOfSampleItemWeightedQtyBarcode.MeasurementUnit = MeasurementUnitDecimal.Oid;
            ibcOfSampleItemWeightedQtyBarcode.RelationFactor = 1;
            ibcOfSampleItemWeightedQtyBarcode.Type = bctWeightedQty.Oid;
            ibcOfSampleItemWeightedQtyBarcode.PluCode = WEIGHTED_BARCODE_QTY_PLU;
            ibcOfSampleItemWeightedQtyBarcode.PluPrefix = WEIGHTED_BARCODE_QTY_PREFIX;

            ItemBarcode ibcOfSampleItem2Code = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItem2Code.Barcode = SampleItem2CodeBarcode.Oid;
            ibcOfSampleItem2Code.Item = this.SampleItem2Vat23.Oid;
            ibcOfSampleItem2Code.MeasurementUnit = MeasurementUnitNonDecimal.Oid;
            ibcOfSampleItem2Code.RelationFactor = 1;
            ibcOfSampleItem2Code.Type = bct.Oid;

            ItemBarcode ibcOfSampleItem2Barcode = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItem2Barcode.Barcode = SampleItem2Barcode.Oid;
            ibcOfSampleItem2Barcode.Item = this.SampleItem2Vat23.Oid;
            ibcOfSampleItem2Barcode.MeasurementUnit = MeasurementUnitNonDecimal.Oid;
            ibcOfSampleItem2Barcode.RelationFactor = 1;
            ibcOfSampleItem2Barcode.Type = bct.Oid;

            ItemBarcode ibcOfSampleItem3Code = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItem3Code.Barcode = SampleItem2CodeBarcode.Oid;
            ibcOfSampleItem3Code.Item = this.SampleItem2Vat23.Oid;
            ibcOfSampleItem3Code.MeasurementUnit = MeasurementUnitNonDecimal.Oid;
            ibcOfSampleItem3Code.RelationFactor = 1;
            ibcOfSampleItem3Code.Type = bct.Oid;

            ItemBarcode ibcOfSampleItem3Barcode = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItem3Barcode.Barcode = SampleItem3Barcode.Oid;
            ibcOfSampleItem3Barcode.Item = this.SampleItem3Vat23.Oid;
            ibcOfSampleItem3Barcode.MeasurementUnit = MeasurementUnitNonDecimal.Oid;
            ibcOfSampleItem3Barcode.RelationFactor = 1;
            ibcOfSampleItem3Barcode.Type = bct.Oid;

            ItemBarcode ibcOfSampleItem4Barcode = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItem4Barcode.Barcode = SampleItem4Barcode.Oid;
            ibcOfSampleItem4Barcode.Item = this.SampleItem4Vat23.Oid;
            ibcOfSampleItem4Barcode.MeasurementUnit = MeasurementUnitNonDecimal.Oid;
            ibcOfSampleItem4Barcode.RelationFactor = 1;
            ibcOfSampleItem4Barcode.Type = bct.Oid;

            ItemBarcode ibcOfSampleItem5Barcode = new ItemBarcode(MemorySessionManager.GetSession<ItemBarcode>());
            ibcOfSampleItem5Barcode.Barcode = SampleItem5Barcode.Oid;
            ibcOfSampleItem5Barcode.Item = this.SampleItem5Vat13.Oid;
            ibcOfSampleItem5Barcode.MeasurementUnit = MeasurementUnitNonDecimal.Oid;
            ibcOfSampleItem5Barcode.RelationFactor = 1;
            ibcOfSampleItem5Barcode.Type = bct.Oid;

            this.RootPriceCatalog = new PriceCatalog(MemorySessionManager.GetSession<PriceCatalog>());
            this.RootPriceCatalog.Code = "1";
            this.RootPriceCatalog.Description = "Root Catalog";
            this.RootPriceCatalog.StartDate = DateTime.MinValue.AddDays(100);
            this.RootPriceCatalog.EndDate = DateTime.MaxValue.AddDays(-100);

            this.PriceCatalogPolicy = new PriceCatalogPolicy(MemorySessionManager.GetSession<PriceCatalogPolicy>());
            this.PriceCatalogPolicy.Code = "1";
            this.PriceCatalogPolicy.Description = "Default Price Catalog Policy";
            this.PriceCatalogPolicy.PriceCatalogPolicyDetails.Add(new PriceCatalogPolicyDetail(MemorySessionManager.GetSession<PriceCatalogPolicy>()) { PriceCatalog = this.RootPriceCatalog.Oid, Sort = 0, PriceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, IsDefault = true});

            this.SampleItemPriceCatalogDetail = new PriceCatalogDetail(MemorySessionManager.GetSession<PriceCatalogDetail>());
            this.SampleItemPriceCatalogDetail.Barcode = SampleItemCodeBarcode.Oid;
            this.SampleItemPriceCatalogDetail.Item = this.SampleItemVat23.Oid;
            this.SampleItemPriceCatalogDetail.PriceCatalog = RootPriceCatalog.Oid;
            this.SampleItemPriceCatalogDetail.VATIncluded = true;
            this.SampleItemPriceCatalogDetail.DatabaseValue = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT;
            PriceCatalogDetailTimeValue sampleItemPriceCatalogDetailTimeValue = new PriceCatalogDetailTimeValue(this.SampleItemPriceCatalogDetail.Session);
            sampleItemPriceCatalogDetailTimeValue.PriceCatalogDetail = this.SampleItemPriceCatalogDetail.Oid;

            this.SampleItem2PriceCatalogDetail = new PriceCatalogDetail(MemorySessionManager.GetSession<PriceCatalogDetail>());
            this.SampleItem2PriceCatalogDetail.Barcode = SampleItem2CodeBarcode.Oid;
            this.SampleItem2PriceCatalogDetail.Item = this.SampleItem2Vat23.Oid;
            this.SampleItem2PriceCatalogDetail.PriceCatalog = RootPriceCatalog.Oid;
            this.SampleItem2PriceCatalogDetail.VATIncluded = true;
            this.SampleItem2PriceCatalogDetail.DatabaseValue = SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT;

            this.SampleItem3PriceCatalogDetail = new PriceCatalogDetail(MemorySessionManager.GetSession<PriceCatalogDetail>());
            this.SampleItem3PriceCatalogDetail.Barcode = SampleItem3CodeBarcode.Oid;
            this.SampleItem3PriceCatalogDetail.Item = this.SampleItem3Vat23.Oid;
            this.SampleItem3PriceCatalogDetail.PriceCatalog = RootPriceCatalog.Oid;
            this.SampleItem3PriceCatalogDetail.VATIncluded = true;
            this.SampleItem3PriceCatalogDetail.DatabaseValue = SAMPLE_ITEM_3_VAT23_PRICE_WITH_VAT;

            this.SampleItem4PriceCatalogDetail = new PriceCatalogDetail(MemorySessionManager.GetSession<PriceCatalogDetail>());
            this.SampleItem4PriceCatalogDetail.Barcode = SampleItem4CodeBarcode.Oid;
            this.SampleItem4PriceCatalogDetail.Item = this.SampleItem4Vat23.Oid;
            this.SampleItem4PriceCatalogDetail.PriceCatalog = RootPriceCatalog.Oid;
            this.SampleItem4PriceCatalogDetail.VATIncluded = true;
            this.SampleItem4PriceCatalogDetail.DatabaseValue = SAMPLE_ITEM_4_VAT23_PRICE_WITH_VAT;

            this.SampleItem5PriceCatalogDetail = new PriceCatalogDetail(MemorySessionManager.GetSession<PriceCatalogDetail>());
            this.SampleItem5PriceCatalogDetail.Barcode = SampleItem5CodeBarcode.Oid;
            this.SampleItem5PriceCatalogDetail.Item = this.SampleItem5Vat13.Oid;
            this.SampleItem5PriceCatalogDetail.PriceCatalog = RootPriceCatalog.Oid;
            this.SampleItem5PriceCatalogDetail.VATIncluded = true;
            this.SampleItem5PriceCatalogDetail.DatabaseValue = SAMPLE_ITEM_5_VAT13_PRICE_WITH_VAT;

            this.SubCatalog = new PriceCatalog(MemorySessionManager.GetSession<PriceCatalog>());
            this.SubCatalog.Code = "2";
            this.SubCatalog.Description = "Sub Catalog";
            this.SubCatalog.StartDate = DateTime.MinValue.AddDays(100);
            this.SubCatalog.EndDate = DateTime.MaxValue.AddDays(-100);
            this.SubCatalog.ParentCatalogOid = RootPriceCatalog.Oid;

            this.SubPriceCatalogPolicy = new PriceCatalogPolicy(MemorySessionManager.GetSession<PriceCatalogPolicy>());
            this.SubPriceCatalogPolicy.Code = "2";
            this.SubPriceCatalogPolicy.Description = "Sub Price Catalog Policy";
            this.SubPriceCatalogPolicy.PriceCatalogPolicyDetails.Add(new PriceCatalogPolicyDetail(MemorySessionManager.GetSession<PriceCatalogPolicy>()) { PriceCatalog = this.SubCatalog.Oid, Sort = 0, PriceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, IsDefault = true });



            VatLevel vatLevel = new VatLevel(MemorySessionManager.GetSession<VatLevel>());
            vatLevel.Code = "1";
            vatLevel.Description = "Normal";

            VatFactor vatFactor23 = new VatFactor(MemorySessionManager.GetSession<VatFactor>());
            vatFactor23.Code = "Factor 23";
            vatFactor23.VatCategory = VatCat23.Oid;
            vatFactor23.VatLevel = vatLevel.Oid;
            vatFactor23.Factor = 0.23m;

            VatFactor vatFactor13 = new VatFactor(MemorySessionManager.GetSession<VatFactor>());
            vatFactor13.Code = "Factor 13";
            vatFactor13.VatCategory = VatCat13.Oid;
            vatFactor13.VatLevel = vatLevel.Oid;
            vatFactor13.Factor = 0.13m;

            Address defaultStoreAddress = new Address(MemorySessionManager.GetSession<Address>());
            defaultStoreAddress.VatLevel = vatLevel.Oid;

            this.DefaultStore = new Store(MemorySessionManager.GetSession<Store>());
            this.DefaultStore.Code = "1";
            this.DefaultStore.DefaultPriceCatalog = RootPriceCatalog.Oid;
            this.DefaultStore.Owner = Owner.Oid;
            this.DefaultStore.Address = defaultStoreAddress.Oid;

            Address secondaryStoreAddress = new Address(MemorySessionManager.GetSession<Address>());
            secondaryStoreAddress.VatLevel = vatLevel.Oid;

            this.SecondaryStore = new Store(MemorySessionManager.GetSession<Store>());
            this.SecondaryStore.Code = "2";
            this.SecondaryStore.DefaultPriceCatalog = RootPriceCatalog.Oid;
            this.SecondaryStore.Owner = Owner.Oid;
            this.SecondaryStore.Address = secondaryStoreAddress.Oid;

            this.ReceiptDocumentSeries = new DocumentSeries(MemorySessionManager.GetSession<DocumentSeries>());
            this.ReceiptDocumentSeries.Code = "1";
            this.ReceiptDocumentSeries.HasAutomaticNumbering = true;

            StoreDocumentSeriesType sdst = new StoreDocumentSeriesType(MemorySessionManager.GetSession<StoreDocumentSeriesType>());
            sdst.DocumentSeries = ReceiptDocumentSeries.Oid;
            sdst.DocumentType = ReceiptDocumentType.Oid;

            DocumentSequence receiptSequence = new DocumentSequence(MemorySessionManager.GetSession<DocumentSequence>());
            receiptSequence.DocumentSeries = ReceiptDocumentSeries.Oid;
            receiptSequence.DocumentNumber = 0;

            StorePriceListDefaultStoreRootCatalog = new StorePriceList(MemorySessionManager.GetSession<StorePriceList>());
            StorePriceListDefaultStoreRootCatalog.PriceList = RootPriceCatalog.Oid;
            StorePriceListDefaultStoreRootCatalog.Store = DefaultStore.Oid;

            StorePriceListDefaultStoreSubCatalog = new StorePriceList(MemorySessionManager.GetSession<StorePriceList>());
            StorePriceListDefaultStoreSubCatalog.PriceList = SubCatalog.Oid;
            StorePriceListDefaultStoreSubCatalog.Store = DefaultStore.Oid;

            StorePriceListSecondaryStoreRootCatalog = new StorePriceList(MemorySessionManager.GetSession<StorePriceList>());
            StorePriceListSecondaryStoreRootCatalog.PriceList = RootPriceCatalog.Oid;
            StorePriceListSecondaryStoreRootCatalog.Store = SecondaryStore.Oid;

            StorePriceListSecondaryStoreSubCatalog = new StorePriceList(MemorySessionManager.GetSession<StorePriceList>());
            StorePriceListSecondaryStoreSubCatalog.PriceList = SubCatalog.Oid;
            StorePriceListSecondaryStoreSubCatalog.Store = SecondaryStore.Oid;

            this.DocumentStatusThatTakesSequence = new DocumentStatus(MemorySessionManager.GetSession<DocumentStatus>());
            this.DocumentStatusThatTakesSequence.Code = "1";
            this.DocumentStatusThatTakesSequence.Description = "Closed";
            this.DocumentStatusThatTakesSequence.TakeSequence = true;


            POS1 = new ITS.POS.Model.Settings.POS(MemorySessionManager.GetSession<ITS.POS.Model.Settings.POS>());
            POS1.DefaultCustomer = DefaultCustomer.Oid;
            POS1.ID = 1;
            POS1.Store = DefaultStore.Oid;


            CurrentUserPOS1 = new User(MemorySessionManager.GetSession<User>());
            CurrentUserPOS1.UserName = "User 1";
            CurrentUserPOS1.Password = "1234";
            CurrentUserPOS1.POSUserName = "1234";
            CurrentUserPOS1.POSUserName = "1234";
            CurrentUserPOS1.POSUserLevel = Enumerations.ePOSUserLevel.LEVEL4;


            UserTypeAccess storeUta = new UserTypeAccess(MemorySessionManager.GetSession<UserTypeAccess>());
            storeUta.EntityOid = DefaultStore.Oid;
            storeUta.EntityType = DefaultStore.GetType().Name;
            storeUta.User = CurrentUserPOS1.Oid;

            UserTypeAccess companyUta = new UserTypeAccess(MemorySessionManager.GetSession<UserTypeAccess>());
            companyUta.EntityOid = Owner.Oid;
            companyUta.EntityType = Owner.GetType().Name;
            companyUta.User = CurrentUserPOS1.Oid;

            DailyTotalsPOS1 = new DailyTotals(MemorySessionManager.GetSession<DailyTotals>());
            DailyTotalsPOS1.FiscalDateOpen = true;
            DailyTotalsPOS1.FiscalDate = DateTime.Now;
            DailyTotalsPOS1.POS = POS1.Oid;
            DailyTotalsPOS1.POSID = POS1.ID;
            DailyTotalsPOS1.Store = DefaultStore.Oid;
            DailyTotalsPOS1.StoreCode = DefaultStore.Code;
            DailyTotalsPOS1.ZReportNumber = 1;

            UserDailyTotalsPOS1 = new UserDailyTotals(MemorySessionManager.GetSession<UserDailyTotals>());
            UserDailyTotalsPOS1.DailyTotals = DailyTotalsPOS1;
            UserDailyTotalsPOS1.FiscalDate = DailyTotalsPOS1.FiscalDate;
            UserDailyTotalsPOS1.POS = POS1.Oid;
            UserDailyTotalsPOS1.POSID = POS1.ID;
            UserDailyTotalsPOS1.Store = DefaultStore.Oid;
            UserDailyTotalsPOS1.User = CurrentUserPOS1.Oid;


            LineDiscountPercentage = new DiscountType(MemorySessionManager.GetSession<DiscountType>());
            LineDiscountPercentage.Code = "1";
            LineDiscountPercentage.Description = "Discount %";
            LineDiscountPercentage.DiscardsOtherDiscounts = false;
            LineDiscountPercentage.eDiscountType = Enumerations.eDiscountType.PERCENTAGE;
            LineDiscountPercentage.Priority = 1;

            LineDiscountValue = new DiscountType(MemorySessionManager.GetSession<DiscountType>());
            LineDiscountValue.Code = "2";
            LineDiscountValue.Description = "Discount (Euro)";
            LineDiscountValue.DiscardsOtherDiscounts = false;
            LineDiscountValue.eDiscountType = Enumerations.eDiscountType.VALUE;
            LineDiscountValue.Priority = 2;

            HeaderDiscountPercentage = new DiscountType(MemorySessionManager.GetSession<DiscountType>());
            HeaderDiscountPercentage.Code = "3";
            HeaderDiscountPercentage.Description = "Header Discount %";
            HeaderDiscountPercentage.DiscardsOtherDiscounts = false;
            HeaderDiscountPercentage.eDiscountType = Enumerations.eDiscountType.PERCENTAGE;
            HeaderDiscountPercentage.IsHeaderDiscount = true;
            HeaderDiscountPercentage.Priority = 99999999;

            HeaderDiscountValue = new DiscountType(MemorySessionManager.GetSession<DiscountType>());
            HeaderDiscountValue.Code = "4";
            HeaderDiscountValue.Description = "Header Discount (Euro)";
            HeaderDiscountValue.DiscardsOtherDiscounts = false;
            HeaderDiscountValue.eDiscountType = Enumerations.eDiscountType.VALUE;
            HeaderDiscountValue.Priority = 99;
            HeaderDiscountValue.IsHeaderDiscount = true;

            DefaultPaymentMethod = new PaymentMethod(MemorySessionManager.GetSession<PaymentMethod>());
            DefaultPaymentMethod.CanExceedTotal = true;
            DefaultPaymentMethod.Description = "Cash";
            DefaultPaymentMethod.GiveChange = true;
            DefaultPaymentMethod.IncreasesDrawerAmount = true;
            DefaultPaymentMethod.OpensDrawer = true;
            DefaultPaymentMethod.PaymentMethodType = ePaymentMethodType.CASH;
            DefaultPaymentMethod.Code = "1";

            LoyaltyPaymentMethod = new PaymentMethod(MemorySessionManager.GetSession<PaymentMethod>());
            LoyaltyPaymentMethod.CanExceedTotal = false;
            LoyaltyPaymentMethod.Description = "Loyalty";
            LoyaltyPaymentMethod.GiveChange = false;
            LoyaltyPaymentMethod.IncreasesDrawerAmount = false;
            LoyaltyPaymentMethod.OpensDrawer = false;
            LoyaltyPaymentMethod.PaymentMethodType = ePaymentMethodType.CREDIT;
            LoyaltyPaymentMethod.Code = "2";

            //Level 0 Item Categories
            RootItemCategory = new ItemCategory(MemorySessionManager.GetSession<ItemCategory>());
            RootItemCategory.Code = "00";
            RootItemCategory.Description = "Root Category";

            //Level 1 Item Categories
            SubItemCategory1 = new ItemCategory(MemorySessionManager.GetSession<ItemCategory>());
            SubItemCategory1.Code = "00-01";
            SubItemCategory1.Description = "Sub Item Category 1";
            SubItemCategory1.ParentOid = RootItemCategory.Oid;

            SubItemCategory2 = new ItemCategory(MemorySessionManager.GetSession<ItemCategory>());
            SubItemCategory2.Code = "00-02";
            SubItemCategory2.Description = "Sub Item Category 2";
            SubItemCategory2.ParentOid = RootItemCategory.Oid;

            //Level 2 Item Categories
            SubSubItemCategory1_1 = new ItemCategory(MemorySessionManager.GetSession<ItemCategory>());
            SubSubItemCategory1_1.Code = "00-01-01";
            SubSubItemCategory1_1.Description = "Sub Sub Item Category 1-1";
            SubSubItemCategory1_1.ParentOid = SubItemCategory1.Oid;

            SubSubItemCategory1_2 = new ItemCategory(MemorySessionManager.GetSession<ItemCategory>());
            SubSubItemCategory1_2.Code = "00-01-02";
            SubSubItemCategory1_2.Description = "Sub Item Category 1-2";
            SubSubItemCategory1_2.ParentOid = SubItemCategory1.Oid;

            SubSubItemCategory2_1 = new ItemCategory(MemorySessionManager.GetSession<ItemCategory>());
            SubSubItemCategory2_1.Code = "00-02-01";
            SubSubItemCategory2_1.Description = "Sub Sub Item Category 2-1";
            SubSubItemCategory2_1.ParentOid = SubItemCategory2.Oid;

            SubSubItemCategory2_2 = new ItemCategory(MemorySessionManager.GetSession<ItemCategory>());
            SubSubItemCategory2_2.Code = "00-02-02";
            SubSubItemCategory2_2.Description = "Sub Sub Item Category 2-2";
            SubSubItemCategory2_2.ParentOid = SubItemCategory2.Oid;


            //Level 0 Customer Categories
            RootCustomerCategory = new CustomerCategory(MemorySessionManager.GetSession<CustomerCategory>());
            RootCustomerCategory.Code = "00";
            RootCustomerCategory.Description = "Root Category";

            //Level 1 Customer Categories
            SubCustomerCategory1 = new CustomerCategory(MemorySessionManager.GetSession<CustomerCategory>());
            SubCustomerCategory1.Code = "00-01";
            SubCustomerCategory1.Description = "Sub Customer Category 1";
            SubCustomerCategory1.ParentOid = RootCustomerCategory.Oid;

            SubCustomerCategory2 = new CustomerCategory(MemorySessionManager.GetSession<CustomerCategory>());
            SubCustomerCategory2.Code = "00-02";
            SubCustomerCategory2.Description = "Sub Customer Category 2";
            SubCustomerCategory2.ParentOid = RootCustomerCategory.Oid;

            //Level 2 Customer Categories
            SubSubCustomerCategory1_1 = new CustomerCategory(MemorySessionManager.GetSession<CustomerCategory>());
            SubSubCustomerCategory1_1.Code = "00-01-01";
            SubSubCustomerCategory1_1.Description = "Sub Sub Customer Category 1-1";
            SubSubCustomerCategory1_1.ParentOid = SubCustomerCategory1.Oid;

            SubSubCustomerCategory1_2 = new CustomerCategory(MemorySessionManager.GetSession<CustomerCategory>());
            SubSubCustomerCategory1_2.Code = "00-01-02";
            SubSubCustomerCategory1_2.Description = "Sub Customer Category 1-2";
            SubSubCustomerCategory1_2.ParentOid = SubCustomerCategory1.Oid;

            SubSubCustomerCategory2_1 = new CustomerCategory(MemorySessionManager.GetSession<CustomerCategory>());
            SubSubCustomerCategory2_1.Code = "00-02-01";
            SubSubCustomerCategory2_1.Description = "Sub Sub Customer Category 2-1";
            SubSubCustomerCategory2_1.ParentOid = SubCustomerCategory2.Oid;

            SubSubCustomerCategory2_2 = new CustomerCategory(MemorySessionManager.GetSession<CustomerCategory>());
            SubSubCustomerCategory2_2.Code = "00-02-02";
            SubSubCustomerCategory2_2.Description = "Sub Sub Customer Category 2-2";
            SubSubCustomerCategory2_2.ParentOid = SubCustomerCategory2.Oid;

            decimal sampleReceiptLineQty = 2;
            decimal sampleReceiptGrossTotal = PlatformRoundingHandler.RoundDisplayValue(sampleReceiptLineQty * SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            decimal sampleReceiptNetTotal = PlatformRoundingHandler.RoundDisplayValue(sampleReceiptLineQty * SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceiptVatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceiptGrossTotal - sampleReceiptNetTotal);

            SampleReceiptOpenWith1LinePOS1 = new DocumentHeader(MemorySessionManager.GetSession<DocumentHeader>())
            {
                Division = eDivision.Sales,
                POS = POS1.Oid,
                CreatedByDevice = POS1.ToString(),
                DocumentType = ReceiptDocumentType.Oid,
                DocumentSeries = ReceiptDocumentSeries.Oid,
                Store = this.DefaultStore.Oid,
                ReferenceCompany = this.DefaultStore.ReferenceCompany,
                MainCompany = this.DefaultStore.Owner,
                Customer = this.DefaultCustomer.Oid,
                CustomerCode = this.DefaultCustomer.Code,
                CustomerName = this.DefaultCustomer.CompanyName,
                Status = DocumentStatusThatTakesSequence.Oid,
                //PriceCatalog = RootPriceCatalog.Oid,
                PriceCatalogPolicy = PriceCatalogPolicy.Oid,
                UserDailyTotals = UserDailyTotalsPOS1,
                DeliveryAddress = "-",
                Source = DocumentSource.POS,
                GrossTotal = sampleReceiptGrossTotal,
                GrossTotalBeforeDiscount = sampleReceiptGrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceiptGrossTotal,
                NetTotalBeforeDiscount = sampleReceiptNetTotal,
                NetTotal = sampleReceiptNetTotal,
                TotalVatAmountBeforeDiscount = sampleReceiptVatAmount,
                TotalVatAmount = sampleReceiptVatAmount,
                TotalQty = sampleReceiptLineQty,
                IsOpen = true,
            };

            SampleReceiptOpenWith1LinePOS1Detail_SampleItemVat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                Barcode = SampleItemBarcode.Oid,
                DocumentHeader = SampleReceiptOpenWith1LinePOS1,
                FinalUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItemVat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 1,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceiptLineQty,
                PriceCatalogDetail = SampleItemPriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceiptLineQty,
                GrossTotal = sampleReceiptGrossTotal,
                GrossTotalBeforeDiscount = sampleReceiptGrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceiptGrossTotal,
                NetTotal = sampleReceiptNetTotal,
                NetTotalBeforeDiscount = sampleReceiptNetTotal,
                TotalVatAmount = sampleReceiptVatAmount,
                TotalVatAmountBeforeDiscount = sampleReceiptVatAmount,
            };

            ////Sample Receipt with multiple lines

            decimal sampleReceipt2Detail1Qty = 2;
            decimal sampleReceipt2Detail2Qty = 5;
            decimal sampleReceipt2Detail3Qty = 7;

            decimal sampleReceipt2Detail1GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt2Detail1Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            decimal sampleReceipt2Detail1NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt2Detail1Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceipt2Detail1VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt2Detail1GrossTotal - sampleReceipt2Detail1NetTotal);

            decimal sampleReceipt2Detail2GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt2Detail2Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            decimal sampleReceipt2Detail2NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt2Detail2Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceipt2Detail2VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt2Detail2GrossTotal - sampleReceipt2Detail2NetTotal);

            decimal sampleReceipt2Detail3GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt2Detail3Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            decimal sampleReceipt2Detail3NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt2Detail3Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceipt2Detail3VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt2Detail3GrossTotal - sampleReceipt2Detail3NetTotal);

            decimal sampleReceipt2GrossTotal = sampleReceipt2Detail1GrossTotal + sampleReceipt2Detail2GrossTotal + sampleReceipt2Detail3GrossTotal;
            decimal sampleReceipt2NetTotal = sampleReceipt2Detail1NetTotal + sampleReceipt2Detail2NetTotal + sampleReceipt2Detail3NetTotal;
            decimal sampleReceipt2VatAmount = sampleReceipt2Detail1VatAmount + sampleReceipt2Detail2VatAmount + sampleReceipt2Detail3VatAmount;

            SampleReceiptOpenWithMultipleLinesPOS1 = new DocumentHeader(MemorySessionManager.GetSession<DocumentHeader>())
            {
                Division = eDivision.Sales,
                POS = POS1.Oid,
                CreatedByDevice = POS1.ToString(),
                DocumentType = ReceiptDocumentType.Oid,
                DocumentSeries = ReceiptDocumentSeries.Oid,
                Store = this.DefaultStore.Oid,
                ReferenceCompany = this.DefaultStore.ReferenceCompany,
                MainCompany = this.DefaultStore.Owner,
                Customer = this.DefaultCustomer.Oid,
                CustomerCode = this.DefaultCustomer.Code,
                CustomerName = this.DefaultCustomer.CompanyName,
                Status = DocumentStatusThatTakesSequence.Oid,
                //PriceCatalog = RootPriceCatalog.Oid,
                PriceCatalogPolicy = PriceCatalogPolicy.Oid,
                UserDailyTotals = UserDailyTotalsPOS1,
                DeliveryAddress = "-",
                Source = DocumentSource.POS,
                GrossTotal = sampleReceipt2GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt2GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt2GrossTotal,
                NetTotalBeforeDiscount = sampleReceipt2NetTotal,
                NetTotal = sampleReceipt2NetTotal,
                TotalVatAmountBeforeDiscount = sampleReceipt2VatAmount,
                TotalVatAmount = sampleReceipt2VatAmount,
                TotalQty = sampleReceipt2Detail1Qty + sampleReceipt2Detail2Qty + sampleReceipt2Detail3Qty,
                IsOpen = true
            };

            SampleReceiptOpenWithMultipleLinesPOS1Detail1_SampleItemVat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                Barcode = SampleItemBarcode.Oid,
                DocumentHeader = SampleReceiptOpenWithMultipleLinesPOS1,
                FinalUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItemVat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 1,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt2Detail1Qty,
                PriceCatalogDetail = SampleItemPriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceipt2Detail1Qty,
                GrossTotal = sampleReceipt2Detail1GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt2Detail1GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt2Detail1GrossTotal,
                NetTotal = sampleReceipt2Detail1NetTotal,
                NetTotalBeforeDiscount = sampleReceipt2Detail1NetTotal,
                TotalVatAmount = sampleReceipt2Detail1VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt2Detail1VatAmount,
            };

            SampleReceiptOpenWithMultipleLinesPOS1Detail2_SampleItemVat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                Barcode = SampleItemBarcode.Oid,
                DocumentHeader = SampleReceiptOpenWithMultipleLinesPOS1,
                FinalUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItemVat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 2,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt2Detail2Qty,
                PriceCatalogDetail = SampleItemPriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceipt2Detail2Qty,
                GrossTotal = sampleReceipt2Detail2GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt2Detail2GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt2Detail2GrossTotal,
                NetTotal = sampleReceipt2Detail2NetTotal,
                NetTotalBeforeDiscount = sampleReceipt2Detail2NetTotal,
                TotalVatAmount = sampleReceipt2Detail2VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt2Detail2VatAmount,
            };

            SampleReceiptOpenWithMultipleLinesPOS1Detail3_SampleItemVat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                Barcode = SampleItemBarcode.Oid,
                DocumentHeader = SampleReceiptOpenWithMultipleLinesPOS1,
                FinalUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItemVat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 3,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt2Detail3Qty,
                PriceCatalogDetail = SampleItemPriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceipt2Detail3Qty,
                GrossTotal = sampleReceipt2Detail3GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt2Detail3GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt2Detail3GrossTotal,
                NetTotal = sampleReceipt2Detail3NetTotal,
                NetTotalBeforeDiscount = sampleReceipt2Detail3NetTotal,
                TotalVatAmount = sampleReceipt2Detail3VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt2Detail3VatAmount
            };
            /////-------------------------------------

            ////Sample Receipt with different items
            decimal sampleReceipt3Detail1Qty = 2;
            decimal sampleReceipt3Detail2Qty = 1;
            decimal sampleReceipt3Detail3Qty = 1;
            decimal sampleReceipt3Detail4Qty = 2;
                                 
            decimal sampleReceipt3Detail1GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt3Detail1Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            decimal sampleReceipt3Detail1NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt3Detail1Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceipt3Detail1VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt3Detail1GrossTotal - sampleReceipt3Detail1NetTotal);
                                 
            decimal sampleReceipt3Detail2GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt3Detail2Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            decimal sampleReceipt3Detail2NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt3Detail2Qty) * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceipt3Detail2VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt3Detail2GrossTotal - sampleReceipt3Detail2NetTotal);
                                 
            decimal sampleReceipt3Detail3GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt3Detail3Qty) * PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT);
            decimal sampleReceipt3Detail3NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt3Detail3Qty) * PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceipt3Detail3VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt3Detail3GrossTotal - sampleReceipt3Detail3NetTotal);

            decimal sampleReceipt3Detail4GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt3Detail4Qty) * PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT);
            decimal sampleReceipt3Detail4NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt3Detail4Qty) * PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceipt3Detail4VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt3Detail4GrossTotal - sampleReceipt3Detail4NetTotal);

            decimal sampleReceipt3GrossTotal = sampleReceipt3Detail1GrossTotal + sampleReceipt3Detail2GrossTotal + sampleReceipt3Detail3GrossTotal + sampleReceipt3Detail4GrossTotal;
            decimal sampleReceipt3NetTotal = sampleReceipt3Detail1NetTotal + sampleReceipt3Detail2NetTotal + sampleReceipt3Detail3NetTotal + sampleReceipt3Detail4NetTotal;
            decimal sampleReceipt3VatAmount = sampleReceipt3Detail1VatAmount + sampleReceipt3Detail2VatAmount + sampleReceipt3Detail3VatAmount + sampleReceipt3Detail4VatAmount;

            SampleReceiptOpenWithDifferentItemsPOS1= new DocumentHeader(MemorySessionManager.GetSession<DocumentHeader>())
            {
                Division = eDivision.Sales,
                POS = POS1.Oid,
                CreatedByDevice = POS1.ToString(),
                DocumentType = ReceiptDocumentType.Oid,
                DocumentSeries = ReceiptDocumentSeries.Oid,
                Store = this.DefaultStore.Oid,
                ReferenceCompany = this.DefaultStore.ReferenceCompany,
                MainCompany = this.DefaultStore.Owner,
                Customer = this.DefaultCustomer.Oid,
                CustomerCode = this.DefaultCustomer.Code,
                CustomerName = this.DefaultCustomer.CompanyName,
                Status = DocumentStatusThatTakesSequence.Oid,
                //PriceCatalog = RootPriceCatalog.Oid,
                PriceCatalogPolicy = PriceCatalogPolicy.Oid,
                UserDailyTotals = UserDailyTotalsPOS1,
                DeliveryAddress = "-",
                Source = DocumentSource.POS,
                GrossTotal = sampleReceipt3GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt3GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt3GrossTotal,
                NetTotalBeforeDiscount = sampleReceipt3NetTotal,
                NetTotal = sampleReceipt3NetTotal,
                TotalVatAmountBeforeDiscount = sampleReceipt3VatAmount,
                TotalVatAmount = sampleReceipt3VatAmount,
                TotalQty = sampleReceipt3Detail1Qty + sampleReceipt3Detail2Qty + sampleReceipt3Detail3Qty + sampleReceipt3Detail4Qty,
                IsOpen = true
            };


            SampleReceiptOpenWithDifferentItemsPOS1Detail1_SampleItemVat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                Barcode = SampleItemBarcode.Oid,
                DocumentHeader = SampleReceiptOpenWithDifferentItemsPOS1,
                FinalUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItemVat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 1,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt3Detail1Qty,
                PriceCatalogDetail = SampleItemPriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceipt3Detail1Qty,
                GrossTotal = sampleReceipt3Detail1GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt3Detail1GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt3Detail1GrossTotal,
                NetTotal = sampleReceipt3Detail1NetTotal,
                NetTotalBeforeDiscount = sampleReceipt3Detail1NetTotal,
                TotalVatAmount = sampleReceipt3Detail1VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt3Detail1VatAmount,
            };

            SampleReceiptOpenWithDifferentItemsPOS1Detail2_SampleItemVat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                Barcode = SampleItemBarcode.Oid,
                DocumentHeader = SampleReceiptOpenWithDifferentItemsPOS1,
                FinalUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItemVat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 2,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt3Detail2Qty,
                PriceCatalogDetail = SampleItemPriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceipt3Detail2Qty,
                GrossTotal = sampleReceipt3Detail2GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt3Detail2GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt3Detail2GrossTotal,
                NetTotal = sampleReceipt3Detail2NetTotal,
                NetTotalBeforeDiscount = sampleReceipt3Detail2NetTotal,
                TotalVatAmount = sampleReceipt3Detail2VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt3Detail2VatAmount,
            };

            SampleReceiptOpenWithDifferentItemsPOS1Detail3_SampleItem2Vat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                Barcode = SampleItem2Barcode.Oid,
                DocumentHeader = SampleReceiptOpenWithDifferentItemsPOS1,
                FinalUnitPrice = SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_2_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItem2Vat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 3,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt3Detail3Qty,
                PriceCatalogDetail = SampleItem2PriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceipt3Detail3Qty,
                GrossTotal = sampleReceipt3Detail3GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt3Detail3GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt3Detail3GrossTotal,
                NetTotal = sampleReceipt3Detail3NetTotal,
                NetTotalBeforeDiscount = sampleReceipt3Detail3NetTotal,
                TotalVatAmount = sampleReceipt3Detail3VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt3Detail3VatAmount
            };

            SampleReceiptOpenWithDifferentItemsPOS1Detail4_SampleItem2Vat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                Barcode = SampleItem2Barcode.Oid,
                DocumentHeader = SampleReceiptOpenWithDifferentItemsPOS1,
                FinalUnitPrice = SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_2_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItem2Vat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 4,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt3Detail4Qty,
                PriceCatalogDetail = SampleItem2PriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceipt3Detail4Qty,
                GrossTotal = sampleReceipt3Detail4GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt3Detail4GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt3Detail4GrossTotal,
                NetTotal = sampleReceipt3Detail4NetTotal,
                NetTotalBeforeDiscount = sampleReceipt3Detail4NetTotal,
                TotalVatAmount = sampleReceipt3Detail4VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt3Detail4VatAmount
            };

            ////---------------------------------------------------

            ////Sample Receipt with different items and a canceled line
            decimal sampleReceipt4Detail1Qty = 1;
            decimal sampleReceipt4Detail2Qty = 1;
            decimal sampleReceipt4Detail3Qty = 1;
                                 
            decimal sampleReceipt4Detail1GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt4Detail1Qty) * PosCommonFixture.SAMPLE_ITEM_3_VAT23_PRICE_WITH_VAT);
            decimal sampleReceipt4Detail1NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt4Detail1Qty) * PosCommonFixture.SAMPLE_ITEM_3_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceipt4Detail1VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt4Detail1GrossTotal - sampleReceipt4Detail1NetTotal);
                                 
            decimal sampleReceipt4Detail2GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt4Detail2Qty) * PosCommonFixture.SAMPLE_ITEM_4_VAT23_PRICE_WITH_VAT);
            decimal sampleReceipt4Detail2NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt4Detail2Qty) * PosCommonFixture.SAMPLE_ITEM_4_VAT23_PRICE_WITHOUT_VAT);
            decimal sampleReceipt4Detail2VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt4Detail2GrossTotal - sampleReceipt4Detail2NetTotal);
                                 
            decimal sampleReceipt4Detail3GrossTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt4Detail3Qty) * PosCommonFixture.SAMPLE_ITEM_5_VAT13_PRICE_WITH_VAT);
            decimal sampleReceipt4Detail3NetTotal = PlatformRoundingHandler.RoundDisplayValue((sampleReceipt4Detail3Qty) * PosCommonFixture.SAMPLE_ITEM_5_VAT13_PRICE_WITHOUT_VAT);
            decimal sampleReceipt4Detail3VatAmount = PlatformRoundingHandler.RoundDisplayValue(sampleReceipt4Detail3GrossTotal - sampleReceipt4Detail3NetTotal);

            decimal sampleReceipt4GrossTotal = sampleReceipt4Detail1GrossTotal + sampleReceipt4Detail2GrossTotal; //(line 3 is canceled) + sampleReceipt4Detail3GrossTotal;
            decimal sampleReceipt4NetTotal = sampleReceipt4Detail1NetTotal + sampleReceipt4Detail2NetTotal;       //(line 3 is canceled) +sampleReceipt4Detail3NetTotal;
            decimal sampleReceipt4VatAmount = sampleReceipt4Detail1VatAmount + sampleReceipt4Detail2VatAmount;    //(line 3 is canceled) +sampleReceipt4Detail3VatAmount;

            SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1 = new DocumentHeader(MemorySessionManager.GetSession<DocumentHeader>())
            {
                Division = eDivision.Sales,
                POS = POS1.Oid,
                CreatedByDevice = POS1.ToString(),
                DocumentType = ReceiptDocumentType.Oid,
                DocumentSeries = ReceiptDocumentSeries.Oid,
                Store = this.DefaultStore.Oid,
                ReferenceCompany = this.DefaultStore.ReferenceCompany,
                MainCompany = this.DefaultStore.Owner,
                Customer = this.DefaultCustomer.Oid,
                CustomerCode = this.DefaultCustomer.Code,
                CustomerName = this.DefaultCustomer.CompanyName,
                Status = DocumentStatusThatTakesSequence.Oid,
                //PriceCatalog = RootPriceCatalog.Oid,
                PriceCatalogPolicy = PriceCatalogPolicy.Oid,
                UserDailyTotals = UserDailyTotalsPOS1,
                DeliveryAddress = "-",
                Source = DocumentSource.POS,
                GrossTotal = sampleReceipt4GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt4GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt4GrossTotal,
                NetTotalBeforeDiscount = sampleReceipt4NetTotal,
                NetTotal = sampleReceipt4NetTotal,
                TotalVatAmountBeforeDiscount = sampleReceipt4VatAmount,
                TotalVatAmount = sampleReceipt4VatAmount,
                TotalQty = sampleReceipt4Detail1Qty + sampleReceipt4Detail2Qty,
                IsOpen = true
            };


            SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail1_SampleItem3Vat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                HasCustomPrice = true,
                CustomUnitPrice = SAMPLE_ITEM_3_VAT23_PRICE_WITH_VAT,
                Barcode = SampleItem3Barcode.Oid,
                FinalUnitPrice = SAMPLE_ITEM_3_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_3_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItem3Vat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 1,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt4Detail1Qty,
                PriceCatalogDetail = SampleItem3PriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_3_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceipt4Detail1Qty,
                GrossTotal = sampleReceipt4Detail1GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt4Detail1GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt4Detail1GrossTotal,
                NetTotal = sampleReceipt4Detail1NetTotal,
                NetTotalBeforeDiscount = sampleReceipt4Detail1NetTotal,
                TotalVatAmount = sampleReceipt4Detail1VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt4Detail1VatAmount,
            };
            SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1.DocumentDetails.Add(SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail1_SampleItem3Vat23);

            SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail2_SampleItem4Vat23 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                HasCustomPrice = true,
                CustomUnitPrice = SAMPLE_ITEM_4_VAT23_PRICE_WITH_VAT,
                Barcode = SampleItem4Barcode.Oid,
                FinalUnitPrice = SAMPLE_ITEM_4_VAT23_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_4_VAT23_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor23.Factor,
                VatFactorGuid = vatFactor23.Oid,
                Item = SampleItem4Vat23.Oid,
                ItemVatCategoryMinistryCode = VatCat23.MinistryVatCategoryCode,
                LineNumber = 2,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt4Detail2Qty,
                PriceCatalogDetail = SampleItem4PriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_4_VAT23_PRICE_WITH_VAT,
                Qty = sampleReceipt4Detail2Qty,
                GrossTotal = sampleReceipt4Detail2GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt4Detail2GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt4Detail2GrossTotal,
                NetTotal = sampleReceipt4Detail2NetTotal,
                NetTotalBeforeDiscount = sampleReceipt4Detail2NetTotal,
                TotalVatAmount = sampleReceipt4Detail2VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt4Detail2VatAmount,
            };
            SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1.DocumentDetails.Add(SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail2_SampleItem4Vat23);

            SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail3_SampleItem5Vat13 = new DocumentDetail((MemorySessionManager.GetSession<DocumentDetail>()))
            {
                IsCanceled = true,
                HasCustomPrice = true,
                CustomUnitPrice = SAMPLE_ITEM_5_VAT13_PRICE_WITH_VAT,
                Barcode = SampleItem5Barcode.Oid,
                FinalUnitPrice = SAMPLE_ITEM_5_VAT13_PRICE_WITH_VAT,
                UnitPrice = PlatformRoundingHandler.RoundDisplayValue(SAMPLE_ITEM_5_VAT13_PRICE_WITHOUT_VAT),
                VatFactor = vatFactor13.Factor,
                VatFactorGuid = vatFactor13.Oid,
                Item = SampleItem5Vat13.Oid,
                ItemVatCategoryMinistryCode = VatCat13.MinistryVatCategoryCode,
                LineNumber = 3,
                MeasurementUnit = MeasurementUnitNonDecimal.Oid,
                PackingQuantity = sampleReceipt4Detail3Qty,
                PriceCatalogDetail = SampleItem5PriceCatalogDetail.Oid,
                PriceCatalogValueVatIncluded = true,
                PriceListUnitPrice = SAMPLE_ITEM_5_VAT13_PRICE_WITH_VAT,
                Qty = sampleReceipt4Detail3Qty,
                GrossTotal = sampleReceipt4Detail3GrossTotal,
                GrossTotalBeforeDiscount = sampleReceipt4Detail3GrossTotal,
                GrossTotalBeforeDocumentDiscount = sampleReceipt4Detail3GrossTotal,
                NetTotal = sampleReceipt4Detail3NetTotal,
                NetTotalBeforeDiscount = sampleReceipt4Detail3NetTotal,
                TotalVatAmount = sampleReceipt4Detail3VatAmount,
                TotalVatAmountBeforeDiscount = sampleReceipt4Detail3VatAmount,
            };
            SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1.DocumentDetails.Add(SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail3_SampleItem5Vat13);
          

            ////---------------------------------------------------


            MemorySessionManager.GetSession<ITS.POS.Model.Master.Item>().CommitChanges();
            MemorySessionManager.GetSession<ITS.POS.Model.Settings.DocumentSeries>().CommitChanges();
            MemorySessionManager.GetSession<ITS.POS.Model.Transactions.DocumentHeader>().CommitChanges();
            MemorySessionManager.GetSession<ITS.POS.Model.Versions.TableVersions>().CommitChanges();
        }


        public void Dispose()
        {
            if (MemorySessionManager != null)
            {
                MemorySessionManager.ClearAllSessions();
            }
        }
    }
}
