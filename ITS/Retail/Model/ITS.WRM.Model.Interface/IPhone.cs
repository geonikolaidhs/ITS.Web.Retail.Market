using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IPhone: IBaseObj
    {
        IPhoneType PhoneType { get; set; }
        string Number { get; set; }
        IAddress Address { get; set; }
        ICompanyNew Owner { get; set; }
    }
}
