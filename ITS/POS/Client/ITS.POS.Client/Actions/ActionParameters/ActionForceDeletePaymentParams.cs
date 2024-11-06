using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionForceDeletePaymentParams : ActionParams
    {
        public DocumentPayment Payment { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.FORCE_DELETE_PAYMENT; }
        }

        public ActionForceDeletePaymentParams(DocumentPayment payment)
        {
            Payment = payment;
        }
    }
}
