using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eApplicationInstance
    {
        [Display(Name = "RETAIL", ResourceType = typeof(ResourcesLib.Resources))]
        RETAIL,
        [Display(Name = "STORE_CONTROLER", ResourceType = typeof(ResourcesLib.Resources))]
        STORE_CONTROLER,

        [Display(Name = "DUAL_MODE", ResourceType = typeof(ResourcesLib.Resources))]
        DUAL_MODE
    }
}
