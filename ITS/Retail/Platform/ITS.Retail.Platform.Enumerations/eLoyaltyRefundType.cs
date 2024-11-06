using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eLoyaltyRefundType
    {
        [Display(Name = "Discount", ResourceType = typeof(Resources))]
        DISCOUNT,
        [Display(Name = "Payment", ResourceType = typeof(Resources))]
        PAYMENT
    }
}
