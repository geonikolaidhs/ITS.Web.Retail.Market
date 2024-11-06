using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common
{
    public class SingleObjectLinqQuery : AbstractLinqQuery
    {
        Type type;
        public SingleObjectLinqQuery(Type type, DataBaseConnectionType DataBaseType) : base(XpoHelper.GetNewUnitOfWork(), DataBaseType)
        {
            this.type = type;
        }
        public override IQueryable MainQuerySet()
        {
            Type xpQuery = typeof(XPQuery<>);
            Type genericType = xpQuery.MakeGenericType(type);
            return Activator.CreateInstance(genericType, new object[] { this.Session }) as IQueryable;
         
        }
        public override FiveIQueryables SupportingQuerySets()
        {
            return new FiveIQueryables();
        }

        public override string KeyExpression
        {
            get { return "Oid"; }
        }
    }
}
