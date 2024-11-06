using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.ScaleDialogProtocol
{
    internal class ScaleRecord09 : IScaleRecord
    {
        public byte[] Command
        {
            get; private set;
        }

        protected byte S0
        {
            get
            {
                return Command[5];
            }
        }

        protected byte S1
        {
            get
            {
                return Command[4];
            }
        }


        public IScaleRecord GetAppropriateRequest(RequestParameters parameters)
        {
            if (S1 == 0x31 && S0 != 0x30)
            {
                return new ScaleRecord05() { Text = parameters.Text, Value = parameters.Value, Tare = parameters.Tare};
            }
            if (S1 == 0x30 && S0 == 0x30)
            {
                return new ScaleRecordDataRequest();
            }
            if (S1 == 0x30 && S0 == 0x31)
            {
                throw new POSUserVisibleException(POSClientResources.GENERAL_SCALE_ERROR);
            }
            if ( (S1 == 0x30 && S0 == 0x32) || (S1 == 0x31))
            {
                throw new POSUserVisibleException(POSClientResources.PROTOCOL_OR_PARITY_ERROR+
                    Environment.NewLine +POSClientResources.ERROR + ":"+ S1.ToString()+S0.ToString());
            }

            if (S1 == 0x32 || S1 == 0x33)
            {
                throw new POSUserVisibleException(POSClientResources.WEIGH_ON_THE_SCALE_IS_NOT_STABLE);
            }


            return null;
        }

        public ScaleRecord09(IEnumerable<byte> response)
        {
            if (response.Count() == 7)
            {
                Command = response.ToArray();
            }
        }
    }
}
