using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    /// <summary>
    /// Defines where a Coupon applies i.e. Item or Document.GrossTotal
    /// </summary>
    public enum CouponAppliesOn
    {
        [Display(Name = "Item", ResourceType = typeof(Resources))]
        ITEM,
        [Display(Name = "DocumentGrossTotal", ResourceType = typeof(Resources))]
        GROSS_TOTAL
    }
}
