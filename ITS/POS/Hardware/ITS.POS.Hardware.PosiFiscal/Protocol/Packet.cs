using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public abstract class Packet
    {
        public const char PacketHeader = (char)ASCIIControlCodes.STX;
        public PacketData PacketData { get; set; }
        public const char PacketFooter = (char)ASCIIControlCodes.ETX;

        public string BuildPacket()
        {
            return PacketHeader + PacketData.BuildPacketData() + PacketFooter;
        }

        public bool ParcePacket(string packet) // or byte[] packet
        {
            //TODO
            string packetData = packet.Substring(1, packet.Length - 2);

            return false;
        }
    }
}
