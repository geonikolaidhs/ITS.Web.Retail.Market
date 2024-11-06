using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class PaymentFieldHelper
    {
        public static List<PropertyInfo> OptionalDocumentPaymentFields()
        {
            List<string> excludedProperties = new List<string>() { "Oid", "CreatedOn", "CreatedOnTicks", "UpdatedOn", "UpdatedOnTicks",
                                                                   "CreatedBy","UpdatedBy","CreatedByDevice","UpdateByDevice",
                                                                   "RowDeleted","IsActive","IsSynchronized","MLValues",
                                                                   "MasterObjOid","ChangedMembers","This","Loading","ClassInfo",
                                                                    "Session","IsLoading","IsDeleted","PaymentMethod","PaymentMethodCode","DocumentHeader"
            };

            return typeof(DocumentPayment).GetProperties()
                .Where(property => property.GetCustomAttributes(typeof(ITS.Retail.Model.RequiredFieldAttribute), false).Count() <= 0)
                .Where(property => excludedProperties.Contains(property.Name) == false)
                .ToList();
        }

        public static List<PropertyInfo> RequiredDocumentPaymentFields()
        {
            return typeof(DocumentPayment).GetProperties()
                .Where(property => property.GetCustomAttributes(typeof(ITS.Retail.Model.RequiredFieldAttribute), false).Count() > 0 )
                .ToList();
        }
    }
}
