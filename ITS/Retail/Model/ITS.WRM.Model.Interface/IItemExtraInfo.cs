using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IItemExtraInfo: IBasicObj, IRequiredOwner
    {
        IItem Item { get; set; }
        string Ingredients { get; set; }
        DateTime PackedAt { get; set; }
        DateTime ExpiresAt { get; set; }

    }
}
