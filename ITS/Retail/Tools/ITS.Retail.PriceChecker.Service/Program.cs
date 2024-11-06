using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ITS.Retail.PriceChecker.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            string tt = Settings.getInstance().StoreControllerURL;


            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ITSPriceCheckerService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        public static void WriteToWindowsEventLog(string message, EventLogEntryType eventLogEntryType)
        {
            EventLog.WriteEntry("PriceChecker Service", message, eventLogEntryType, 0);
        }
    }
}
