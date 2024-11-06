using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eKeyStatus
    {
        [Display(Name = "UNKNOWN_POSITION", ResourceType = typeof(Resources))]
        UNKNOWN = -1,
        [Display(Name = "POSITION0", ResourceType = typeof(Resources))]
        POSITION0 = 0,
        [Display(Name = "POSITION1", ResourceType = typeof(Resources))]
        POSITION1 = 1,
        [Display(Name = "POSITION2", ResourceType = typeof(Resources))]
        POSITION2 = 2,
        [Display(Name = "POSITION3", ResourceType = typeof(Resources))]
        POSITION3 = 3,
        [Display(Name = "POSITION4", ResourceType = typeof(Resources))]
        POSITION4 = 4
    }
}
