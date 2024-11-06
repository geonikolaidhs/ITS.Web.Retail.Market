using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 21, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class TaxOffice : LookUp2Fields, ITaxOffice
    {
        public TaxOffice()
               : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TaxOffice(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public string Municipality { get; set; }
        
        public string PostCode { get; set; }

        public string Street { get; set; }

    }
}