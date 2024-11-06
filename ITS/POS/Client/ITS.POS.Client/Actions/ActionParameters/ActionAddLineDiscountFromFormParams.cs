using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddLineDiscountFromFormParams : ActionParams
    {

        public decimal ValueOrPercentage { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.ADD_LINE_DISCOUNT_FROM_FORM; }
        }

        public ActionAddLineDiscountFromFormParams(decimal valueOrPercentage)
        {
            this.ValueOrPercentage = valueOrPercentage;
        }
    }
}
