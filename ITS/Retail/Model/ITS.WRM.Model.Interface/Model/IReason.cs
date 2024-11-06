using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface.Model
{
    public interface IReason:ILookUp2Fields
    {
        IReasonCategory Category { get; set; }
    }
}
