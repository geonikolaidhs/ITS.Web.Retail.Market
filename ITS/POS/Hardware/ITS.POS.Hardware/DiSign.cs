using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ITS.Common.Communication;
using ITS.POS.Fiscal.Common;
using ITS.POS.Hardware.Common;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Hardware.Common
{
    public class DiSign : Device
    {
        public const int TIMEOUT = 10000;
        MessageClient client;
        /// <summary>
        /// Only ConnectionType.Ethernet is supported.
        /// </summary>
        /// <param name="conType"></param>
        public DiSign(ConnectionType conType, string deviceName)
        {
            ConType = conType;
            DeviceName = deviceName;

            if (conType != ConnectionType.ETHERNET)
            {
                throw new NotSupportedException(conType.ToString() + " is not supported");
            }
        }

        ~DiSign()
        {
            //client.Shutdown();
        }

        public void PrepareConnection()
        {
            client = new MessageClient(Constants.ApplicationIdentifier, this.Settings.Ethernet.IPAddress, this.Settings.Ethernet.Port);
            client.Connect();

        }

        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);
            PrepareConnection();
        }



        public int SignDocument(string fileToSign, string eRecordString, ref string exResult)
        {
            if (client == null)
            {
                PrepareConnection();
            }
            if (client.NetNodeStatus == Lidgren.Network.NetPeerStatus.NotRunning || client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Disconnected)
            {
                client.Connect();
            }

            FiscalSignDocumentResponse signDocumentResponse = null;
            FiscalSignDocumentRequest request = new FiscalSignDocumentRequest(fileToSign, eRecordString);

            signDocumentResponse = client.SendMessageAndWaitResponse<FiscalSignDocumentResponse>(request, TIMEOUT);
            if (signDocumentResponse == null)
                return -1;
            exResult = signDocumentResponse.Signature;
            return signDocumentResponse.ErrorCode;
        }


    }
}
