using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    public class QtyField : Field
    {
        /// <summary>
        /// QTY is used to specified quantities of any kind.
        /// </summary>
        public QtyField()
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
            this._MaxDigits = 10;
            this._MinDigits = 1;
            this._MaxValue = 99999.999;
            this._MinValue = -99999.999;
        }

    }
}
