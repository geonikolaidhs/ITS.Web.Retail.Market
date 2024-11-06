using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public class RateField : Field
    {
        /// <summary>
        /// RATE is used to specify currencies of foreign notes or euro to drachmas rate and vice versa
        /// </summary>
        public RateField()
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
            this._MaxDigits = 11;
            this._MinDigits = 1;
            this._MaxValue = 9999.999999;
            this._MinValue = 0.000000;
        }

    }
}
