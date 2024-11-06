using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum DocumentSource
    {
        [Display(Name = "WRM_DOCUMENTSOURCE", ResourceType = typeof(Resources))]
        WRM,
        [Display(Name = "POS_DOCUMENTSOURCE", ResourceType = typeof(Resources))]
        POS,
        [Display(Name = "ANDROID_DOCUMENTSOURCE", ResourceType = typeof(Resources))]
        ANDROID,
        [Display(Name = "MOBILE_DOCUMENTSOURCE", ResourceType = typeof(Resources))]
        MOBILE,
        [Display(Name = "B2C_DOCUMENTSOURCE", ResourceType = typeof(Resources))]
        B2C,
        [Display(Name = "CASHIER_DOCUMENTSOURCE", ResourceType = typeof(Resources))]
        CASHIER,
        [Display(Name = "SFA", ResourceType = typeof(Resources))]
        SFA
    }
}
