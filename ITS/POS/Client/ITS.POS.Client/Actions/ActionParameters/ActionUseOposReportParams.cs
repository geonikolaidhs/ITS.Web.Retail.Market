using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionUseOposReportParams : ActionParams
    {
        public string PosReportCode { get; set; }

        public override eActions ActionCode
        {
            get
            {
                return eActions.USE_OPOS_REPORT;
            }
        }

        public ActionUseOposReportParams(string posReportCode)
        {
            this.PosReportCode = posReportCode;
        }
    }
}
