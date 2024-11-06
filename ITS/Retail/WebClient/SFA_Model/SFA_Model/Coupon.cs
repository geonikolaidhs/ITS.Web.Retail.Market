using DevExpress.Xpo;
using ITS.WRM.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    public class Coupon : CouponBase
    {

        public decimal Amount { get; set; }
       
        public string Code { get; set; }
        
        public Guid CouponCategory { get; set; }
        
        public Guid CouponMask { get; set; }
        

        public int NumberOfTimesUsed { get; set; }

        public Guid Owner { get; set; }
        

    }
}