using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;

using SFA_Model;
using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 269, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class CouponMask : CouponBase
    {
        public CouponMask()
           : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CouponMask(Session session)
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
        public CouponCategory CouponCategory { get; set; }
        //public ICouponCategory CouponCategory { get; set; }

        public string Mask { get; set; }

        public string Prefix { get; set; }

        public string PropertyName { get; set; }

        public Guid Owner { get; set; }
        public Guid PaymentMethod { get; set; }
        public Guid DiscountType { get; set; }

    }
}