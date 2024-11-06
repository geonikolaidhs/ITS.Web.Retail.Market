using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public class DiscountHelper
    {
        public static List<PropertyInfo> OptionalDiscountFields()
        {
            List<string> excludedProperties = new List<string>() { "Oid", "CreatedOn", "CreatedOnTicks", "UpdatedOn", "UpdatedOnTicks",
                                                                   "CreatedBy","UpdatedBy","CreatedByDevice","UpdateByDevice",
                                                                   "RowDeleted","IsActive","IsSynchronized","MLValues",
                                                                   "MasterObjOid","ChangedMembers","This","Loading","ClassInfo",
                                                                    "Session","IsLoading","IsDeleted"
            };

            return typeof(CustomFieldStorage).GetProperties()
                .Where(property => property.GetCustomAttributes(typeof(ITS.Retail.Model.RequiredFieldAttribute), false).Count() <= 0)
                .Where(property => excludedProperties.Contains(property.Name) == false)
                .ToList();
        }


        public static DocumentDetailDiscount CreatePriceCatalogDetailDiscount(UnitOfWork uow, decimal priceCatalogDiscount)
        {
            DocumentDetailDiscount pcDiscount = new DocumentDetailDiscount(uow)
            {
                Percentage = priceCatalogDiscount,
                Priority = -1,
                DiscountSource = eDiscountSource.PRICE_CATALOG,
                DiscountType = eDiscountType.PERCENTAGE
            };

            return pcDiscount;
        }
    }
}
