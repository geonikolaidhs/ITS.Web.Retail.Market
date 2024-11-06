using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


namespace ITS.Process.Guard
{
    static class Program
    {

        private static String[] _arguments { get; set; }

        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE9F}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {


            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                if (args.Length > 0)
                {
                    _arguments = args;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ProcessGuard());
            }
        }

        public static String[] GetArguments()
        {
            return _arguments;
        }

        public static void WriteToWindowsEventLog(string message, EventLogEntryType eventLogEntryType)
        {
            EventLog.WriteEntry("Service Guard", message, eventLogEntryType, 0);
        }
    }
}
