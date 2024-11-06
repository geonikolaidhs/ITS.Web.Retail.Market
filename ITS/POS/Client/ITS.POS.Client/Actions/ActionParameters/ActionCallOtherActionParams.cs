using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Resources;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Exceptions;
using ITS.Retail.Platform.Enumerations;


namespace ITS.POS.Client.Actions.ActionParameters
{
    class ActionCallOtherActionParams : ActionParams
    {
        public string NewActionCode
        {
            get;
            protected set;
        }

        public override eActions ActionCode
        {
            get
            {
                return eActions.CALL_OTHER_ACTION;
            }
        }


        public ActionCallOtherActionParams(string actionCode)
        {
            NewActionCode = actionCode;
            
        }
    }
}
