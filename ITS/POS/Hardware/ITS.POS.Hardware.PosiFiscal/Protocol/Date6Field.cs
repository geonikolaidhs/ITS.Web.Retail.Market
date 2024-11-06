using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    /// <summary>
    /// Specifies a date. Date format is DDMMYY.
    /// </summary>
    public class Date6Field : Field
    {
        public Date6Field()
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
            this._MaxValue = 311240;
            this._MinValue = 010199;
            this._MinDigits = 6;
            this._MaxDigits = 6;
        }

    }
}
