using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public interface IOwner
    {
        CompanyNew Owner { get; }
        String Description { get; }
    }

    public interface IRequiredOwner : IOwner
    {
        
    }
}
