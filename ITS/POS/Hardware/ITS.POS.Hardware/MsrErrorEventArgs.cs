using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware
{
    public class MsrErrorEventArgs : EventArgs
    {
        public string Error { get; set; }
    }
}
