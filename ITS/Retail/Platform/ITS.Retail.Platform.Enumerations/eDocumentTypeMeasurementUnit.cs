using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eDocumentTypeMeasurementUnit
    {
        [Display(Name = "NonOrderUnit", ResourceType = typeof(Resources))]
        DEFAULT,
        [Display(Name = "OrderUnit", ResourceType = typeof(Resources))]
        PACKING
    }
}
