using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Receipt
{
    public class DynamicLineCell
    {
        public eAlignment CellAlignment { get; set; }
        public bool IsBold { get; set; }
        //public uint Order { get; set; }
        public string Content { get; set; }
        public uint MaxCharacters { get; set; }

    }
}
