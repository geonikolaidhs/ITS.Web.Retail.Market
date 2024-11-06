using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions.ActionParameters
{
    class ActionAddDocumentDiscountFromFormParams : ActionParams
    {
        public decimal ValueOrPercentage { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.ADD_DOCUMENT_DISCOUNT_FROM_FORM; }
        }

        public ActionAddDocumentDiscountFromFormParams(decimal valueOrPercentage)
        {
            this.ValueOrPercentage = valueOrPercentage;
        }
    }
}
