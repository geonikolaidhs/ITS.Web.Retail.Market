using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Common.ViewModel;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddPaymentParams : ActionParams
    {
        public decimal Amount { get; set;}
        public PaymentMethod PaymentMethod { get; set; }
        public CouponViewModel CouponViewModel { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.ADD_PAYMENT; }
        }

        public ActionAddPaymentParams(PaymentMethod paymentMethod,decimal amount,CouponViewModel couponViewModel = null)
        {
            Amount = amount;
            PaymentMethod = paymentMethod;
            CouponViewModel = couponViewModel;
        }
    }
}
