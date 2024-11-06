using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Helpers
{
    public class DailyTotalDetailMachine
    {
        public int receips { get; set; }
        public decimal amount { get; set; }
        public eDailyRecordTypes dailyType { get; set;}
    }
}
