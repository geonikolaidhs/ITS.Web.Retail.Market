using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Receipt
{
    public class DynamicLine : ReceiptLine
    {
        public List<DynamicLineCell> Cells { get; set; }

        public DynamicLine()
        {
            Condition = eCondition.NONE;
            Cells = new List<DynamicLineCell>();

        }
    }
}
