using ITS.Common.Communication;
using ITS.Retail.PrintServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Service
{
    public class PrintServerGetPrintersListener : IMessageListener<PrintServerGetPrintersRequest>
    {
        PrintServer printServer;

        public PrintServerGetPrintersListener(PrintServer server)
        {
            this.printServer = server;
        }

        public IMessage OnMessageReceived(MessageListenerEventArgs messageListenerEventArgs)
        {
            try
            {
                return new PrintServerGetPrintersResponse()
                {
                    ErrorCode = 0,
                    Explanation = string.Empty,
                    Result = ePrintServerResponseType.SUCCESS,
                    Printers = printServer.Settings.Printers.Select(printer => printer.NickName).ToList(),
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
                return new PrintServerGetPrintersResponse()
                {
                    ErrorCode = 10,
                    Explanation = exception.Message,
                    Result = ePrintServerResponseType.FAILURE,
                    Printers = new List<string>(),
                    ErrorMessage = errorMessage
                };
            }
        }
    }
}
