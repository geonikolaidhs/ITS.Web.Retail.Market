using ITS.Common.Communication;
using ITS.Retail.PrintServer.Common;
using System;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Service
{
    public class PrintServerPrintLabelListener : IMessageListener<PrintServerPrintLabelRequest>
    {
        PrintServer printServer;
        public PrintServerPrintLabelListener(PrintServer server)
        {
            this.printServer = server;
        }

        public IMessage OnMessageReceived(MessageListenerEventArgs args)
        {
            try
            {
                PrintServerPrintLabelRequest printServerPrintLabelRequest = args.Message as PrintServerPrintLabelRequest;
                string printerName = printServer.Settings.DefaultPrinter;
                if ( !String.IsNullOrWhiteSpace(printServerPrintLabelRequest.PrinterNickName))
                {
                    PrinterInfo printerInfo = printServer.Settings.Printers.FirstOrDefault(printer => printer.NickName == printServerPrintLabelRequest.PrinterNickName);
                    if (printerInfo == null)
                    {
                        throw new Exception(String.Format("Could not find printer with NickName {0}", printServerPrintLabelRequest.PrinterNickName));
                    }
                    printerName = printerInfo.Name;
                }
                
                byte[] labelTextBytes = Encoding.GetEncoding(printServerPrintLabelRequest.Encoding).GetBytes(printServerPrintLabelRequest.LabelText);
                PrinterServiceHelper.SendByteArrayToPrinter(printerName, labelTextBytes);

                return new PrintServerPrintLabelResponse()
                {
                    ErrorCode = 0,
                    Explanation = string.Empty,
                    Result = ePrintServerResponseType.SUCCESS,
                    ErrorMessage = String.Empty
                };
            }
            catch(Exception exception)
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
                return new PrintServerPrintLabelResponse()
                {
                    ErrorCode = 10,
                    Explanation = exception.Message,
                    Result = ePrintServerResponseType.FAILURE,
                    ErrorMessage = errorMessage
                };
            }
        }
    }
}
