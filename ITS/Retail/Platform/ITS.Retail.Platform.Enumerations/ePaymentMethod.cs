using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum ePaymentMethodType
    {
        [Display(Name = "Undefined", ResourceType = typeof(Resources))]
        UNDEFINED=0,
        [Display(Name = "Cash", ResourceType = typeof(Resources))]
        CASH = 1,
        [Display(Name = "Cards", ResourceType = typeof(Resources))]
        CARDS = 2,
        [Display(Name = "Credit", ResourceType = typeof(Resources))]
        CREDIT = 3,      
    }
}
