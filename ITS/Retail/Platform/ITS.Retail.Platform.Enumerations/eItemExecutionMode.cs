using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eItemExecutionMode
    {
        [Display(Name = "Discount", ResourceType = typeof(ITS.Retail.ResourcesLib.Resources))]
        DISCOUNT,
        [Display(Name = "Price", ResourceType = typeof(ITS.Retail.ResourcesLib.Resources))]
        SET_PRICE
    }
}