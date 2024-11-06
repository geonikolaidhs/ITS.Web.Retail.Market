using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public class RequestPacketData : PacketData
    {
        private StringField _RequestCode; // TODO

        public virtual StringField RequestCode
        {
            get
            {
                return _RequestCode;
            }
        }

        public override string BuildPacketData()
        {
            string fieldsString = "";
            foreach (Field field in this.Fields)
            {
                fieldsString += field.Value + PacketData.FieldSeperator;
            }
            return RequestCode.Value + PacketData.FieldSeperator + PacketData.FieldSeperator + fieldsString + this.CheckSum.Value;
        }
    }
}
