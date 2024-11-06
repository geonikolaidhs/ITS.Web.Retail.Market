using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ITaxOffice: ILookUp2Fields
    {
        string Street { get; set; }
        string PostCode { get; set; }
        string Municipality { get; set; }
    }
}
