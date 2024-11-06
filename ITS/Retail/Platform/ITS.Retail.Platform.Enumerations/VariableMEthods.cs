using ITS.Retail.ResourcesLib;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    public enum VariableMethods
    {
        [Display(Name = "NoneFemale", ResourceType = typeof(Resources))]
        NONE,
        [Display(Name = "INCREASE", ResourceType = typeof(Resources))]
        INCREASE,
        [Display(Name = "DECREASE", ResourceType = typeof(Resources))]
        DECREASE
    }
}
