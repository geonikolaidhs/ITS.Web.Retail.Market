using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public class PrintServerGetPrintersResponse : PrintServerResponse
    {
        public string Explanation { get; set; }
        public short ErrorCode { get; set; }

        public List<string> Printers { get; set; }

        public PrintServerGetPrintersResponse()
        {
            this.Printers = new List<string>();
        }
    }
}
