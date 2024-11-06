using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionFiscalPrinterReprintZReportsParams : ActionParams
    {
        public override eActions ActionCode
        {
            get { return eActions.FISCAL_PRINTER_REPRINT_Z_REPORTS ; }
        }

        public bool UseDateFilter { get; protected set; }
        public eReprintZReportsMode Mode { get; protected set; }

        public ActionFiscalPrinterReprintZReportsParams(bool useDateFilter, eReprintZReportsMode mode)
        {
            this.UseDateFilter = useDateFilter;
            this.Mode = mode;
        }
    }
}
