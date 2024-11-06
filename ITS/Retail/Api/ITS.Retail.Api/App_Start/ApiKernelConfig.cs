using ITS.Retail.WRM.Kernel;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.Api.App_Start
{
    public class ApiKernelConfig
    {
        public static IWRMKernel RegisterKernel()
        {
            IWRMKernel WRM_Kernel = new WRMKernel();

            WRMLogModule logModule = new WRMLogModule();
            WebApiConfig.ApiLogger = logModule;
            WRM_Kernel.RegisterModule<IWRMLogModule, WRMLogModule>(logModule);
            return WRM_Kernel;
        }
    }
}
