using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eStoreDocumentType
    {
        [Display(ResourceType = typeof(Resources), Name = "NONE")]
        [EnumMember]
        NONE,
        [Display(ResourceType = typeof(Resources), Name = "Order")]
        [EnumMember]
        ORDER
    }
}
