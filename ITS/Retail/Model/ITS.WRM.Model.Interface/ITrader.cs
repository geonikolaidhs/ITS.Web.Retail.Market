using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ITrader
    {
        string TaxOffice { get; set; }
        string Code { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string TaxCode { get; set; }
        eTraderType TraderType { get; set; }
        Guid? TaxOfficeLookUpOid { get; set; }
    }
}
