using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eDiscountType
    {
        [Display(Name = "Percentage", ResourceType = typeof(Resources))]
        PERCENTAGE,
        [Display(Name = "Value", ResourceType = typeof(Resources))]
        VALUE
    }
}
