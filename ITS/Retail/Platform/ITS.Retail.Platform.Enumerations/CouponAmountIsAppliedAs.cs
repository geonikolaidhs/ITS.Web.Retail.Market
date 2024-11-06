using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    /// <summary>
    /// Defines how a coupon is applied i.e as a Discount or as a Payment Method or Points
    /// </summary>
    public enum CouponAmountIsAppliedAs
    {
        [Display(Name = "Discount", ResourceType = typeof(Resources))]
        DISCOUNT,
        [Display(Name = "PaymentMethod", ResourceType = typeof(Resources))]
        PAYMENT_METHOD,
        [Display(Name = "Points", ResourceType = typeof(Resources))]
        POINTS
    }
}
