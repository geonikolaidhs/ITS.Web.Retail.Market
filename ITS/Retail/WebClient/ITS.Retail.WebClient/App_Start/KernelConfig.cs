using ITS.Retail.WRM.Kernel;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.App_Start
{
    public class KernelConfig
    {
        public static IWRMKernel RegisterKernel()
        {
            IWRMKernel WRM_Kernel = new WRMKernel();

            WRMLogModule logModule = new WRMLogModule();
            WRMDbModule dbModule = new WRMDbModule();
            WRMUserModule userModule = new WRMUserModule(dbModule);
            MvcApplication.WRMLogModule = logModule;

            WRM_Kernel.RegisterModule<IWRMLogModule, WRMLogModule>(logModule);
            WRM_Kernel.RegisterModule<IWRMDbModule, WRMDbModule>(dbModule);
            WRM_Kernel.RegisterModule<IWRMUserModule, WRMUserModule>(userModule);

            //TODO implement the rest...
            return WRM_Kernel;
        }
    }
}
