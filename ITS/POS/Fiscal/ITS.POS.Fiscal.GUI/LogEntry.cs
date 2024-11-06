using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.GUI
{
    public class LogEntry
    {
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }
    }
}
