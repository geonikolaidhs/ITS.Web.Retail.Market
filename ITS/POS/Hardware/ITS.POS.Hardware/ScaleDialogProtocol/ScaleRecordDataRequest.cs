using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{

    internal class ScaleRecordDataRequest : IScaleRecord
    {
        private static readonly byte[] _Command = new byte[] { DialogHelper.EOT, DialogHelper.ENQ };
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
