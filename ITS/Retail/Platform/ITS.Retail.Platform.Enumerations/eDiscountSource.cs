using ITS.POS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eDiscountSource
    {
        [Display(Name = "CUSTOM_DISCOUNT", ResourceType = typeof(POSClientResources))]
        CUSTOM,
        [Display(Name = "PRICE_CATALOG_DISCOUNT", ResourceType = typeof(POSClientResources))]
        PRICE_CATALOG,
        [Display(Name = "DOCUMENT_DISCOUNT", ResourceType = typeof(POSClientResources))]
        DOCUMENT,
        [Display(Name = "CUSTOMER_DISCOUNT", ResourceType = typeof(POSClientResources))]
        CUSTOMER,
        [Display(Name = "POINTS_DISCOUNT", ResourceType = typeof(POSClientResources))]
        POINTS,
        [Display(Name = "PROMOTION_DISCOUNT", ResourceType = typeof(POSClientResources))]
        PROMOTION_LINE_DISCOUNT,
        [Display(Name = "PROMOTION_DISCOUNT", ResourceType = typeof(POSClientResources))]
        PROMOTION_DOCUMENT_DISCOUNT,
        [Display(Name = "DEFAULT_DOCUMENT_DISCOUNT", ResourceType = typeof(POSClientResources))]
        DEFAULT_DOCUMENT_DISCOUNT
    }
}
