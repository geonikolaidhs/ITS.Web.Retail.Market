using ITS.Retail.ResourcesLib;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    public enum ItemStockAffectionOptions
    {
        [Display(Name = "NO_AFFECTION", ResourceType = typeof(Resources))]
        NO_AFFECTION,
        [Display(Name = "AFFECTS", ResourceType = typeof(Resources))]
        AFFECTS,
        [Display(Name = "INITIALISES", ResourceType = typeof(Resources))]
        INITIALISES
    }
}
