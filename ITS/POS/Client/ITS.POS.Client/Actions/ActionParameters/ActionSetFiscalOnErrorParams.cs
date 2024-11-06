using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionSetFiscalOnErrorParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.SET_FISCAL_ON_ERROR; }
        }

        public bool SetOnError { get; set; }

        public ActionSetFiscalOnErrorParams(bool setOnError)
        {
            this.SetOnError = setOnError;
        }
    }
}
