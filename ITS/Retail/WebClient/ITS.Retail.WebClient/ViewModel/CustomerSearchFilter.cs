using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    [DefaultValue(NA)]
    public enum eBooleanExtended
    {
        NA = -1, False = 0, True = 1
    }

    public class CustomerSearchFilter
    {

        public String Code { get; set; }

        public eBooleanExtended IsActive { get; set; }

        public String CompanyName { get; set; }

        public DateTime? NewCustomersAfter { get; set; }

        public String TaxCode { get; set; }

        public DateTime? UpdatedAfter { get; set; }


    }
}