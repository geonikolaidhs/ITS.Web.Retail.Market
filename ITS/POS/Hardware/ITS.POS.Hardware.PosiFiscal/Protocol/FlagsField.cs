using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.PosiFiscal.Protocol
{

    /// <summary>
    /// Flags type is used to minimize packet fields where a single "true"/"false" or 
    /// "yes"/"no" type of information must be passed for various attributes.
    /// </summary>
    public class FlagsField : Field
    {
        List<bool> Flags { get; set; }

        public FlagsField(List<bool> flags)
        {
            this._FieldClass = Protocol.FieldClass.NUMERIC;
            this.Flags = flags;
        }

        public override string Value
        {
            get
            {
                string result = "";
                foreach (bool flag in Flags)
                {
                    result += flag ? 1 : 0;
                }
                return result;
            }
            set
            {
                foreach (char ch in value)
                {
                    Flags = new List<bool>();
                    if (ch == '1')
                        Flags.Add(true);

                    else
                        Flags.Add(false);

                }
            }
        }

    }
}
