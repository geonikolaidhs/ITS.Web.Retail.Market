using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Class for encapsulating the arguments for a ReadEventHandler event
    /// </summary>
    public class ScannerReadEventArgs : EventArgs
    {
        public string Data { get; set; }
        public BarCodeSymbology  BarcodeSymbology { get ; set ; }
        public Scanner Sender { get; set; }

        public ScannerReadEventArgs(string data, BarCodeSymbology symbology)
        {
            Data = data;
            BarcodeSymbology = symbology;
        }
    }
}
