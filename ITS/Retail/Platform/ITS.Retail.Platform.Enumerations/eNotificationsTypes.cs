using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eNotificationsTypes
    {
        [Display(Name = "Key", ResourceType = typeof(Resources))]
        KEY=0,
        [Display(Name = "Action", ResourceType = typeof(Resources))]
        ACTION=1
    }
}
