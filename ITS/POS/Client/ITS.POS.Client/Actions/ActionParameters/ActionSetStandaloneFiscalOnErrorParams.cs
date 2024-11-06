using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionSetStandaloneFiscalOnErrorParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.SET_STANDALONE_FISCAL_ON_ERROR; }
        }

        public bool SetOnError { get; set; }

        public ActionSetStandaloneFiscalOnErrorParams(bool setOnError)
        {
            this.SetOnError = setOnError;
        }
    }
}
