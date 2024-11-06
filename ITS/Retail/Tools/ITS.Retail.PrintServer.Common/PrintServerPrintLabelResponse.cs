using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public class PrintServerPrintLabelResponse : PrintServerResponse
    {
        public string Explanation { get; set; }
        public short ErrorCode { get; set; }

        public PrintServerPrintLabelResponse()
        {

        }
    }
}
