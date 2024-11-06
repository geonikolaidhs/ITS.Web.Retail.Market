using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ICustomEnumerationValue: IBaseObj
    {
        ICustomEnumerationDefinition CustomEnumerationDefinition { get; set; }
        string Description { get; set; }
        int Ordering { get; set; }
    }
}
