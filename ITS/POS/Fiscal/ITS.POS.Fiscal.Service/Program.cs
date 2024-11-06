using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using ITS.POS.Fiscal.Common;
using NLog;
using ITS.Retail.Platform.Enumerations;
using System.Diagnostics;
using ITS.POS.Fiscal.Service.RequestResponceLogging;

namespace ITS.POS.Fiscal.Service
{
    class Program
    {
        static string applicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");
        static string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static string applicationTitle = Constants.ServiceName;
        public static Logger Logger { get; set; }
        public static int MessagesReceived { get; set; }

        public static RequestResponseLogger RequestResponseLogger { get; set; }

        static bool ServiceCheck(bool autoInstall)
        {
            bool installed = false;

            ServiceController[] controllers = ServiceController.GetServices();
            foreach (ServiceController con in controllers)
            {
                if (con.ServiceName == applicationName)
                {
                    installed = true;
                    break;
                }
            }

            if (installed) return true;

            if (autoInstall)
            {
                
                ServiceInstaller.InstallService("\"" + applicationPath + "\\" + applicationName + ".exe\" --SERVICE", applicationName, applicationTitle, true, false);
            }

            return false;
        }

        static void Main(string[] arg)
        {
            WriteToWindowsEventLog("Trying to create file logger", EventLogEntryType.Information);
            Logger = LogManager.GetLogger("DiSign Service");
            WriteToWindowsEventLog("File logger created", EventLogEntryType.Information);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            MessagesReceived = 0;
            try
            {
                Arguments args = new Arguments(arg);

                if (arg.Length == 0 || arg[0] == "")
                {
                    FiscalServer bridge = new FiscalServer(true);
                    Thread.Sleep(1800000);
                }
                else if (args["NOSERVICE"] != null)
                {
                    if (ServiceCheck(false))
                    {
                        WriteToWindowsEventLog("Starting DiSign windows service uninstall", EventLogEntryType.Information);
                        ServiceController controller = new ServiceController(applicationName);
                        if (controller.Status == ServiceControllerStatus.Running) controller.Stop();
                        ServiceInstaller.UnInstallService(applicationName);
                        WriteToWindowsEventLog("DiSign windows service uninstalled", EventLogEntryType.Information);
                    }
                }
                else if (args["SERVICE"] != null)
                {
                    if (ServiceCheck(true) == false)
                    {
                        WriteToWindowsEventLog("Starting DiSign as windows service", EventLogEntryType.Information);
                        ServiceController controller = new ServiceController(applicationName);
                        controller.Start();
                        WriteToWindowsEventLog("DiSign started as windows service", EventLogEntryType.Information);
                        return;
                    }
                    WriteToWindowsEventLog("Calling DiSign to start as windows service", EventLogEntryType.Information);
                    ServiceBase[] services = new ServiceBase[] { new FiscalService() };
                    ServiceBase.Run(services);
                    WriteToWindowsEventLog("End of calling DiSign to start as windows service", EventLogEntryType.Information);
                }
            }
            catch (Exception ex)
            {
                WriteToWindowsEventLog(ex.GetFullMessage(), EventLogEntryType.Error);
                Logger.Error(ex, "General Exception");
            }
                /*
            else if (args["EXPORT"] != null)
            {
                string username;
                string password;
                string exportDirectory;
                string storeCode;
                if (args["username"] == null || args["password"] == null || args["d"] == null)
                {
                    Console.Out.WriteLine("Invalid parameters; usage ITS.Retail.Bridge.Client.exe -export -username \"username\" -password \"password\" [-store \"storeCode\"] -d \"exportDirectory\"");
                    return;
                }
                username = args["username"];
                password = args["password"];
                storeCode = args["store"];
                exportDirectory = args["d"];
                if (!Directory.Exists(args["d"]))
                {
                    Console.Out.WriteLine("Invalid parameters; Directory '" + args["d"] + "' does not exist.");
                    return;
                }
                BridgeClient bridge = new BridgeClient(Retail.Bridge.Service.BridgeClient.Mode.EXPORT, username, password, exportDirectory, storeCode, true);
            }*/
        }

        private static void WriteToWindowsEventLog(string message, EventLogEntryType eventLogEntryType)
        {
            EventLog.WriteEntry("DiSign Service", message,eventLogEntryType, 0);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e.ExceptionObject as Exception, "Unhandled Exception");
        }
    }
}
