using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ITS.Common.Communication
{
    public abstract class MessageNode : IDisposable
    {
        // private TcpListener listener;
        private List<object> messageListeners;
        public TcpListener Listener { get; protected set; }
        public Logger Logger { get; set; }

        public MessageNode(Logger logger)
        {
            messageListeners = new List<object>();
            this.Logger = logger;
        }

        protected List<byte> ReadFromStream(NetworkStream stream, int timeout)
        {
            List<byte> allBytes = new List<byte>();

            if (timeout > 0)
            {
                stream.ReadTimeout = timeout;
            }

            int read = 0;
            byte[] buffer = new byte[sizeof(char)];
            char ch = Constants.EndOfMessageChar;
            do
            {
                read = stream.Read(buffer, 0, buffer.Length);
                ch = (char)buffer[0];
                allBytes.AddRange(buffer.Take(read));
            }
            while (stream.DataAvailable && ch != Constants.EndOfMessageChar);
            return allBytes;
        }

        protected void ListenCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient client = Listener.EndAcceptTcpClient(ar);
                try
                {
                    Listener.BeginAcceptTcpClient(ListenCallback, null);
                    NetworkStream stream = client.GetStream();
                    List<byte> allBytes = ReadFromStream(stream, -1);

                    string decoratedMessage = Utilities.GetString(allBytes.ToArray());
                    string messageTypeName = MessageDecorator.GetMessageTypeName(decoratedMessage);
                    string serializedMessage = MessageDecorator.UndecorateMessage(decoratedMessage);

                    foreach (object messageListener in messageListeners)
                    {
                        Type listenerMessageType = messageListener.GetType().GetInterfaces().Where(x => x.Name == "IMessageListener`1").First().GetGenericArguments()[0];
                        if (messageTypeName != listenerMessageType.Name)  //Check if listener receives this message
                        {
                            continue;
                        }

                        IMessage message = Activator.CreateInstance(listenerMessageType) as IMessage;
                        try
                        {
                            message.Deserialize(serializedMessage);
                            MessageListenerEventArgs messageReadEventArgs = new MessageListenerEventArgs(message);
                            IMessage response = (messageListener as IMessageListener<IMessage>).OnMessageReceived(messageReadEventArgs);
                            if (response != null)
                            {
                                string serializedResponse = MessageDecorator.DecorateMessage(response);
                                Byte[] data = Utilities.GetBytes(serializedResponse);
                                stream.BeginWrite(data, 0, data.Length, WriteCallback, client);
                            }
                            else
                            {
                                client.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Logger != null)
                            {

                                Logger.Error(ex, "Error sending response");
                            }
                            if (client != null)
                            {
                                client.Close();

                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (client != null)
                    {
                        client.Close();
                        if (Logger != null)
                        {
                            Logger.Error(ex, "Listen callback error");
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.Error(ex, "Listen callback error");
                }
            }
        }

        public void AddListener<T>(IMessageListener<T> listener) where T : IMessage
        {
            messageListeners.Add(listener);
        }

        public void RemoveListener<T>(IMessageListener<T> listener) where T : IMessage
        {
            messageListeners.Remove(listener);
        }

        protected TcpClient SendMessage(IMessage message, string recipientIP, int recipientPort, bool async)
        {
            TcpClient client = new TcpClient();
            try
            {
                string serializedData = MessageDecorator.DecorateMessage(message);
                client = new TcpClient(recipientIP, recipientPort);

                Byte[] data = Utilities.GetBytes(serializedData);
                NetworkStream stream = client.GetStream();
                if (async)
                {
                    stream.BeginWrite(data, 0, data.Length, WriteCallback, client);
                }
                else
                {
                    stream.Write(data, 0, data.Length);
                }
                return client;
            }    
            catch (SocketException ex)
            {
                if (Logger != null)
                {
                    Logger.Error(ex, "SendMessage Error Could Not Find Disign Service");
                }
                return client;
            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.Error(ex, "SendMessage Error Could Not Find Disign Service");                   
                }
                return client;
            }
            return client;
        }

        protected void WriteCallback(IAsyncResult result)
        {
            try
            {
                TcpClient client = result.AsyncState as TcpClient;
                NetworkStream stream;
                try
                {
                    stream = client.GetStream();
                    try
                    {

                        stream.EndWrite(result);
                        stream.Close();
                        client.Close();
                    }
                    catch (ObjectDisposedException ex)
                    {
                        if (Logger != null)
                        {
                            Logger.Error(ex, "WriteCallback Error");
                        }
                        if (client != null)
                        {
                            client.Close();

                        }

                        if (stream != null)
                        {
                            stream.Close();
                        }

                    }
                    catch (Exception ex)
                    {
                        if (Logger != null)
                        {
                            Logger.Error(ex, "WriteCallback Error");
                        }
                        stream.Close();
                        client.Close();
                    }

                }
                catch (Exception ex)
                {
                    client.Close();
                    if (Logger != null)
                    {
                        Logger.Error(ex, "WriteCallback Error");
                    }
                }
            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.Error(ex, "WriteCallback Error");
                }
            }
        }

        private volatile bool disposed;

        public void Dispose()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
