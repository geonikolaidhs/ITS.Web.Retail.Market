using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Represents a fiscal printer's "illegal" line to be printed
    /// </summary>
    public class FiscalLine
    {
        public ePrintType Type { get; set; }
        public string Value { get; set; }
    }
}
