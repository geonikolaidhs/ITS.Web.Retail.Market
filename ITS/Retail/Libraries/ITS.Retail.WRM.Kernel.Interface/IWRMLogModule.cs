using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;

namespace ITS.Retail.WRM.Kernel.Interface
{
    public interface IWRMLogModule : IKernelModule
    {
        void Log(string logMessage, KernelLogLevel kernelLogLevel = PlatformConstants.DefaultKernelLogLevel);

        void Log(Exception exception, string errorMessage, KernelLogLevel kernelLogLevel = PlatformConstants.DefaultKernelLogLevel);

        void Log(Exception exception, string errorMessage = "", string controller = "", string action = "", string userAgent = "", string ipAddress = "",
            string result = "", Guid? userId = null, KernelLogLevel kernelLogLevel = PlatformConstants.DefaultKernelLogLevel);
    }
}
