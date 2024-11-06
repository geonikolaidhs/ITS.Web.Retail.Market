using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    /// <summary>
    /// AMOUNT is usually used to specify prices, discounts, payment values, totals, etc. 
    /// When used to specify payments, this type will always be expressed in the active note (i.e.: drachmas or euro)
    /// </summary>
    public class AmountField : Field
    {

        public AmountField()
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
            this._MaxDigits = 12;
            this._MinDigits = 1;
            this._MaxValue = 99999999.99;
            this._MinValue = -99999999.99;
        }
    }
}
