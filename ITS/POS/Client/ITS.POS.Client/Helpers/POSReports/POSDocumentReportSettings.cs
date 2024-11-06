using ITS.POS.Client.Receipt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Helpers.POSReports
{
    public class POSDocumentReportSettings
    {
        public Guid DocumentTypeOid { get; set; }

        public ReceiptSchema XMLPrintFormat { get; set; }

        public Guid CustomReportOid { get; set; }

        public Guid PrintFormatOid { get; set; }

        public string Printer { get; set; }
    }
}
