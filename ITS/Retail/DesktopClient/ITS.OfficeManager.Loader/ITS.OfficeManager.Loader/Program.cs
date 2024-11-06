using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ITS.OfficeManager.Loader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Ionic.Zip,"))
            {
                Assembly execAssembly = Assembly.GetExecutingAssembly();
                Stream stream = execAssembly.GetManifestResourceStream("ITS.OfficeManager.Loader.Ionic.Zip.dll");
                byte[] block = new byte[stream.Length];
                stream.Read(block, 0, block.Length);
                Assembly loadedAssembly = Assembly.Load(block);
                return loadedAssembly;
            }
            else if (args.Name.Contains("ITS.Retail.Platform.Enumerations"))
            {
                Assembly execAssembly = Assembly.GetExecutingAssembly();
                string manifestName = execAssembly.GetManifestResourceNames().Where(s => s.EndsWith("ITS.Retail.Platform.Enumerations.dll")).FirstOrDefault();
                Stream stream = execAssembly.GetManifestResourceStream(manifestName);
                byte[] block = new byte[stream.Length];
                stream.Read(block, 0, block.Length);
                Assembly loadedAssembly = Assembly.Load(block);
                return loadedAssembly;
            }
            return null;
        }
    }
}
