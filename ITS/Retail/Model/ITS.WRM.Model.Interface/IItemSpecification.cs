using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IItemSpecification: IBasicObj
    {
        IItem Item { get; set; }
        bool CanBenSold { get; set; }
        bool RequiresPrice { get; set; }
    }
}
