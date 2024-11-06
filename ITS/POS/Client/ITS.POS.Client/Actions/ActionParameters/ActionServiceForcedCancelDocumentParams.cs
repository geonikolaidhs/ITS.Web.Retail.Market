using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionServiceForcedCancelDocumentParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.SERVICE_FORCED_CANCEL_DOCUMENT; }
        }

        public bool WarnUser { get; protected set; }

        public ActionServiceForcedCancelDocumentParams(bool warnUser)
        {
            WarnUser = warnUser;
        }
    }
}
