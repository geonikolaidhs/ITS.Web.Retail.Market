using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Master;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddItemParams : ActionParams
    {
        public string ItemCode;
        public decimal Quantity;
        public bool CheckForWeightedItem;
        public bool ReadWeight;

        public override eActions ActionCode
        {
            get { return eActions.ADD_ITEM; }
        }

        public ActionAddItemParams(string itemCode, decimal qty)
        {
            this.ItemCode = itemCode;
            this.Quantity = qty;
            this.CheckForWeightedItem = false;
            this.ReadWeight = false;
        }
    }
}
