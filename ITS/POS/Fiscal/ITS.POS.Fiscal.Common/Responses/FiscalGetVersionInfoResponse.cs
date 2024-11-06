using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Responses
{
    public class FiscalGetVersionInfoResponse : FiscalResponse
    {
        public String ExResult { get; set; }
        public int ErrorCode { get; set; }
        public FiscalGetVersionInfoResponse() { }

        public FiscalGetVersionInfoResponse(String exResult, eFiscalResponseType result, int errorCode)
        {
            ExResult = exResult;
            Result = result;
            ErrorCode = errorCode;
        }
    }

}
