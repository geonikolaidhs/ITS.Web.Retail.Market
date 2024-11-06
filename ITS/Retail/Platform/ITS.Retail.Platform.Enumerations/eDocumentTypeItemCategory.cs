using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eDocumentTypeItemCategory
    {

        [Display(Name = "NoneSelect", ResourceType = typeof(Resources))]
        NONE,
        [Display(Name = "IncludeItemCategories", ResourceType = typeof(Resources))]
        INCLUDE_ITEM_CATEGORIES,
        [Display(Name = "ExcludeItemCategories", ResourceType = typeof(Resources))]
        EXCLUDE_ITEM_CATEGORIES,
    }
}
