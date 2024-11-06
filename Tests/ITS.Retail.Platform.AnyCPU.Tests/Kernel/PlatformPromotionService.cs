using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.Platform.Tests.Fixtures;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Tests.Kernel
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
    public class PlatformPromotionServiceTests
    {
        protected KernelCommonFixture KernelCommonFixture { get; set; }

        /// <summary>
        /// Runs before every test
        /// </summary>
        [SetUp]
        public void SetFixture()
        {
            KernelCommonFixture = new KernelCommonFixture();

        }

        /// <summary>
        /// Runs after every test
        /// </summary>
        [TearDown]
        public void TearDownFixture()
        {
            KernelCommonFixture = null;
        }

        PlatformPromotionService CreateDefaultPlatformPromotionService()
        {
            Mock<IIntermediateModelManager> mockIntermediateModelManager = new Mock<IIntermediateModelManager>(MockBehavior.Strict);
            Mock<IIntermediateDocumentService> mockIntermediateDocumentService = new Mock<IIntermediateDocumentService>(MockBehavior.Strict);
            IPlatformRoundingHandler roundingHandler = new PlatformRoundingHandler();
            roundingHandler.SetOwnerApplicationSettings(KernelCommonFixture.OwnerApplicationSettings);
            Mock<IPlatformDocumentDiscountService> mockPlatformDocumentDiscountService = new Mock<IPlatformDocumentDiscountService>(MockBehavior.Strict);

            PlatformPromotionService platformPromotionService = new PlatformPromotionService(mockIntermediateDocumentService.Object,
                                                                                                mockIntermediateModelManager.Object,
                                                                                                roundingHandler,
                                                                                                mockPlatformDocumentDiscountService.Object,
                                                                                                KernelCommonFixture.DefaultCustomerOid);

            return platformPromotionService;
        }


        [Test]
        public void ExecutePromotions_SimpleOnePlusOneFreeScenario_CalculatedCorrectly()
        {
            //Arrange
            PlatformPromotionService platformPromotionService = CreateDefaultPlatformPromotionService();


            //Act

            //Assert
            Assert.Fail();
        }
    }
}
