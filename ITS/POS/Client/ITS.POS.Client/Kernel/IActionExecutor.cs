using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface IActionExecutor : IKernelModule
    {
        void ExecuteAction(eActions actionCode, string actionParameters, bool dontCheckPermissions = false, bool validateMachineStatus = true);
    }
}
