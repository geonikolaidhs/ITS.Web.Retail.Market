using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    /// <summary>
    /// Specifies a date. Date format is DDMMYYYY.
    /// </summary>
    public class Date8Field : Field
    {
        public Date8Field()
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
            this._MaxValue = 31122040;
            this._MinValue = 01011999;
            this._MinDigits = 8;
            this._MaxDigits = 8;
        }

    }
}
