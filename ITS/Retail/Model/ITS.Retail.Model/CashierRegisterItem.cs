using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class CashierRegisterItem:BasicObj
    {
        public int DeviceIndex { get; set; }
        public string ItemCode { get; set; }
        public POSDevice POS { get; set; }
        public Item Item { get; set; }
        public decimal Qty { get; set; }
        public bool IsAvaledToSale { get; set; }
        public DateTime DeviceUpdatedOn { get; set; }
    }
}
