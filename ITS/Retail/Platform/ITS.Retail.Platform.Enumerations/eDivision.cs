using ITS.Retail.Platform.Enumerations.Attributes;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace ITS.Retail.Platform.Enumerations
{
    public enum eDivision
    {
        [Display(Name = "Sales", ResourceType = typeof(Resources))]
        [AvailabeTraderType(eDocumentTraderType.CUSTOMER)]
        Sales,

        [Display(Name = "Purchases", ResourceType = typeof(Resources))]
        [AvailabeTraderType(eDocumentTraderType.SUPPLIER)]
        Purchase,

        [Display(Name = "Storage", ResourceType = typeof(Resources))]
        [AvailabeTraderType(eDocumentTraderType.STORE)]
        Store,

        [AvailabeTraderType(eDocumentTraderType.CUSTOMER | eDocumentTraderType.STORE | eDocumentTraderType.SUPPLIER)]
        [Display(Name = "Other", ResourceType = typeof(Resources))]
        Other,

        [AvailabeTraderType(eDocumentTraderType.CUSTOMER| eDocumentTraderType.STORE | eDocumentTraderType.SUPPLIER)]
        [Display(Name = "Financial", ResourceType = typeof(Resources))]
        Financial,
    }
}
