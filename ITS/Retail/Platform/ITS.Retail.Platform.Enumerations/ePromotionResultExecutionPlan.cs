using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.Platform.Enumerations
{
    public enum ePromotionResultExecutionPlan
    {
        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "AFTER_DOCUMENT_CLOSED")]
        AFTER_DOCUMENT_CLOSED = 0,
        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "BEFORE_DOCUMENT_CLOSED")]
        BEFORE_DOCUMENT_CLOSED = 1
    }
}
