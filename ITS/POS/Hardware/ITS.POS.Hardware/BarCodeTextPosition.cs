using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware
{
    public enum BarCodeTextPosition
    {
        /// <summary>
        /// Indicates to print the text below the bar code.
        /// </summary>
        Below = -13,
        /// <summary>
        /// Indicates to print the text above the bar code.
        /// </summary>
        Above = -12,
        /// <summary>
        /// Indicates that no text is printed. Only prints the bar code.
        /// </summary>
        None = -11,
    }
}
