using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{

    internal class ScaleRecordNAK : IScaleRecord
    {
        public byte[] Command { get; private set; }

        public ScaleRecordNAK(IEnumerable<byte> response)
        {
            if (!(response.Count() == 1 && response.First() == DialogHelper.NAK))
            {
                throw new InvalidOperationException("Invalid response. Expected ACK");
            }
        }

        public IScaleRecord GetAppropriateRequest(RequestParameters parameters)
        {
            if(parameters.NAKCounter > 5)
            {
                return new ScaleRecord08();
            }
            return new ScaleRecordDataRequest();
        }
    }
}