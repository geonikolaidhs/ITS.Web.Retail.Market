using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public abstract class RequestPacket : Packet
    {
        private StringField _RequestCode; //TODO

        public virtual StringField RequestCode
        {
            get
            {
                return _RequestCode;
            }
        }

    }
}
