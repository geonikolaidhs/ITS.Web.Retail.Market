using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.Common
{
    public class PaddingSettings : IXmlSubitems
    {
        public PaddingMode Mode { get; set; }
        public int Length { get; set; }
        public char Character { get; set; }

        public PaddingSettings()
        {
            Mode = PaddingMode.NONE;
            Length = 0;
            Character = ' ';
        }
    }
}
