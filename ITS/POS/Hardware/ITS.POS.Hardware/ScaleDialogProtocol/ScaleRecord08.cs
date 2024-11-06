using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{
    internal class ScaleRecord08 : IScaleRecord
    {
        static byte[] _Command = new byte[] { DialogHelper.EOT, DialogHelper.STX, 0x30, 0x38, DialogHelper.ETX };
        public byte[] Command
        {
            get
            {
                return _Command;
            }
        }

        public IScaleRecord GetAppropriateRequest(RequestParameters parameters)
        {
            return null;
        }
    }
}
