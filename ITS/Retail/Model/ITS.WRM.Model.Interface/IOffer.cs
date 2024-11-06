using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IOffer: ILookUp2Fields
    {
        string Description2 { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        IPriceCatalog PriceCatalog { get; set; }
    }
}
