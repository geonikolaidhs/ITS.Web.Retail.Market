using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eItemCustomPriceOptions
    {
        [Display(Name = "No", ResourceType = typeof(Resources))]
        NONE,
        [Display(Name = "IsOptional", ResourceType = typeof(Resources))]
        CUSTOM_PRICE_IS_OPTIONAL,
        [Display(Name = "IsMandatory", ResourceType = typeof(Resources))]
        CUSTOM_PRICE_IS_MANDATORY////
    }
}
