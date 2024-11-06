using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Receipt
{
    public abstract class ReceiptLine : ReceiptElement
    {
        //public int Order { get; set; }
        public eAlignment LineAlignment { get; set; }
        public bool IsBold { get; set; }
        public uint MaxCharacters { get; set; }
    }
}
