using ITS.Retail.Common;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ITS.Retail.MigrationTool
{
    static class Program
    {

        public static eApplicationInstance ApplicationInstance { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string basePath = Path.GetFullPath("../../");

            AppDomainSetup domaininfo = new AppDomainSetup();
            domaininfo.ApplicationBase = Path.GetDirectoryName(basePath);
            domaininfo.PrivateBinPath = "bin;Tools/Configuration - Migration Tool";
            
            Evidence adevidence = AppDomain.CurrentDomain.Evidence;
            AppDomain domain = AppDomain.CreateDomain("MyDomain", adevidence, domaininfo);

            Type type = typeof(Proxy);
            Proxy value = (Proxy)domain.CreateInstanceAndUnwrap(
                type.Assembly.FullName,
                type.FullName);

            string assembly = value.GetAssembly("ITS.Retail.WebClient");
            switch(assembly)
            {
                case "HQ":
                    ApplicationInstance = eApplicationInstance.RETAIL;
                    break;
                case "DUAL":
                    ApplicationInstance = eApplicationInstance.DUAL_MODE;
                    break;
                case "STORECONTROLLER":
                    ApplicationInstance = eApplicationInstance.STORE_CONTROLER;
                    break;
                default:
                    throw new NotSupportedException();
            }
            AppDomain.Unload(domain);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        
    }

    public class Proxy : MarshalByRefObject
    {
        public string GetAssembly(string assemblyPath)
        {
            try
            {
                Assembly assembly = Assembly.Load(assemblyPath);
                AssemblyConfigurationAttribute configAttribute = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), true).OfType<AssemblyConfigurationAttribute>().FirstOrDefault();
                return configAttribute.Configuration;
            }
            catch (Exception)
            {
                return null;
                // throw new InvalidOperationException(ex);
            }
        }
    }
}
