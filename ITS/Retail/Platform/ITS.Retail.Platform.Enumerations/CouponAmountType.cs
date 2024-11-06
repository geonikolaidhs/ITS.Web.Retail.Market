using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    /// <summary>
    /// Describes the type of the Coupon amount i.e. it refers to a currency value or to a percentage of the relevant value
    /// </summary>
    public enum CouponAmountType
    {
        [Display(Name = "Percentage", ResourceType = typeof(Resources))]
        PERCENTAGE,
        [Display(Name = "Value", ResourceType = typeof(Resources))]
        VALUE
    }
}
