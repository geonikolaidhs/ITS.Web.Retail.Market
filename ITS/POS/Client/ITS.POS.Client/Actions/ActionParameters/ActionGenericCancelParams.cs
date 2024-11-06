using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public enum eActionGenericCancelMode
    {
        CANCEL_LINE = 1,
        CANCEL_DISCOUNT = 2,
        CANCEL_PAYMENT = 3,
        CANCEL_DOCUMENT = 4,
    }

    class ActionGenericCancelParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.GENERIC_CANCEL; }
        }

        public eActionGenericCancelMode CancelMode { get; protected set; }

        public ActionGenericCancelParams(eActionGenericCancelMode cancelMode)
        {
            this.CancelMode = cancelMode;
        }
    }
}
