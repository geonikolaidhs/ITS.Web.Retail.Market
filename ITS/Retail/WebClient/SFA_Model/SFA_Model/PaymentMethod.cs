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
    [CreateOrUpdaterOrder(Order = 185, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PaymentMethod : LookUp2Fields, IPaymentMethod
    {
        public PaymentMethod()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PaymentMethod(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public PaymentMethod(string code, string description)
            : base()
        {

        }
        public PaymentMethod(Session session, string code, string description)
            : base(session, code, description)
        {

        }
        public bool CanExceedTotal { get; set; }
        
        public bool ForceEdpsOffline { get; set; }

        public bool GiveChange { get; set; }

        public bool IncreasesDrawerAmount { get; set; }

        public bool IsNegative { get; set; }

        public bool NeedsRatification { get; set; }

        public bool NeedsValidation { get; set; }

        public bool OpensDrawer { get; set; }
        
        public ePaymentMethodType PaymentMethodType { get; set; }
        
        public bool UseEDPS { get; set; }

        public bool UsesInstallments { get; set; }

    }
}