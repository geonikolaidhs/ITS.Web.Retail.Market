using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public class PrintServerPrintDocumentResponse: PrintServerResponse
    {
        public String Explanation { get; set; }
        public short ErrorCode { get; set; }

        public PrintServerPrintDocumentResponse()
        {
        }
        public PrintServerPrintDocumentResponse(String explanation, ePrintServerResponseType result, short errorCode)
        {
            Explanation = explanation;
            Result = result;
            ErrorCode = errorCode;
        }
    }
}
