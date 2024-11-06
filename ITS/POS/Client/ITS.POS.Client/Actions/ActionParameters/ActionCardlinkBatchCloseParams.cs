using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionCardlinkBatchCloseParams : ActionParams
    {
        public override eActions ActionCode { get { return eActions.CARDLINK_BATCH_CLOSE; } }

        public Guid UserDailyTotals { get; set; }
    }
}
