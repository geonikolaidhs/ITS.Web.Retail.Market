using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionDeletePaymentParams : ActionParams
    {
        public DocumentPayment Payment { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.DELETE_PAYMENT; }
        }

        public ActionDeletePaymentParams(DocumentPayment payment)
        {
            Payment = payment;
        }
    }
}
