using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum PaddingDirection
    {
        [Display(Name = "FromLeft", ResourceType = typeof(Resources))]
        LEFT,
        [Display(Name = "FromRight", ResourceType = typeof(Resources))]
        RIGHT
    }
}
