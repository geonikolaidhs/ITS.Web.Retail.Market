using ITS.POS.Client.Actions;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.Retail.Platform.Enumerations;
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
    public class ScannedCodeHandlerTests
    {
        PosCommonFixture CommonFixture { get; set; }
        Mock<IItemService> MockItemService { get; set; }
        Mock<IAction> MockAddItemInternalAction { get; set; }
        Mock<IAction> MockAddCustomerInternalAction { get; set; }

        /// <summary>
        /// Runs before every test
        /// </summary>
        /// <param name="data"></param>
        [SetUp]
        public void SetFixture()
        {
            CommonFixture = new PosCommonFixture();
            CommonFixture.Setup();
        }

        /// <summary>
        /// Runs after every test
        /// </summary>
        /// <param name="data"></param>
        [TearDown]
        public void TearDownFixture()
        {
            CommonFixture.Dispose();
            CommonFixture = null;
        }

        ScannedCodeHandler CreateDefaultTestScannedCodeHandler()
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;
            Mock<IFormManager> mockFormManager = new Mock<IFormManager>(MockBehavior.Strict);
            Mock<ICustomerService> mockCustomerService = new Mock<ICustomerService>(MockBehavior.Strict);
            mockCustomerService.Setup(mock => mock.SearchCustomer<Customer>(PosCommonFixture.LOYALTY_CUSTOMER_CARD))
                               .Returns(CommonFixture.LoyaltyCustomer);
            Mock<IAppContext> mockAppContext = new Mock<IAppContext>(MockBehavior.Strict);
            mockAppContext.Setup(mock => mock.CurrentDocument).Returns(CommonFixture.SampleReceiptOpenWith1LinePOS1);
            Mock<IActionManager> mockActionManager = new Mock<IActionManager>(MockBehavior.Strict);
            Mock<IDeviceManager> deviceManager = new Mock<IDeviceManager>(MockBehavior.Strict);
            MockAddItemInternalAction = new Mock<IAction>(MockBehavior.Loose);
            mockActionManager.Setup(mock => mock.GetAction(eActions.ADD_ITEM_INTERNAL))
                             .Returns(MockAddItemInternalAction.Object);
            MockAddCustomerInternalAction = new Mock<IAction>(MockBehavior.Loose);
            mockActionManager.Setup(mock => mock.GetAction(eActions.ADD_CUSTOMER_INTERNAL))
                             .Returns(MockAddCustomerInternalAction.Object);
            

            PriceCatalogDetail pcd = CommonFixture.SampleItemPriceCatalogDetail;
            MockItemService = new Mock<IItemService>(MockBehavior.Strict);

            //MockItemService.Setup(mock => mock.GetUnitPrice(CommonFixture.RootPriceCatalog, CommonFixture.SampleItemVat23, out pcd,
            //    CommonFixture.SampleItemCodeBarcode, true,false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //   .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            //MockItemService.Setup(mock => mock.GetUnitPrice(CommonFixture.RootPriceCatalog, CommonFixture.SampleItemVat23, out pcd,
            //    CommonFixture.SampleItemBarcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //   .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            //MockItemService.Setup(mock => mock.GetUnitPrice(CommonFixture.RootPriceCatalog, CommonFixture.SampleItemVat23, out pcd,
            //    CommonFixture.SampleItemWeightedBarcodeValue, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            //MockItemService.Setup(mock => mock.GetUnitPrice(CommonFixture.RootPriceCatalog, CommonFixture.SampleItemVat23, out pcd,
            //    CommonFixture.SampleItemWeightedBarcodeQty, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);


            //MockItemService.Setup(mock => mock.GetUnitPriceFromPolicy(CommonFixture.PriceCatalogPolicy, CommonFixture.SampleItemVat23, out pcd,
            //    CommonFixture.SampleItemCodeBarcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //   .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            //MockItemService.Setup(mock => mock.GetUnitPriceFromPolicy(CommonFixture.PriceCatalogPolicy, CommonFixture.SampleItemVat23, out pcd,
            //    CommonFixture.SampleItemBarcode, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //   .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            //MockItemService.Setup(mock => mock.GetUnitPriceFromPolicy(CommonFixture.PriceCatalogPolicy, CommonFixture.SampleItemVat23, out pcd,
            //    CommonFixture.SampleItemWeightedBarcodeValue, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);
            //MockItemService.Setup(mock => mock.GetUnitPriceFromPolicy(CommonFixture.PriceCatalogPolicy, CommonFixture.SampleItemVat23, out pcd,
            //    CommonFixture.SampleItemWeightedBarcodeQty, true, false, PriceCatalogSearchMethod.PRICECATALOG_TREE))
            //    .Returns(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT);


            return new ScannedCodeHandler(MockItemService.Object, memorySessionManager, mockActionManager.Object, mockFormManager.Object, mockAppContext.Object, mockCustomerService.Object,deviceManager.Object);
        }

        [Test]
        public void HandleScannedCode_InputIsItemCode_AddItemExecutedWithCorrectData()
        {
            //Arrange
            ScannedCodeHandler scannedCodeHandler = CreateDefaultTestScannedCodeHandler();
            Mock<IGetItemPriceForm> getItemPriceForm = new Mock<IGetItemPriceForm>(MockBehavior.Strict);
            string scannedCode = PosCommonFixture.SAMPLE_ITEM_VAT23_CODE;
            bool foundButInactive = false;
            bool includeInactive = false;
            MockItemService.Setup(mock => mock.GetItemAndBarcodeByCode(scannedCode, includeInactive, out foundButInactive, null))
                           .Returns(new KeyValuePair<Item, Barcode>(CommonFixture.SampleItemVat23, CommonFixture.SampleItemCodeBarcode));
            ActionAddItemInternalParams expectedParams = new ActionAddItemInternalParams(CommonFixture.SampleItemVat23,
                                                                        CommonFixture.SampleItemPriceCatalogDetail, 
                                                                        CommonFixture.SampleItemCodeBarcode, 1, false, 
                                                                        PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT, "", false, false);

            //Act
            scannedCodeHandler.HandleScannedCode(getItemPriceForm.Object, CommonFixture.AppSettings, includeInactive, scannedCode, 1, false, false, false,false);

            //Assert
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.Item.Oid == expectedParams.Item.Oid), false,true), Times.Once(), "Invalid Item");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.PriceCatalogDetail.Oid == expectedParams.PriceCatalogDetail.Oid), false, true), Times.Once(), "Invalid Pcd");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.Quantity == expectedParams.Quantity), false, true), Times.Once(), "Invalid Quantity");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.UserBarcode.Oid == expectedParams.UserBarcode.Oid), false, true), Times.Once(), "Invalid Barcode");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.HasCustomPrice == expectedParams.HasCustomPrice), false, true), Times.Once(), "Invalid HasCustomPrice");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.FromScanner == expectedParams.FromScanner), false, true), Times.Once(), "Invalid FromScanner");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.CustomDescription == expectedParams.CustomDescription), false, true), Times.Once(), "Invalid CustomDescription");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.CustomPrice == expectedParams.CustomPrice), false, true), Times.Once(), "Invalid CustomPrice");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.IsReturn == expectedParams.IsReturn), false, true), Times.Once(), "Invalid IsReturn");
        }

        [Test]
        public void HandleScannedCode_InputIsItemBarcode_AddItemExecutedWithCorrectData()
        {
            //Arrange
            ScannedCodeHandler scannedCodeHandler = CreateDefaultTestScannedCodeHandler();
            Mock<IGetItemPriceForm> getItemPriceForm = new Mock<IGetItemPriceForm>(MockBehavior.Strict);
            string scannedCode = PosCommonFixture.SAMPLE_ITEM_VAT23_BARCODE;
            bool foundButInactive = false;
            bool includeInactive = false;
            MockItemService.Setup(mock => mock.GetItemAndBarcodeByCode(scannedCode, includeInactive, out foundButInactive, null))
                           .Returns(new KeyValuePair<Item, Barcode>(CommonFixture.SampleItemVat23, CommonFixture.SampleItemBarcode));
            ActionAddItemInternalParams expectedParams = new ActionAddItemInternalParams(CommonFixture.SampleItemVat23,
                                                                        CommonFixture.SampleItemPriceCatalogDetail, CommonFixture.SampleItemBarcode, 1, false, PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT, "", false, false);

            //Act
            scannedCodeHandler.HandleScannedCode(getItemPriceForm.Object, CommonFixture.AppSettings, includeInactive, scannedCode, 1, false, false, false, false);

            //Assert
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.Item.Oid == expectedParams.Item.Oid), false, true), Times.Once(), "Invalid Item");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.PriceCatalogDetail.Oid == expectedParams.PriceCatalogDetail.Oid), false, true), Times.Once(), "Invalid Pcd");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.Quantity == expectedParams.Quantity), false, true), Times.Once(), "Invalid Quantity");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.UserBarcode.Oid == expectedParams.UserBarcode.Oid), false, true), Times.Once(), "Invalid Barcode");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.HasCustomPrice == expectedParams.HasCustomPrice), false, true), Times.Once(), "Invalid HasCustomPrice");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.FromScanner == expectedParams.FromScanner), false, true), Times.Once(), "Invalid FromScanner");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.CustomDescription == expectedParams.CustomDescription), false, true), Times.Once(), "Invalid CustomDescription");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.CustomPrice == expectedParams.CustomPrice), false, true), Times.Once(), "Invalid CustomPrice");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.IsReturn == expectedParams.IsReturn), false, true), Times.Once(), "Invalid IsReturn");

        }

        [Test]
        public void HandleScannedCode_InputIsWeightedBarcodeWithValue_AddItemExecutedWithCorrectData()
        {
            //Arrange
            ScannedCodeHandler scannedCodeHandler = CreateDefaultTestScannedCodeHandler();
            Mock<IGetItemPriceForm> getItemPriceForm = new Mock<IGetItemPriceForm>(MockBehavior.Strict);

            string scannedCode = PosCommonFixture.SAMPLE_ITEM_VAT23_SCANNED_WEIGHTED_BARCODE_VALUE;
            bool foundButInactive = false;
            bool includeInactive = false;
            MockItemService.Setup(mock => mock.GetItemAndBarcodeByCode(scannedCode, includeInactive, out foundButInactive, null))
                           .Returns(new KeyValuePair<Item, Barcode>(null, null));
            MockItemService.Setup(mock => mock.GetItemAndBarcodeByCode(PosCommonFixture.SAMPLE_ITEM_VAT23_WEIGHTED_BARCODE_VALUE, includeInactive, out foundButInactive, null))
                           .Returns(new KeyValuePair<Item, Barcode>(CommonFixture.SampleItemVat23, CommonFixture.SampleItemWeightedBarcodeValue));
            decimal expectedQty = Math.Round( PosCommonFixture.SAMPLE_ITEM_VAT23_WEIGHTED_VALUE / PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT, 3, MidpointRounding.AwayFromZero);
            ActionAddItemInternalParams expectedParams = new ActionAddItemInternalParams(CommonFixture.SampleItemVat23,
                                                                        CommonFixture.SampleItemPriceCatalogDetail,
                                                                        CommonFixture.SampleItemWeightedBarcodeValue, expectedQty, false,
                                                                        PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT, "", false, false);

            //Act
            scannedCodeHandler.HandleScannedCode(getItemPriceForm.Object, CommonFixture.AppSettings, includeInactive, scannedCode, 1, false, false, false, false);

            //Assert
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.Item.Oid == expectedParams.Item.Oid), false, true), Times.Once(), "Invalid Item");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.PriceCatalogDetail.Oid == expectedParams.PriceCatalogDetail.Oid), false, true), Times.Once(), "Invalid Pcd");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.Quantity == expectedParams.Quantity), false, true), Times.Once(), "Invalid Quantity");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.UserBarcode.Oid == expectedParams.UserBarcode.Oid), false, true), Times.Once(), "Invalid Barcode");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.HasCustomPrice == expectedParams.HasCustomPrice), false, true), Times.Once(), "Invalid HasCustomPrice");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.FromScanner == expectedParams.FromScanner), false, true), Times.Once(), "Invalid FromScanner");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.CustomDescription == expectedParams.CustomDescription), false, true), Times.Once(), "Invalid CustomDescription");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.CustomPrice == expectedParams.CustomPrice), false, true), Times.Once(), "Invalid CustomPrice");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.IsReturn == expectedParams.IsReturn), false, true), Times.Once(), "Invalid IsReturn");
        }

        [Test]
        public void HandleScannedCode_InputIsWeightedBarcodeWithQty_AddItemExecutedWithCorrectData()
        {
            //Arrange
            ScannedCodeHandler scannedCodeHandler = CreateDefaultTestScannedCodeHandler();
            Mock<IGetItemPriceForm> getItemPriceForm = new Mock<IGetItemPriceForm>(MockBehavior.Strict);

            string scannedCode = PosCommonFixture.SAMPLE_ITEM_VAT23_SCANNED_WEIGHTED_BARCODE_QTY;
            bool foundButInactive = false;
            bool includeInactive = false;
            MockItemService.Setup(mock => mock.GetItemAndBarcodeByCode(scannedCode, includeInactive, out foundButInactive, null))
                           .Returns(new KeyValuePair<Item, Barcode>(null, null));
            MockItemService.Setup(mock => mock.GetItemAndBarcodeByCode(PosCommonFixture.SAMPLE_ITEM_VAT23_WEIGHTED_BARCODE_QTY, includeInactive, out foundButInactive, null))
                           .Returns(new KeyValuePair<Item, Barcode>(CommonFixture.SampleItemVat23, CommonFixture.SampleItemWeightedBarcodeQty));
            decimal expectedQty = PosCommonFixture.SAMPLE_ITEM_VAT23_WEIGHTED_QTY;
            ActionAddItemInternalParams expectedParams = new ActionAddItemInternalParams(CommonFixture.SampleItemVat23,
                                                                        CommonFixture.SampleItemPriceCatalogDetail,
                                                                        CommonFixture.SampleItemWeightedBarcodeQty, expectedQty, false,
                                                                        PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT, "", false, false);

            //Act
            scannedCodeHandler.HandleScannedCode(getItemPriceForm.Object, CommonFixture.AppSettings, includeInactive, scannedCode, 1, false, false, false, false);

            //Assert
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.Item.Oid == expectedParams.Item.Oid), false, true), Times.Once(), "Invalid Item");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.PriceCatalogDetail.Oid == expectedParams.PriceCatalogDetail.Oid), false, true), Times.Once(), "Invalid Pcd");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.Quantity == expectedParams.Quantity), false, true), Times.Once(), "Invalid Quantity");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.UserBarcode.Oid == expectedParams.UserBarcode.Oid), false, true), Times.Once(), "Invalid Barcode");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.HasCustomPrice == expectedParams.HasCustomPrice), false, true), Times.Once(), "Invalid HasCustomPrice");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.FromScanner == expectedParams.FromScanner), false, true), Times.Once(), "Invalid FromScanner");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.CustomDescription == expectedParams.CustomDescription), false, true), Times.Once(), "Invalid CustomDescription");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.CustomPrice == expectedParams.CustomPrice), false, true), Times.Once(), "Invalid CustomPrice");
            MockAddItemInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddItemInternalParams>(par => par.IsReturn == expectedParams.IsReturn), false, true), Times.Once(), "Invalid IsReturn");
        }

        [Test]
        public void HandleScannedCode_InputIsCustomerCard_AddCustomerExecutedWithCorrectData()
        {
            //Arrange
            ScannedCodeHandler scannedCodeHandler = CreateDefaultTestScannedCodeHandler();
            Mock<IGetItemPriceForm> getItemPriceForm = new Mock<IGetItemPriceForm>(MockBehavior.Strict);
            string scannedCode = PosCommonFixture.LOYALTY_CUSTOMER_SCANNED_CARD;
            bool includeInactive = false;
            bool foundButInactive = false;
            Customer expectedCustomer = CommonFixture.LoyaltyCustomer;
            string customerCard = PosCommonFixture.LOYALTY_CUSTOMER_CARD;
            MockItemService.Setup(mock => mock.GetItemAndBarcodeByCode(scannedCode, includeInactive, out foundButInactive, null))
                           .Returns(new KeyValuePair<Item, Barcode>(null, null));
            ActionAddCustomerInternalParams expectedParams = new ActionAddCustomerInternalParams(expectedCustomer, customerCard,null);

            //Act
            scannedCodeHandler.HandleScannedCode(getItemPriceForm.Object, CommonFixture.AppSettings, includeInactive, scannedCode, 1, false, false, false, false);

            //Assert
            MockAddCustomerInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddCustomerInternalParams>(par => par.Customer.Oid == expectedParams.Customer.Oid), false, true), Times.Once(), "Invalid Customer");
            MockAddCustomerInternalAction.Verify(mock => mock.Execute(It.Is<ActionAddCustomerInternalParams>(par => par.CustomerLookupCode == expectedParams.CustomerLookupCode), false, true), Times.Once(), "Invalid CustomerLookupCode");
        }
    }
}
