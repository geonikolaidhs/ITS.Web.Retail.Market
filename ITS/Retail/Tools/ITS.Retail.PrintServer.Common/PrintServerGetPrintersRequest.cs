using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public class PrintServerGetPrintersRequest : PrintServerRequest
    {
        public PrintServerGetPrintersRequest()
        {
            Command = ePrintServerRequestType.GET_PRINTERS;
        }
    }
}
