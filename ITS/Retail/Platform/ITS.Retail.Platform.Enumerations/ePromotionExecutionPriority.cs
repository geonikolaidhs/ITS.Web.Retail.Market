using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum ePromotionExecutionPriority
    {
        [Display(Name = "BestValueForCompany", ResourceType = typeof(Resources))]
        BEST_VALUE_FOR_CUSTOMER,
        [Display(Name = "WorstValueForCompany", ResourceType = typeof(Resources))]
        WORST_VALUE_FOR_CUSTOMER
    }
}
