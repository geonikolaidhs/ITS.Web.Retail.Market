using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


namespace ITS.Service.Guard
{
    static class Program
    {

        private static Form _mainForm { get; set; }
        static Mutex mutex = new Mutex(true, "{9F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE9F}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            Debugger.Launch();
#endif


            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                _mainForm = new ServiceGuard();
                Application.Run(_mainForm);
            }
        }


        public static Form GetMainForm()
        {
            return _mainForm;
        }

        public static void WriteToWindowsEventLog(string message, EventLogEntryType eventLogEntryType)
        {
            EventLog.WriteEntry("Service Guard", message, eventLogEntryType, 0);
        }
    }
}
