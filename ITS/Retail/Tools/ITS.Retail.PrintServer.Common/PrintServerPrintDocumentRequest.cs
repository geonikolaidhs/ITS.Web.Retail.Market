using System;

namespace ITS.Retail.PrintServer.Common
{
    public class PrintServerPrintDocumentRequest : PrintServerRequest
    {
        /// <summary>
        /// Gets or sets the OID of the Document that will be printed
        /// </summary>
        public Guid DocumentID { get; set; }

        /// <summary>
        /// Gets or sets the OID of the User that initates the print command
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the POS that initates the print command
        /// </summary>
        public int PosID { get; set; }

        /// <summary>
        /// Gets or sets the Printer NickName that the print command will be send
        /// </summary>
        public string PrinterNickName { get; set; }

        public PrintServerPrintDocumentRequest()
        {
            Command = ePrintServerRequestType.PRINT_DOCUMENT;
        }

        public PrintServerPrintDocumentRequest(Guid documentID, Guid userID, int posID, string printerNickName = "")
        {
            Command = ePrintServerRequestType.PRINT_DOCUMENT;
            DocumentID = documentID;
            UserID = userID;
            PosID = posID;
            PrinterNickName = printerNickName;
        }
    }
}
