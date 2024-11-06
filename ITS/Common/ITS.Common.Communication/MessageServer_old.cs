using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;

namespace ITS.Common.Communication
{

    public abstract class Messager
    {
        protected abstract NetPeer NetNode
        {
            get;
        }

        public Messager()
        {
            listeners = new List<object>();
        }

        protected List<object> listeners;
        //CustomThread listeningThread;

        public void AddListener<T>(IMessageListener<T> listener) where T : IMessage
        {
            listeners.Add(listener);
        }
        //Thread listeningThread;

        public void RemoveListener<T>(IMessageListener<T> listener) where T : IMessage
        {
            listeners.Remove(listener);
        }

        public virtual void SendMessage(IMessage message, NetConnection connection)
        {
            String messageString = MessageDecorator.DecorateMessage(message);
            NetOutgoingMessage om = NetNode.CreateMessage(messageString);
            NetNode.SendMessage(om, connection, NetDeliveryMethod.ReliableUnordered);
        }

        protected void ListeningRunnable(object state)
        {
            NetIncomingMessage im;
            while ((im = NetNode.ReadMessage()) != null)
            {
                String serializedMessage = im.ReadString();
#if DEBUG
                Console.WriteLine("Received Message :" + serializedMessage);
#endif
                if (im.MessageType == NetIncomingMessageType.Data)
                {
                    string messageTypeName = MessageDecorator.GetMessageTypeName(serializedMessage);
                    string undecoratedMessage = MessageDecorator.UndecorateMessage(serializedMessage);

                    foreach (object messageListener in listeners)
                    {
                        Type listenerMessageType = messageListener.GetType().GetInterfaces().Where(x => x.Name == "IMessageListener`1").First().GetGenericArguments()[0];
                        if (messageTypeName != listenerMessageType.Name)  //Check if listener receives this message
                        {
                            continue;
                        }

                        IMessage message = Activator.CreateInstance(listenerMessageType) as IMessage;
                        try
                        {
                            message.Deserialize(undecoratedMessage);
                            MessageListenerEventArgs messageListenerEventArgs = new MessageListenerEventArgs(message);
                            IMessage response = (messageListener as IMessageListener<IMessage>).OnMessageReceived(messageListenerEventArgs);
                            if (response != null)
                            {
                                SendMessage(response, im.SenderConnection);
                            }
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
                Thread.Sleep(2);
            }
        }
        public NetPeerStatus NetNodeStatus
        {
            get
            {
                return NetNode.Status;
            }
        }


    }

    public class MessageServer : Messager
    {
        protected NetPeerConfiguration Settings { get; set; }
        protected NetServer Server { get; set; }

        protected override NetPeer NetNode
        {
            get
            {
                return Server;
            }
        }

        public int Port
        {
            get
            {
                return Server.Port;
            }
        }



        public MessageServer(String applicationIdentifier, int port, int maximumConnection = 100)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(applicationIdentifier);
            config.MaximumConnections = maximumConnection;
            config.Port = port;
           
            Server = new NetServer(config);
            Server.RegisterReceivedCallback(ListeningRunnable);

        }

        public void StartServer()
        {
            if (Server.Status == NetPeerStatus.NotRunning)
            {
                Server.Start();
            }
        }


        public void StopServer(String reason)
        {
            if (Server.Status == NetPeerStatus.Running)
            {
                Server.Shutdown(reason);

            }
        }


    }
}
