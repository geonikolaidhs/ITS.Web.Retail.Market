using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers.AuxilliaryClasses
{
    public class PriceCatalogPolicyPriceResult
    {
        public PriceCatalogDetail PriceCatalogDetail { get; set; }
        public Barcode SearchBarcode { get; set; }
    }
}
