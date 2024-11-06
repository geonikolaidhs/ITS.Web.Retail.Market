using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
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
    public class CustomerServiceTests
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

        CustomerService CreateDefaultTestCustomerService()
        {
            ISessionManager memorySessionManager = CommonFixture.MemorySessionManager;
            return new CustomerService(memorySessionManager);
        }

        [Test]
        public void GetPriceCatalog_FindPriceCatalogOfCustomerThatHasNoPriceCatalogForCurrentStore_ReturnsStoreDefaultCatalog()
        {
            //Arrange
            CustomerService customerService = CreateDefaultTestCustomerService();
            Customer customer = CommonFixture.DefaultCustomer;
            Store store = CommonFixture.DefaultStore;
            Guid expectedPriceCatalogOid = CommonFixture.DefaultStore.DefaultPriceCatalog;

            //Act
            PriceCatalog foundPriceCatalog = customerService.GetPriceCatalog(customer.Oid, store.Oid);

            //Assert
            Assert.AreEqual(expectedPriceCatalogOid, foundPriceCatalog.Oid);
        }

        [Test]
        public void GetPriceCatalog_FindPriceCatalogOfCustomerThatHasPriceCatalogForCurrentStore_ReturnsCustomerCatalogForCurrentStore()
        {
            //Arrange
            CustomerService customerService = CreateDefaultTestCustomerService();
            Customer customer = CommonFixture.DefaultCustomer;
            Store store = CommonFixture.DefaultStore;
            CustomerStorePriceList cspl = new CustomerStorePriceList(CommonFixture.MemorySessionManager.GetSession<CustomerStorePriceList>());
            cspl.Customer = customer.Oid;
            cspl.StorePriceList = CommonFixture.StorePriceListDefaultStoreSubCatalog.Oid;
            cspl.Save();
            cspl.Session.CommitTransaction();
            Guid expectedPriceCatalogOid = CommonFixture.SubCatalog.Oid;

            //Act
            PriceCatalog foundPriceCatalog = customerService.GetPriceCatalog(customer.Oid, store.Oid);
            
            //Assert
            Assert.AreEqual(expectedPriceCatalogOid, foundPriceCatalog.Oid);
        }

        [Test]
        public void SearchCustomer_SearchCustomerByCardID_CustomerFound()
        {
            //Arrange
            CustomerService customerService = CreateDefaultTestCustomerService();
            string lookupString = CommonFixture.LoyaltyCustomer.CardID;
            Guid expectedCustomer = CommonFixture.LoyaltyCustomer.Oid;

            //Act
            Customer foundCustomer = customerService.SearchCustomer<Customer>(lookupString);

            //Assert
            Assert.AreEqual(expectedCustomer, foundCustomer.Oid);
        }

        [Test]
        public void SearchCustomer_SearchCustomerByTaxCode_CustomerFound()
        {
            //Arrange
            CustomerService customerService = CreateDefaultTestCustomerService();
            string lookupString = PosCommonFixture.LOYALTY_CUSTOMER_TAXCODE;
            Guid expectedCustomer = CommonFixture.LoyaltyCustomer.Oid;

            //Act
            Customer foundCustomer = customerService.SearchCustomer<Customer>(lookupString);

            //Assert
            Assert.AreEqual(expectedCustomer, foundCustomer.Oid);
        }

        [Test]
        public void SearchCustomer_SearchCustomerByCode_CustomerFound()
        {
            //Arrange
            CustomerService customerService = CreateDefaultTestCustomerService();
            string lookupString = CommonFixture.LoyaltyCustomer.Code;
            Guid expectedCustomer = CommonFixture.LoyaltyCustomer.Oid;

            //Act
            Customer foundCustomer = customerService.SearchCustomer<Customer>(lookupString);

            //Assert
            Assert.AreEqual(expectedCustomer, foundCustomer.Oid);
        }

    }
}
