using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{
    internal class ScaleRecord10 : IScaleRecord
    {
        public ScaleRecord10()
        {
            this.Data = new List<byte>();

        }

        public ScaleRecord10(ScaleRecord11 response)
        {

            byte[] z = response.Z.ToArray();
            this.Data = DialogHelper.GetUnlockVal(z).ToList();
        }
        public List<byte> Data { get; set; }


        //EOT STX 10 ESC CS1 KW1 CS2 KW2 .. CSn KWn ETX
        public byte[] Command
        {
            get
            {

                List<byte> result = new List<byte>(6 + Data.Count);
                result.Add(DialogHelper.EOT);
                result.Add(DialogHelper.STX);
                result.Add(0x31);
                result.Add(0x30);
                result.Add(DialogHelper.ESC);
                result.AddRange(Data);
                result.Add(DialogHelper.ETX);
                return result.ToArray();

            }
        }

        public IScaleRecord GetAppropriateRequest(RequestParameters parameters)
        {
            return null;
        }
    }


}
