using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    /// <summary>
    /// PERCENTAGE is used to specify a discount percentage, a mark up percentage etc.
    /// </summary>
    public class PercentField : Field
    {
        public PercentField()
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
            this._MaxDigits = 6;
            this._MinDigits = 1;
            this._MaxValue = 100.00;
            this._MinValue = 0.00;
        }

    }
}
