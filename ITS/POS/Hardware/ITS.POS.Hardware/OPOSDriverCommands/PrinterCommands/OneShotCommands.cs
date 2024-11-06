using System;
using System.Collections.Generic;

namespace ITS.POS.Hardware.OPOSDriverCommands.PrinterCommands
{
    /// <summary>
    /// Perform indicated action.
    /// </summary>
    public static class OneShotCommands
    {
        /// <summary>
        /// Cuts receipt paper.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static string PaperCut(uint percentage = 100)
        {
            string perc = "";
            if (percentage != 100)
            {
                perc = percentage.ToString();
            }

            return String.Format("\x1B|{0}P", perc);
        }

        /// <summary>
        /// Cuts receipt paper, after feeding the paper the number of lines specified by the RecLinesToPaperCut property.
        /// </summary>
        /// <returns></returns>
        public static string FeedAndPaperCut(uint percentage = 100)
        {
            string perc = "";
            if (percentage != 100)
            {
                perc = percentage.ToString();
            }
            return String.Format("\x1B|{0}fP", perc);
        }

        /// <summary>
        /// Prints the pre-stored top logo.
        /// </summary>
        /// <returns></returns>
        public static string PrintTopLogo()
        {
            return "\x1B|tL";
        }

        /// <summary>
        /// Prints the pre-stored bottom logo.
        /// </summary>
        /// <returns></returns>
        public static string PrintBottomLogo()
        {
            return "\x1B|bL";
        }

        /// <summary>
        /// Feed the paper forward by the specified lines.
        /// </summary>
        /// <param name="numberOfLines"></param>
        /// <returns></returns>
        public static string FeedLines(int numberOfLines = 1)
        {
            if (numberOfLines == 1)
            {
                return "\x1B|lF";
            }
            else
            {
                return String.Format("\x1B|{0}lF", numberOfLines);
            }
        }

        public static string FeedReverse(int numberOfLines = 1)
        {
            if (numberOfLines == 1)
            {
                return "\x1B|rF";
            }
            else
            {
                return String.Format("\x1B|{0}rF", numberOfLines);
            }
        }
    }
    
}
