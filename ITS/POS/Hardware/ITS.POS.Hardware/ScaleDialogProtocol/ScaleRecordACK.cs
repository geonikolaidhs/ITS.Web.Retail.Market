using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{

    internal class ScaleRecordACK : IScaleRecord
    {
        public byte[] Command { get; private set; }

        public ScaleRecordACK(IEnumerable<byte> response)
        {
            if(!(response.Count() == 1 && response.First()==DialogHelper.ACK ))
            {
                throw new InvalidOperationException("Invalid response. Expected ACK");
            }
        }

        public IScaleRecord GetAppropriateRequest(RequestParameters parameters)
        {
            return new ScaleRecordDataRequest();
        }
    }
}
