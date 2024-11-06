using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eTransformationStatus
    {
        [Display(Name = "CANNOT_BE_TRANSFORMED", ResourceType = typeof(ITS.Retail.ResourcesLib.Resources))]
        CANNOT_BE_TRANSFORMED,
        [Display(Name = "NOT_TRANSFORMED", ResourceType = typeof(ITS.Retail.ResourcesLib.Resources))]
        NOT_TRANSFORMED,
        [Display(Name = "PARTIALLY_TRANSFORMED", ResourceType = typeof(ITS.Retail.ResourcesLib.Resources))]
        PARTIALLY_TRANSFORMED,
        [Display(Name = "FULLY_TRANSFORMED", ResourceType = typeof(ITS.Retail.ResourcesLib.Resources))]
        FULLY_TRANSFORMED
    }
}
