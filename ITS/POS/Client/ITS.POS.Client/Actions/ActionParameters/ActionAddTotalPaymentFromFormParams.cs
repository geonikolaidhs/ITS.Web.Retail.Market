using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddTotalPaymentFromFormParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.ADD_TOTAL_PAYMENT_FROM_FORM; }
        }

        public decimal? Amount { get; set; }

        public ActionAddTotalPaymentFromFormParams(decimal? amount)
        {
            this.Amount = amount;
        }
    }
}
