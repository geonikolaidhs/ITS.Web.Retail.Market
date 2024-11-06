using ITS.Retail.ResourcesLib;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    public enum ePriceSuggestionType
    {
        [Display(Name = "NoneSelect", ResourceType = typeof(Resources))]
        NONE,
        [Display(Name = "LastPriceType", ResourceType = typeof(Resources))]
        LAST_PRICE,
        [Display(Name = "LastSupplierPriceType", ResourceType = typeof(Resources))]
        LAST_SUPPLIER_PRICE,
    }
}
