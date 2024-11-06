using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface.Model.NonPersistant
{
    public interface ILookUpFields:IBaseObj
    {
        string Description { get; set; }
        bool Update { get; set;}
        bool IsDefault { get; set; }
       
    }
}
