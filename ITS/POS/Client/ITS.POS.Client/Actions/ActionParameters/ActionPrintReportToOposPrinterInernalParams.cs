using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPrintReportToOposPrinterInernalParams : ActionParams
    {
        public PosReport PosReport { get; set; }

        public PosOposReportSettings PosReportSettings { get; set; }

        public override eActions ActionCode
        {
            get
            {
                return eActions.PRINT_REPORT_TO_OPOS_PRINTER;
            }
        }

        public ActionPrintReportToOposPrinterInernalParams(PosReport posReportCode, PosOposReportSettings posOposReportSettings)
        {
            this.PosReport = posReportCode;
            this.PosReportSettings = posOposReportSettings;
        }
    }
}
