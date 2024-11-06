using ITS.POS.Fiscal.Common;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Responses
{
    public class FiscalIssueZResponse : FiscalResponse
    {
        public String ExResult { get; set; }
        public int ErrorCode { get; set; }
        public string UploadZErrorMessage { get; set; }
        public FiscalIssueZResponse() { }

        public FiscalIssueZResponse(String exResult, eFiscalResponseType result, int errorCode)
        {
            ExResult = exResult;
            Result = result;
            ErrorCode = errorCode;
        }

        public FiscalIssueZResponse(String exResult, eFiscalResponseType result, int errorCode, string uploadZErrorMessage)
        {
            UploadZErrorMessage = uploadZErrorMessage;
            ExResult = exResult;
            Result = result;
            ErrorCode = errorCode;
        }
    }
}
