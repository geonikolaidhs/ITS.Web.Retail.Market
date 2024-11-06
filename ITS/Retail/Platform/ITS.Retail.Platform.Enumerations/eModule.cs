using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{

    public enum eModule
    {
        [Display(ResourceType = typeof(Resources), Name = "ALL")]
        ALL,
        [Display(ResourceType = typeof(Resources), Name = "NONE")]
        NONE,
        [Display(ResourceType = typeof(Resources), Name = "HEADQUARTERS")]
        HEADQUARTERS,
        [Display(ResourceType = typeof(Resources), Name = "STORECONTROLLER")]
        STORECONTROLLER,
        [Display(ResourceType = typeof(Resources), Name = "DUAL")]
        DUAL,
        [Display(ResourceType = typeof(Resources), Name = "POS")]
        POS,
        MOBILE,
        SFA
    }
}
