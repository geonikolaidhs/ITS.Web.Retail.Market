using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
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
    public class ItemServiceTests
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

        ItemService CreateDefaultTestCategoryService()
        {
            ISessionManager sessionManager = CommonFixture.MemorySessionManager;
            Mock<IConfigurationManager> mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(mock => mock.GetAppSettings()).Returns(CommonFixture.AppSettings);
            //mockConfigurationManager.Setup(mock => mock.DefaultCustomerOid).Returns(CommonFixture.DefaultCustomer.Oid);
            //mockConfigurationManager.Setup(mock => mock.CurrentStoreOid).Returns(CommonFixture.DefaultStore.Oid);

            return new ItemService(sessionManager, mockConfigurationManager.Object);
        }

        //[Test]
        //public void GetUnitPrice_ItemThatHasPriceInTheGivenCatalog_NormalPriceFound()
        //{
        //    //Arrange
        //    ItemService itemService = CreateDefaultTestCategoryService();
        //    PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;
        //    Item item = CommonFixture.SampleItemVat23;
        //    PriceCatalogDetail pcd;
        //    decimal expectedPrice = PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT;

        //    //Act
        //    decimal price = itemService.GetUnitPrice(priceCatalog, item, out pcd);

        //    //Assert
        //    Assert.AreEqual(expectedPrice, price, "Invalid Price");
        //}

        //[Test]
        //public void GetUnitPrice_ItemThatHasPriceInTheParentCatalog_NormalPriceFound()
        //{
        //    //Arrange
        //    ItemService itemService = CreateDefaultTestCategoryService();
        //    PriceCatalog priceCatalog = CommonFixture.SubCatalog;
        //    Item item = CommonFixture.SampleItemVat23;
        //    PriceCatalogDetail pcd;
        //    decimal expectedPrice = PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT;

        //    //Act
        //    decimal price = itemService.GetUnitPrice(priceCatalog, item, out pcd);

        //    //Assert
        //    Assert.AreEqual(expectedPrice, price, "Invalid Price");
        //}

        //[Test]
        //public void GetUnitPrice_ItemThatHasNoPriceInTheCatalogTree_ThrowsException()
        //{
        //    //Arrange
        //    ItemService itemService = CreateDefaultTestCategoryService();
        //    PriceCatalogPolicy priceCatalogPolicy = CommonFixture.SubPriceCatalogPolicy;
        //    Item item = CommonFixture.SampleItemVat23;
        //    CommonFixture.SampleItemPriceCatalogDetail.Delete();
        //    CommonFixture.SampleItemPriceCatalogDetail.Session.CommitTransaction();
        //    PriceCatalogDetail priceCatalogDetail;

        //    //Act
        //    /* Assert.Throws(typeof(PriceNotFoundException), () =>
        //     {
        //         decimal price = itemService.GetUnitPriceFromPolicy(priceCatalogPolicy, item, out priceCatalogDetail);
        //     });*/
        //    decimal price = itemService.GetUnitPriceFromPolicy(priceCatalogPolicy, item, out priceCatalogDetail);
        //    Assert.AreEqual(price, -1);

        //    //Assert
        //}

        //[Test]
        //public void GetUnitPrice_ItemThatHasPriceInTheGivenCatalogAndHasAValidDatePrice_DatePriceFound()
        //{
        //    //Arrange
        //    ItemService itemService = CreateDefaultTestCategoryService();
        //    PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;
        //    CommonFixture.SampleItemPriceCatalogDetail.TimeValues[0].TimeValue = PosCommonFixture.SAMPLE_ITEM_VAT23_DATE_PRICE_WITH_VAT;
        //    CommonFixture.SampleItemPriceCatalogDetail.TimeValues[0].TimeValueValidFromDate = DateTime.Now.AddDays(-1);
        //    CommonFixture.SampleItemPriceCatalogDetail.TimeValues[0].TimeValueValidUntilDate = DateTime.Now.AddDays(1);
        //    Item item = CommonFixture.SampleItemVat23;
        //    PriceCatalogDetail pcd;
        //    decimal expectedPrice = PosCommonFixture.SAMPLE_ITEM_VAT23_DATE_PRICE_WITH_VAT;

        //    //Act
        //    decimal price = itemService.GetUnitPrice(priceCatalog, item, out pcd);
            
        //    //Assert
        //    Assert.AreEqual(expectedPrice, price, "Invalid Price");
        //}

        [Test]
        public void GetPointsOfItem_ItemDoesNotHavePointsInCategoryTree_CalculatedCorrectly()
        {
            //Arrange
            ItemService itemService = CreateDefaultTestCategoryService();
            Item item = CommonFixture.SampleItemVat23;
            item.Points = 10;
            DocumentType documentType = CommonFixture.ReceiptDocumentType;
            documentType.SupportLoyalty = true;
            CommonFixture.AppSettings.SupportLoyalty = true;
            PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;
            priceCatalog.SupportLoyalty = true;
            decimal expectedPoints = item.Points;

            //Act
            decimal points = itemService.GetPointsOfItem(item, documentType);

            //Assert
            Assert.AreEqual(expectedPoints, points,"Invalid Points");
        }

        [Test]
        public void GetPointsOfItem_ItemHasPointsInCategoryTree_CalculatedCorrectly()
        {
            //Arrange
            ItemService itemService = CreateDefaultTestCategoryService();
            Item item = CommonFixture.SampleItemVat23;
            ItemAnalyticTree iat = new ItemAnalyticTree(CommonFixture.MemorySessionManager.GetSession<CustomerCategory>());
            iat.Object = item.Oid;
            iat.Node = CommonFixture.SubSubItemCategory2_1.Oid;
            iat.Root = CommonFixture.RootItemCategory.Oid;
            iat.Save();
            iat.Session.CommitTransaction();
            DocumentType documentType = CommonFixture.ReceiptDocumentType;
            documentType.SupportLoyalty = true;
            CommonFixture.AppSettings.SupportLoyalty = true;
            PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;
            priceCatalog.SupportLoyalty = true;
            item.Points = 10;
            CommonFixture.SubItemCategory2.Points = 20;
            CommonFixture.SubSubItemCategory2_1.Points = 5;

            decimal expectedPoints = item.Points + CommonFixture.SubItemCategory2.Points + CommonFixture.SubSubItemCategory2_1.Points;

            //Act
            decimal points = itemService.GetPointsOfItem(item, documentType);

            //Assert
            Assert.AreEqual(expectedPoints, points, "Invalid Points");
        }

        [Test]
        public void GetItemAndBarcodeByCode_InputIsItemCode_ItemWithITemCodeBarcodeReturned()
        {
            //Arrange
            ItemService itemService = CreateDefaultTestCategoryService();
            string code = PosCommonFixture.SAMPLE_ITEM_VAT23_CODE;
            Item expectedItem = CommonFixture.SampleItemVat23;
            Barcode expectedBarcode = CommonFixture.SampleItemCodeBarcode;

            //Act
            bool foundButInactive;
            KeyValuePair<Item,Barcode> result = itemService.GetItemAndBarcodeByCode(code,false,out foundButInactive);

            //Assert
            Assert.AreEqual(expectedItem, result.Key);
            Assert.AreEqual(expectedBarcode, result.Value);
        }

        [Test]
        public void GetItemAndBarcodeByCode_InputIsItemBarcode_ItemWithItemBarcodeReturned()
        {
            //Arrange
            ItemService itemService = CreateDefaultTestCategoryService();
            string code = PosCommonFixture.SAMPLE_ITEM_VAT23_BARCODE;
            Item expectedItem = CommonFixture.SampleItemVat23;
            Barcode expectedBarcode = CommonFixture.SampleItemBarcode;

            //Act
            bool foundButInactive;
            KeyValuePair<Item, Barcode> result = itemService.GetItemAndBarcodeByCode(code, false, out foundButInactive);

            //Assert
            Assert.AreEqual(expectedItem, result.Key);
            Assert.AreEqual(expectedBarcode, result.Value);
        }

        [Test]
        public void GetItemAndBarcodeByCode_SearchByPlu_ItemWithWeightedBarcodeReturned()
        {
            //Arrange
            ItemService itemService = CreateDefaultTestCategoryService();
            string code = "irrelevant code";
            string plu = PosCommonFixture.WEIGHTED_BARCODE_QTY_PLU;
            Item expectedItem = CommonFixture.SampleItemVat23;
            Barcode expectedBarcode = CommonFixture.SampleItemWeightedBarcodeQty;

            //Act
            bool foundButInactive;
            KeyValuePair<Item, Barcode> result = itemService.GetItemAndBarcodeByCode(code, false, out foundButInactive, plu);

            //Assert
            Assert.AreEqual(expectedItem, result.Key);
            Assert.AreEqual(expectedBarcode, result.Value);
        }
    }
}
