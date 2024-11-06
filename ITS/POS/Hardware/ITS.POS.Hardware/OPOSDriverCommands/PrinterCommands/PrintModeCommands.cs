using System;
using System.Collections.Generic;

namespace ITS.POS.Hardware.OPOSDriverCommands.PrinterCommands
{
    /// <summary>
    /// Characteristics that are remembered until explicitly changed.
    /// </summary>
    public static class PrintModeCommands
    {
        /// <summary>
        /// Selects a new typeface for the following data.
        /// </summary>
        /// <param name="typeface">0 = Default typeface.1 = Select first typeface from the FontTypefaceList property.2 = Select second typeface from the FontTypefaceList property.And so on.</param>
        /// <returns></returns>
        public static string FontTypefaceSelection(int typeface=0)
        {
            return String.Format("\x1B|{0}fT", typeface);
        }
    }
}
