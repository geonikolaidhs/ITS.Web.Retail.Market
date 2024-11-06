using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;

namespace ITS.Common.Communication
{
    public class MessageClient : Messager
    {
        protected override NetPeer NetNode
        {
            get
            {
                return Client;
            }
        }
        protected NetClient Client { get; set; }
        protected String Host { get; set; }
        protected int Port { get; set; }
        

        public NetConnectionStatus ConnectionStatus
        {
            get
            {
                return Client.ConnectionStatus;
            }
        }

        public AutoResetEvent MessageReceivedEvent
        {
            get
            {
                return Client.MessageReceivedEvent;
            }
        }

        public MessageClient(String applicationIdentifier, String host, int port)
        {
            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }

            NetPeerConfiguration config = new NetPeerConfiguration(applicationIdentifier);
            config.AutoFlushSendQueue = true;            
            Host = host;
            Port = port;
            Client = new NetClient(config);
            Client.RegisterReceivedCallback(ListeningRunnable);
            
        }

        public void Connect()
        {
            Client.Start();
            NetOutgoingMessage hail = Client.CreateMessage("This is the hail message");
            NetConnection conn = Client.Connect(Host, Port, hail);
            Client.MessageReceivedEvent.WaitOne(2000);            
        }

        public void Shutdown()
        {
            Client.Disconnect("Requested by user");            
        }
        public void SendMessage(IMessage message)
        {
            //SendMessage(message, null);
            String messageString = MessageDecorator.DecorateMessage(message);
            NetOutgoingMessage om = Client.CreateMessage(messageString);
            Client.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
        }

        public T SendMessageAndWaitResponse<T>(IMessage message, int Timeout) where T : class, IMessage
        {
            //SendMessage(message, null);
            String messageString = MessageDecorator.DecorateMessage(message);
            NetOutgoingMessage om = Client.CreateMessage(messageString);
            Client.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
            NetIncomingMessage im;
            DateTime endDt = DateTime.Now.AddMilliseconds(Timeout);
            
            do
            {
                TimeSpan span = endDt - DateTime.Now;
                if (span.Milliseconds > 0)
                {
                    MessageReceivedEvent.WaitOne(span);
                }
                im = NetNode.ReadMessage();
            } while ((im == null || im.MessageType != NetIncomingMessageType.Data) && DateTime.Now.Ticks < endDt.Ticks);

            if (im == null)
            {
                return null;
            }

            String serializedMessage = im.ReadString();
            if (im.MessageType == NetIncomingMessageType.Data)
            {
                string messageTypeName = MessageDecorator.GetMessageTypeName(serializedMessage);
                string undecoratedMessage = MessageDecorator.UndecorateMessage(serializedMessage);
                if (messageTypeName != typeof(T).Name)  //Check if listener receives this message
                {
                    return null;
                }
                IMessage returnMessage = Activator.CreateInstance(typeof(T)) as IMessage;
                try
                {
                    returnMessage.Deserialize(undecoratedMessage);
                    return returnMessage as T;
                }
                catch
                {
                    throw;
                }
            }
            return null;
        }

        public override void SendMessage(IMessage message, NetConnection connection)
        {
            SendMessage(message);
        }

    }
}
