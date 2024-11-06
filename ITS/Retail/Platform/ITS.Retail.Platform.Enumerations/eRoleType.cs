using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eRoleType
    {
        [Display(Name = "Customer", ResourceType = typeof(Resources))]
        Customer = 0,
        [Display(Name = "Supplier", ResourceType = typeof(Resources))]
        Supplier = 1,
        [Display(Name = "CompanyUser", ResourceType = typeof(Resources))]
        CompanyUser = 2,
        [Display(Name = "CompanyAdmin", ResourceType = typeof(Resources))]
        CompanyAdministrator = 3,
        [Display(Name = "SystemAdmin", ResourceType = typeof(Resources))]
        SystemAdministrator = 4,
    }
}
