using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eSex
    {
        [Display(ResourceType = typeof(Resources), Name = "Undefined")]
        UNDEFINED,
        [Display(ResourceType = typeof(Resources), Name = "Male")]
        MALE,
        [Display(ResourceType = typeof(Resources), Name = "Female")]
        FEMALE,
        
    }
}
