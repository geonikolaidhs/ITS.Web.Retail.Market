using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.POS.Model.Master
{
    public class CustomerCategory : CategoryNode, ICustomerCategory
    {
        public CustomerCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomerCategory(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        [NonPersistent]
        public string FullCode
        {
            get
            {
                return GetFullCode();
            }
        }
                

        public string GetFullCode()
        {
            if (this.ParentOid != null)
            {
                return String.Format("{0}:{1}", Code, (this.Session.GetObjectByKey<CustomerCategory>(ParentOid)).GetFullCode());
            }
            return Code;
        }

        public string GetFullDescription()
        {
            if (this.ParentOid != null)
            {
                return String.Format("{0}>>{1}", Description, (this.Session.GetObjectByKey<CustomerCategory>(ParentOid)).GetFullDescription());
            }
            return Description;
        }

        public XPCollection<T> GetAllNodeTreeItems<T>(CriteriaOperator crop = null)
        {
            CustomerAnalyticTree iat = new CustomerAnalyticTree(this.Session);
            List<Guid> lstIDs = iat.GetNodeObjectIDs<T>(this as CategoryNode);
            XPCursor cursor = new XPCursor(this.Session, typeof(T), lstIDs);
            XPCollection<T> tcol = new XPCollection<T>(this.Session, false);
            tcol.AddRange(cursor.OfType<T>());
            return tcol;
        }

        public CriteriaOperator GetAllNodeTreeItemsFilter(string propertyName = "Oid")
        {
            ItemAnalyticTree iat = new ItemAnalyticTree(this.Session);
            List<Guid> lstItemIDs = iat.GetNodeObjectIDs<Item>(this as CategoryNode);
            return new InOperator(propertyName, lstItemIDs);
        }
    }
}
