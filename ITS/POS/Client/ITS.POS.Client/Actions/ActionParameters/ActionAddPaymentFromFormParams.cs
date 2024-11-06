using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddPaymentFromFormParams : ActionParams
    {
        public decimal Amount { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.ADD_PAYMENT_FROM_FORM; }
        }

        public ActionAddPaymentFromFormParams(decimal amount)
        {
            Amount = amount;
        }
    }
}
