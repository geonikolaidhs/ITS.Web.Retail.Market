using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPrintDocumentToWindowsPrinterParams : ActionParams
    {
        public DocumentHeader DocumentHeader { get; set; }

        public override eActions ActionCode
        {
            get
            {
                return eActions.PRINT_DOCUMENT_TO_WINDOWS_PRINTER;
            }
        }

        public ActionPrintDocumentToWindowsPrinterParams(DocumentHeader documentHeader)
        {
            this.DocumentHeader = documentHeader;
        }
    }
}
