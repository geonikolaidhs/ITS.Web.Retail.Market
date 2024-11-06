using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPublishDocumentQuantityInfoParams : ActionParams
    {
        public override Retail.Platform.Enumerations.eActions ActionCode
        {
            get { return eActions.PUBLISH_DOCUMENT_QUANTITY; }
        }

        public decimal Qty { get; set; }
        public string Format { get; set; }


        public ActionPublishDocumentQuantityInfoParams(decimal qty, string format)
        {
            this.Qty = qty;
            this.Format = format;
        }
    }
}
