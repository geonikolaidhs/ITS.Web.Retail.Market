using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ITS.Common.Communication;
using ITS.POS.Hardware.Common;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Fiscal.Common;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;
using ITS.POS.Fiscal.Service.Requests;

namespace ITS.POS.Hardware.Common
{
    public class DiSign : Device
    {
        public const int TIMEOUT = 20000;
        MessageClient client;
        /// <summary>
        /// Only ConnectionType.Ethernet is supported.
        /// </summary>
        /// <param name="conType"></param>
        public DiSign(ConnectionType conType, string deviceName)
        {
            ConType = conType;
            DeviceName = deviceName;

            if (conType != ConnectionType.ETHERNET && conType != ConnectionType.EMULATED)
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
            if (this.ConType == ConnectionType.ETHERNET)
            {
                PrepareConnection();
            }
        }



        public int SignDocument(string fileToSign, string eRecordString, ref string exResult)
        {
            switch (ConType)
            {
                case ConnectionType.ETHERNET:
                    if (client == null)
                    {
                        PrepareConnection();
                        bool b = client.MessageReceivedEvent.WaitOne(1000);
                    }
                    if (client.NetNodeStatus == Lidgren.Network.NetPeerStatus.NotRunning || client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Disconnected)
                    {
                        client.Connect();
                        bool b = client.MessageReceivedEvent.WaitOne(1000);
                    }

                    FiscalSignDocumentResponse signDocumentResponse = null;
                    FiscalSignDocumentRequest request = new FiscalSignDocumentRequest(fileToSign, eRecordString);

                    signDocumentResponse = client.SendMessageAndWaitResponse<FiscalSignDocumentResponse>(request, TIMEOUT);
                    if (signDocumentResponse == null)
                    {
                        return -1;
                    }
                    exResult = signDocumentResponse.Signature;
                    return signDocumentResponse.ErrorCode;
                case ConnectionType.EMULATED:
                    exResult = "EMULATED SIGNATURE 69F4D3750BA1CD6873BEE5B0FD2D49640BE5A634 0136 00000137 1311211608 EHR120064";
                    return 0;
            }

            throw new NotSupportedException(ConType.ToString() + " is not supported");
        }

        public int IssueZ(ref string exResult)
        {
            switch (ConType)
            {
                case ConnectionType.ETHERNET:
                    if (client == null)
                    {
                        PrepareConnection();
                        bool b = client.MessageReceivedEvent.WaitOne(1000);
                    }
                    if (client.NetNodeStatus == Lidgren.Network.NetPeerStatus.NotRunning || client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Disconnected)
                    {
                        client.Connect();
                        bool b = client.MessageReceivedEvent.WaitOne(1000);
                    }

                    FiscalIssueZResponse issueZResponse = null;
                    FiscalIssueZRequest request = new FiscalIssueZRequest();

                    issueZResponse = client.SendMessageAndWaitResponse<FiscalIssueZResponse>(request, TIMEOUT);
                    if (issueZResponse == null)
                    {
                        return -1;
                    }
                    exResult = issueZResponse.ExResult;
                    return issueZResponse.ErrorCode;
                case ConnectionType.EMULATED:
                    exResult = "SUCCESS";
                    return 0;
            }

            throw new NotSupportedException(ConType.ToString() + " is not supported");
        }



        public bool SetFiscalOnError(bool setOnError, out string errorMessage)
        {
            errorMessage = null;
            switch (ConType)
            {
                case ConnectionType.ETHERNET:
                    if (client == null)
                    {
                        PrepareConnection();
                        bool b = client.MessageReceivedEvent.WaitOne(1000);
                    }
                    if (client.NetNodeStatus == Lidgren.Network.NetPeerStatus.NotRunning ||
                        client.ConnectionStatus == Lidgren.Network.NetConnectionStatus.Disconnected)
                    {
                        client.Connect();
                        bool b = client.MessageReceivedEvent.WaitOne(1000);
                    }

                    FiscalSetOnErrorResponse setOnErrorResponse = null;
                    FiscalSetOnErrorRequest request = new FiscalSetOnErrorRequest(setOnError);

                    setOnErrorResponse = client.SendMessageAndWaitResponse<FiscalSetOnErrorResponse>(request, TIMEOUT);
                    if (setOnErrorResponse == null)
                    {
                        return false;
                    }
                    else
                    {
                        errorMessage = setOnErrorResponse.ErrorMessage;
                        return setOnErrorResponse.Result == eFiscalResponseType.SUCCESS;
                    }
                case ConnectionType.EMULATED:
                    return true;
                default:
                    return false;
            }
        }


    }
}
