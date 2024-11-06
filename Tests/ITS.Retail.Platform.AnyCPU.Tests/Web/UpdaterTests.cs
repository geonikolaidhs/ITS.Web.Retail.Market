using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace ITS.Retail.Platform.Tests.Web
{
    /// <summary>
    /// Test naming standards derived from http://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
    /// 
    /// [UnitOfWork_StateUnderTest_ExpectedBehavior]
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
    /// </summary>
    [TestFixture]
    public class UpdaterTests
    {

        [TestCase(eUpdateDirection.MASTER_TO_STORECONTROLLER)]
        [TestCase(eUpdateDirection.POS_TO_STORECONTROLLER)]
        [TestCase(eUpdateDirection.STORECONTROLLER_TO_MASTER)]
        public void Updater_CheckUpdaterOrder_ShouldBeInCorrectOrder(eUpdateDirection direction)
        {
            Dictionary<Type, int> classesWithOrder = typeof(BaseObj).Assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(UpdaterAttribute), false).Count() > 0 &&
                                (type.GetCustomAttributes(typeof(UpdaterAttribute), false).First() as UpdaterAttribute).Permissions.HasFlag(direction))
                .ToDictionary(type => type, type => (type.GetCustomAttributes(typeof(UpdaterAttribute), false).First() as UpdaterAttribute).Order);

            ////Exceptions. Do not check these properties
            List<string> excludedProperties = new List<string>();
            excludedProperties.Add("CreatedBy");
            excludedProperties.Add("UpdatedBy");


            foreach (KeyValuePair<Type, int> typeWithOrder in classesWithOrder.OrderBy(pair => pair.Value))
            {
                Type currentType = typeWithOrder.Key;
                int currentTypeOrder = typeWithOrder.Value;

                foreach (PropertyInfo prop in currentType.GetProperties())
                {
                    if (typeof(BaseObj).IsAssignableFrom(prop.PropertyType) &&
                        prop.CanWrite &&
                        (prop.GetCustomAttributes(typeof(UpdaterIgnoreFieldAttribute), false).Count() == 0 ||
                        (prop.GetCustomAttributes(typeof(UpdaterIgnoreFieldAttribute), false).FirstOrDefault() as UpdaterIgnoreFieldAttribute)
                           .IgnoreWhenDirection.HasFlag(direction) == false) &&
                        prop.PropertyType.GetCustomAttributes(typeof(UpdaterAttribute), false).Count() > 0 &&
                        (prop.PropertyType.GetCustomAttributes(typeof(UpdaterAttribute), false).First() as UpdaterAttribute).Permissions.HasFlag(direction))
                    {
                        if (excludedProperties.Contains(prop.Name))
                        {
                            continue;
                        }

                        int currentPropertyOrder = (prop.PropertyType.GetCustomAttributes(typeof(UpdaterAttribute), false).First() as UpdaterAttribute).Order;
                        Assert.True(currentTypeOrder > currentPropertyOrder,
                            String.Format("Entity '{0}'({1}) Updater Order is less or equal than the referenced entity's '{2}'({3}) Updater Order",
                                            currentType.Name, currentTypeOrder, prop.PropertyType.Name, currentPropertyOrder));
                    }
                }
            }
        }
    }
}
