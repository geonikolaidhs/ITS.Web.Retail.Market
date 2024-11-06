using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Reports
{
    public class ReportPrintResult
    {
        public string ErrorMessage { get; set; }

        public PrintResult PrintResult { get; set; }
    }
}
