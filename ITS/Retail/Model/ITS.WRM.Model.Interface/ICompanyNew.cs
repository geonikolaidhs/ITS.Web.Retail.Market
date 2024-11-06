using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ICompanyNew : IBasicObj, ICompanyModel
    {
        string B2CURL { get; set; }
        double Balance { get; set; }
        string Code { get; set; }
        string CompanyName { get; set; }
        Guid? DefaultAddressOid { get; set; }
        string Profession { get; set; }

    }
}
