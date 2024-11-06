using DevExpress.Xpo;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ITS.Retail.Platform.Enumerations;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 90, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class VatFactor : LookUp2Fields, IVatFactor
    {
        public VatFactor()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VatFactor(Session session)
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
        public decimal Factor { get; set; }
        public Guid VatCategory { get; set; }
        
        public Guid VatLevel { get; set; }

    }
}