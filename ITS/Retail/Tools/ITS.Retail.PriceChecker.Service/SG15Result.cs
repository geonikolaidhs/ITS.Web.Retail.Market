using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.Retail.PriceChecker.Service
{
    public class SG15Result
    {
        public string GeneralInfo { get; set; }
        public string GeneralResult { get; set; }
        public string MainInfo { get; set; }
        public string MainResult { get; set; }
        public string error { get; set; }

        public String PrepareSG15Text()
        {
            try
            {
                StringBuilder stb = new StringBuilder();

                if (string.IsNullOrEmpty(error))
                {
                    stb.AppendLine();
                    stb.AppendLine("<ESC>%<ESC>");
                    stb.AppendLine("<ESC><S><L>" + GeneralInfo + "<T>");
                    stb.AppendLine("<ESC><S><K>" + GeneralResult + "<T>");
                    stb.AppendLine("<ESC><S><M>" + MainInfo + ": " + MainResult + "<T>");
                }
                else
                {
                    stb.AppendLine();
                    stb.AppendLine("<ESC>%<ESC>");
                    stb.AppendLine("<ESC><S><K>" + error + "<T>");
                }

                string st1 = stb.ToString().Replace("<ESC>", "\x1B");
                string st2 = st1.Replace("<S>", "\x2e");
                string st3 = st2.Replace("<L>", "\x31");
                string st4 = st3.Replace("<T>", "\x03");
                string st5 = st4.Replace("<K>", "\x3a");
                string st6 = st5.Replace("<M>", "\x37");
                return st6;
            }
            catch (Exception e) { Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error); return ""; }

        }

    }
}
