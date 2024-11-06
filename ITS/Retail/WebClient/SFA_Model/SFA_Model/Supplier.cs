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
    [CreateOrUpdaterOrder(Order = 410, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Supplier: BaseObj
    {
        public Supplier()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Supplier(Session session)
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
        public string CompanyName { get; set; }
        
        public Guid DefaultAddress { get; set; }
        
        public Guid Items { get; set; }
        
        public Guid Owner { get; set; }
        
        public string Profession { get; set; }
        
        public Guid Trader { get; set; }
        
        public Guid VatLevel { get; set; }
        
    }
}