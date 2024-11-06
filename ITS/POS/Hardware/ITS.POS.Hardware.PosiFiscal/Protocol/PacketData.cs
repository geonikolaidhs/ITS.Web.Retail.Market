using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public abstract class PacketData
    {
        public const string FieldSeperator = "/";
        public List<Field> Fields { get; set; }
        public Checksum CheckSum { get; set; }

        public virtual string BuildPacketData()
        {
            //Implemented by subclasses
            return "";
        }
    }
}
