using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Receipt
{
    public class SimpleLine : ReceiptLine
    {
        public string Content { set; get; }

        public SimpleLine(string Content)
        {
            this.Condition = eCondition.NONE;
            this.Content = Content ?? "";
        }
    }
}
