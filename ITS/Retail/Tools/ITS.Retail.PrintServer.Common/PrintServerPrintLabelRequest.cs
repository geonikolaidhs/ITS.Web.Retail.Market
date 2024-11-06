using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public class PrintServerPrintLabelRequest : PrintServerRequest
    {

        /// <summary>
        /// Gets or sets the Printer NickName that the print command will be send
        /// </summary>
        public string PrinterNickName { get; set; }

        public string LabelText { get; set; }

        public int Encoding { get; set; }

        public PrintServerPrintLabelRequest()
        {
            Command = ePrintServerRequestType.PRINT_LABEL;
        }

    }
}
