using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{
    /// <summary>
    /// A generic string type, useful for various non-numerical cases.
    /// Character range: 1 to 240 (if not exceeding max packet size)
    /// </summary>
    public class StringField : Field
    {
        public StringField()
        {
            this._FieldClass = Protocol.FieldClass.STRING;
        }
    }
}
