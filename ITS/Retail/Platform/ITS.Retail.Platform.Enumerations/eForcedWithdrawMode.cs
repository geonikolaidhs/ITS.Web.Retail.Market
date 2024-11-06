using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eForcedWithdrawMode
    {
        [Display(Name = "No", ResourceType = typeof(Resources))]
        NO,
        [Display(Name = "Skippable", ResourceType = typeof(Resources))]
        SKIPPABLE,
        [Display(Name = "Yes", ResourceType = typeof(Resources))]
        YES
    }
}
