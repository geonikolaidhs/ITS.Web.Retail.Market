using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eCurrencyPattern
    {
        [Display(Name = "BEFORE_NUMBER", ResourceType = typeof(Resources))]
        BEFORE_NUMBER= 0,
        [Display(Name = "AFTER_NUMBER", ResourceType = typeof(Resources))]
        AFTER_NUMBER= 1,
        [Display(Name = "BEFORE_NUMBER_WITH_SPACE", ResourceType = typeof(Resources))]
        BEFORE_NUMBER_WITH_SPACE= 2,
        [Display(Name = "AFTER_NUMBER_WITH_SPACE", ResourceType = typeof(Resources))]
        AFTER_NUMBER_WITH_SPACE= 3
    }
}
