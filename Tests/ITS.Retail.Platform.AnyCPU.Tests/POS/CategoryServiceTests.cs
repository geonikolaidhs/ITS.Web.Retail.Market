using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
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
    public class CategoryServiceTests
    {
        PosCommonFixture CommonFixture { get; set; }

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

        CategoryService CreateDefaultTestCategoryService()
        {
            return new CategoryService();
        }


        [Test]
        public void GetAllChildCategories_ItemRootCategoryChildCategoriesRequested_AllChildCategoriesReturned()
        {
            //Arrange
            CategoryService categoryService = CreateDefaultTestCategoryService();
            ItemCategory parentCategory = CommonFixture.RootItemCategory;
            List<Guid> expectedCategories = new List<Guid>();
            expectedCategories.AddRange(new List<Guid>() 
            { 
                CommonFixture.SubItemCategory1.Oid,
                CommonFixture.SubItemCategory2.Oid,
                CommonFixture.SubSubItemCategory1_1.Oid,
                CommonFixture.SubSubItemCategory1_2.Oid,
                CommonFixture.SubSubItemCategory2_1.Oid,
                CommonFixture.SubSubItemCategory2_2.Oid
            });

            //Act
            List<Guid> foundChildCategories = categoryService.GetAllChildCategories<ItemCategory>(parentCategory.Oid, CommonFixture.MemorySessionManager.GetSession<ItemCategory>());

            //Assert
            CollectionAssert.AreEquivalent(expectedCategories,foundChildCategories);
        }

        [Test]
        public void GetAllChildCategories_ItemSubCategoryChildCategoriesRequested_AllChildCategoriesReturned()
        {
            //Arrange
            CategoryService categoryService = CreateDefaultTestCategoryService();
            ItemCategory parentCategory = CommonFixture.SubItemCategory1;
            List<Guid> expectedCategories = new List<Guid>();
            expectedCategories.AddRange(new List<Guid>() 
            { 
                CommonFixture.SubSubItemCategory1_1.Oid,
                CommonFixture.SubSubItemCategory1_2.Oid
            });

            //Act
            List<Guid> foundChildCategories = categoryService.GetAllChildCategories<ItemCategory>(parentCategory.Oid, CommonFixture.MemorySessionManager.GetSession<ItemCategory>());

            //Assert
            CollectionAssert.AreEquivalent(expectedCategories, foundChildCategories);
        }

        [Test]
        public void GetAllChildCategories_CustomerRootCategoryChildCategoriesRequested_AllChildCategoriesReturned()
        {
            //Arrange
            CategoryService categoryService = CreateDefaultTestCategoryService();
            CustomerCategory parentCategory = CommonFixture.RootCustomerCategory;
            List<Guid> expectedCategories = new List<Guid>();
            expectedCategories.AddRange(new List<Guid>() 
            { 
                CommonFixture.SubCustomerCategory1.Oid,
                CommonFixture.SubCustomerCategory2.Oid,
                CommonFixture.SubSubCustomerCategory1_1.Oid,
                CommonFixture.SubSubCustomerCategory1_2.Oid,
                CommonFixture.SubSubCustomerCategory2_1.Oid,
                CommonFixture.SubSubCustomerCategory2_2.Oid
            });

            //Act
            List<Guid> foundChildCategories = categoryService.GetAllChildCategories<CustomerCategory>(parentCategory.Oid, CommonFixture.MemorySessionManager.GetSession<CustomerCategory>());

            //Assert
            CollectionAssert.AreEquivalent(expectedCategories, foundChildCategories);
        }

        [Test]
        public void GetAllChildCategories_CustomerSubCategoryChildCategoriesRequested_AllChildCategoriesReturned()
        {
            //Arrange
            CategoryService categoryService = CreateDefaultTestCategoryService();
            CustomerCategory parentCategory = CommonFixture.SubCustomerCategory2;
            List<Guid> expectedCategories = new List<Guid>();
            expectedCategories.AddRange(new List<Guid>() 
            { 
                CommonFixture.SubSubCustomerCategory2_1.Oid,
                CommonFixture.SubSubCustomerCategory2_2.Oid
            });

            //Act
            List<Guid> foundChildCategories = categoryService.GetAllChildCategories<CustomerCategory>(parentCategory.Oid, CommonFixture.MemorySessionManager.GetSession<CustomerCategory>());

            //Assert
            CollectionAssert.AreEquivalent(expectedCategories, foundChildCategories);
        }
    }
}
