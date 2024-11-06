using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WRM.Kernel
{
    public static class KernelExtensions
    {
        public static NLog.LogLevel GetNLogLogLevel(this KernelLogLevel logLevel)
        {
            switch (logLevel)
            {
                case KernelLogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case KernelLogLevel.Error:
                    return NLog.LogLevel.Error;
                case KernelLogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
                case KernelLogLevel.Info:
                    return NLog.LogLevel.Info;
                case KernelLogLevel.Warn:
                    return NLog.LogLevel.Warn;
                case KernelLogLevel.Trace:
                    return NLog.LogLevel.Trace;
                case KernelLogLevel.Off:
                    return NLog.LogLevel.Off;
                default:
                    return NLog.LogLevel.Debug;
            }
        }
    }
}
