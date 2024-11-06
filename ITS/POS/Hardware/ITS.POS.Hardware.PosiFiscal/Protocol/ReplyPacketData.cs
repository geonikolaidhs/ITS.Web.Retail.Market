using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public class ReplyPacketData : PacketData
    {

        /// <summary>
        /// 'Stat-1' is a single numeric field of 2 hexadecimal digits. 
        /// It's binary value maps to several device ‘flags’, which inform the host application of some hardware related events of the device.
        /// </summary>
        State1 State1 { get; set; }

        /// <summary>
        /// 'Stat-2' is a single numeric field of 2 hexadecimal digits. 
        /// It's binary value maps to several ‘flags’, which inform the host application of the current application state of the device.
        /// </summary>
        State2 State2 { get; set; }

        private StringField _ReplyCode; //TODO

        public virtual StringField ReplyCode
        {
            get
            {
                return _ReplyCode;
            }
        }

        public override string BuildPacketData()
        {
            string fieldsString = "";
            foreach(Field field in this.Fields)
            {
                fieldsString +=   field.Value + PacketData.FieldSeperator;
            }
            return ReplyCode.Value + PacketData.FieldSeperator + this.State1.Value + PacketData.FieldSeperator + this.State2.Value + PacketData.FieldSeperator + fieldsString + this.CheckSum.Value;
        }

    }
}
