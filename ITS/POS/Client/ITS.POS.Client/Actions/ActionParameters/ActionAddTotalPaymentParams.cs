using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddTotalPaymentParams : ActionParams
    {
        public PaymentMethod PaymentMethod { get; set; }
        public decimal? Amount { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.ADD_TOTAL_PAYMENT; }
        }

        public ActionAddTotalPaymentParams(PaymentMethod paymentMethod, decimal? amount)
        {
            this.PaymentMethod = paymentMethod;
            this.Amount = amount;
        }
    }
}
