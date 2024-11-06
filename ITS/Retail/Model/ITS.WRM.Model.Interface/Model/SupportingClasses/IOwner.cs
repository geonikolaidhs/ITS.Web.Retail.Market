using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface.Model.SupportingClasses
{
    public interface IOwner
    {
        ICompanyNew Owner { get; }
        String Description { get; }
    }
    public interface IRequiredOwner : IOwner
    {

    }
}
