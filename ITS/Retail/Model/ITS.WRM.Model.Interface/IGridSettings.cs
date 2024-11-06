using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IGridSettings: IBaseObj
    {
        string GridName { get; set; }
        string GridLayout { get; set; }
        //IUser User { get; set; }
    }
}
