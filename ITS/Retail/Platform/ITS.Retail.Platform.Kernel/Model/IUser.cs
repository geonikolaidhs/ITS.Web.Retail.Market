using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IUser : IPersistentObject
    {
        string UserName { get; }
        string Password { get; }

        IRole Role { get; }
    }
}
