using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IVariableActionType: IBaseObj
    {
        //Variable Variable { get; set; }
        IActionType ActionType { get; set; }
        VariableMethods VariableAction { get; set; }
        //VariableReplaceMethod VariableReplaceMethod { get; set; }
        //bool UpdateFieldOnRecalculate { get; set; }
        string VariableName { get; set; }
    }
}
