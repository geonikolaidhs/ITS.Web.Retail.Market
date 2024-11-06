using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Common.Communication
{
    //public interface IMessageListener
    //{
        
    //}
    public class MessageListenerEventArgs : EventArgs
    {
        public IMessage Message {get; protected set;}

        public MessageListenerEventArgs(IMessage message)
        {
            this.Message = message;
            //this.Sender = sender;
        }
    }


    public interface IMessageListener<out T> where T:IMessage
    {
        IMessage OnMessageReceived(MessageListenerEventArgs args);
    }
}
