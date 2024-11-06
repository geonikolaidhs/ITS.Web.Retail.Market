using ITS.Retail.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    public class DailyCashierShift
    {
        public Guid Oid { get; set; }

        public DailyCashierShift()
        {
            Oid = Guid.NewGuid();
        }
        public List<CashierShift> CashierShifts { get; set; }

        public DateTime ReportDate { get; set; }

        public User User { get; set; }
        public Store Store { get; set; }

        public decimal TotalSellValue { get; set; }
    }
}
