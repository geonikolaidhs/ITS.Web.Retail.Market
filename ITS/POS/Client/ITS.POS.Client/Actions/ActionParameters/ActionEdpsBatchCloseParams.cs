using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionEdpsBatchCloseParams : ActionParams
    {
        public override eActions ActionCode { get { return eActions.EDPS_BATCH_CLOSE; } }

        public Guid UserDailyTotals { get; set; }
    }
}
