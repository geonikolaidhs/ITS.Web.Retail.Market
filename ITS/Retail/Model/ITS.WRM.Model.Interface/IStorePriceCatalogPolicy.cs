using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IStorePriceCatalogPolicy: IBaseObj
    {
        IStore Store { get; set;  }
        IPriceCatalogPolicy PriceCatalogPolicy { get; set; }
        bool IsDefault { get; set; }
    }
}
