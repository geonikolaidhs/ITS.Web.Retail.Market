using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ITS.Retail.Platform.Enumerations
{
    public enum UserType
    {
        [Display(Name = "CompanyUser", ResourceType = typeof(Resources))]
        COMPANYUSER,
        [Display(Name = "Trader", ResourceType = typeof(Resources))]
        TRADER,
        [Display(Name = "AllUserType", ResourceType = typeof(Resources))]
        ALL,
        [Display(Name = "Admin", ResourceType = typeof(Resources))]
        ADMIN,
        [Display(Name = "NoneUserType", ResourceType = typeof(Resources))]
        NONE
    }
}