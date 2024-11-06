using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.SupportingClasses
{
    [NonPersistent]
    public class CashierDepartment
    {
        public string Description { get; set; }
        public eMinistryVatCategoryCode VatRateCode { get; set; }
        public String CategoryCode { get; set; }
        public decimal FirstPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal SecondPrice { get; set; }
        public string Settings { get; set; }
        public decimal SoldQuantity { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AccumulatedQuantity { get; set; }
        public decimal AccumulatedSales { get; set; }
    }
}
