using ITS.Retail.ResourcesLib;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    public enum PriceCatalogSearchMethod
    {
        [Display(Name = "CurrentPriceCatalog", ResourceType = typeof(Resources))]
        CURRENT_PRICECATALOG,
        [Display(Name = "PriceCatalogTree", ResourceType = typeof(Resources))]
        PRICECATALOG_TREE
    }
}
