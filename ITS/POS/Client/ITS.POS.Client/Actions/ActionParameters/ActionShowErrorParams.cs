using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionShowErrorParams : ActionParams
    {
        public string ErrorMessage { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.SHOW_ERROR; }
        }

        public ActionShowErrorParams(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }



    }
}
