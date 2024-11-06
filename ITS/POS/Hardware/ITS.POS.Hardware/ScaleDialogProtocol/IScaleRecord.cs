using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{
    internal interface IScaleRecord
    {
        byte[] Command { get; }

        IScaleRecord GetAppropriateRequest(RequestParameters parameters);
    }
}
