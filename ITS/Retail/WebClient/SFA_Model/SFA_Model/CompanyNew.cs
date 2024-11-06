using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA_Model.NonPersistant;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 20, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class CompanyNew : BasicObj, ICompanyNew
    {
        public CompanyNew()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CompanyNew(Session session)
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
        public string B2CURL { get; set; }

        public double Balance { get; set; }

        public string Code { get; set; }

        public string CompanyName { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? DefaultAddressOid { get; set; }

        public Guid OwnerApplicationSettings { get; set; }

        public string Profession { get; set; }

        //public ITrader Trader { get; set; }
        public Guid Trader { get; set; }

        public DateTime UpdatedOn { get; set; }

    }
}
