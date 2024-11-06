using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class PriceCheckerViewModel
    {
        public ItemBarcode itemBarcode { get; set; }

        public PriceCatalogDetail priceCatalogDetail { get; set; }

        public CompanyNew Owner { get; set; }

        public WeightedBarcodeInfo weightedBarcodeInfo { get; set; }

        public Customer customer { get; set; }
    }
}