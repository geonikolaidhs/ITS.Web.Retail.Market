using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace POSLoader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///         
        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [STAThread]
        static void Main()
        {
//#if DEBUG
            //Debugger.Launch();
//#endif

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Check if ITS.POS.Client.exe is already running
            Process p = null;
            if (ShouldLoaderStart(out p))
            {
                Application.Run(new MainForm());
            }
            else
            {
                MessageBox.Show("Application is already open. Switching to existing process");
                try
                {
                    SetForegroundWindow(p.MainWindowHandle);
                    ShowWindow(p.MainWindowHandle, 1);
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                    MessageBox.Show("Switch to application failed. Please try again in a few moments.");
                }
            }


        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Ionic.Zip,"))
            {
                Assembly a1 = Assembly.GetExecutingAssembly();
                Stream s = a1.GetManifestResourceStream("ITS.POS.Client.Loader.Ionic.Zip.dll");
                byte[] block = new byte[s.Length];
                s.Read(block, 0, block.Length);
                Assembly a2 = Assembly.Load(block);
                return a2;
            }
            return null;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Exception ex = e.ExceptionObject as Exception;
            string message = ex.Message + "\n" + ex.StackTrace + (ex.InnerException != null ? ex.InnerException.Message + "\n" + ex.InnerException.StackTrace : "");
            File.WriteAllText(Application.StartupPath + "\\Unhandled Exception.txt", message);
        }

        static bool ShouldLoaderStart(out Process p)
        {
            String name = "ITS.POS.Client";
            p = Process.GetProcesses().FirstOrDefault(g => g.ProcessName == name);
            return (p == null);
        }
    }
}
