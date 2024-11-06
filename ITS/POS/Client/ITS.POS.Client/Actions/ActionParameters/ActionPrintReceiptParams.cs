using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Hardware;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPrintReceiptParams : ActionParams
    {
        public Printer Printer { get; set; }
        //public Drawer Drawer { get; set; }
        public bool CutPaper { get; set; }
        public DocumentHeader DocumentHeader { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.PRINT_RECEIPT; }
        }

        public ActionPrintReceiptParams(Printer printer, bool cutPaper, DocumentHeader documentHeader) //, Drawer drawer
        {
            Printer = printer;
            //Drawer = drawer;
            CutPaper = cutPaper;
            DocumentHeader = documentHeader;
        }
    }
}
