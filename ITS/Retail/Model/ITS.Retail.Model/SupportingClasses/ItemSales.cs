using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.SupportingClasses
{
    [NonPersistent]
    public class ItemSales:CashDeviceItem
    {
        public decimal SoldQTY { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public Guid DeviceOid { get; set; }
        public String DeviceName { get; set; }
        public decimal NetValue { get; set; }
        public decimal VatValue { get; set; }
        public Guid? ItemOid { get; set; }
        public eMinistryVatCategoryCode MinistryVatCategoryCode { get; set; }
        public Guid VatCategoryOid { get; set; }
        public Guid VatFactorOid { get; set; }
    }
}
