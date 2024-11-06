using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Receipt
{
    public class ReceiptPart
    {
       public List<ReceiptElement> Elements { get; set; }

       public ReceiptPart()
        {
            Elements = new List<ReceiptElement>();
        }
    }
}
