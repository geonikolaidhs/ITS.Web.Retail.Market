using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eTransformationLevel
    {
        [Display(Name = "DEFAULT_TRANSFORM_LEVEL", ResourceType = typeof(Resources))]
        DEFAULT,//Document is not derived from transformation so the Document Functionality is the default
        [Display(Name = "FREEZE_VALUES", ResourceType = typeof(Resources))]
        FREEZE_VALUES,// Customer, PriceCatalog etc can be modified BUT not The values!
        [Display(Name = "FREEZE_EDIT", ResourceType = typeof(Resources))]
        FREEZE_EDIT,// Nor the values nor Customer,PriceCAtalog etc. cn be modified
        //FULL_EDIT// All Properties may be modified
    }
}
