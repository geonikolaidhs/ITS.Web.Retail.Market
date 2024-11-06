using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA_Model.NonPersistant
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateOrUpdaterOrderAttribute : Attribute
    {
        public int Order;
        public eUpdateDirection Permissions = eUpdateDirection.MASTER_TO_SFA | eUpdateDirection.SFA_TO_MASTER;
        public bool IgnoreInsert;
    }
}
