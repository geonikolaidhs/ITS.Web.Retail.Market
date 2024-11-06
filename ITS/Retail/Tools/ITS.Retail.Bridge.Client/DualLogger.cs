using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Common.Logging;

namespace Retail
{
    public static class DualLogger
    {
        public static void SetLogFilePath(string filePath)
        {
            Logger.SetLogFilePath(filePath);
        }

        public static void Warn(object callingClass, string methodName, string message, Exception ex = null, bool dislpay = false)
        {
            Logger.Warn(callingClass,methodName, message,ex);
            if(dislpay)
                LogMessage(callingClass,methodName,message, ex, LogLevel.WARN);
        }

        public static void Debug(object callingClass, string methodName, string message, Exception ex = null, bool dislpay = false)
        {
            Logger.Debug(callingClass, methodName, message, ex);
            if (dislpay)
                LogMessage(callingClass, methodName, message, ex, LogLevel.DEBUG);
        }

        public static void Error(object callingClass, string methodName, string message, Exception ex = null, bool dislpay = false)
        {
            Logger.Error(callingClass, methodName, message, ex);
            if (dislpay)
                LogMessage(callingClass, methodName, message, ex, LogLevel.ERROR);
        }

        public static void Fatal(object callingClass, string methodName, string message, Exception ex = null, bool dislpay = false)
        {
            Logger.Fatal(callingClass, methodName, message, ex);
            if (dislpay)
                LogMessage(callingClass, methodName, message, ex, LogLevel.FATAL);
        }

        public static void Info(object callingClass, string methodName, string message, Exception ex = null, bool dislpay = false)
        {
            Logger.Info(callingClass, methodName, message, ex);
            if (dislpay)
                LogMessage(callingClass, methodName, message, ex, LogLevel.INFO);
        }

        
        private static void LogMessage(object callingClass, string methodName, string message, Exception t, LogLevel level)
        {          
            try
            {                
                string logMessage = string.Empty;
                logMessage += DateTime.Now.ToString();
                logMessage += ";";
                logMessage += callingClass == null ? "" : callingClass.GetType().ToString();
                logMessage += callingClass == null ? "" : ";";
                logMessage += methodName;
                logMessage += ";";
                logMessage += message;
                logMessage += ";";
                logMessage += t == null ? "" : t.Message;
                logMessage += t == null ? "" : ";";
                logMessage += level.ToString();
                System.Console.WriteLine(logMessage);                
            }
            catch
            {
            }
        }
    }
}
