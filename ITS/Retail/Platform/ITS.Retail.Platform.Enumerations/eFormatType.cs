using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eFormatType
    {
        X,
        Z,
        [Display(Name = "FormatType_Receipt", ResourceType = typeof(Resources))]
        Receipt,
        DocumentHeader
    }
}
