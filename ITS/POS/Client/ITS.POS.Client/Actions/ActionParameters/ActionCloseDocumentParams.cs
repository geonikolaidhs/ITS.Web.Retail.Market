using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionCloseDocumentParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.CLOSE_DOCUMENT; }
        }

        public bool SkipPrint { get; set; }
        public bool SkipZCheck { get; set; }

        public ActionCloseDocumentParams(bool skipPrint, bool skipZCheck)
        {
            this.SkipPrint = skipPrint;
            this.SkipZCheck = skipZCheck;
        }
    }
}
