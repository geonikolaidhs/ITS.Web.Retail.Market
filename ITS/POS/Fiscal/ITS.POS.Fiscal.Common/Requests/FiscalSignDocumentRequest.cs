using ITS.POS.Fiscal.Common;
using ITS.POS.Fiscal.Common.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Service.Requests
{
    public class FiscalSignDocumentRequest : FiscalRequest
    {
        public String DocumentToSignPrintFormat { get; set; }
        public String DocumentToSignOfficialFormat { get; set; }
        public FiscalSignDocumentRequest()
        {
            Command = eFiscalRequestType.SIGN_DOCUMENT;
        }

        public FiscalSignDocumentRequest(String documentToSign, String documentToSignOfficialFormat)
        {
            Command = eFiscalRequestType.SIGN_DOCUMENT;
            DocumentToSignPrintFormat = documentToSign;
            DocumentToSignOfficialFormat = documentToSignOfficialFormat;
        }
    }
}
