using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionChangeItemPriceParams : ActionParams
    {

        public ActionChangeItemPriceParams(DocumentDetail documentDetail)
        {
            this.DocumentDetail = documentDetail;
        }

        public override eActions ActionCode
        {
            get { return eActions.CHANGE_ITEM_PRICE; }
        }

        public DocumentDetail DocumentDetail { get; set; }
    }
}
