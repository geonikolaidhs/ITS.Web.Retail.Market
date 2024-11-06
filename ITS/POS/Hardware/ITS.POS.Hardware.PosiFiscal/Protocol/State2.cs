using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    /// <summary>
    /// 'Stat-2' is a single numeric field of 2 hexadecimal digits. 
    /// It's binary value maps to several ‘flags’, which inform the host application of the current application state of the device.
    /// </summary>
    public class State2 : Field
    {
        /// <summary>
        /// Bit 0: Open Receipt in progress
        /// </summary>
        public bool REC_OPEN { get; set; }

        /// <summary>
        /// Bit 1: Day Open. Transactions in progress
        /// </summary>
        public bool DAY_OPEN { get; set; }

        /// <summary>
        /// Bit 2: The receipt is in Payment Mode
        /// </summary>
        public bool IN_PAYMENT { get; set; }

        /// <summary>
        /// Bit 3: There is space for less than 50 records in the Fiscal Memory.
        /// </summary>
        public bool FM_WARN { get; set; }

        /// <summary>
        /// Bit 4: Fiscal Memory is full. Only Fiscal Reports can be issued.
        /// </summary>
        public bool FM_FULL { get; set; }

        /// <summary>
        /// Bit 5: The Flash Card’s capacity is low
        /// </summary>
        public bool FLASH_WARN { get; set; }

        /// <summary>
        /// Bit 6: The Flash Card is full, if an open receipt exists, it can be closed or cancelled.
        /// </summary>
        public bool FLASH_FULL { get; set; }

        /// <summary>
        /// Bit 7: Flash Card Error. Call Service
        /// </summary>
        public bool FLASH_NOT_WORK { get; set; }

        public State2()
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
        }

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
                string binary = (this.FLASH_NOT_WORK ? "1" : "0") + (this.FLASH_FULL ? "1" : "0") + (this.FLASH_WARN ? "1" : "0") + (this.FM_FULL ? "1" : "0") + (this.FM_WARN ? "1" : "0")
                    + (this.IN_PAYMENT ? "1" : "0") + (this.DAY_OPEN ? "1" : "0") + (this.REC_OPEN ? "1" : "0");

                return ProtocolHelper.BinaryStringToHexString(binary);
            }
            catch (Exception e)
            {
                return "Invalid Values: " + e.Message;
            }
        }

        protected void SetValue(string hexDigits)
        {
            try
            {
                string binary = ProtocolHelper.HexStringToBinaryString(hexDigits);
                this.FLASH_NOT_WORK = binary[0] == '1';
                this.FLASH_FULL = binary[1] == '1';
                this.FLASH_WARN = binary[2] == '1';
                this.FM_FULL = binary[3] == '1';
                this.FM_WARN = binary[4] == '1';
                this.IN_PAYMENT = binary[5] == '1';
                this.DAY_OPEN = binary[6] == '1';
                this.REC_OPEN = binary[7] == '1';
            }
            catch (Exception e)
            {
                throw new Exception("Invalid value: " + e.Message);
            }
        }

    }
}
