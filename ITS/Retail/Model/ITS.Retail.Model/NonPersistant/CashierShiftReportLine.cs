using System;

namespace ITS.Retail.Model.NonPersistant
{
    public class CashierShiftReportLine
    {
        public CashierShiftReportLine()
        {
            Oid = Guid.NewGuid();
        }

        public Guid Oid { get; set; }
        public string Description { get; set; }
        public DateTime ReportDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DocumentType DocumentType { get; set; }
        public ShiftType ShiftType { get; set; }
        public eCashierLineType LineType { get; set; }
        public decimal DeviceAmount { get; set; }
        public decimal DeviceQty { get; set; }
        public decimal CashierAmount { get; set; }
        public decimal CashierQty { get; set; }
        public decimal DeviceDrawer { get; set; }
        public decimal DrawerDifference { get; set; }
        public decimal FinalDrawerDifference { get; set; }
        public decimal QtyDifference { get; set; }
        public decimal ValueDifference { get; set; }
        public bool AfftectsDrawer { get; set; }
        public int index { get; set; }
        public decimal FinalDrawer { get; set; }
        public bool IsProforma { get; set; } = false;
        public Store Store { get; set; }
        public CompanyNew Company { get; set; }
        public User User { get; set; }
        public string Device { get; set; }
        public bool IsWarningRow { get; set; } = false;

    }
}
