using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum LicenseServerInstance
    {
        [Display(Name = "Undefined", ResourceType = typeof(Resources))]
        UNDEFINED,
        [Display(Name = "MasterOrDual", ResourceType = typeof(Resources))]
        MASTER_OR_DUAL,
        [Display(Name = "STORECONTROLLER", ResourceType = typeof(Resources))]
        STORE_CONTROLLER
    }
}
