using ITS.POS.Fiscal.Common;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Responses
{
    public class FiscalSignDocumentResponse : FiscalResponse
    {
        public String Signature { get; set; }
        public int ErrorCode { get; set; }
        public FiscalSignDocumentResponse() { }

        public FiscalSignDocumentResponse(String signature, eFiscalResponseType result, int errorCode)
        {
            Signature = signature;
            Result = result;
            ErrorCode = errorCode;
        }
    }
}
