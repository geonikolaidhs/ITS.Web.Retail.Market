using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public class ActionRecalculateCriteria
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public CompanyNew Owner { get; set; }

        public List<Guid> ActionTypeOids { get; set; }

        public ActionRecalculateCriteria()
        {
            this.ActionTypeOids = new List<Guid>();
        }

        public XPCollection<ActionType> ActionTypes(UnitOfWork uow)
        {
            if( ActionTypeOids == null )
            {
                ActionTypeOids = new List<Guid>();
            }
            CriteriaOperator criteria = new InOperator("Oid", ActionTypeOids);
            return new XPCollection<ActionType>(uow, criteria);
        }

        public string ActionTypesString
        {
            get
            {
                return string.Join(",",this.ActionTypeOids.Select(oid => oid.ToString()).ToList());
            }
        }
    }
}
