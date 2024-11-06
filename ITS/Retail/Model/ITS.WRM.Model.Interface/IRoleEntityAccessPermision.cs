using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IRoleEntityAccessPermision:IBasicObj
    {
        //IRole Role { get; set; }
        IEntityAccessPermision EnityAccessPermision { get; set; }
    }
}
