using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eUpdaterMode
    {
        [Display(Name = "AUTOMATIC", ResourceType = typeof(Resources))]
        AUTOMATIC,
        [Display(Name = "MANUAL", ResourceType = typeof(Resources))]
        MANUAL
    }
}
