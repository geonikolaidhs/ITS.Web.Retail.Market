using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface.Model.SupportingClasses
{
    public interface IUserModel
    {
        string UserName { get; }
        string Password { get; }

        //IRole Role { get; }
    }
}
