using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IOwnerCategories: ILookUpFields
    {
        ICategoryNode CategoryNode { get; set; }
        ICompanyNew Owner { get; set; }
    }
}
