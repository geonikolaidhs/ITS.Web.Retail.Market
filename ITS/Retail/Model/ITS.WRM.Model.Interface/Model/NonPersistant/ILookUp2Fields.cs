using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface.Model.NonPersistant
{
    public interface ILookUp2Fields: ILookUpFields, IOwner
    {
        string Code { get; set; }
        string ReferenceCode { get; set; }
    }
}
