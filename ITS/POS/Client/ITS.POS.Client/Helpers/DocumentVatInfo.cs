using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Helpers
{
    public class DocumentVatInfo
    {

        public decimal VatFactor { get; set; }
        public int NumberOfItems { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal ItemsQuantity { get; set; }
        public decimal NetTotal { get; set; }
        public decimal GrossTotal { get; set; }
        public Guid VatFactorOid { get; set; }
        public string VatFactorCode { get; set; }
        public string VatCategoryDescription { get; set; }
    }
}
