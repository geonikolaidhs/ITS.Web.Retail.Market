using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddCouponParams : ActionParams 
    {
        public string CouponCode { get; set; }

        public override eActions ActionCode
        {
            get
            {
                return eActions.ADD_COUPON;
            }
        }

        public ActionAddCouponParams(string couponCode)
        {
            this.CouponCode = couponCode;
        }
    }
}
