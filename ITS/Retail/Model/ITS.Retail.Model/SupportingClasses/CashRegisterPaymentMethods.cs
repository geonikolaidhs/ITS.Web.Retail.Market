
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.SupportingClasses
{
    [NonPersistent]
    public class CashRegisterPaymentMethods
    {
        public string Description { get; set; }
        public string ShortcutDescription { get; set; }
        public string Code { get; set; }
        public string CreditType { get; set; }
        public decimal DailySum { get; set; }
        public decimal CashIn { get; set; }
        public decimal CashOut { get; set; }
        public decimal TotalSum { get; set; }
    }
}
