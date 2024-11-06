using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ITS.Common.Communication;
using ITS.POS.Hardware.Common;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Resources;
using ITS.Retail.PrintServer.Common;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Represents a connection to a DiSign service.
    /// </summary>
    [RequireOnlyOneSuccessfulDeviceType]
    public class RemotePrint : Device
    {
        public const int TIMEOUT = 20000;
        MessageClient client;

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            message = "NOT CHECKED";
            return eDeviceCheckResult.INFO;
        }

        /// <summary>
        /// Only ConnectionType.Ethernet is supported.
        /// </summary>
        /// <param name="conType"></param>
        public RemotePrint(ConnectionType conType, string deviceName)
        {
            ConType = conType;
            DeviceName = deviceName;
            if (conType != ConnectionType.ETHERNET)
            {
                throw new NotSupportedException(conType.ToString() + " is not supported");
            }
        }

        /// <summary>
        /// Initializes the tcp client.
        /// </summary>
        public void PrepareConnection()
        {
            client = new MessageClient(this.Settings.Ethernet.IPAddress, this.Settings.Ethernet.Port, this.GetLogger());
        }

        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);
            if (this.ConType == ConnectionType.ETHERNET)
            {
                PrepareConnection();
            }
        }

        public int PrintRemoteDocument(Guid documentOid, Guid userOid, int posId, string printerNickName, out string message)
        {
            if (client == null)
            {
                PrepareConnection();
            }
            PrintServerPrintDocumentRequest request = new PrintServerPrintDocumentRequest(documentOid, userOid, posId, printerNickName);
            PrintServerPrintDocumentResponse response = client.SendMessageAndWaitResponse<PrintServerPrintDocumentResponse>(request, TIMEOUT);
            if (response == null)
            {
                message = POSClientResources.TIMEOUT;
                return -1;
            }
            if (String.IsNullOrWhiteSpace(response.ErrorMessage) == false)
            {
                message = response.ErrorMessage;
                return response.ErrorCode;
            }
            message = response.Explanation;
            return response.ErrorCode;
        }


    }
}

