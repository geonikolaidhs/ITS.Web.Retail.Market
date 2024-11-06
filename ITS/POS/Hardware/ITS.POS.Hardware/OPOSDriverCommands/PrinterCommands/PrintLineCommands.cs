using System;
using System.Collections.Generic;

namespace ITS.POS.Hardware.OPOSDriverCommands.PrinterCommands
{
    /// <summary>
    ///  Characteristics that are reset at the end of each print method or by a “Normal” command.
    /// </summary>
    public static class PrintLineCommands
    {

        /// <summary>
        /// Prints in bold or double-strike.
        /// </summary>
        /// <returns></returns>
        public static string Bold()
        {
            return "\x1B|bC";
        }

        /// <summary>
        /// Prints with underline. 
        /// </summary>
        /// <param name="thickness">The thickness of the underline in printer dot units. If ‘0’, then a printer-specific default thickness is used.</param>
        /// <returns></returns>
        public static string Underline(uint thickness=0)
        {
            string thicknessStr = "";
            if(thickness != 0)
            {
                thicknessStr=thickness.ToString();
            }
            return String.Format("\x1B|{0}uC", thicknessStr);
        }

        /// <summary>
        /// Aligns following text in the center.
        /// </summary>
        /// <returns></returns>
        public static string Center()
        {
            return "\x1B|cA";
        }

        /// <summary>
        /// Aligns following text at the right.
        /// </summary>
        /// <returns></returns>
        public static string RightJustify()
        {
            return "\x1B|rA";
        }

        /// <summary>
        /// Restores printer characteristics to normal condition.
        /// </summary>
        /// <returns></returns>
        public static string Normal()
        {
            return "\x1B|N";
        }
    }
}
