using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IActionType:IBasicObj,IRequiredOwner
    {

        //ActionEntityCategory Category { get; set; }
        eTotalizersUpdateMode UpdateMode { get; set; }
        IStore Store { get; set; }
    }
}
