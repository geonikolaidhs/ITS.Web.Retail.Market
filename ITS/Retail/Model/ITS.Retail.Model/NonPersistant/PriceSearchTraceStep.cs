using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class PriceSearchTraceStep
    {
        public int Number { get; set; }
        public PriceCatalogSearchMethod SearchMethod { get; set; }
        public string PriceCatalogDescription { get; set; }

    }
}
