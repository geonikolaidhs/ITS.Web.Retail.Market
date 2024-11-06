using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public class IgnoreCommonAttribute: Attribute
    {
        public bool RetainCustomJS { get; set; }
        public bool RetainTermsAndConditions { get; set; }
        public bool RetainPasswordCheck { get; set; }
        public bool RetainToolbar { get; set; }
        public bool RetainCookies { get; set; }
        public bool RetaineFormsMessages { get; set; }
        public bool RetaineMenu { get; set; }
        public bool RetainTracking { get; set; }
        public bool RetainViewBags { get; set; }

    }
}