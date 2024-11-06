using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Master;
using ITS.POS.Model.Transactions;
using ITS.POS.Hardware;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddWeightedItemParams : ActionParams
    {
        public string ItemCode;

        public override eActions ActionCode
        {
            get { return eActions.ADD_ITEM_WEIGHTED; }
        }

        public ActionAddWeightedItemParams(string itemCode)
        {
            this.ItemCode = itemCode;
        }
    }
}
