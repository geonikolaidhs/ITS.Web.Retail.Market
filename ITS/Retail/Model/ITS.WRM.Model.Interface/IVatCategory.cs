using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IVatCategory
    {
        eMinistryVatCategoryCode MinistryVatCategoryCode { get; set; }
    }
}
