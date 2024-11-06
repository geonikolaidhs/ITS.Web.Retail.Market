using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.SupportingClasses
{
    [NonPersistent]
    public class DailyTotal
    {
        public decimal VatCategoryA { get; set; }
        public decimal VatCategoryB { get; set; }
        public decimal VatCategoryC { get; set; }
        public decimal VatCategoryD { get; set; }
        public decimal VatCategoryE { get; set; }
        public decimal DailyTotals { get; set; }
        public int ReceiptNumber { get; set; }
        public int IllegalReceiptNumber { get; set; }
        public decimal VoidsTotal { get; set; }
        public decimal RefundsTotal { get; set; }
        public decimal CancelsTotal { get; set; }
    }
}
