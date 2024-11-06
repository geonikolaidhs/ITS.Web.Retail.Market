using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Common.Responses
{
    public class FiscalCheckSystemResponse : FiscalResponse
    {
        public String Explanation { get; set; }
        public short ErrorCode { get; set; }
        public FiscalCheckSystemResponse()
        {
        }
        public FiscalCheckSystemResponse(String explanation, eFiscalResponseType result, short errorCode)
        {
            Explanation = explanation;
            Result = result;
            ErrorCode = errorCode;
        }
    }
}
