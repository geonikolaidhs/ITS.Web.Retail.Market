using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;

namespace ITS.Retail.Common
{
    public static class XPCollectionExtender
    {
        public static XPCollection<T> Customize<T>(this XPCollection<T> xpCollection)
        {
            xpCollection.Criteria = CriteriaOperator.And(new BinaryOperator("RowDeleted", false), xpCollection.Criteria);
            return xpCollection;
        }
    }
}