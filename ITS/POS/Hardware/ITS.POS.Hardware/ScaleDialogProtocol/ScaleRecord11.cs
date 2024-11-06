using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{

    internal class ScaleRecord11 : IScaleRecord
    {
        public ScaleRecord11(IEnumerable<byte> response)
        {
            if (response.Count() < 6)
            {
                throw new NotSupportedException("Record 11 expects at leaset7 byte response");
            }
            byte[] _command = response.ToArray();
            //STX 11 ESC D0 Z ETX
            if (_command[0] != DialogHelper.STX || _command[3] != DialogHelper.ESC 
                || _command[_command.Length - 1] != DialogHelper.ETX || _command[1] != _command[2] || _command[1] != 0x31)
            {
                throw new NotSupportedException("Record 11 Validation failed");
            }
            Command = _command;
        }


        public byte D0 { get { return Command[4]; } }

        public IEnumerable<byte> Z { get { int val = Command.Length - 6; return Command.Skip(5).Take(val); } }

        public byte[] Command { get; private set; }

        public IScaleRecord GetAppropriateRequest(RequestParameters parameters)
        {
            if(D0=='2')
            {
                return new ScaleRecord10(this);
            }
            return new ScaleRecord05() { Value = parameters.Value, Text = parameters.Text, Tare = parameters.Tare };
        }
    }


}
