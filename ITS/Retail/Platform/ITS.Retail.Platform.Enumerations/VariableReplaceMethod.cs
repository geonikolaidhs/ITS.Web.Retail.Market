using ITS.Retail.ResourcesLib;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    public enum VariableReplaceMethod
    {
        [Display(Name = "Replace", ResourceType = typeof(Resources))]
        REPLACE,
        [Display(Name = "Sum", ResourceType = typeof(Resources))]
        SUM
    }
}
