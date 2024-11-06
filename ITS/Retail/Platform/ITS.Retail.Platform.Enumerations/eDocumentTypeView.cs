using ITS.Retail.ResourcesLib;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eDocumentTypeView
    {
        [Display(Name = "SimpleFormView", ResourceType = typeof(Resources))]
        Simple,
        [Display(Name = "AdvancedFormView", ResourceType = typeof(Resources))]
        Advanced,
        [Display(Name = "CompositionDecomposition", ResourceType = typeof(Resources))]
        CompositionDecomposition
    }
}
