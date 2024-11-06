using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPublishLineQuantityInfoParams : ActionParams
    {
        public decimal Qty { get; set; }
        public string Format { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.PUBLISH_LINE_QUANTITY_INFO; }
        }

        public ActionPublishLineQuantityInfoParams(decimal qty, string format)
        {
            this.Qty = qty;
            this.Format = format;
        }
    }
}
