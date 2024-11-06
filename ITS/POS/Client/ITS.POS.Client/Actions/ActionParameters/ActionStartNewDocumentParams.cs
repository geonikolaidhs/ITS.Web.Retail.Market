using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionStartNewDocumentParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.START_NEW_DOCUMENT; }
        }

        public bool OpenFiscalPrinterReceipt { get; protected set; }

        public ActionStartNewDocumentParams(bool openFiscalPrinterReceipt)
        {
            this.OpenFiscalPrinterReceipt = openFiscalPrinterReceipt;
        }
    }
}
