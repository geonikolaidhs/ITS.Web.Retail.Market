using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    /// <summary>
    /// Fields of this type must not contain any decimal part or decimal point. This type is usually used as a counter field or an index.
    /// </summary>
    public class IntegerField : Field
    {
        public IntegerField()
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
            this._MaxDigits = 6;
            this._MinDigits = 1;
            this._MaxValue = 999999;
            this._MinValue = -999999;
        }

    }
}
