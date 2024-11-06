using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Platform.Enumerations;

using DevExpress.Xpo;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 10, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Trader : BasicObj, ITrader
    {
        public Trader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Trader(Session session)
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

        public string Code { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string TaxCode { get; set; }

        public string TaxOffice { get; set; }

        public Guid? TaxOfficeLookUpOid { get; set; }

        public eTraderType TraderType { get; set; }

    }
}