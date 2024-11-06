using ITS.POS.Client.Actions;
using ITS.POS.Client.Actions.Permission;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface IActionManager : IKernelModule
    {
        IAction GetAction(eActions actionCode);
        bool IsExternalAction(eActions actionCode);
        List<eActions> GetExternalActionCodes();
        List<ActionPermission> ActionPermissions { get; }
        List<CustomActionCode> CustomActionCodes { get; }
    }
}
