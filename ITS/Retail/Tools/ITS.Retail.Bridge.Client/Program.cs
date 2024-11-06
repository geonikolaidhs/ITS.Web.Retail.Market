using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;
using System.ServiceProcess;


namespace ITS.Retail.Bridge.Service
{
    static class Program
    {
        static string applicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");
        static string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static string applicationTitle = "ITS.Retail.Bridge.Service";

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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String[] arg)
        {
            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();//TOCHECK

            Arguments args = new Arguments(arg);

            if(arg.Length == 0|| arg[0] == "")
            {
                BridgeClient bridge = new BridgeClient(Retail.Bridge.Service.BridgeClient.Mode.FILEWATCHER, display:true);
                Thread.Sleep(1800000);
            }
            else if (args["NOSERVICE"] != null )
            {
                if (ServiceCheck(false))
                {
                    ServiceController controller = new ServiceController(applicationName);
                    if (controller.Status == ServiceControllerStatus.Running) controller.Stop();
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

                ServiceBase[] services = new ServiceBase[] { new BridgeService() };
                ServiceBase.Run(services);
            }
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
                if(!Directory.Exists(args["d"]))
                {
                    Console.Out.WriteLine("Invalid parameters; Directory '" + args["d"] + "' does not exist.");
                    return;
                }
                BridgeClient bridge = new BridgeClient(Retail.Bridge.Service.BridgeClient.Mode.EXPORT,username,password,storeCode,exportDirectory,true);
            }
        }
    }
}
