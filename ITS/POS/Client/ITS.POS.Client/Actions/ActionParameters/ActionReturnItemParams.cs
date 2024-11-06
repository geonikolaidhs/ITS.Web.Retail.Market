using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionReturnItemParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.RETURN_ITEM; }
        }

        public string Code { get; protected set; }
        public decimal Quantity { get; protected set; }
        public bool FromScanner { get; protected set; }


        public ActionReturnItemParams(string code,decimal qty, bool fromScanner)
        {
            Code = code;
            Quantity = qty;
            FromScanner = fromScanner;
        }

    }
}
