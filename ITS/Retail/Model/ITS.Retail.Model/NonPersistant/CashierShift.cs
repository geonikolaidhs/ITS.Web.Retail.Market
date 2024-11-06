using System;
using System.Collections.Generic;

namespace ITS.Retail.Model.NonPersistant
{
    public class CashierShift
    {

        public CashierShift()
        {
            Oid = Guid.NewGuid();
        }
        Store Store { get; set; }
        public Guid Oid { get; set; }
        public string Device { get; set; }
        public decimal StartingAmount { get; set; }
        public ShiftType ShiftType { get; set; }
        public User User { get; set; }
        public decimal ShiftSellValue { get; set; }
        public UserDailyTotals UserDailyTotals { get; set; }
        public List<CashierShiftReportLine> CashierShiftReportLines { get; set; }
        public DateTime ReportDate { get; set; }
    }
    public enum eCashierLineType
    {
        Payment,
        Document
    }
    public enum ShiftType
    {
        POS,
        RECEPTION
    }

}
