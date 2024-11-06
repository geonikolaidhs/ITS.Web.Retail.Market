using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware;

namespace ITS.POS.Client.Actions.ActionParameters
{
	class ActionIssueXReportParams : ActionParams
	{
        public bool SkipNonFiscalReportPrint { get; protected set; }

        public override eActions ActionCode
        {
            get { return eActions.ISSUE_X; }
        }

		public ActionIssueXReportParams(bool skipNonFiscalReportPrint)
        {
            SkipNonFiscalReportPrint = skipNonFiscalReportPrint;
        }
	}
}
