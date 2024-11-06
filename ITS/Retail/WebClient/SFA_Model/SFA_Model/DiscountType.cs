using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

using ITS.WRM.Model.Interface;

using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 195, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DiscountType : LookUp2Fields, IDiscountType
    {
        public DiscountType()
        {

        }
        public DiscountType(Session session)
            : base(session)
        {

        }
        
        public DiscountType(Session session, string code, string description)
            : base(session, code, description)
        {

        }

        public bool DiscardsOtherDiscounts { get; set; }
        
        public eDiscountType EDiscountType { get; set; }

        public bool IsHeaderDiscount { get; set; }

        public bool IsUnique { get; set; }

        public int Priority { get; set; }

    }
}