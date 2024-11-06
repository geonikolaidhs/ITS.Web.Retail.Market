using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eMeasurementUnitType
    {
        [Display(Name = "OrderMeasurementUnit", ResourceType = typeof(Resources))]
        ORDER,
        [Display(Name = "PackingMeasurementUnit", ResourceType = typeof(Resources))]
        PACKING,
        [Display(Name = "OrderAndPackingMeasurementUnit", ResourceType = typeof(Resources))]
        PACKING_AND_ORDER
    }
}
