using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Class for encapsulating the arguments for an ErrorEventHandler event
    /// </summary>
    public class ScannerErrorEventArgs : EventArgs
    {
        public string Error { get; set; }
    }
}
