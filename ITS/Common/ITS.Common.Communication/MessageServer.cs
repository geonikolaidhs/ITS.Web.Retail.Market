using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ITS.Common.Communication
{
    public class MessageServer : MessageNode
    {
        public int ListeningPort { get; protected set;}

        public MessageServer(int listeningPort, Logger logger) : base(logger)
        {
            ListeningPort = listeningPort;
            if (listeningPort > 0)
            {
                IPAddress anyAdress = IPAddress.Any;
                Listener = new TcpListener(anyAdress, listeningPort);
                Listener.Start();
                Listener.BeginAcceptTcpClient(ListenCallback, null);
            }
        }
    }
}
