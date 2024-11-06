using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
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
    public class PromotionServiceTests
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

        PromotionService CreateDefaultPromotionService()
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;
            //// Platform and itermediate services are not mocked because they are core parts of the unit. 
            //// DocumentService is not mocked because it is difficult to mock efficiently. However because of this, these tests are no longer "pure" Unit Tests
            //// Further refactoring should be applied to remove this complexity.

            PlatformRoundingHandler platformRoundingHandler = new PlatformRoundingHandler();
            platformRoundingHandler.SetOwnerApplicationSettings(CommonFixture.AppSettings);

            Mock<IConfigurationManager> mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(mock => mock.GetAppSettings()).Returns(CommonFixture.AppSettings);
            mockConfigurationManager.Setup(mock => mock.DefaultCustomerOid).Returns(CommonFixture.DefaultCustomer.Oid);
            mockConfigurationManager.Setup(mock => mock.DefaultDocumentTypeOid).Returns(CommonFixture.ReceiptDocumentType.Oid);

            Mock<IItemService> mockItemService = new Mock<IItemService>(MockBehavior.Strict);
            mockItemService.Setup(mock => mock.GetPointsOfItem(It.IsAny<Item>(), It.IsAny<DocumentType>())).Returns(0);

            PlatformDocumentDiscountService platformDocumentDiscountService = new PlatformDocumentDiscountService();
            DocumentService documentService = new DocumentService(CommonFixture.MemorySessionManager, mockConfigurationManager.Object, mockItemService.Object,
                                                                  platformDocumentDiscountService, platformRoundingHandler);

            PersistentObjectMap objectMap = new PersistentObjectMap();
            IntermediateModelManager intermediateModelManager = new IntermediateModelManager(CommonFixture.MemorySessionManager, objectMap);
            PlatformPromotionService platformPromotionService = new PlatformPromotionService(documentService,
                                                                                             intermediateModelManager,
                                                                                             platformRoundingHandler,
                                                                                             platformDocumentDiscountService,
                                                                                             CommonFixture.DefaultCustomer.Oid
                                                                                             );

            Mock<IReceiptBuilder> receiptBuilder = new Mock<IReceiptBuilder>(MockBehavior.Strict);
            Mock<IDocumentService> mockDocumentService = new Mock<IDocumentService>(MockBehavior.Strict);

            PromotionService promotionService = new PromotionService(CommonFixture.MemorySessionManager,
                                                                     mockConfigurationManager.Object,
                                                                     mockDocumentService.Object,
                                                                     receiptBuilder.Object,
                                                                     platformPromotionService);

            return promotionService;
        }


        [Test]
        public void ExecutePromotions_ItemsAThreeTimesGivesOneOfThemAsGift_CalculatedCorrectly()
        {
            //Arrange
            PromotionService promotionService = CreateDefaultPromotionService();
            DateTime activeAtDate = DateTime.Now;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithDifferentItemsPOS1;
            decimal expectedGrossTotal = documentHeader.GrossTotal - PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT;
            UnitOfWork uow = CommonFixture.MemorySessionManager.GetSession<Promotion>();
            Item itemA = CommonFixture.SampleItemVat23;
            PriceCatalogPolicy priceCatalogPolicy = CommonFixture.PriceCatalogPolicy;
            //PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;

            //-------------------
            //Create Promotion  ItemA(18) x 3 , you get one ItemA(18) free
            //-------------------
            Promotion promotion = new Promotion(uow);
            promotion.StartDate = DateTime.MinValue.AddYears(1950);
            promotion.EndDate = DateTime.MaxValue;
            promotion.StartTime = DateTime.Now.Date;
            promotion.EndTime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            promotion.ActiveDaysOfWeek = DaysOfWeek.Friday | DaysOfWeek.Monday | DaysOfWeek.Saturday | DaysOfWeek.Sunday | DaysOfWeek.Thursday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday;
            promotion.IsActive = true;
            promotion.MaxExecutionsPerReceipt = 1;
            promotion.Description = "Test Promotion";
            promotion.Code = "001";

            //PriceCatalogPromotion priceCatalogPromotion = new PriceCatalogPromotion(uow);
            //priceCatalogPromotion.PriceCatalog = priceCatalog.Oid;
            //priceCatalogPromotion.Promotion = promotion.Oid;
            PriceCatalogPolicyPromotion priceCatalogPolicyPromotion = new PriceCatalogPolicyPromotion(uow);
            priceCatalogPolicyPromotion.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            priceCatalogPolicyPromotion.Promotion = promotion.Oid;

            PromotionApplicationRuleGroup rootRuleGroup = new PromotionApplicationRuleGroup(uow);
            rootRuleGroup.Operator = eGroupOperatorType.And;
            rootRuleGroup.Promotion = promotion.Oid;
            promotion.PromotionApplicationRuleGroupOid = rootRuleGroup.Oid;

            PromotionItemApplicationRule itemRule = new PromotionItemApplicationRule(uow);
            itemRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule.Item = itemA.Oid;
            itemRule.Quantity = 3;

            PromotionItemExecution itemExec = new PromotionItemExecution(uow);
            itemExec.Promotion = promotion.Oid;
            itemExec.Item = itemA.Oid;
            itemExec.Quantity = 1;
            itemExec.DiscountType = CommonFixture.LineDiscountPercentage.Oid;
            itemExec.Percentage = 1; //100% discount

            uow.CommitChanges();

            //Act
            promotionService.ExecutePromotions(documentHeader,
                                               priceCatalogPolicy,
                                               CommonFixture.AppSettings,
                                               activeAtDate,
                                               false);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
        }

        [Test]
        public void ExecutePromotions_ThreeItemsAPlusOneItemBGivesItemBAsGift_CalculatedCorrectly()
        {
            //Arrange
            PromotionService promotionService = CreateDefaultPromotionService();
            DateTime activeAtDate = DateTime.Now;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithDifferentItemsPOS1;
            decimal expectedGrossTotal = documentHeader.GrossTotal - PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT;
            UnitOfWork uow = CommonFixture.MemorySessionManager.GetSession<Promotion>();
            Item itemA = CommonFixture.SampleItemVat23;
            Item itemB = CommonFixture.SampleItem2Vat23;
            PriceCatalogPolicy priceCatalogPolicy = CommonFixture.PriceCatalogPolicy;
            //PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;

            //-------------------
            //Create Promotion  ItemA(18) x 3 + ItemB(67) x 1, you get ItemB(67) free
            //-------------------
            Promotion promotion = new Promotion(uow);
            promotion.StartDate = DateTime.MinValue.AddYears(1950);
            promotion.EndDate = DateTime.MaxValue;
            promotion.StartTime = DateTime.Now.Date;
            promotion.EndTime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            promotion.ActiveDaysOfWeek = DaysOfWeek.Friday | DaysOfWeek.Monday | DaysOfWeek.Saturday | DaysOfWeek.Sunday | DaysOfWeek.Thursday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday;
            promotion.IsActive = true;
            promotion.MaxExecutionsPerReceipt = 1;
            promotion.Description = "Test Promotion";
            promotion.Code = "001";

            //PriceCatalogPromotion priceCatalogPromotion = new PriceCatalogPromotion(uow);
            //priceCatalogPromotion.PriceCatalog = priceCatalog.Oid;
            //priceCatalogPromotion.Promotion = promotion.Oid;
            PriceCatalogPolicyPromotion priceCatalogPolicyPromotion = new PriceCatalogPolicyPromotion(uow);
            priceCatalogPolicyPromotion.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            priceCatalogPolicyPromotion.Promotion = promotion.Oid;

            PromotionApplicationRuleGroup rootRuleGroup = new PromotionApplicationRuleGroup(uow);
            rootRuleGroup.Operator = eGroupOperatorType.And;
            rootRuleGroup.Promotion = promotion.Oid;
            promotion.PromotionApplicationRuleGroupOid = rootRuleGroup.Oid;

            PromotionItemApplicationRule itemRule = new PromotionItemApplicationRule(uow);
            itemRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule.Item = itemA.Oid;
            itemRule.Quantity = 3;

            PromotionItemApplicationRule itemRule2 = new PromotionItemApplicationRule(uow);
            itemRule2.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule2.Item = itemB.Oid;
            itemRule2.Quantity = 1;

            PromotionItemExecution itemExec = new PromotionItemExecution(uow);
            itemExec.Promotion = promotion.Oid;
            itemExec.Item = itemB.Oid;
            itemExec.Quantity = 1;
            itemExec.DiscountType = CommonFixture.LineDiscountPercentage.Oid;
            itemExec.Percentage = 1; //100% discount

            uow.CommitChanges();

            //Act
            promotionService.ExecutePromotions(documentHeader,
                                              priceCatalogPolicy,
                                               CommonFixture.AppSettings,
                                               activeAtDate,
                                               false);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
        }


        [Test]
        public void ExecutePromotions_OneItemsAPlusOneItemBGivesItemBAsGift_RanThreeTimesAndCalculatedCorrectly()
        {
            //Arrange
            PromotionService promotionService = CreateDefaultPromotionService();
            DateTime activeAtDate = DateTime.Now;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithDifferentItemsPOS1;
            decimal expectedGrossTotal = documentHeader.GrossTotal - PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT * 3;
            UnitOfWork uow = CommonFixture.MemorySessionManager.GetSession<Promotion>();
            Item itemA = CommonFixture.SampleItemVat23;
            Item itemB = CommonFixture.SampleItem2Vat23;
            PriceCatalogPolicy priceCatalogPolicy = CommonFixture.PriceCatalogPolicy;
            //PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;

            //-------------------
            //Create Promotion  ItemA(18) x 1 + ItemB(67) x 1, you get ItemB(67) free
            //-------------------
            Promotion promotion = new Promotion(uow);
            promotion.StartDate = DateTime.MinValue.AddYears(1950);
            promotion.EndDate = DateTime.MaxValue;
            promotion.StartTime = DateTime.Now.Date;
            promotion.EndTime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            promotion.ActiveDaysOfWeek = DaysOfWeek.Friday | DaysOfWeek.Monday | DaysOfWeek.Saturday | DaysOfWeek.Sunday | DaysOfWeek.Thursday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday;
            promotion.IsActive = true;
            promotion.MaxExecutionsPerReceipt = 9999;
            promotion.Description = "Test Promotion";
            promotion.Code = "001";

            //PriceCatalogPromotion priceCatalogPromotion = new PriceCatalogPromotion(uow);
            //priceCatalogPromotion.PriceCatalog = priceCatalog.Oid;
            //priceCatalogPromotion.Promotion = promotion.Oid;
            PriceCatalogPolicyPromotion priceCatalogPolicyPromotion = new PriceCatalogPolicyPromotion(uow);
            priceCatalogPolicyPromotion.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            priceCatalogPolicyPromotion.Promotion = promotion.Oid;

            PromotionApplicationRuleGroup rootRuleGroup = new PromotionApplicationRuleGroup(uow);
            rootRuleGroup.Operator = eGroupOperatorType.And;
            rootRuleGroup.Promotion = promotion.Oid;
            promotion.PromotionApplicationRuleGroupOid = rootRuleGroup.Oid;

            PromotionItemApplicationRule itemRule = new PromotionItemApplicationRule(uow);
            itemRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule.Item = itemA.Oid;
            itemRule.Quantity = 1;

            PromotionItemApplicationRule itemRule2 = new PromotionItemApplicationRule(uow);
            itemRule2.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule2.Item = itemB.Oid;
            itemRule2.Quantity = 1;

            PromotionItemExecution itemExec = new PromotionItemExecution(uow);
            itemExec.Promotion = promotion.Oid;
            itemExec.Item = itemB.Oid;
            itemExec.Quantity = 1;
            itemExec.DiscountType = CommonFixture.LineDiscountPercentage.Oid;
            itemExec.Percentage = 1; //100% discount

            uow.CommitChanges();

            //Act
            promotionService.ExecutePromotions(documentHeader,
                                              priceCatalogPolicy,
                                               CommonFixture.AppSettings,
                                               activeAtDate,
                                               false);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
        }


        [TestCase(0.05)]
        [TestCase(0.10)]
        [TestCase(0.20)]
        [TestCase(0.22)]
        [TestCase(0.34)]
        [TestCase(0.50)]
        [TestCase(0.53)]
        [TestCase(0.80)]
        public void ExecutePromotions_TwoItemsAOrTwoItemsBGiveDocumentDiscountPercentage_CalculatedCorrectly(decimal documentDiscountPercentage)
        {
            //Arrange
            PromotionService promotionService = CreateDefaultPromotionService();
            DateTime activeAtDate = DateTime.Now;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithDifferentItemsPOS1;
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(documentHeader.GrossTotal * (1 - documentDiscountPercentage));
            UnitOfWork uow = CommonFixture.MemorySessionManager.GetSession<Promotion>();
            Item itemA = CommonFixture.SampleItemVat23;
            Item itemB = CommonFixture.SampleItem2Vat23;
            PriceCatalogPolicy priceCatalogPolicy = CommonFixture.PriceCatalogPolicy;
            //PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;

            //-------------------
            //Create Promotion  ItemA(0028563) x 2  or ItemB(0003517) x 2 , you get Document Discount
            //-------------------
            Promotion promotion = new Promotion(uow);
            promotion.StartDate = DateTime.MinValue.AddYears(1950);
            promotion.EndDate = DateTime.MaxValue;
            promotion.StartTime = DateTime.Now.Date;
            promotion.EndTime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            promotion.ActiveDaysOfWeek = DaysOfWeek.Friday | DaysOfWeek.Monday | DaysOfWeek.Saturday | DaysOfWeek.Sunday | DaysOfWeek.Thursday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday;
            promotion.IsActive = true;
            promotion.MaxExecutionsPerReceipt = 1;
            promotion.Description = "Test Promotion";
            promotion.Code = "001";

            //PriceCatalogPromotion priceCatalogPromotion = new PriceCatalogPromotion(uow);
            //priceCatalogPromotion.PriceCatalog = priceCatalog.Oid;
            //priceCatalogPromotion.Promotion = promotion.Oid;
            PriceCatalogPolicyPromotion priceCatalogPolicyPromotion = new PriceCatalogPolicyPromotion(uow);
            priceCatalogPolicyPromotion.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            priceCatalogPolicyPromotion.Promotion = promotion.Oid;

            PromotionApplicationRuleGroup rootRuleGroup = new PromotionApplicationRuleGroup(uow);
            rootRuleGroup.Operator = eGroupOperatorType.Or;
            rootRuleGroup.Promotion = promotion.Oid;
            promotion.PromotionApplicationRuleGroupOid = rootRuleGroup.Oid;

            PromotionItemApplicationRule itemRule = new PromotionItemApplicationRule(uow);
            itemRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule.Item = itemA.Oid;
            itemRule.Quantity = 2;

            PromotionItemApplicationRule itemRule2 = new PromotionItemApplicationRule(uow);
            itemRule2.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule2.Item = itemB.Oid;
            itemRule2.Quantity = 2;

            PromotionDocumentExecution documentExec = new PromotionDocumentExecution(uow);
            documentExec.Promotion = promotion.Oid;
            documentExec.DiscountType = CommonFixture.HeaderDiscountPercentage.Oid;
            documentExec.Percentage = documentDiscountPercentage;

            uow.CommitChanges();

            //Act
            promotionService.ExecutePromotions(documentHeader,
                                               priceCatalogPolicy,
                                               CommonFixture.AppSettings,
                                               activeAtDate,
                                               false);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
        }

        [Test]
        public void ExecutePromotions_ThreeItemsOfCategoryAPlusOneItemBGivesItemBAsGift_CalculatedCorrectly()
        {
            //Arrange
            PromotionService promotionService = CreateDefaultPromotionService();
            DateTime activeAtDate = DateTime.Now;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithDifferentItemsPOS1;
            decimal expectedGrossTotal = documentHeader.GrossTotal - PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT;
            UnitOfWork uow = CommonFixture.MemorySessionManager.GetSession<Promotion>();
            ItemCategory rootCategoryOfItemA = CommonFixture.RootItemCategory;
            ItemCategory categoryOfItemA = CommonFixture.SubSubItemCategory1_1;
            Item itemA = CommonFixture.SampleItemVat23;
            Item itemB = CommonFixture.SampleItem2Vat23;
            PriceCatalogPolicy priceCatalogPolicy = CommonFixture.PriceCatalogPolicy;
            //PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;


            ItemAnalyticTree sampleItemAnalyticTree = new ItemAnalyticTree(uow);
            sampleItemAnalyticTree.Object = itemA.Oid;
            sampleItemAnalyticTree.Node = categoryOfItemA.Oid;
            sampleItemAnalyticTree.Root = rootCategoryOfItemA.Oid;

            //-------------------
            //Create Promotion  ItemOfCategoryA(18) x 3 + ItemB(67) x 1, you get ItemB(67) free
            //-------------------
            Promotion promotion = new Promotion(uow);
            promotion.StartDate = DateTime.MinValue.AddYears(1950);
            promotion.EndDate = DateTime.MaxValue;
            promotion.StartTime = DateTime.Now.Date;
            promotion.EndTime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            promotion.ActiveDaysOfWeek = DaysOfWeek.Friday | DaysOfWeek.Monday | DaysOfWeek.Saturday | DaysOfWeek.Sunday | DaysOfWeek.Thursday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday;
            promotion.IsActive = true;
            promotion.MaxExecutionsPerReceipt = 1;
            promotion.Description = "Test Promotion";
            promotion.Code = "001";

            //PriceCatalogPromotion priceCatalogPromotion = new PriceCatalogPromotion(uow);
            //priceCatalogPromotion.PriceCatalog = priceCatalog.Oid;
            //priceCatalogPromotion.Promotion = promotion.Oid;
            PriceCatalogPolicyPromotion priceCatalogPolicyPromotion = new PriceCatalogPolicyPromotion(uow);
            priceCatalogPolicyPromotion.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            priceCatalogPolicyPromotion.Promotion = promotion.Oid;

            PromotionApplicationRuleGroup rootRuleGroup = new PromotionApplicationRuleGroup(uow);
            rootRuleGroup.Operator = eGroupOperatorType.And;
            rootRuleGroup.Promotion = promotion.Oid;
            promotion.PromotionApplicationRuleGroupOid = rootRuleGroup.Oid;

            PromotionItemCategoryApplicationRule itemCategoryRule = new PromotionItemCategoryApplicationRule(uow);
            itemCategoryRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemCategoryRule.ItemCategory = rootCategoryOfItemA.Oid;
            itemCategoryRule.Quantity = 3;

            PromotionItemApplicationRule itemRule = new PromotionItemApplicationRule(uow);
            itemRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule.Item = itemB.Oid;
            itemRule.Quantity = 1;

            PromotionItemExecution itemExec = new PromotionItemExecution(uow);
            itemExec.Promotion = promotion.Oid;
            itemExec.Item = itemB.Oid;
            itemExec.Quantity = 1;
            itemExec.DiscountType = CommonFixture.LineDiscountPercentage.Oid;
            itemExec.Percentage = 1; //100% discount

            uow.CommitChanges();

            //Act
            promotionService.ExecutePromotions(documentHeader,
                                               priceCatalogPolicy,
                                               CommonFixture.AppSettings,
                                               activeAtDate,
                                               false);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
        }

        [TestCase(0.05, 0.80)]
        [TestCase(0.10, 0.53)]
        [TestCase(0.20, 0.50)]
        [TestCase(0.22, 0.34)]
        [TestCase(0.34, 0.22)]
        [TestCase(0.50, 0.20)]
        [TestCase(0.53, 0.10)]
        [TestCase(0.80, 0.05)]
        public void ExecutePromotions_TwoItemAPlusCustomerCategoryAGetDocumentDiscountPercentageAndItemDiscountPercentage_CalculatedCorrently(decimal documentDiscountPercentage, decimal itemDiscountPercentage)
        {
            //Arrange
            PromotionService promotionService = CreateDefaultPromotionService();
            DateTime activeAtDate = DateTime.Now;
            Customer customer = CommonFixture.LoyaltyCustomer;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithDifferentItemsPOS1;
            documentHeader.Customer = customer.Oid;
            documentHeader.CustomerCode = customer.Code;
            UnitOfWork uow = CommonFixture.MemorySessionManager.GetSession<Promotion>();
            CustomerCategory rootCategoryOfCustomer = CommonFixture.RootCustomerCategory;
            CustomerCategory categoryOfCustomer = CommonFixture.SubSubCustomerCategory2_1;
            Item itemA = CommonFixture.SampleItemVat23;

            PriceCatalogPolicy priceCatalogPolicy = CommonFixture.PriceCatalogPolicy;
            //PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;
            CustomerAnalyticTree sampleCustomerAnalyticTree = new CustomerAnalyticTree(uow);
            sampleCustomerAnalyticTree.Object = customer.Oid;
            sampleCustomerAnalyticTree.Node = categoryOfCustomer.Oid;
            sampleCustomerAnalyticTree.Root = rootCategoryOfCustomer.Oid;

            decimal itemDiscount = CommonFixture.PlatformRoundingHandler.RoundDisplayValue(PosCommonFixture.SAMPLE_ITEM_VAT23_PRICE_WITH_VAT * itemDiscountPercentage);
            decimal expectedGrossTotal = CommonFixture.PlatformRoundingHandler.RoundDisplayValue((documentHeader.GrossTotal - itemDiscount) * (1 - documentDiscountPercentage));

            //-------------------
            //Create Promotion  ItemA(18) x 2 + CustomerCategory 2-1, you get ItemA discount and document discount
            //-------------------
            Promotion promotion = new Promotion(uow);
            promotion.StartDate = DateTime.MinValue.AddYears(1950);
            promotion.EndDate = DateTime.MaxValue;
            promotion.StartTime = DateTime.Now.Date;
            promotion.EndTime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            promotion.ActiveDaysOfWeek = DaysOfWeek.Friday | DaysOfWeek.Monday | DaysOfWeek.Saturday | DaysOfWeek.Sunday | DaysOfWeek.Thursday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday;
            promotion.IsActive = true;
            promotion.MaxExecutionsPerReceipt = 1;
            promotion.Description = "Test Promotion";
            promotion.Code = "001";

            //PriceCatalogPromotion priceCatalogPromotion = new PriceCatalogPromotion(uow);
            //priceCatalogPromotion.PriceCatalog = priceCatalog.Oid;
            //priceCatalogPromotion.Promotion = promotion.Oid;
            PriceCatalogPolicyPromotion priceCatalogPolicyPromotion = new PriceCatalogPolicyPromotion(uow);
            priceCatalogPolicyPromotion.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            priceCatalogPolicyPromotion.Promotion = promotion.Oid;

            PromotionApplicationRuleGroup rootRuleGroup = new PromotionApplicationRuleGroup(uow);
            rootRuleGroup.Operator = eGroupOperatorType.And;
            rootRuleGroup.Promotion = promotion.Oid;
            promotion.PromotionApplicationRuleGroupOid = rootRuleGroup.Oid;

            PromotionCustomerCategoryApplicationRule itemCategoryRule = new PromotionCustomerCategoryApplicationRule(uow);
            itemCategoryRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemCategoryRule.CustomerCategory = rootCategoryOfCustomer.Oid;

            PromotionItemApplicationRule itemRule = new PromotionItemApplicationRule(uow);
            itemRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule.Item = itemA.Oid;
            itemRule.Quantity = 2;

            PromotionItemExecution itemExec = new PromotionItemExecution(uow);
            itemExec.Promotion = promotion.Oid;
            itemExec.Item = itemA.Oid;
            itemExec.Quantity = 1;
            itemExec.DiscountType = CommonFixture.LineDiscountPercentage.Oid;
            itemExec.Percentage = itemDiscountPercentage;

            PromotionDocumentExecution documentExec = new PromotionDocumentExecution(uow);
            documentExec.Promotion = promotion.Oid;
            documentExec.DiscountType = CommonFixture.HeaderDiscountPercentage.Oid;
            documentExec.Percentage = documentDiscountPercentage;

            uow.CommitChanges();

            //Act
            promotionService.ExecutePromotions(documentHeader,
                                               priceCatalogPolicy,
                                               CommonFixture.AppSettings,
                                               activeAtDate,
                                               false);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
        }

        [Test]
        public void ExecutePromotions_CheckTestModePromotionHavingDemoPOS_PromotionExecutedCorrectly()
        {
            //Arrange
            PromotionService promotionService = CreateDefaultPromotionService();
            DateTime activeAtDate = DateTime.Now;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithDifferentItemsPOS1;
            decimal expectedGrossTotal = documentHeader.GrossTotal - PosCommonFixture.SAMPLE_ITEM_2_VAT23_PRICE_WITH_VAT;
            UnitOfWork uow = CommonFixture.MemorySessionManager.GetSession<Promotion>();
            Item itemA = CommonFixture.SampleItemVat23;
            Item itemB = CommonFixture.SampleItem2Vat23;
            PriceCatalogPolicy priceCatalogPolicy = CommonFixture.PriceCatalogPolicy;
            //PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;

            //-------------------
            //Create Promotion  ItemA(18) x 3 + ItemB(67) x 1, you get ItemB(67) free
            //-------------------
            Promotion promotion = new Promotion(uow);
            promotion.StartDate = DateTime.MinValue.AddYears(1950);
            promotion.EndDate = DateTime.MaxValue;
            promotion.StartTime = DateTime.Now.Date;
            promotion.EndTime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            promotion.ActiveDaysOfWeek = DaysOfWeek.Friday | DaysOfWeek.Monday | DaysOfWeek.Saturday | DaysOfWeek.Sunday | DaysOfWeek.Thursday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday;
            promotion.IsActive = true;
            promotion.MaxExecutionsPerReceipt = 1;
            promotion.Description = "Test Promotion";
            promotion.Code = "001";
            promotion.TestMode = true;

            //PriceCatalogPromotion priceCatalogPromotion = new PriceCatalogPromotion(uow);
            //priceCatalogPromotion.PriceCatalog = priceCatalog.Oid;
            //priceCatalogPromotion.Promotion = promotion.Oid;
            PriceCatalogPolicyPromotion priceCatalogPolicyPromotion = new PriceCatalogPolicyPromotion(uow);
            priceCatalogPolicyPromotion.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            priceCatalogPolicyPromotion.Promotion = promotion.Oid;

            PromotionApplicationRuleGroup rootRuleGroup = new PromotionApplicationRuleGroup(uow);
            rootRuleGroup.Operator = eGroupOperatorType.And;
            rootRuleGroup.Promotion = promotion.Oid;
            promotion.PromotionApplicationRuleGroupOid = rootRuleGroup.Oid;

            PromotionItemApplicationRule itemRule = new PromotionItemApplicationRule(uow);
            itemRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule.Item = itemA.Oid;
            itemRule.Quantity = 3;

            PromotionItemApplicationRule itemRule2 = new PromotionItemApplicationRule(uow);
            itemRule2.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule2.Item = itemB.Oid;
            itemRule2.Quantity = 1;

            PromotionItemExecution itemExec = new PromotionItemExecution(uow);
            itemExec.Promotion = promotion.Oid;
            itemExec.Item = itemB.Oid;
            itemExec.Quantity = 1;
            itemExec.DiscountType = CommonFixture.LineDiscountPercentage.Oid;
            itemExec.Percentage = 1; //100% discount

            uow.CommitChanges();

            //Act
            promotionService.ExecutePromotions(documentHeader,
                                              priceCatalogPolicy,
                                               CommonFixture.AppSettings,
                                               activeAtDate,
                                               true);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
        }

        [Test]
        public void ExecutePromotions_CheckTestModePromotionHavingNonDemoPOS_PromotionNotExecuted()
        {
            //Arrange
            PromotionService promotionService = CreateDefaultPromotionService();
            DateTime activeAtDate = DateTime.Now;
            DocumentHeader documentHeader = CommonFixture.SampleReceiptOpenWithDifferentItemsPOS1;
            decimal expectedGrossTotal = documentHeader.GrossTotal;
            UnitOfWork uow = CommonFixture.MemorySessionManager.GetSession<Promotion>();
            Item itemA = CommonFixture.SampleItemVat23;
            Item itemB = CommonFixture.SampleItem2Vat23;
            PriceCatalogPolicy priceCatalogPolicy = CommonFixture.PriceCatalogPolicy;
            //PriceCatalog priceCatalog = CommonFixture.RootPriceCatalog;

            //-------------------
            //Create Promotion  ItemA(18) x 3 + ItemB(67) x 1, you get ItemB(67) free
            //-------------------
            Promotion promotion = new Promotion(uow);
            promotion.StartDate = DateTime.MinValue.AddYears(1950);
            promotion.EndDate = DateTime.MaxValue;
            promotion.StartTime = DateTime.Now.Date;
            promotion.EndTime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            promotion.ActiveDaysOfWeek = DaysOfWeek.Friday | DaysOfWeek.Monday | DaysOfWeek.Saturday | DaysOfWeek.Sunday | DaysOfWeek.Thursday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday;
            promotion.IsActive = true;
            promotion.MaxExecutionsPerReceipt = 1;
            promotion.Description = "Test Promotion";
            promotion.Code = "001";
            promotion.TestMode = true;

            //PriceCatalogPromotion priceCatalogPromotion = new PriceCatalogPromotion(uow);
            //priceCatalogPromotion.PriceCatalog = priceCatalog.Oid;
            //priceCatalogPromotion.Promotion = promotion.Oid;
            PriceCatalogPolicyPromotion priceCatalogPolicyPromotion = new PriceCatalogPolicyPromotion(uow);
            priceCatalogPolicyPromotion.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            priceCatalogPolicyPromotion.Promotion = promotion.Oid;

            PromotionApplicationRuleGroup rootRuleGroup = new PromotionApplicationRuleGroup(uow);
            rootRuleGroup.Operator = eGroupOperatorType.And;
            rootRuleGroup.Promotion = promotion.Oid;
            promotion.PromotionApplicationRuleGroupOid = rootRuleGroup.Oid;

            PromotionItemApplicationRule itemRule = new PromotionItemApplicationRule(uow);
            itemRule.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule.Item = itemA.Oid;
            itemRule.Quantity = 3;

            PromotionItemApplicationRule itemRule2 = new PromotionItemApplicationRule(uow);
            itemRule2.PromotionApplicationRuleGroup = rootRuleGroup.Oid;
            itemRule2.Item = itemB.Oid;
            itemRule2.Quantity = 1;

            PromotionItemExecution itemExec = new PromotionItemExecution(uow);
            itemExec.Promotion = promotion.Oid;
            itemExec.Item = itemB.Oid;
            itemExec.Quantity = 1;
            itemExec.DiscountType = CommonFixture.LineDiscountPercentage.Oid;
            itemExec.Percentage = 1; //100% discount

            uow.CommitChanges();

            //Act
            promotionService.ExecutePromotions(documentHeader,
                                              priceCatalogPolicy,
                                               CommonFixture.AppSettings,
                                               activeAtDate,
                                               false);

            //Assert
            Assert.AreEqual(expectedGrossTotal, documentHeader.GrossTotal);
            Assert.AreEqual(0, documentHeader.DocumentPromotions.Count);
        }

    }
}
