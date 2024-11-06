using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware
{
    public class MsrReadEventArgs : EventArgs
    {
        public string Track1Data { get; set; }
        public string Track2Data { get; set; }
        public string Track3Data { get; set; }
        public string Track4Data { get; set; }

        public MsrReadEventArgs()
        {
            Track1Data = "";
            Track2Data = "";
            Track3Data = "";
            Track4Data = "";
        }
    }
}
