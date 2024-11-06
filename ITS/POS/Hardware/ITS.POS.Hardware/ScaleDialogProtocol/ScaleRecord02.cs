using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{
    internal class ScaleRecord02 : IScaleRecord
    {
        public byte[] Command { get; private set; }
        public ScaleRecord02(IEnumerable<byte> response)
        {
            Command = response.ToArray();
            byte[] weightBytes = Command.Skip(6).Take(5).ToArray();
            byte[] unitPriceBytes = Command.Skip(12).Take(5).ToArray();
            byte[] sellingPriceBytes = Command.Skip(19).Take(5).ToArray();

            WeightString = Encoding.UTF8.GetString(weightBytes);
            UnitPrice = Encoding.UTF8.GetString(unitPriceBytes);
            SellingPrice = Encoding.UTF8.GetString(sellingPriceBytes);
        }

        public string WeightString { get; private set; }
        public string UnitPrice { get; private set; }
        public string SellingPrice { get; private set; }

        public IScaleRecord GetAppropriateRequest(RequestParameters parameters)
        {
            return null;
        }
    }

}
