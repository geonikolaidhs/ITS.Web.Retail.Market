using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public abstract class PrintServerResponse : PrintServerMessage
    {
        public ePrintServerResponseType Result
        {
            get;
            set;
        }
    }
}
