using ITS.Retail.ResourcesLib;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    public enum ComparisonOperator
    {
        [Display(Name = "EQUAL", ResourceType = typeof(Resources))]
        EQUAL,
        [Display(Name = "NOT_EQUAL", ResourceType = typeof(Resources))]
        NOT_EQUAL,
        [Display(Name = "GREATER_THAN", ResourceType = typeof(Resources))]
        GREATER_THAN,
        [Display(Name = "GREATER_OR_EQUAL", ResourceType = typeof(Resources))]
        GREATER_OR_EQUAL,
        [Display(Name = "LESS_THAN", ResourceType = typeof(Resources))]
        LESS_THAN,
        [Display(Name = "LESS_OR_EQUAL", ResourceType = typeof(Resources))]
        LESS_OR_EQUAL
    }
}
