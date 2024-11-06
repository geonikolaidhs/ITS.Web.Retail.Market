using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Common.Communication
{
    public class MessageClient : MessageNode
    {
        public string ServerIP { get; protected set; }
        public int ServerPort { get; protected set; }


        public MessageClient(string serverIP, int serverPort, Logger logger)
            : base(logger)
        {
            this.ServerIP = serverIP;
            this.ServerPort = serverPort;
        }

        public T SendMessageAndWaitResponse<T>(IMessage message, int timeout) where T : class,IMessage
        {
            try
            {
                TcpClient client = SendMessage(message, this.ServerIP, this.ServerPort, false);
                NetworkStream stream = client.GetStream();
                List<byte> allBytes = ReadFromStream(stream, timeout);
                if (allBytes.Count > 0)
                {
                    string decoratedMessage = Utilities.GetString(allBytes.ToArray());
                    string messageTypeName = MessageDecorator.GetMessageTypeName(decoratedMessage);
                    string serializedMessage = MessageDecorator.UndecorateMessage(decoratedMessage);
                    IMessage response = Activator.CreateInstance(typeof(T)) as IMessage;
                    response.Deserialize(serializedMessage);
                    return response as T;
                }
                else
                {
                    string errorMessage = "SendMessageAndWaitResponse, response timeout (" + timeout + " ms) reached. MessageType: '" + message.GetType().Name + "'";
                    if (Logger != null)
                    {
                        Logger.Error(errorMessage);
                    }
                    IMessage response = Activator.CreateInstance(typeof(T)) as IMessage;
                    response.ErrorMessage = errorMessage;
                    PropertyInfo resultProperty = response.GetType().GetProperty("Result");
                    if ( resultProperty !=null && resultProperty.CanWrite )
                    {
                        resultProperty.SetValue(response, eFiscalResponseType.FAILURE,null);
                    }
                    return response as T;//return null;
                }
            }
            catch (Exception exception)
            {
                if (Logger != null)
                {
                    Logger.Error(exception, "Error Sending Message " + message.GetType().Name);                    
                }
                IMessage response = Activator.CreateInstance(typeof(T)) as IMessage;
                string errorMessage = exception.Message;
                if (exception.InnerException != null && String.IsNullOrWhiteSpace(exception.InnerException.Message) == false)
                {
                    errorMessage += Environment.NewLine + exception.InnerException.Message;
                }
                if (String.IsNullOrWhiteSpace(exception.StackTrace) == false)
                {
                    errorMessage += Environment.NewLine + exception.StackTrace;
                }
                response.ErrorMessage = errorMessage;
                PropertyInfo resultProperty = response.GetType().GetProperty("Result");
                if (resultProperty != null && resultProperty.CanWrite)
                {
                    resultProperty.SetValue(response, eFiscalResponseType.FAILURE, null);
                }
                return response as T;//return null;
            }
        }
    }
}
