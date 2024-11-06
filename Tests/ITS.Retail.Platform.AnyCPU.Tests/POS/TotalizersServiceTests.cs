using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Tests.Fixtures;
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
    public class TotalizersServiceTests
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

        private TotalizersService CreateDefaultTestTotalizersService()
        {
            ISessionManager sessionManager = CommonFixture.MemorySessionManager;
            IPlatformRoundingHandler platformRoundingHandler = new PlatformRoundingHandler();
            TotalizersService totalizerService = new TotalizersService(sessionManager, platformRoundingHandler);
            
            return totalizerService;
        }


        [Test]
        public void CheckIfMustIssueZ_24HoursHavePassedSinceDayStarted_ReturnsTrue()
        {
            //Arrange
            TotalizersService totalizerService = CreateDefaultTestTotalizersService();
            ISessionManager sessionManager = CommonFixture.MemorySessionManager;
            DailyTotals currentDailyTotals = new DailyTotals(sessionManager.GetSession<DailyTotals>());
            currentDailyTotals.FiscalDateOpen = true;
            currentDailyTotals.FiscalDate = DateTime.Now.AddDays(-1).AddHours(-3);
            currentDailyTotals.Session.CommitTransaction();
            bool expectedResult = true;

            //Act
            bool result = totalizerService.CheckIfMustIssueZ(currentDailyTotals);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }


        [Test]
        public void CheckIfMustIssueZ_24HoursHaveNOTPassedSinceDayStarted_ReturnsFalse()
        {
            //Arrange
            TotalizersService totalizerService = CreateDefaultTestTotalizersService();
            ISessionManager sessionManager = CommonFixture.MemorySessionManager;
            DailyTotals currentDailyTotals = new DailyTotals(sessionManager.GetSession<DailyTotals>());
            currentDailyTotals.FiscalDateOpen = true;
            currentDailyTotals.FiscalDate = DateTime.Now.AddHours(-3);
            currentDailyTotals.Session.CommitTransaction();
            bool expectedResult = false;

            //Act
            bool result = totalizerService.CheckIfMustIssueZ(currentDailyTotals);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateDailyTotals_ThereIsNoOpenDay_DailyTotalCreatedCorrectly()
        {
            //Arrange
            TotalizersService totalizerService = CreateDefaultTestTotalizersService();
            ISessionManager sessionManager = CommonFixture.MemorySessionManager;
            Store store = CommonFixture.DefaultStore;
            ITS.POS.Model.Settings.POS pos = CommonFixture.POS1;

            //Act
            DateTime currentFiscalDate;
            DailyTotals result = totalizerService.CreateDailyTotals(pos.Oid, store.Oid,Guid.Empty, out currentFiscalDate);

            //Assert
            Assert.AreEqual(currentFiscalDate, result.FiscalDate);
            Assert.AreEqual(true, result.FiscalDateOpen);
            Assert.AreEqual(store.Oid, result.Store);
            Assert.AreEqual(pos.Oid, result.POS);
        }

        [Test]
        public void CreateDailyTotals_ThereIsAlreadyAnOpenDay_TheOpenDayIsFoundAndReturned()
        {
            //Arrange
            TotalizersService totalizerService = CreateDefaultTestTotalizersService();
            ISessionManager sessionManager = CommonFixture.MemorySessionManager;
            DailyTotals currentDailyTotals = new DailyTotals(sessionManager.GetSession<DailyTotals>());
            Store store = CommonFixture.DefaultStore;
            ITS.POS.Model.Settings.POS pos = CommonFixture.POS1;
            currentDailyTotals.FiscalDateOpen = true;
            currentDailyTotals.FiscalDate = DateTime.Now.AddHours(-3);
            currentDailyTotals.Session.CommitTransaction();

            //Act
            DateTime currentFiscalDate;
            DailyTotals result = totalizerService.CreateDailyTotals(pos.Oid, store.Oid,currentDailyTotals.Oid,out currentFiscalDate);

            //Assert
            Assert.AreEqual(currentDailyTotals.Oid,result.Oid);
        }
    }
}
