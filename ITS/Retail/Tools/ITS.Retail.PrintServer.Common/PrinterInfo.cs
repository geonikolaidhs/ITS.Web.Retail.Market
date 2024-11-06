using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public class PrinterInfo
    {
        public string NickName { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public string Status { get; set; }

        public bool IsNetworkPrinter { get; set; }
    }
}
