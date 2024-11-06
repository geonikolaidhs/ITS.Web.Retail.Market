using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{
    public class RequestParameters
    {
        public int Value { get; set; }
        public int Tare  { get; set; }
        public string Text { get; set; }

        public int NAKCounter { get; set; }
    }
}
