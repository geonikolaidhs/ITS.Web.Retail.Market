using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eStoreControllerCommand
    {
        [Display(Name = "NONE_COMMAND", ResourceType = typeof(Resources))]//ctrl + .
        NONE = 0,
        [Display(Name = "ON_LINE", ResourceType = typeof(Resources))]
        ON_LINE = 1,
        [Display(Name = "OFF_LINE", ResourceType = typeof(Resources))]
        OFF_LINE = 2,
        [Display(Name = "RESTART", ResourceType = typeof(Resources))]
        RESTART = 4,
    }
}
