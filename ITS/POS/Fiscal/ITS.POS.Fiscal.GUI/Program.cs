using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ITS.POS.Fiscal.GUI
{
    static class Program
    {
        public static Logger Logger { get; set; }
        static Mutex mutex = new Mutex(true, "{5DB92E90-57E2-4C1A-9497-77774BE6DB50}");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
#if DEBUG
                Debugger.Launch();
#endif

                Logger = LogManager.GetLogger("DiSign Configuration");
                try
                {
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new CoffeeSettingsForm());
                }
                catch (Exception ex)
                {
                    string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                    MessageBox.Show(message);
                    Logger.Error(ex, "General Exception");
                }
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled Exception");
            Exception ex = e.ExceptionObject as Exception;
            string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            MessageBox.Show(message);
            Logger.Error(ex, "Unhandled Exception");
        }
    }
}
