using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{
    internal class ScaleRecord04 : IScaleRecord
    {
        public int Value { get; set; }

        public string Text { get; set; }

        public byte[] Command
        {
            get
            {
                List<byte> returnBytes = new List<byte>
                {                
                    DialogHelper.EOT, DialogHelper.STX,
                    0x30,0x34,
                    DialogHelper.ESC
                };
                returnBytes.AddRange(Encoding.ASCII.GetBytes(this.Value.ToString("000000")).Take(6));
                returnBytes.Add(DialogHelper.ESC);
                returnBytes.AddRange(Encoding.ASCII.GetBytes(this.Text).Take(13));
                returnBytes.Add(DialogHelper.ETX);

                return returnBytes.ToArray();
            }
        }

        public IScaleRecord GetAppropriateRequest(RequestParameters parameters)
        {
            return null;
        }
    }
}
