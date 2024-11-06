using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    /// <summary>
    /// 'Stat-1' is a single numeric field of 2 hexadecimal digits. 
    /// It's binary value maps to several device ‘flags’, which inform the host application of some hardware related events of the device.
    /// </summary>
    public class State1 : Field
    {
        /// <summary>
        /// Bit 0: Device busy
        ///This bit when set to '1' indicates that the device is currently busy executing a previous command or other task. When busy, the device may execute some non-critical commands and refuse to execute others replying an error 'Device busy - Unable to execute' (See error codes).
        ///The host must check this bit (requesting a 'status') before issuing any critical commands, or, must keep sending the command until the command is executed (or failed by other reason). BUSY state is a temporary state but, due to very different tasks the device may cause the BUSY state, the time which the BUSY flag will be found set is varying from a few milliseconds to few minutes. A host may inform the user after (for example) one minute, that the device is busy in other task and ask for a 'retry' or 'cancel' of the requested operation. An example in which a BUSY flag will be set for long time is a fiscal report issuing: When the host (or the device user) requests a fiscal report with many records, the report will take long time to finish, thus keeping the BUSY flag set for long. It is highly recommended though that a host should NOT produce a 'device busy' error message to the application user before (at least) ten (10) seconds. It is also recommended that the host application must allow the user to cancel or retry the operation.
        /// </summary>
        public bool Busy {get; set;}

        /// <summary>
        /// Bit 1: After CMOS 
        /// If this bit is 1 then the machine is in an after-zero state and The Flash Memory Data must be read. The Flash Memory Data are correct.
        /// </summary>
        public bool AfterCMOS { get; set; }

        /// <summary>
        /// Bit 2: Printer Paper End | Cover Open
        ///This bit indicates (when set to one) that the printer is out of paper and must be replaced before the previous task completed its printing duty. Usually, when this flag is set, the 'device busy' flag may be set also, if a previous command that used the printer caused the paper end error. So, it is recommended that the paper end bit MUST be checked before the busy bit. Host application
        ///may inform the user of the need to insert new role of paper to the printing mechanism. After doing so, this bit will be cleared and the command (that detected the paper end) may be retransmitted normally.
        /// </summary>
        public bool PPEND { get; set; }

        /// <summary>
        /// Bit 3: Printer Disconnect
        ///This bit indicates (when zero) that the printing device is not responding to printing commands. Recommended action is to power off the printer and on again and retry the command. If the problem persists, the device needs to be serviced.
        /// </summary>
        public bool PRNDisconnect { get; set; }

        /// <summary>
        /// Bit 4: Printer Timeout
        ///This bit indicates (when zero) that the printing device is not responding to printing commands. Recommended action is to power off the printer and on again and retry the command. If the problem persists, the device needs to be serviced.
        /// </summary>
        public bool PRNTimeout { get; set; }

        /// <summary>
        /// Bit 5: User operation in progress
        ///This bit indicates that the device is currently being used through its keyboard driven menu system by the operator. This causes a BUSY state, in which no command execution is allowed through the protocol mechanism until the operator finishes browsing the device. Any command transmitting will fail with error code 0x17 (see error codes). Applications should either tolerate this delay indefinitely or inform user about this situation after a while.
        /// </summary>
        public bool User { get; set; }

        /// <summary>
        /// Bit 6: Fiscal unit online
        ///This bit indicates (when zero) that a fiscal physical unit is not responding to commands. Because this is a critical error, bit 1 may be also set. Device may need to be serviced.
        /// </summary>
        public bool FCONN { get; set; }

        /// <summary>
        /// Bit 7: Battery Good
        ///This bit indicates (when set) that the board's battery is in good condition. If this bit is zero, commands may return error.
        /// </summary>
        public bool BATOK { get; set; }


        public override string Value
        {
            get
            {
                return CalculateValue();
            }
            set
            {
                SetValue(value);
            }
        }

        protected string CalculateValue()
        {
            try
            {
                string binary = (this.BATOK ? "1" : "0") + (this.FCONN ? "1" : "0") + (this.User ? "1" : "0") + (this.PRNTimeout ? "1" : "0") + (this.PRNDisconnect ? "1" : "0")
                    + (this.PPEND ? "1" : "0") + (this.AfterCMOS ? "1" : "0") + (this.Busy ? "1" : "0");

                return ProtocolHelper.BinaryStringToHexString(binary);
            }
            catch(Exception e)
            {
                return "Invalid Values: " + e.Message;
            }
        }

        protected void SetValue(string hexDigits)
        {
            try
            {
                string binary = ProtocolHelper.HexStringToBinaryString(hexDigits);
                this.BATOK = binary[0] == '1';
                this.FCONN = binary[1] == '1';
                this.User = binary[2] == '1';
                this.PRNTimeout = binary[3] == '1';
                this.PRNDisconnect = binary[4] == '1';
                this.PPEND = binary[5] == '1';
                this.AfterCMOS = binary[6] == '1';
                this.Busy = binary[7] == '1';
            }
            catch(Exception e)
            {
                throw new Exception("Invalid value: "+e.Message);
            }
        }
    }
}
