using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.Platform.Tests.Fixtures;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Tests.POS
{
    /// <summary>
    /// Test naming standards derived from http://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
    /// 
    /// [Method_Scenario_Expectation]
    /// 
    /// (same as [UnitOfWork_StateUnderTest_ExpectedBehavior] )
    /// 
    /// A unit of work is a use case in the system that startes with a public method and ends up with one of three types of results:
    /// a return value/exception, a state change to the system which changes its behavior, or a call to a third party (when we use mocks). 
    /// so a unit of work can be a small as a method, or as large as a class, or even multiple classes. as long is it all runs in memory,
    /// and is fully under our control.
    /// 
    /// Examples:
    /// Public void Sum_NegativeNumberAs1stParam_ExceptionThrown()
    /// Public void Sum_NegativeNumberAs2ndParam_ExceptionThrown ()
    /// Public void Sum_simpleValues_Calculated ()
    /// 
    /// 
    /// Code Snippet for test methods included in the SolutionDir\Visual Studio Code Snippets
    /// Install by going to Tools -> Code Snippets Manager -> Language C# -> Import
    /// Then type aaa and press tab
    /// </summary>
    [TestFixture]
    public class DocumentServiceTests
    {
        PosCommonFixture CommonFixture { get; set; }

        /// <summary>
        /// Runs before every test
        /// </summary>
        [SetUp]
        public void SetFixture()
        {
            CommonFixture = new PosCommonFixture();
            CommonFixture.Setup();
        }

        /// <summary>
        /// Runs after every test
        /// </summary>
        [TearDown]
        public void TearDownFixture()
        {
            CommonFixture.Dispose();
            CommonFixture = null;
        }

        DocumentService CreateDefaultTestDocumentService()
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;

            Mock<IConfigurationManager> mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(mock => mock.GetAppSettings()).Returns(CommonFixture.AppSettings);
            mockConfigurationManager.Setup(mock => mock.DefaultCustomerOid).Returns(CommonFixture.DefaultCustomer.Oid);
            mockConfigurationManager.Setup(mock => mock.CurrentStoreOid).Returns(CommonFixture.DefaultStore.Oid);
            mockConfigurationManager.Setup(mock => mock.DefaultDocumentTypeOid).Returns(CommonFixture.ReceiptDocumentType.Oid);

            Mock<IItemService> mockItemService = new Mock<IItemService>(MockBehavior.Strict);
            Mock<IActionManager> mockActionManager = new Mock<IActionManager>(MockBehavior.Strict);
            PriceCatalogDetail pcd = CommonFixture.SampleItemPriceCatalogDetail;

            //mockItemService.Setup(mock => mock.GetUnitPrice(CommonFixture.RootPriceCatalog, CommonFixture.SampleItemVat23, out pcd, CommonFixture.SampleItemBarcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            //mockItemService.Setup(mock =>mock.GetUnitPriceFromPolicy(CommonFixture.PriceCatalogPolicy, CommonFixture.SampleItemVat23, out pcd, CommonFixture.SampleItemBarcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);


            PriceCatalogDetail pcd2 = CommonFixture.SampleItem2PriceCatalogDetail;
            //mockItemService.Setup(mock => mock.GetUnitPrice(CommonFixture.RootPriceCatalog, CommonFixture.SampleItem2Vat23, out pcd2, CommonFixture.SampleItem2Barcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT);

            //mockItemService.Setup(mock => mock.GetUnitPriceFromPolicy(CommonFixture.PriceCatalogPolicy, CommonFixture.SampleItem2Vat23, out pcd2, CommonFixture.SampleItem2Barcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT);

            PriceCatalogDetail pcd3 = CommonFixture.SampleItem3PriceCatalogDetail;
            //mockItemService.Setup(mock => mock.GetUnitPrice(CommonFixture.RootPriceCatalog, CommonFixture.SampleItem3Vat23, out pcd3, CommonFixture.SampleItem3Barcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_3_VAT23_PRICE_WITH_VAT);
            //mockItemService.Setup(mock => mock.GetUnitPriceFromPolicy(CommonFixture.PriceCatalogPolicy, CommonFixture.SampleItem3Vat23, out pcd3, CommonFixture.SampleItem3Barcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_3_VAT23_PRICE_WITH_VAT);

            PriceCatalogDetail pcd4 = CommonFixture.SampleItem4PriceCatalogDetail;
            //mockItemService.Setup(mock => mock.GetUnitPrice(CommonFixture.RootPriceCatalog, CommonFixture.SampleItem4Vat23, out pcd4, CommonFixture.SampleItem4Barcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_4_VAT23_PRICE_WITH_VAT);
            //mockItemService.Setup(mock => mock.GetUnitPriceFromPolicy(CommonFixture.PriceCatalogPolicy, CommonFixture.SampleItem4Vat23, out pcd4, CommonFixture.SampleItem4Barcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_4_VAT23_PRICE_WITH_VAT);

            PriceCatalogDetail pcd5 = CommonFixture.SampleItem5PriceCatalogDetail;
            //mockItemService.Setup(mock => mock.GetUnitPrice(CommonFixture.RootPriceCatalog, CommonFixture.SampleItem5Vat13, out pcd5, CommonFixture.SampleItem5Barcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_5_VAT13_PRICE_WITH_VAT);

            //mockItemService.Setup(mock => mock.GetUnitPriceFromPolicy(CommonFixture.PriceCatalogPolicy, CommonFixture.SampleItem5Vat13, out pcd5, CommonFixture.SampleItem5Barcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_5_VAT13_PRICE_WITH_VAT);


            mockItemService.Setup(mock => mock.GetPointsOfItem(CommonFixture.SampleItemVat23, CommonFixture.ReceiptDocumentType))
                .Returns(0);



            //Mock<IPlatformDocumentDiscountService> mockPlatformDocumentDiscountService = new Mock<IPlatformDocumentDiscountService>(MockBehavior.Strict);
            IPlatformDocumentDiscountService platformDocumentDiscountService = new PlatformDocumentDiscountService();
            IPlatformPersistentObjectMap platformPersistentObjectMap = new PlatformPersistentObjectMap();
            platformPersistentObjectMap.RegisterType<IPriceCatalog, PriceCatalog>();

            //Mock<IPlatformRoundingHandler> mockPlatformRoundingHandler = new Mock<IPlatformRoundingHandler>(MockBehavior.Strict);

            return new DocumentService(memorySessionManager, mockConfigurationManager.Object,
                mockItemService.Object, platformDocumentDiscountService, CommonFixture.PlatformRoundingHandler);
        }

        [Test]
        public void CreateDocumentHeader_CreateReceipt_CreatedWithCorrectType()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();

            //Act
            DocumentHeader documentHeader = documentService.CreateDocumentHeader(eDivision.Sales,
                                                                                CommonFixture.POS1.Oid,
                                                                                CommonFixture.ReceiptDocumentType.Oid,
                                                                                CommonFixture.ReceiptDocumentSeries.Oid,
                                                                                CommonFixture.DefaultStore.Oid,
                                                                                CommonFixture.DefaultCustomer.Oid,
                                                                                CommonFixture.DefaultCustomer.Code,
                                                                                CommonFixture.DefaultCustomer.CompanyName,
                                                                                CommonFixture.RootPriceCatalog.Oid,
                                                                                CommonFixture.DocumentStatusThatTakesSequence.Oid,
                                                                                CommonFixture.UserDailyTotalsPOS1.Oid);

            //Assert
            Assert.NotNull(documentHeader);
            Assert.AreEqual(eDivision.Sales, documentHeader.Division);
            Assert.AreEqual(CommonFixture.ReceiptDocumentType.Oid, documentHeader.DocumentType);
        }

        [TestCase(2)]
        [TestCase(2.5)]
        [TestCase(2.8)]
        public void CreateDocumentLine_CreateReceiptWith1Line_CreatedWithCorrectTotals(decimal qty)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(qty * CommonFixture.PlatformRoundingHandler.RoundDisplayValue(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT));
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(qty * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT);
            decimal expectedVatAmount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotal - expectedNetTotal);

            //Act
            DocumentHeader documentHeader = documentService.CreateDocumentHeader(eDivision.Sales,
                                                                                CommonFixture.POS1.Oid,
                                                                                CommonFixture.ReceiptDocumentType.Oid,
                                                                                CommonFixture.ReceiptDocumentSeries.Oid,
                                                                                CommonFixture.DefaultStore.Oid,
                                                                                CommonFixture.DefaultCustomer.Oid,
                                                                                CommonFixture.DefaultCustomer.Code,
                                                                                CommonFixture.DefaultCustomer.CompanyName,
                                                                                CommonFixture.RootPriceCatalog.Oid,
                                                                                CommonFixture.DocumentStatusThatTakesSequence.Oid,
                                                                                CommonFixture.UserDailyTotalsPOS1.Oid);

            DocumentDetail detail = documentService.CreateDocumentLine(documentHeader, CommonFixture.CurrentUserPOS1.Oid, CommonFixture.SampleItemVat23,
                                               CommonFixture.SampleItemPriceCatalogDetail, CommonFixture.SampleItemBarcode, qty, 0, false, false);

            //Assert
            Assert.AreEqual(qty, detail.Qty);
            Assert.AreEqual(expectedGrossTotal, detail.GrossTotal, "Unexpected GrossTotal");
            Assert.AreEqual(expectedNetTotal, detail.NetTotal, "Unexpected NetTotal");
            Assert.AreEqual(expectedVatAmount, detail.TotalVatAmount, "Unexpected TotalVatAmount");
        }

        [TestCase(2, 7, 12)]
        [TestCase(2.5, 7.3, 1.2)]
        [TestCase(2.1, 7.6, 13.3)]
        public void ComputeDocumentHeader_CreateReceiptWith3Lines_CreatedWithCorrectTotals(decimal detail1Qty, decimal detail2Qty, decimal detail3Qty)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            decimal totalQty = detail1Qty + detail2Qty + detail3Qty;

            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail1Qty * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT)
                                         + CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail2Qty * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT)
                                         + CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail3Qty * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);

            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail1Qty * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT)
                                        + CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail2Qty * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT)
                                        + CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail3Qty * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT);
            decimal expectedVatAmount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotal - expectedNetTotal);

            //Act
            DocumentHeader documentHeader = documentService.CreateDocumentHeader(eDivision.Sales,
                                                                               CommonFixture.POS1.Oid,
                                                                               CommonFixture.ReceiptDocumentType.Oid,
                                                                               CommonFixture.ReceiptDocumentSeries.Oid,
                                                                               CommonFixture.DefaultStore.Oid,
                                                                               CommonFixture.DefaultCustomer.Oid,
                                                                               CommonFixture.DefaultCustomer.Code,
                                                                               CommonFixture.DefaultCustomer.CompanyName,
                                                                               CommonFixture.RootPriceCatalog.Oid,
                                                                               CommonFixture.DocumentStatusThatTakesSequence.Oid,
                                                                               CommonFixture.UserDailyTotalsPOS1.Oid);


            DocumentDetail detail1 = documentService.CreateDocumentLine(documentHeader, CommonFixture.CurrentUserPOS1.Oid, CommonFixture.SampleItemVat23,
                                              CommonFixture.SampleItemPriceCatalogDetail, CommonFixture.SampleItemBarcode, detail1Qty, 0, false, false);

            documentService.ComputeDocumentHeader(ref documentHeader, false, detail1);

            DocumentDetail detail2 = documentService.CreateDocumentLine(documentHeader, CommonFixture.CurrentUserPOS1.Oid, CommonFixture.SampleItemVat23,
                                              CommonFixture.SampleItemPriceCatalogDetail, CommonFixture.SampleItemBarcode, detail2Qty, 0, false, false);

            documentService.ComputeDocumentHeader(ref documentHeader, false, detail2);

            DocumentDetail detail3 = documentService.CreateDocumentLine(documentHeader, CommonFixture.CurrentUserPOS1.Oid, CommonFixture.SampleItemVat23,
                                  CommonFixture.SampleItemPriceCatalogDetail, CommonFixture.SampleItemBarcode, detail3Qty, 0, false, false);

            documentService.ComputeDocumentHeader(ref documentHeader, false, detail3);

            //Assert
            Assert.AreEqual(totalQty, documentHeader.TotalQty);
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal);
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount);
        }

        [Test]
        public void FindLine_SearchForSampleItem_Found()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;

            //Act
            DocumentDetail foundDetail = documentService.FindLine(PosCommonFixture.SAMPLE_ITEM_VAT23_CODE, documentHeader);

            //Assert
            Assert.IsNotNull(foundDetail);
            Assert.AreEqual(foundDetail.Item, CommonFixture.SampleItemVat23.Oid);
        }

        [Test]
        public void FindLine_SearchForSampleItemBarcode_Found()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;

            //Act
            DocumentDetail foundDetail = documentService.FindLine(PosCommonFixture.SAMPLE_ITEM_VAT23_BARCODE, documentHeader);

            //Assert
            Assert.IsNotNull(foundDetail);
            Assert.AreEqual(foundDetail.Item, CommonFixture.SampleItemVat23.Oid);
        }

        [Test]
        public void RecalculateDocumentDetail_ChangeQtyAndRecalculate_Calculated()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            DocumentDetail detail = CommonFixture.SampleReceiptOpenWith1LinePOS1Detail_SampleItemVat23;
            decimal detailNewQty = detail.Qty * 2;

            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detailNewQty * detail.FinalUnitPrice);
            decimal expectedGrossTotalBeforeDiscount = expectedGrossTotal;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detailNewQty * PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT);
            decimal expectedNetTotalBeforeDiscount = expectedNetTotal;
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;
            detail.Qty = detailNewQty;

            //Act
            documentService.RecalculateDocumentDetail(detail, documentHeader);

            //Assert
            Assert.AreEqual(detailNewQty, detail.Qty);
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, detail.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotal, detail.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, detail.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedNetTotal, detail.NetTotal, "Unexpected Net Total");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, detail.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(expectedVatAmount, detail.TotalVatAmount, "Unexpected Vat Total");
            Assert.AreEqual(detail.GrossTotal, detail.NetTotal + detail.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(detail.GrossTotalBeforeDiscount, detail.NetTotalBeforeDiscount + detail.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

        }

        [TestCase(0.06)]
        [TestCase(0.11)]
        [TestCase(0.22)]
        [TestCase(0.25)]
        [TestCase(0.26)]
        [TestCase(0.50)]
        [TestCase(0.75)]
        [TestCase(0.84)]
        [TestCase(0.94)]
        [TestCase(0.99)]
        public void CreateOrUpdateCustomDiscount_CreateDetailCustomPercentageDiscount_CreatedWithCorrectData(decimal discountPercentage)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            DocumentDetail detail = CommonFixture.SampleReceiptOpenWith1LinePOS1Detail_SampleItemVat23;

            //Act
            DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountPercentage, discountPercentage, detail);

            //Assert
            Assert.AreEqual(discountPercentage, discount.Percentage);
            Assert.AreEqual(0, discount.Value);
            Assert.AreEqual(eDiscountType.PERCENTAGE, discount.DiscountType);
            Assert.AreEqual(eDiscountSource.CUSTOM, discount.DiscountSource);
        }

        [TestCase(0.50)]
        [TestCase(0.75)]
        [TestCase(1.36)]
        [TestCase(1.5)]
        [TestCase(1.67)]
        [TestCase(2.99)]
        [TestCase(3.99)]
        [TestCase(4.32)]
        [TestCase(4.99)]
        public void CreateOrUpdateCustomDiscount_CreateDetailCustomValueDiscount_CreatedWithCorrectData(decimal discountValue)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            DocumentDetail detail = CommonFixture.SampleReceiptOpenWith1LinePOS1Detail_SampleItemVat23;


            //Act
            DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountValue, discountValue, detail);

            //Assert
            Assert.AreEqual(0, discount.Percentage);
            Assert.AreEqual(discountValue, discount.Value);
            Assert.AreEqual(eDiscountType.VALUE, discount.DiscountType);
            Assert.AreEqual(eDiscountSource.CUSTOM, discount.DiscountSource);
        }

        [TestCase(0.06)]
        [TestCase(0.11)]
        [TestCase(0.22)]
        [TestCase(0.25)]
        [TestCase(0.26)]
        [TestCase(0.50)]
        [TestCase(0.75)]
        [TestCase(0.84)]
        [TestCase(0.94)]
        [TestCase(0.99)]
        public void CreateOrUpdateCustomDiscount_CreateCustomPercentageDiscountAndRecalculateLine_CalculatedCorrectly(decimal discountPercentage)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            DocumentDetail detail = CommonFixture.SampleReceiptOpenWith1LinePOS1Detail_SampleItemVat23;
            decimal expectedGrossTotalBeforeDiscount = detail.GrossTotal;
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDiscount * (1 - discountPercentage));
            decimal expectedNetTotalBeforeDiscount = detail.NetTotal;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT * detail.Qty * (1 - discountPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountPercentage, discountPercentage, detail);
            detail.DocumentDetailDiscounts.Add(discount);
            documentService.RecalculateDocumentDetail(detail, documentHeader);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, detail.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotal, detail.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, detail.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedNetTotal, detail.NetTotal, "Unexpected Net Total");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, detail.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(expectedVatAmount, detail.TotalVatAmount, "Unexpected Vat Total");
            Assert.AreEqual(detail.GrossTotal, detail.NetTotal + detail.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(detail.GrossTotalBeforeDiscount, detail.NetTotalBeforeDiscount + detail.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");
        }

        [TestCase(0.50)]
        [TestCase(0.75)]
        [TestCase(1.36)]
        [TestCase(1.5)]
        [TestCase(1.67)]
        [TestCase(2.99)]
        [TestCase(3.99)]
        [TestCase(4.32)]
        [TestCase(4.99)]
        public void CreateOrUpdateCustomDiscount_CreateCustomValueDiscountAndRecalculateLine_CalculatedCorrectly(decimal discountValue)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            DocumentDetail detail = CommonFixture.SampleReceiptOpenWith1LinePOS1Detail_SampleItemVat23;
            decimal expectedGrossTotalBeforeDiscount = detail.GrossTotal;
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDiscount - discountValue);
            decimal expectedNetTotalBeforeDiscount = detail.NetTotal;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT * detail.Qty * (expectedGrossTotal / expectedGrossTotalBeforeDiscount));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountValue, discountValue, detail);
            detail.DocumentDetailDiscounts.Add(discount);
            documentService.RecalculateDocumentDetail(detail, documentHeader);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, detail.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotal, detail.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, detail.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedNetTotal, detail.NetTotal, "Unexpected Net Total");
            Assert.AreEqual(expectedVatAmount, detail.TotalVatAmount, "Unexpected Vat Total");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, detail.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(detail.GrossTotal, detail.NetTotal + detail.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(detail.GrossTotalBeforeDiscount, detail.NetTotalBeforeDiscount + detail.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");
        }

        [TestCase(0.06)]
        [TestCase(0.11)]
        [TestCase(0.22)]
        [TestCase(0.25)]
        [TestCase(0.26)]
        [TestCase(0.50)]
        [TestCase(0.75)]
        [TestCase(0.84)]
        [TestCase(0.94)]
        [TestCase(0.99)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomPercentageDocumentDiscountAndRecalculateDocumentWithOneLine_CalculatedCorrectly(decimal discountPercentage)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;

            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDiscount * (1 - discountPercentage));
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT * detail.Qty) * (1 - discountPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, discountPercentage, CommonFixture.HeaderDiscountPercentage);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");
        }

        [TestCase(0.50)]
        [TestCase(0.75)]
        [TestCase(1.36)]
        [TestCase(1.5)]
        [TestCase(1.67)]
        [TestCase(2.99)]
        [TestCase(3.99)]
        [TestCase(4.32)]
        [TestCase(4.99)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomValueDocumentDiscountAndRecalculateDocumentWithOneLine_CalculatedCorrectly(decimal discountValue)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDiscount - discountValue);
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITHOUT_VAT * detail.Qty)
                                                                            * (expectedGrossTotal / expectedGrossTotalBeforeDiscount));
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;

            //Act
            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, discountValue, CommonFixture.HeaderDiscountValue);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

        }

        [TestCase(0.06)]
        [TestCase(0.11)]
        [TestCase(0.22)]
        [TestCase(0.25)]
        [TestCase(0.26)]
        [TestCase(0.50)]
        [TestCase(0.75)]
        [TestCase(0.84)]
        [TestCase(0.94)]
        [TestCase(0.99)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomPercentageDocumentDiscountAndRecalculateDocumentWithMultipleLines_CalculatedCorrectly(decimal headerPercentage)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDiscount * (1 - headerPercentage));
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.UnitPrice * detail.Qty) * (1 - headerPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, headerPercentage, CommonFixture.HeaderDiscountPercentage);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");

        }


        [TestCase(0.50)]
        [TestCase(0.75)]
        [TestCase(1.36)]
        [TestCase(1.5)]
        [TestCase(1.67)]
        [TestCase(2.99)]
        [TestCase(3.99)]
        [TestCase(4.32)]
        [TestCase(4.99)]
        [TestCase(8.34)]
        [TestCase(10.54)]
        [TestCase(12.46)]
        [TestCase(26.97)]
        [TestCase(29.32)]
        [TestCase(32.48)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomValueDocumentDiscountAndRecalculateDocumentWithMultipleLines_CalculatedCorrectly(decimal discountValue)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDiscount - discountValue);
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.UnitPrice * detail.Qty) * (expectedGrossTotal / expectedGrossTotalBeforeDiscount));
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;

            //Act
            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, discountValue, CommonFixture.HeaderDiscountValue);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");
        }


        [TestCase(0.01, 0.01)]
        [TestCase(0.06, 0.34)]
        [TestCase(0.06, 0.98)]
        [TestCase(0.10, 0.10)]
        [TestCase(0.11, 0.67)]
        [TestCase(0.11, 0.22)]
        [TestCase(0.13, 0.01)]
        [TestCase(0.22, 0.21)]
        [TestCase(0.22, 0.93)]
        [TestCase(0.25, 0.39)]
        [TestCase(0.25, 0.12)]
        [TestCase(0.26, 0.86)]
        [TestCase(0.26, 0.90)]
        [TestCase(0.50, 0.42)]
        [TestCase(0.50, 0.38)]
        [TestCase(0.75, 0.97)]
        [TestCase(0.75, 0.47)]
        [TestCase(0.84, 0.43)]
        [TestCase(0.84, 0.87)]
        [TestCase(0.94, 0.14)]
        [TestCase(0.94, 0.25)]
        [TestCase(0.99, 0.74)]
        [TestCase(0.99, 0.07)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomPercentageDocumentDiscountAndValueLineDiscountsAndRecalculateDocumentWithMultipleLines_CalculatedCorrectly(decimal headerDiscountPercentage, decimal lineDiscountValue)
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;

            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.GrossTotal - lineDiscountValue));
            decimal headerDiscountValue = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDocumentDiscount * headerDiscountPercentage);
            decimal expectedGrossTotal = expectedGrossTotalBeforeDocumentDiscount - headerDiscountValue;
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.UnitPrice * detail.Qty * ((detail.GrossTotal - lineDiscountValue) / detail.GrossTotal)));
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedNetTotalBeforeDocumentDiscount * (1 - headerDiscountPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmountBeforeDocumentDiscount = expectedGrossTotalBeforeDocumentDiscount - expectedNetTotalBeforeDocumentDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            foreach (DocumentDetail detail in documentHeader.DiscountableDocumentDetails())
            {
                DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountValue, lineDiscountValue, detail);

                detail.DocumentDetailDiscounts.Add(discount);
                documentService.RecalculateDocumentDetail(detail, documentHeader);
                documentService.RecalculateDocumentCosts(documentHeader, false);
            }

            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, headerDiscountPercentage, CommonFixture.HeaderDiscountPercentage);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotalBeforeDocumentDiscount, documentHeader.GrossTotalBeforeDocumentDiscount, "Unexpected Gross Total Before Document Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");

        }

        [TestCase(0.01, 0.01)]
        [TestCase(0.85, 0.01)]
        [TestCase(2.32, 0.34)]
        [TestCase(5.23, 0.98)]
        [TestCase(6.10, 0.10)]
        [TestCase(7.41, 0.67)]
        [TestCase(8.12, 0.22)]
        [TestCase(9.13, 0.01)]
        [TestCase(10.22, 0.21)]
        [TestCase(11.18, 0.93)]
        [TestCase(12.24, 0.39)]
        [TestCase(13.78, 0.12)]
        [TestCase(14.30, 0.86)]
        [TestCase(15.86, 0.90)]
        [TestCase(16.47, 0.42)]
        [TestCase(17.74, 0.38)]
        [TestCase(18.43, 0.97)]
        [TestCase(18.75, 0.47)]
        [TestCase(19.84, 0.43)]
        [TestCase(20.59, 0.87)]
        [TestCase(21.24, 0.14)]
        [TestCase(30.94, 0.25)]
        [TestCase(31.99, 0.74)]
        [TestCase(34.99, 0.43)]
        [TestCase(34.99, 4.43)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomValueDocumentDiscountAndValueLineDiscountsAndRecalculateDocumentWithMultipleLines_CalculatedCorrectly(decimal headerDiscountValue, decimal lineDiscountValue)
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;

            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.GrossTotal - lineDiscountValue));
            decimal expectedGrossTotal = expectedGrossTotalBeforeDocumentDiscount - headerDiscountValue;
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.UnitPrice * detail.Qty * ((detail.GrossTotal - lineDiscountValue) / detail.GrossTotal)));
            decimal headerDiscountPercentage = (1 - expectedGrossTotal / expectedGrossTotalBeforeDocumentDiscount);
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedNetTotalBeforeDocumentDiscount * (1 - headerDiscountPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmountBeforeDocumentDiscount = expectedGrossTotalBeforeDocumentDiscount - expectedNetTotalBeforeDocumentDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            foreach (DocumentDetail detail in documentHeader.DiscountableDocumentDetails())
            {
                DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountValue, lineDiscountValue, detail);

                detail.DocumentDetailDiscounts.Add(discount);
                documentService.RecalculateDocumentDetail(detail, documentHeader);
                documentService.RecalculateDocumentCosts(documentHeader, false);
            }

            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, headerDiscountValue, CommonFixture.HeaderDiscountValue);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotalBeforeDocumentDiscount, documentHeader.GrossTotalBeforeDocumentDiscount, "Unexpected Gross Total Before Document Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");

        }


        [TestCase(0.01, 0.01)]
        [TestCase(0.01, 0.99)]
        [TestCase(0.06, 0.99)]
        [TestCase(0.06, 0.94)]
        [TestCase(0.10, 0.94)]
        [TestCase(0.11, 0.84)]
        [TestCase(0.11, 0.84)]
        [TestCase(0.13, 0.75)]
        [TestCase(0.22, 0.75)]
        [TestCase(0.22, 0.50)]
        [TestCase(0.25, 0.50)]
        [TestCase(0.25, 0.26)]
        [TestCase(0.26, 0.26)]
        [TestCase(0.26, 0.25)]
        [TestCase(0.50, 0.25)]
        [TestCase(0.50, 0.22)]
        [TestCase(0.75, 0.22)]
        [TestCase(0.75, 0.13)]
        [TestCase(0.84, 0.11)]
        [TestCase(0.84, 0.11)]
        [TestCase(0.94, 0.10)]
        [TestCase(0.94, 0.06)]
        [TestCase(0.99, 0.06)]
        [TestCase(0.99, 0.01)]
        [TestCase(0.99, 0.99)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomPercentageDocumentDiscountAndPercentageLineDiscountsAndRecalculateDocumentWithMultipleLines_CalculatedCorrectly(decimal headerDiscountPercentage, decimal lineDiscountPercentage)
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;

            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.GrossTotal - CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail.GrossTotal * lineDiscountPercentage)));

            decimal headerDiscountValue = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDocumentDiscount * headerDiscountPercentage);
            decimal expectedGrossTotal = expectedGrossTotalBeforeDocumentDiscount - headerDiscountValue;
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.UnitPrice * detail.Qty * (1 - lineDiscountPercentage)));
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedNetTotalBeforeDocumentDiscount * (1 - headerDiscountPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmountBeforeDocumentDiscount = expectedGrossTotalBeforeDocumentDiscount - expectedNetTotalBeforeDocumentDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            foreach (DocumentDetail detail in documentHeader.DiscountableDocumentDetails())
            {
                DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountPercentage, lineDiscountPercentage, detail);

                detail.DocumentDetailDiscounts.Add(discount);
                documentService.RecalculateDocumentDetail(detail, documentHeader);
                documentService.RecalculateDocumentCosts(documentHeader, false);
            }

            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, headerDiscountPercentage, CommonFixture.HeaderDiscountPercentage);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotalBeforeDocumentDiscount, documentHeader.GrossTotalBeforeDocumentDiscount, "Unexpected Gross Total Before Document Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");

        }


        [TestCase(0.01, 0.01)]
        [TestCase(0.85, 0.99)]
        [TestCase(2.32, 0.99)]
        [TestCase(5.23, 0.94)]
        [TestCase(6.10, 0.94)]
        [TestCase(7.41, 0.84)]
        [TestCase(8.12, 0.84)]
        [TestCase(9.13, 0.75)]
        [TestCase(10.22, 0.75)]
        [TestCase(11.18, 0.50)]
        [TestCase(12.24, 0.50)]
        [TestCase(13.78, 0.26)]
        [TestCase(14.30, 0.26)]
        [TestCase(15.86, 0.25)]
        [TestCase(16.47, 0.25)]
        [TestCase(17.74, 0.22)]
        [TestCase(18.43, 0.22)]
        [TestCase(18.75, 0.13)]
        [TestCase(19.84, 0.11)]
        [TestCase(20.59, 0.11)]
        [TestCase(21.24, 0.10)]
        [TestCase(30.94, 0.06)]
        [TestCase(31.99, 0.06)]
        [TestCase(34.99, 0.01)]
        [TestCase(34.99, 0.99)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomValueDocumentDiscountAndPercentageLineDiscountsAndRecalculateDocumentWithMultipleLines_CalculatedCorrectly(decimal headerDiscountValue, decimal lineDiscountPercentage)
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.GrossTotal - CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail.GrossTotal * lineDiscountPercentage)));
            decimal expectedGrossTotal = expectedGrossTotalBeforeDocumentDiscount - headerDiscountValue;
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.UnitPrice * detail.Qty * (1 - lineDiscountPercentage)));
            decimal headerDiscountPercentage = (1 - expectedGrossTotal / expectedGrossTotalBeforeDocumentDiscount);
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedNetTotalBeforeDocumentDiscount * (1 - headerDiscountPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmountBeforeDocumentDiscount = expectedGrossTotalBeforeDocumentDiscount - expectedNetTotalBeforeDocumentDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            foreach (DocumentDetail detail in documentHeader.DiscountableDocumentDetails())
            {
                DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountPercentage, lineDiscountPercentage, detail);

                detail.DocumentDetailDiscounts.Add(discount);
                documentService.RecalculateDocumentDetail(detail, documentHeader);
                documentService.RecalculateDocumentCosts(documentHeader, false);
            }

            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, headerDiscountValue, CommonFixture.HeaderDiscountValue);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotalBeforeDocumentDiscount, documentHeader.GrossTotalBeforeDocumentDiscount, "Unexpected Gross Total Before Document Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");

        }


        [TestCase(0.01, 0.01, 0.01)]
        [TestCase(0.01, 0.99, 0.01)]
        [TestCase(0.06, 0.99, 0.34)]
        [TestCase(0.06, 0.94, 0.98)]
        [TestCase(0.10, 0.94, 0.10)]
        [TestCase(0.11, 0.84, 0.67)]
        [TestCase(0.11, 0.84, 0.22)]
        [TestCase(0.13, 0.75, 0.01)]
        [TestCase(0.22, 0.75, 0.21)]
        [TestCase(0.22, 0.50, 0.93)]
        [TestCase(0.25, 0.50, 0.39)]
        [TestCase(0.25, 0.26, 0.12)]
        [TestCase(0.26, 0.26, 0.86)]
        [TestCase(0.26, 0.25, 0.90)]
        [TestCase(0.50, 0.25, 0.42)]
        [TestCase(0.50, 0.22, 0.38)]
        [TestCase(0.75, 0.22, 0.97)]
        [TestCase(0.75, 0.13, 0.47)]
        [TestCase(0.84, 0.11, 0.43)]
        [TestCase(0.84, 0.11, 0.87)]
        [TestCase(0.94, 0.10, 0.14)]
        [TestCase(0.94, 0.06, 0.25)]
        [TestCase(0.99, 0.06, 0.74)]
        [TestCase(0.99, 0.01, 0.43)]
        [TestCase(0.99, 0.99, 4.43)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomPercentageDocumentDiscountAndMultipleLineDiscountsAndRecalculateDocumentWithMultipleLines_CalculatedCorrectly(decimal headerDiscountPercentage,
            decimal lineDiscountPercentage, decimal lineSecondDiscountValue)
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail =>
            {
                decimal grossAfterFirstDiscount = detail.GrossTotal - CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail.GrossTotal * lineDiscountPercentage);
                decimal grossBeforeDocumentDiscount = grossAfterFirstDiscount - lineSecondDiscountValue;
                return grossBeforeDocumentDiscount;
            }));
            decimal headerDiscountValue = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDocumentDiscount * headerDiscountPercentage);
            decimal expectedGrossTotal = expectedGrossTotalBeforeDocumentDiscount - headerDiscountValue;
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail =>
                {
                    decimal netTotalAfterFirstDiscount = detail.UnitPrice * detail.Qty * (1 - lineDiscountPercentage);
                    decimal grossAfterFirstDiscount = detail.GrossTotal - CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail.GrossTotal * lineDiscountPercentage);
                    decimal netTotalBeforeDocumentDiscount = netTotalAfterFirstDiscount * ((grossAfterFirstDiscount - lineSecondDiscountValue) / grossAfterFirstDiscount);
                    return netTotalBeforeDocumentDiscount;
                }));
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedNetTotalBeforeDocumentDiscount * (1 - headerDiscountPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmountBeforeDocumentDiscount = expectedGrossTotalBeforeDocumentDiscount - expectedNetTotalBeforeDocumentDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            foreach (DocumentDetail detail in documentHeader.DiscountableDocumentDetails())
            {
                DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountPercentage, lineDiscountPercentage, detail);
                detail.DocumentDetailDiscounts.Add(discount);
                documentService.RecalculateDocumentDetail(detail, documentHeader);
                documentService.RecalculateDocumentCosts(documentHeader, false);

                DocumentDetailDiscount secondDiscount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountValue, lineSecondDiscountValue, detail);
                detail.DocumentDetailDiscounts.Add(secondDiscount);
                documentService.RecalculateDocumentDetail(detail, documentHeader);
                documentService.RecalculateDocumentCosts(documentHeader, false);
            }

            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, headerDiscountPercentage, CommonFixture.HeaderDiscountPercentage);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotalBeforeDocumentDiscount, documentHeader.GrossTotalBeforeDocumentDiscount, "Unexpected Gross Total Before Document Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");
        }

        [TestCase(0.01, 0.01, 0.01)]
        [TestCase(0.85, 0.99, 0.01)]
        [TestCase(2.32, 0.99, 0.34)]
        [TestCase(5.23, 0.94, 0.98)]
        [TestCase(6.10, 0.94, 0.10)]
        [TestCase(7.41, 0.84, 0.67)]
        [TestCase(8.12, 0.84, 0.22)]
        [TestCase(9.13, 0.75, 0.01)]
        [TestCase(10.22, 0.75, 0.21)]
        [TestCase(11.18, 0.50, 0.93)]
        [TestCase(12.24, 0.50, 0.39)]
        [TestCase(13.78, 0.26, 0.12)]
        [TestCase(14.30, 0.26, 0.86)]
        [TestCase(15.86, 0.25, 0.90)]
        [TestCase(16.47, 0.25, 0.42)]
        [TestCase(17.74, 0.22, 0.38)]
        [TestCase(18.43, 0.22, 0.97)]
        [TestCase(18.75, 0.13, 0.47)]
        [TestCase(19.84, 0.11, 0.43)]
        [TestCase(20.59, 0.11, 0.87)]
        [TestCase(21.24, 0.10, 0.14)]
        [TestCase(30.94, 0.06, 0.25)]
        [TestCase(31.99, 0.06, 0.74)]
        [TestCase(34.99, 0.01, 0.43)]
        [TestCase(34.99, 0.99, 4.43)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomValueDocumentDiscountAndMultipleLineDiscountsAndRecalculateDocumentWithMultipleLines_CalculatedCorrectly(decimal headerDiscountValue,
            decimal lineDiscountPercentage, decimal lineSecondDiscountValue)
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail =>
            {
                decimal grossAfterFirstDiscount = detail.GrossTotal - CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail.GrossTotal * lineDiscountPercentage);
                decimal grossBeforeDocumentDiscount = grossAfterFirstDiscount - lineSecondDiscountValue;
                return grossBeforeDocumentDiscount;
            }));
            decimal expectedGrossTotal = expectedGrossTotalBeforeDocumentDiscount - headerDiscountValue;
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail =>
            {
                decimal netTotalAfterFirstDiscount = detail.UnitPrice * detail.Qty * (1 - lineDiscountPercentage);
                decimal grossAfterFirstDiscount = detail.GrossTotal - CommonFixture.PlatformRoundingHandler.RoundDisplayValue(detail.GrossTotal * lineDiscountPercentage);
                decimal netTotalBeforeDocumentDiscount = netTotalAfterFirstDiscount * ((grossAfterFirstDiscount - lineSecondDiscountValue) / grossAfterFirstDiscount);
                return netTotalBeforeDocumentDiscount;
            }));
            decimal headerDiscountPercentage = (1 - expectedGrossTotal / expectedGrossTotalBeforeDocumentDiscount);
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedNetTotalBeforeDocumentDiscount * (1 - headerDiscountPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmountBeforeDocumentDiscount = expectedGrossTotalBeforeDocumentDiscount - expectedNetTotalBeforeDocumentDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            foreach (DocumentDetail detail in documentHeader.DiscountableDocumentDetails())
            {
                DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountPercentage, lineDiscountPercentage, detail);
                detail.DocumentDetailDiscounts.Add(discount);
                documentService.RecalculateDocumentDetail(detail, documentHeader);
                documentService.RecalculateDocumentCosts(documentHeader, false);

                DocumentDetailDiscount secondDiscount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountValue, lineSecondDiscountValue, detail);
                detail.DocumentDetailDiscounts.Add(secondDiscount);
                documentService.RecalculateDocumentDetail(detail, documentHeader);
                documentService.RecalculateDocumentCosts(documentHeader, false);
            }

            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, headerDiscountValue, CommonFixture.HeaderDiscountValue);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotalBeforeDocumentDiscount, documentHeader.GrossTotalBeforeDocumentDiscount, "Unexpected Gross Total Before Document Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");
        }

        [TestCase(1, 0.05)]
        [TestCase(0.79, 0.12)]
        [TestCase(0.75, 0.24)]
        [TestCase(0.53, 0.47)]
        [TestCase(0.5, 0.58)]
        [TestCase(0.32, 0.66)]
        [TestCase(0.24 ,0.67)]
        [TestCase(0.16, 0.74)]
        public void ApplyCustomDocumentHeaderDiscount_CreateCustomValueDocumentDiscountAndPercentageLineDiscountHavingACanceledLineAndRecalculateDocumentWithMultipleLines_CalculatedCorrectly(decimal headerDiscountValue, decimal lineDiscountPercentage)
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1;
            DocumentDetail lineToDiscount = CommonFixture.SampleReceiptOpenWithDifferentItemsAndACanceledLinePOS1Detail2_SampleItem4Vat23;
            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Where(detail => !detail.IsCanceled && detail != lineToDiscount)
                                                               .Sum(detail => detail.GrossTotal) + (lineToDiscount.GrossTotal * (1 - lineDiscountPercentage)));
            decimal expectedGrossTotal = expectedGrossTotalBeforeDocumentDiscount - headerDiscountValue;
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotalBeforeDocumentDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Where(detail => !detail.IsCanceled && detail != lineToDiscount)
                                                               .Sum(detail => detail.UnitPrice * detail.Qty) + ((lineToDiscount.UnitPrice * lineToDiscount.Qty) * (1 - lineDiscountPercentage)));
            decimal headerDiscountPercentage = (1 - expectedGrossTotal / expectedGrossTotalBeforeDocumentDiscount);
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedNetTotalBeforeDocumentDiscount * (1 - headerDiscountPercentage));
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedVatAmountBeforeDocumentDiscount = expectedGrossTotalBeforeDocumentDiscount - expectedNetTotalBeforeDocumentDiscount;
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(CommonFixture.LineDiscountPercentage, lineDiscountPercentage, lineToDiscount);

            lineToDiscount.DocumentDetailDiscounts.Add(discount);
            documentService.RecalculateDocumentDetail(lineToDiscount, documentHeader);
            documentService.RecalculateDocumentCosts(documentHeader, false);

            documentService.ApplyCustomDocumentHeaderDiscount(ref documentHeader, headerDiscountValue, CommonFixture.HeaderDiscountValue);

            //Assert
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotalBeforeDocumentDiscount, documentHeader.GrossTotalBeforeDocumentDiscount, "Unexpected Gross Total Before Document Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");

        }

        [Test]
        public void CancelDocumentLine_CancelLineAndRecalculateDocument_CalculatedCorrectly()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            DocumentDetail documentDetailToBeCanceled = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1Detail1_SampleItemVat23;

            decimal expectedGrossTotal = documentHeader.GrossTotal - documentDetailToBeCanceled.GrossTotal;
            decimal expectedVatAmount = documentHeader.TotalVatAmount - documentDetailToBeCanceled.TotalVatAmount;
            decimal expectedNetTotal = documentHeader.NetTotal - documentDetailToBeCanceled.NetTotal;

            //Act
            documentService.CancelDocumentLine(documentDetailToBeCanceled);

            //Act
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");
        }

        [Test]
        public void CancelDocumentLine_Cancel2LinesAndRecalculateDocument_CalculatedCorrectly()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            DocumentDetail firstDetailToBeCanceled = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1Detail2_SampleItemVat23;
            DocumentDetail secondDetailToBeCanceled = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1Detail3_SampleItemVat23;
            decimal expectedGrossTotal = documentHeader.GrossTotal - firstDetailToBeCanceled.GrossTotal - secondDetailToBeCanceled.GrossTotal;
            decimal expectedVatAmount = documentHeader.TotalVatAmount - firstDetailToBeCanceled.TotalVatAmount - secondDetailToBeCanceled.TotalVatAmount;
            decimal expectedNetTotal = documentHeader.NetTotal - firstDetailToBeCanceled.NetTotal - secondDetailToBeCanceled.NetTotal;

            //Act
            documentService.CancelDocumentLine(firstDetailToBeCanceled);
            documentService.CancelDocumentLine(secondDetailToBeCanceled);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");
        }

        [TestCase(0.55, 100, 5000)]
        [TestCase(1.65, 30, 140)]
        [TestCase(2.78, 20, 200)]
        [TestCase(3.32, 85, 300)]
        [TestCase(4.43, 50, 1000000)]
        [TestCase(6, 200, 200)]
        public void ApplyLoyalty_LoyaltyIsAppliedAsValueDiscount_CalculatedCorrectly(decimal discountAmount, decimal refundPoints, decimal customerCollectedPoints)
        {
            //Arrange
            Mock<IActionManager> mockActionManager = new Mock<IActionManager>(MockBehavior.Strict);
            DocumentService documentService = CreateDefaultTestDocumentService();
            CommonFixture.AppSettings.LoyaltyRefundType = eLoyaltyRefundType.DISCOUNT;
            CommonFixture.AppSettings.DiscountAmount = discountAmount;
            CommonFixture.AppSettings.RefundPoints = refundPoints;
            CommonFixture.LoyaltyCustomer.CollectedPoints = customerCollectedPoints;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            documentHeader.Customer = CommonFixture.LoyaltyCustomer.Oid;
            int applicationTimesFromPoints = (int)(customerCollectedPoints / refundPoints);
            int applicationTimesFromValue = (int)(documentHeader.GrossTotal / discountAmount);
            int maxApplicationTimes = applicationTimesFromPoints > applicationTimesFromValue ? applicationTimesFromValue : applicationTimesFromPoints;

            decimal discountValue = CommonFixture.AppSettings.DiscountAmount * maxApplicationTimes;
            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDiscount - discountValue);
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.UnitPrice * detail.Qty) * (expectedGrossTotal / expectedGrossTotalBeforeDiscount));
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;

            decimal expectedConsumedPoints = maxApplicationTimes * refundPoints;

            //Act
            documentService.ApplyLoyalty(documentHeader, mockActionManager.Object);

            //Assert
            Assert.AreEqual(expectedConsumedPoints, documentHeader.ConsumedPointsForDiscount, "Unexpected Consumed Points");
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");
        }

        [TestCase(0.09, 50, 1000000)]
        [TestCase(0.11, 200, 200)]
        [TestCase(0.24, 100, 5000)]
        [TestCase(0.34, 20, 200)]
        [TestCase(0.54, 85, 300)]
        [TestCase(0.95, 30, 140)]
        public void ApplyLoyalty_LoyaltyIsAppliedAsPercentageDiscount_CalculatedCorrectly(decimal discountPercentage, decimal refundPoints, decimal customerCollectedPoints)
        {
            //Arrange
            Mock<IActionManager> mockActionManager = new Mock<IActionManager>(MockBehavior.Strict);
            DocumentService documentService = CreateDefaultTestDocumentService();
            CommonFixture.AppSettings.LoyaltyRefundType = eLoyaltyRefundType.DISCOUNT;
            CommonFixture.AppSettings.DiscountPercentage = discountPercentage;
            CommonFixture.AppSettings.RefundPoints = refundPoints;
            CommonFixture.LoyaltyCustomer.CollectedPoints = customerCollectedPoints;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithMultipleLinesPOS1;
            documentHeader.Customer = CommonFixture.LoyaltyCustomer.Oid;

            decimal expectedGrossTotalBeforeDiscount = documentHeader.GrossTotal;
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotalBeforeDiscount * (1 - discountPercentage));
            decimal expectedNetTotalBeforeDiscount = documentHeader.NetTotal;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.DocumentDetails.Sum(detail => detail.UnitPrice * detail.Qty) * (1 - discountPercentage));
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;
            decimal expectedVatAmountBeforeDiscount = expectedGrossTotalBeforeDiscount - expectedNetTotalBeforeDiscount;
            decimal expectedConsumedPoints = refundPoints;

            //Act
            documentService.ApplyLoyalty(documentHeader, mockActionManager.Object);

            //Assert
            Assert.AreEqual(expectedConsumedPoints, documentHeader.ConsumedPointsForDiscount, "Unexpected Consumed Points");
            Assert.AreEqual(expectedGrossTotalBeforeDiscount, documentHeader.GrossTotalBeforeDiscount, "Unexpected Gross Total Before Discount");
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedNetTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount, "Unexpected Net Total Before Discount");
            Assert.AreEqual(expectedVatAmountBeforeDiscount, documentHeader.TotalVatAmountBeforeDiscount, "Unexpected Vat Total Before Discount");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");

            //The following will never be correct. NetTotal and VatAmount will sometimes be +/- with each other
            //Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");   
            //Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            decimal netDeviation = expectedNetTotal - documentHeader.NetTotal;
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount + (netDeviation * (-1)), "Unexpected Vat Total");
            decimal vatDeviation = expectedVatAmount - documentHeader.TotalVatAmount;
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal + (vatDeviation * (-1)), "Unexpected Net Total");
        }

        [Test]
        public void ChangeDocumentLinePrice_ChangeThePriceOfTheDetailAndRecalculate_CalculatedCorrectly()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            DocumentDetail documentDetail = CommonFixture.SampleReceiptOpenWith1LinePOS1Detail_SampleItemVat23;
            decimal newUnitPriceWithVat = 10;
            decimal expectedGrossTotal = newUnitPriceWithVat * documentDetail.Qty;
            decimal expectedNetTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(expectedGrossTotal / (1 + documentDetail.VatFactor));
            decimal expectedVatAmount = expectedGrossTotal - expectedNetTotal;

            //Act
            documentService.ChangeDocumentLinePrice(documentDetail, newUnitPriceWithVat);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal, "Unexpected Gross Total");
            Assert.AreEqual(expectedVatAmount, documentHeader.TotalVatAmount, "Unexpected Vat Total");
            Assert.AreEqual(expectedNetTotal, documentHeader.NetTotal, "Unexpected Net Total");
            Assert.AreEqual(documentHeader.GrossTotal, documentHeader.NetTotal + documentHeader.TotalVatAmount, "Totals are not valid");
            Assert.AreEqual(documentHeader.GrossTotalBeforeDiscount, documentHeader.NetTotalBeforeDiscount + documentHeader.TotalVatAmountBeforeDiscount, "Totals before discount are not valid");
        }

        [Test]
        public void CheckLoyaltyRefund_NonDefaultCustomerHasEnoughPointsForRefundAndTypeAndAppSettingsSupportIt_ReturnsTrue()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.Customer = CommonFixture.LoyaltyCustomer.Oid;
            CommonFixture.LoyaltyCustomer.CollectedPoints = 1000;
            CommonFixture.AppSettings.RefundPoints = 200;
            bool expectedResult = true;

            //Act
            bool result = documentService.CheckLoyaltyRefund(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckLoyaltyRefund_NonDefaultCustomerDoesNotHaveEnoughPointsForRefundAndTypeAndAppSettingsSupportIt_ReturnsFalse()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.Customer = CommonFixture.LoyaltyCustomer.Oid;
            CommonFixture.LoyaltyCustomer.CollectedPoints = 100;
            CommonFixture.AppSettings.RefundPoints = 200;
            bool expectedResult = false;

            //Act
            bool result = documentService.CheckLoyaltyRefund(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckLoyaltyRefund_NonDefaultCustomerHasEnoughPointsForRefundButTypeDoesNotSupportIt_ReturnsFalse()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.Customer = CommonFixture.LoyaltyCustomer.Oid;
            CommonFixture.LoyaltyCustomer.CollectedPoints = 1000;
            CommonFixture.AppSettings.RefundPoints = 200;
            CommonFixture.ReceiptDocumentType.SupportLoyalty = false;
            bool expectedResult = false;

            //Act
            bool result = documentService.CheckLoyaltyRefund(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckLoyaltyRefund_NonDefaultCustomerHasEnoughPointsForRefundButAppSettingsDoNotSupportIt_ReturnsFalse()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.Customer = CommonFixture.LoyaltyCustomer.Oid;
            CommonFixture.LoyaltyCustomer.CollectedPoints = 1000;
            CommonFixture.AppSettings.RefundPoints = 200;
            CommonFixture.AppSettings.SupportLoyalty = false;
            bool expectedResult = false;

            //Act
            bool result = documentService.CheckLoyaltyRefund(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckLoyaltyRefund_DefaultCustomerHasEnoughPointsForRefundAndTypeAndAppSettingsSupportIt_ReturnsFalse()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.Customer = CommonFixture.DefaultCustomer.Oid;
            CommonFixture.DefaultCustomer.CollectedPoints = 1000;
            CommonFixture.AppSettings.RefundPoints = 200;
            CommonFixture.AppSettings.SupportLoyalty = false;
            bool expectedResult = false;

            //Act
            bool result = documentService.CheckLoyaltyRefund(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckLoyaltyRefund_NonDefaultCustomerHasEnoughPointsForRefundButBaseStoreIsRequiredAndIsNotTheCurrentStore_ReturnsFalse()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.Customer = CommonFixture.LoyaltyCustomer.Oid;
            CommonFixture.LoyaltyCustomer.CollectedPoints = 1000;
            CommonFixture.LoyaltyCustomer.RefundStore = CommonFixture.SecondaryStore.Oid;
            CommonFixture.AppSettings.RefundPoints = 200;
            CommonFixture.AppSettings.OnlyRefundStore = true;
            bool expectedResult = false;

            //Act
            bool result = documentService.CheckLoyaltyRefund(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckLoyaltyRefund_NonDefaultCustomerHasEnoughPointsForRefundButBaseStoreIsRequiredAndIsTheCurrentStore_ReturnsTrue()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.Customer = CommonFixture.LoyaltyCustomer.Oid;
            CommonFixture.LoyaltyCustomer.CollectedPoints = 1000;
            CommonFixture.LoyaltyCustomer.RefundStore = CommonFixture.DefaultStore.Oid;
            CommonFixture.AppSettings.RefundPoints = 200;
            CommonFixture.AppSettings.OnlyRefundStore = true;
            bool expectedResult = true;


            //Act
            bool result = documentService.CheckLoyaltyRefund(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckIfShouldGiveChange_DocumentHasAtLeastOnePaymentThatGivesChange_ReturnsTrue()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.DocumentPayments.Add(new DocumentPayment(CommonFixture.MemorySessionManager.GetSession<DocumentPayment>())
                {
                    Amount = documentHeader.GrossTotal,
                    PaymentMethod = CommonFixture.DefaultPaymentMethod.Oid,
                    IncreasesDrawerAmount = CommonFixture.DefaultPaymentMethod.IncreasesDrawerAmount,
                    PaymentMethodCode = CommonFixture.DefaultPaymentMethod.Code,
                    PaymentMethodType = CommonFixture.DefaultPaymentMethod.PaymentMethodType

                });
            bool expectedResult = true;

            //Act
            bool result = documentService.CheckIfShouldGiveChange(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckIfShouldGiveChange_DocumentHasNoPaymentThatGivesChange_ReturnsFalse()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.DocumentPayments.Add(new DocumentPayment(CommonFixture.MemorySessionManager.GetSession<DocumentPayment>())
            {
                Amount = documentHeader.GrossTotal,
                PaymentMethod = CommonFixture.LoyaltyPaymentMethod.Oid,
                IncreasesDrawerAmount = CommonFixture.LoyaltyPaymentMethod.IncreasesDrawerAmount,
                PaymentMethodCode = CommonFixture.LoyaltyPaymentMethod.Code,
                PaymentMethodType = CommonFixture.LoyaltyPaymentMethod.PaymentMethodType
            });
            bool expectedResult = false;

            //Act
            bool result = documentService.CheckIfShouldGiveChange(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckIfShouldOpenDrawer_DocumentHasAtLeastOnePaymentThatOpensDrawer_ReturnsTrue()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.DocumentPayments.Add(new DocumentPayment(CommonFixture.MemorySessionManager.GetSession<DocumentPayment>())
            {
                Amount = documentHeader.GrossTotal,
                PaymentMethod = CommonFixture.DefaultPaymentMethod.Oid,
                IncreasesDrawerAmount = CommonFixture.DefaultPaymentMethod.IncreasesDrawerAmount,
                PaymentMethodCode = CommonFixture.DefaultPaymentMethod.Code,
                PaymentMethodType = CommonFixture.DefaultPaymentMethod.PaymentMethodType

            });
            bool expectedResult = true;

            //Act
            bool result = documentService.CheckIfShouldOpenDrawer(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CheckIfShouldOpenDrawer_DocumentHasNoPaymentThatOpensDrawer_ReturnsFalse()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.DocumentPayments.Add(new DocumentPayment(CommonFixture.MemorySessionManager.GetSession<DocumentPayment>())
            {
                Amount = documentHeader.GrossTotal,
                PaymentMethod = CommonFixture.LoyaltyPaymentMethod.Oid,
                IncreasesDrawerAmount = CommonFixture.LoyaltyPaymentMethod.IncreasesDrawerAmount,
                PaymentMethodCode = CommonFixture.LoyaltyPaymentMethod.Code,
                PaymentMethodType = CommonFixture.LoyaltyPaymentMethod.PaymentMethodType
            });
            bool expectedResult = false;

            //Act
            bool result = documentService.CheckIfShouldOpenDrawer(documentHeader);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ClearAppliedLoyalty_RemoveTheDocumentDiscountThatIsAppliedFromLoyalty_CalculatedCorrectly()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            DocumentDetail detail = CommonFixture.SampleReceiptOpenWith1LinePOS1Detail_SampleItemVat23;

            decimal discountPercentage = 0.20m;
            decimal discountAmount = documentHeader.GrossTotal * discountPercentage;
            decimal expectedGrossTotal = documentHeader.GrossTotal;
            decimal expectedDiscount = 0;
            decimal expectedDiscountPerLine = 0;
            decimal expectedConsumedPoints = 0;
            documentHeader.ConsumedPointsForDiscount = 500;
            documentHeader.PointsDiscountPercentage = discountPercentage;
            documentHeader.PointsDiscountPercentagePerLine = discountPercentage;
            documentHeader.PointsDiscountAmount = discountAmount;
            documentHeader.GrossTotal = documentHeader.GrossTotal - discountAmount;
            documentHeader.TotalDiscountAmount = discountAmount;
            detail.GrossTotal = detail.GrossTotal - detail.GrossTotal * discountPercentage;
            detail.DocumentDetailDiscounts.Add(new DocumentDetailDiscount(CommonFixture.MemorySessionManager.GetSession<DocumentDetailDiscount>())
            {
                DiscountSource = eDiscountSource.POINTS,
                Value = discountAmount
            });
            detail.TotalDiscount = discountAmount;

            //Act
            documentService.ClearAppliedLoyalty(documentHeader);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
            Assert.AreEqual(expectedDiscount, documentHeader.TotalDiscountAmount);
            Assert.AreEqual(expectedDiscountPerLine, documentHeader.PointsDiscountPercentagePerLine);
            Assert.AreEqual(expectedConsumedPoints, documentHeader.ConsumedPointsForDiscount);
        }

        [Test]
        public void ClearPayments_RemoveThePaymentsFromADocument_PaymentsRemoved()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            documentHeader.DocumentPayments.Add(new DocumentPayment(CommonFixture.MemorySessionManager.GetSession<DocumentPayment>())
            {
                Amount = documentHeader.GrossTotal,
                PaymentMethod = CommonFixture.LoyaltyPaymentMethod.Oid,
                IncreasesDrawerAmount = CommonFixture.LoyaltyPaymentMethod.IncreasesDrawerAmount,
                PaymentMethodCode = CommonFixture.LoyaltyPaymentMethod.Code,
                PaymentMethodType = CommonFixture.LoyaltyPaymentMethod.PaymentMethodType
            });
            int expectedPaymentsCount = 0;

            //Act
            documentService.ClearPayments(documentHeader);

            //Assert
            Assert.AreEqual(expectedPaymentsCount, documentHeader.DocumentPayments.Count);
        }

        [Test]
        public void CancelDocument_SimpleReceipt_CanceledCorrectly()
        {
            //Arrange
            DocumentService documentService = CreateDefaultTestDocumentService();
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWith1LinePOS1;
            bool expectedIsCanceled = true;
            bool expectedDocumentOnHold = false;
            bool expectedIsOpen = false;
            int expectedDocumentNumber = 0;

            //Act
            documentService.CancelDocument(documentHeader,DateTime.Now);

            //Assert
            Assert.AreEqual(expectedIsCanceled, documentHeader.IsCanceled);
            Assert.AreEqual(expectedDocumentOnHold, documentHeader.DocumentOnHold);
            Assert.AreEqual(expectedIsOpen, documentHeader.IsOpen);
            Assert.AreEqual(expectedDocumentNumber, documentHeader.DocumentNumber);
        }
    }
}
