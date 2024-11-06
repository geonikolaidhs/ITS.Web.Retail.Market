using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public enum ASCIIControlCodes
    {
        /// <summary>
        /// Acknowledge (positive)
        /// </summary>
        ACK = 0x06,

        /// <summary>
        /// Not Acknowledge (negative)
        /// </summary>
        NAK=0x15,

        /// <summary>
        /// Start of text
        /// </summary>
        STX=0x02,

        /// <summary>
        /// End of text
        /// </summary>
        ETX=0x03,

        /// <summary>
        /// Cancel
        /// </summary>
        CAN=0x18,

        /// <summary>
        /// Enquire
        /// </summary>
        ENQ= 0x05
    }
}
