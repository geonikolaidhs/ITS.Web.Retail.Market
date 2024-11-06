using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ICustomerStorePriceList: IOwner, IBaseObj
    {
        int Gravity { get; set; }
        ICustomer Customer { get; set; }
        IStorePriceList StorePriceList { get; set; }
        bool IsDefault { get; set; }
    }
}
