using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    /// <summary>
    /// Specifies a time. Time format is HHMMSS.
    /// </summary>
    public class TimeField : Field
    {
        public TimeField()
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
            this._MaxValue = 235959;
            this._MinValue = 000000;
            this._MinDigits = 6;
            this._MaxDigits = 6;
        }

    }
}
