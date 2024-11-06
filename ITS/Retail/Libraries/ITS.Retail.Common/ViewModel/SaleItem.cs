using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public class SaleItem
    {
        public SaleItem() { }

        public SaleItem(string code)
        {
            Code = code;
            Qty = 0;
        }

        public SaleItem(string code, decimal qty)
        {
            Code = code;
            Qty = qty;
        }

        public string Code { get; set; }

        public decimal Qty { get; set; }

    }
}
