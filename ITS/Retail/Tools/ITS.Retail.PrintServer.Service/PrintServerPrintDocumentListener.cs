using DevExpress.Pdf;
using DevExpress.XtraPdfViewer;
using ITS.Common.Communication;
using ITS.Retail.PrintServer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ITS.Retail.PrintServer.Service
{
    public class PrintServerPrintDocumentListener : IMessageListener<PrintServerPrintDocumentRequest>
    {

        PrintServer server;
        
        public PrintServerPrintDocumentListener(PrintServer server)
        {
            this.server = server;            
        }

        public IMessage OnMessageReceived(MessageListenerEventArgs args)
        {
            try
            {
                PrintServerPrintDocumentRequest receivedMessage = args.Message as PrintServerPrintDocumentRequest;
                string printerName = server.Settings.DefaultPrinter;
                if ( String.IsNullOrWhiteSpace(receivedMessage.PrinterNickName) == false )
                {
                    PrinterInfo printerInfo = server.Settings.Printers.FirstOrDefault(printer => printer.NickName == receivedMessage.PrinterNickName);
                    if( printerInfo == null)
                    {
                        throw new Exception(String.Format("Could not find printer with NickName {0}",receivedMessage.PrinterNickName));
                    }
                    printerName = printerInfo.Name;
                }
                PrinterPOSAssociation association = server.Settings.PrinterPOSAssociations.FirstOrDefault(assoc => assoc.POSID == receivedMessage.PosID);
                if (association != null)
                {
                    printerName = association.Printer;
                }

                //Step 1 Get 
                string url = string.Format("{0}/Document/AnonymousPrint?DOid={1}&directPrint=true&userid={2}", server.Settings.StoreControllerURL.TrimEnd('/'), receivedMessage.DocumentID, receivedMessage.UserID);
                using (WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(new Uri(url));
                    using (MemoryStream memstream = new MemoryStream())
                    {
                        memstream.Write(data, 0, data.Length);
                        using (PdfViewer pdfViewer = new PdfViewer())
                        {
                            PdfPrinterSettings printSettings = new PdfPrinterSettings() { PrintingDpi = 150 };
                            pdfViewer.LoadDocument(memstream);
                            printSettings.Settings.PrinterName = printerName;
                            pdfViewer.Print(printSettings);
                            pdfViewer.CloseDocument();
                        }
                    }
                }
                return new PrintServerPrintDocumentResponse() { Result = ePrintServerResponseType.SUCCESS, ErrorMessage = String.Empty };
            }
            catch (Exception exception)
            {
                string errorMessage = exception.Message;
                if (exception.InnerException != null && String.IsNullOrWhiteSpace(exception.InnerException.Message) == false)
                {
                    errorMessage += Environment.NewLine + exception.InnerException.Message;
                }
                if (String.IsNullOrWhiteSpace(exception.StackTrace) == false)
                {
                    errorMessage += exception.StackTrace;
                }
                return new PrintServerPrintDocumentResponse() { Result = ePrintServerResponseType.FAILURE, ErrorCode = 10, Explanation = exception.Message , ErrorMessage = errorMessage }; 
            }
        }
    }
}
