using System;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WRM.Kernel.Interface;
using ITS.Retail.Platform;
using ITS.Retail.Common;
using ITS.Retail.Model;
using NLog;
using DevExpress.Xpo;


namespace ITS.Retail.WRM.Kernel
{
    public class WRMLogModule : IWRMLogModule
    {
        Logger Logger;
        public WRMLogModule()
        {
            Logger = LogManager.GetLogger("ITS.Retail");
        }

        public WRMLogModule(string loggerName)
        {
            Logger = LogManager.GetLogger(loggerName);
        }

        public WRMLogModule(string loggerName, bool logOnDatabase)
        {
            LogOnDatabase = logOnDatabase;
            Logger = LogManager.GetLogger(loggerName);
        }

        private bool LogOnDatabase = true;

        public void Log(Exception exception, string errorMessage = "", string controller = "", string action = ""
            , string userAgent = "", string ipAddress = ""
            , string result = "", Guid? userId = null
            , KernelLogLevel kernelLogLevel = PlatformConstants.DefaultKernelLogLevel)
        {
            Logger.Log(kernelLogLevel.GetNLogLogLevel(), exception, errorMessage);
            if (LogOnDatabase)
            {
                if (kernelLogLevel == KernelLogLevel.Error || kernelLogLevel == KernelLogLevel.Fatal)
                {
                    try
                    {
                        using (UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork())
                        {
                            ApplicationLog applicationLog = new ApplicationLog(unitOfWork);
                            string error = ((exception == null) ? "" : exception.GetFullMessage() + Environment.NewLine + exception.StackTrace);
                            applicationLog.Controller = controller;
                            applicationLog.Action = action;
                            applicationLog.Error = error;
                            applicationLog.Result = errorMessage + " " + result;
                            applicationLog.UserAgent = userAgent;
                            applicationLog.IPAddress = ipAddress;
                            //applicationLog.Result = errorMessage;

                            if (userId.HasValue && userId.Value != Guid.Empty)
                            {
                                applicationLog.CreatedBy = unitOfWork.GetObjectByKey<User>(userId.Value);
                            }
                            applicationLog.Save();
                            XpoHelper.CommitTransaction(unitOfWork);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

        }

        public void Log(string logMessage, KernelLogLevel kernelLogLevel = PlatformConstants.DefaultKernelLogLevel)
        {
            Exception exception = null;
            if (kernelLogLevel == KernelLogLevel.Error
             || kernelLogLevel == KernelLogLevel.Fatal
              )
            {
                exception = new Exception(logMessage);
            }
            Log(exception, logMessage, kernelLogLevel: kernelLogLevel);
        }
        public void Log(Exception exception, string errorMessage, KernelLogLevel kernelLogLevel = PlatformConstants.DefaultKernelLogLevel)
        {
            if (exception == null
                && (kernelLogLevel == KernelLogLevel.Error
              || kernelLogLevel == KernelLogLevel.Fatal
                )
             )
            {
                exception = new Exception(errorMessage);
            }
            Log(exception, errorMessage, "", kernelLogLevel: kernelLogLevel);
        }
    }
}
