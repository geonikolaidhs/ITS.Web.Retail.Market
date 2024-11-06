using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public abstract class PrintServerRequest : PrintServerMessage
    {
        public ePrintServerRequestType Command  { get;set; }

    }
}
