using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eDocTypeCustomerCategory
    {

        [Display(Name = "NoneSelect", ResourceType = typeof(Resources))]
        NONE,
        [Display(Name = "IncludeCustCategories", ResourceType = typeof(Resources))]
        INCLUDE_CUSTOMER_CATEGORIES,
        [Display(Name = "ExcludeCustCategories", ResourceType = typeof(Resources))]
        EXCLUDE_CUSTOMER_CATEGORIES,
    }
}
