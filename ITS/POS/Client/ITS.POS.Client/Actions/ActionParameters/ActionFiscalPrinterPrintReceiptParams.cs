using ITS.POS.Hardware;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionFiscalPrinterPrintReceiptParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.FISCAL_PRINTER_PRINT_RECEIPT; }
        }

        public FiscalPrinter Printer { get; set; }
        public bool CutPaper { get; set; }
        public bool PrintAsCanceled { get; set; }
        public bool SkipPrint { get; set; }

        public ActionFiscalPrinterPrintReceiptParams(FiscalPrinter printer, bool cutPaper, bool printAsCanceled, bool skipPrint)
        {
            this.Printer = printer;
            this.CutPaper = cutPaper;
            this.PrintAsCanceled = printAsCanceled;
            SkipPrint = skipPrint;
        }
    }
}
