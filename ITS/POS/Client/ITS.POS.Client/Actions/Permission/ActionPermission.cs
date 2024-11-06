using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions.Permission
{
    public class ActionPermission
    {
        public eKeyStatus KeyStatus { get;  set; }
        public eActions ActionCode { get;  set; }        
    }
}
