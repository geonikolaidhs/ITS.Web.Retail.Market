using ITS.Retail.ResourcesLib;
using System;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    [Flags]
    public enum eDocumentTraderType
    {
        [Display(Name = "NONE", ResourceType = typeof(Resources))]
        NONE = 0,
        [Display(Name = "Customer", ResourceType = typeof(Resources))]
        CUSTOMER = 1,
        [Display(Name = "Supplier", ResourceType = typeof(Resources))]
        SUPPLIER = 2,
        [Display(Name = "Store", ResourceType = typeof(Resources))]
        STORE = 4
    }
}
