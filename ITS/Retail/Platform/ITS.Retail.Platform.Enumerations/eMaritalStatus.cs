using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eMaritalStatus
    {
        [Display(ResourceType = typeof(Resources), Name = "Other")]
        OTHER,
        [Display(ResourceType = typeof(Resources), Name = "Married")]
        MARRIED,
        [Display(ResourceType = typeof(Resources), Name = "MaritalStatusSingle")]
        SINGLE,
        [Display(ResourceType = typeof(Resources), Name = "Widowed")]
        WIDOWED,
        [Display(ResourceType = typeof(Resources), Name = "Divorced")]
        DIVORCED

    }
}
