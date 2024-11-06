using NLog;
using ITS.Retail.PrintServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceProcess;
using System.IO;
using System.Reflection;

namespace ITS.Retail.PrintServer.Service
{
    class Program
    {

        static string applicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");
        static string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static string applicationTitle = PrintServerConstants.ServiceName;
        public static Logger Logger { get; set; }
        static void Main(string[] arg)
        {
            Logger = LogManager.GetLogger("WRM Print Service");
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                Arguments args = new Arguments(arg);

                if (arg.Length == 0 || arg[0] == "")
                {
                    PrintServer bridge = new PrintServer(true);
                    Thread.Sleep(1800000);
                }
                else if (args["NOSERVICE"] != null)
                {
                    if (ServiceCheck(false))
                    {
                        ServiceController controller = new ServiceController(applicationName);
                        if (controller.Status == ServiceControllerStatus.Running)
                        {
                            controller.Stop();
                        }
                        ServiceInstaller.UnInstallService(applicationName);
                    }
                }
                else if (args["SERVICE"] != null)
                {                   
                    if (ServiceCheck(true) == false)
                    {
                        ServiceController controller = new ServiceController(applicationName);
                        controller.Start();
                        return;
                    }

                    ServiceBase[] services = new ServiceBase[] { new PrintService() };
                    ServiceBase.Run(services);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "General Exception");
            }
        }


        static bool ServiceCheck(bool autoInstall)
        {
            bool installed = false;

            ServiceController[] controllers = ServiceController.GetServices();
            foreach (ServiceController con in controllers)
            {
                if (con.ServiceName == applicationTitle)
                {
                    installed = true;
                    break;
                }
            }

            if (installed)
            {
                return true;
            }

            if (autoInstall)
            {
                ServiceInstaller.InstallService("\"" + applicationPath + "\\" + applicationName + ".exe\" --SERVICE", applicationTitle, applicationTitle, true, false);
            }

            return false;
        }
    }
}
