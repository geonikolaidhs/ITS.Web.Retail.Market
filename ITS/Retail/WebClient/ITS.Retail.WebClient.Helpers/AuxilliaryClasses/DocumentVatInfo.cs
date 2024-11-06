using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers.AuxilliaryClasses
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
    }
}
