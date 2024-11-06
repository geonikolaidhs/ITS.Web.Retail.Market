using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public class ReplyPacket : Packet
    {
        protected StringField _ReplyCode;

        public virtual StringField ReplyCode
        {
            get
            {
                return _ReplyCode;
            }
        }
        
    }
}
