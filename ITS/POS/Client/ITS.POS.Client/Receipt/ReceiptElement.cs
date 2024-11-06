using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Receipt
{
    public abstract class ReceiptElement
    {
        public eSource Source { get; set; }
        public eCondition Condition { get; set; }

    }
}
