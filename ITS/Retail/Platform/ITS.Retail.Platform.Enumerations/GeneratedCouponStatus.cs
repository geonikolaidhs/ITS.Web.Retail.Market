using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum GeneratedCouponStatus
    {
        [Display(Name = "Requested", ResourceType = typeof(Resources))]
        Requested,
        [Display(Name = "Used", ResourceType = typeof(Resources))]
        Used
    }
}
