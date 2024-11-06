using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IPriceCatalogPolicyDetail: IBaseObj
    {
        IPriceCatalog PriceCatalog { get; set; }
        int Sort { get; set; }
        PriceCatalogSearchMethod PriceCatalogSearchMethod { get; set; }
        bool IsDefault { get; set; }
        IPriceCatalogPolicy PriceCatalogPolicy { get; set; }
    }
}
