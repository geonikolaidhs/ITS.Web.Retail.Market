using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace ITS.Retail.Platform.Tests
{
    public class UpdaterTests
    {

        public static IEnumerable<object[]> CheckUpdaterInput
        {
            get
            {
                return new[]
                {
                    new object[]{eUpdateDirection.MASTER_TO_STORECONTROLLER},
                    new object[]{eUpdateDirection.POS_TO_STORECONTROLLER},
                    new object[]{eUpdateDirection.STORECONTROLLER_TO_MASTER},
                    //new object[]{eUpdateDirection.STORECONTROLLER_TO_POS} //no need to check, oids are used
                };
            }
        }


        //[Theory, PropertyData("CheckUpdaterInput")]
        //public static void CheckUpdaterReferencesForAttribute(eUpdateDirection direction)
        //{
        //    IEnumerable<Type> classesWithOrder = typeof(BaseObj).Assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(UpdaterAttribute), false).Count() > 0 &&
        //            (type.GetCustomAttributes(typeof(UpdaterAttribute), false).First() as UpdaterAttribute).Permissions.HasFlag(direction));

        //    ////Exceptions. Do not check these properties
        //    List<string> excludedProperties = new List<string>();
        //    excludedProperties.Add("CreatedBy");
        //    excludedProperties.Add("UpdatedBy");

        //    foreach (Type currentType in classesWithOrder)
        //    {
        //        foreach (PropertyInfo prop in currentType.GetProperties())
        //        {
        //            if (typeof(BaseObj).IsAssignableFrom(prop.PropertyType) &&
        //               prop.CanWrite &&
        //               prop.GetCustomAttributes(typeof(UpdaterIgnoreFieldAttribute), false).Count() == 0)
        //            {
        //                UpdaterAttribute updaterAttribute = prop.PropertyType.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() as UpdaterAttribute;
        //                Assert.True(updaterAttribute != null,
        //                   String.Format("Entity '{0}' has a referenced entity '{1}' that does not have the UpdaterAttribute",
        //                                   currentType.Name, prop.PropertyType.Name, direction));

        //            }
        //        }
        //    }
        //}

        //[Theory, PropertyData("CheckUpdaterInput")]
        //public static void CheckUpdaterReferencesForSameDirection(eUpdateDirection direction)
        //{
        //    IEnumerable<Type> classesWithOrder = typeof(BaseObj).Assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(UpdaterAttribute), false).Count() > 0 &&
        //            (type.GetCustomAttributes(typeof(UpdaterAttribute), false).First() as UpdaterAttribute).Permissions.HasFlag(direction));

        //    ////Exceptions. Do not check these properties
        //    List<string> excludedProperties = new List<string>();
        //    excludedProperties.Add("CreatedBy");
        //    excludedProperties.Add("UpdatedBy");

        //    foreach (Type currentType in classesWithOrder)
        //    {
        //        foreach (PropertyInfo prop in currentType.GetProperties())
        //        {
        //            if (typeof(BaseObj).IsAssignableFrom(prop.PropertyType) &&
        //               prop.CanWrite &&
        //               prop.GetCustomAttributes(typeof(UpdaterIgnoreFieldAttribute), false).Count() == 0 &&
        //                prop.PropertyType.GetCustomAttributes(typeof(UpdaterAttribute), false).Count() > 0)
        //            {
        //                UpdaterAttribute updaterAttribute = prop.PropertyType.GetCustomAttributes(typeof(UpdaterAttribute), false).First() as UpdaterAttribute;
        //                bool sameDirection = updaterAttribute.Permissions.HasFlag(direction);
        //                Assert.True(sameDirection,
        //                   String.Format("Entity '{0}' has a referenced entity '{1}' that does not have the same direction '{2}'",
        //                                   currentType.Name, prop.PropertyType.Name, direction));
        //            }
        //        }
        //    }
        //}


        [Theory, PropertyData("CheckUpdaterInput")]
        public static void CheckUpdaterOrder(eUpdateDirection direction)
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
                        prop.GetCustomAttributes(typeof(UpdaterIgnoreFieldAttribute), false).Count() == 0 &&
                        prop.PropertyType.GetCustomAttributes(typeof(UpdaterAttribute), false).Count() > 0)
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
