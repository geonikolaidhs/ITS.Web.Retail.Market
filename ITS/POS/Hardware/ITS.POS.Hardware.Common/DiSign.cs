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
using ITS.POS.Resources;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Represents a connection to a DiSign service.
    /// </summary>
    [RequireOnlyOneSuccessfulDeviceType]
    public class DiSign : Device
    {
        public const int TIMEOUT = 180000;
        MessageClient client;

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            string version = GetFiscalVersion();
            if (version == null)
            {
                message = "OFFLINE";
                return eDeviceCheckResult.FAILURE;
            }
            message = string.Empty;
            return eDeviceCheckResult.SUCCESS;
        }

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

        /// <summary>
        /// Signs the given document by communicating with the DiSign service.
        /// </summary>
        /// <param name="fileToSign"></param>
        /// <param name="eRecordString"></param>
        /// <param name="exResult"></param>
        /// <returns></returns>
        public int SignDocument(string fileToSign, string eRecordString, ref string exResult)
        {
            switch (ConType)
            {
                case ConnectionType.ETHERNET:
                    if (client == null)
                    {
                        PrepareConnection();
                    }

                    FiscalSignDocumentResponse signDocumentResponse = null;
                    FiscalSignDocumentRequest request = new FiscalSignDocumentRequest(fileToSign, eRecordString);

                    signDocumentResponse = client.SendMessageAndWaitResponse<FiscalSignDocumentResponse>(request, TIMEOUT);
                    if (signDocumentResponse == null)
                    {
                        return -1;
                    }
                    if (signDocumentResponse.Result == eFiscalResponseType.FISCAL_IS_ON_ERROR)
                    {
                        exResult = signDocumentResponse.Signature;
                        return -2;
                    }
                    if (signDocumentResponse.Result == eFiscalResponseType.FISCAL_MUST_ISSUE_Z)
                    {
                        return -4;
                    }
                    if (String.IsNullOrWhiteSpace(signDocumentResponse.ErrorMessage) == false)
                    {
                        exResult = signDocumentResponse.ErrorMessage;
                        return signDocumentResponse.ErrorCode != 0 ? signDocumentResponse.ErrorCode : -3;
                    }
                    exResult = signDocumentResponse.Signature;
                    return signDocumentResponse.ErrorCode;
                case ConnectionType.EMULATED:
                    exResult = "====================";
                    return 0;
            }

            throw new NotSupportedException(ConType.ToString() + " is not supported");
        }

        /// <summary>
        /// Issues a Z report to the DiSign service.
        /// </summary>
        /// <param name="exResult"></param>
        /// <returns></returns>
        public int IssueZ(ref string exResult, ref string uploadErrorMessage)
        {
            switch (ConType)
            {
                case ConnectionType.ETHERNET:
                    if (client == null)
                    {
                        PrepareConnection();
                    }

                    FiscalIssueZResponse issueZResponse = null;
                    FiscalIssueZRequest request = new FiscalIssueZRequest();

                    issueZResponse = client.SendMessageAndWaitResponse<FiscalIssueZResponse>(request, TIMEOUT);
                    if (issueZResponse == null)
                    {
                        return -1;
                    }
                    if (issueZResponse.Result == eFiscalResponseType.FISCAL_IS_ON_ERROR)
                    {
                        return -2;
                    }
                    if (String.IsNullOrWhiteSpace(issueZResponse.ErrorMessage) == false)
                    {
                        exResult = issueZResponse.ErrorMessage;
                        uploadErrorMessage = issueZResponse.UploadZErrorMessage;
                        return issueZResponse.ErrorCode == 0 ? -1 : issueZResponse.ErrorCode;
                    }
                    exResult = issueZResponse.ExResult;
                    uploadErrorMessage = issueZResponse.UploadZErrorMessage;
                    return issueZResponse.ErrorCode;
                case ConnectionType.EMULATED:
                    exResult = "SUCCESS";
                    return 0;
            }

            throw new NotSupportedException(ConType.ToString() + " is not supported");
        }

        /// <summary>
        /// Sets the remote DiSign service "On Error". 
        /// All the POSs that listen to the service will begin printing receipts with "Fiscal On Error" status.
        /// </summary>
        /// <param name="setOnError"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool SetFiscalOnError(bool setOnError, out string errorMessage)
        {
            errorMessage = null;
            switch (ConType)
            {
                case ConnectionType.ETHERNET:
                    if (client == null)
                    {
                        PrepareConnection();
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
            }

            throw new NotSupportedException(ConType.ToString() + " is not supported");
        }

        /// <summary>
        /// Gets the EAFDSS fiscal version from the DiSign service.
        /// </summary>
        /// <returns></returns>
        public string GetFiscalVersion()
        {
            switch (ConType)
            {
                case ConnectionType.ETHERNET:
                    if (client == null)
                    {
                        PrepareConnection();
                    }

                    FiscalGetVersionInfoResponse versionResponse = null;
                    FiscalGetVersionInfoRequest request = new FiscalGetVersionInfoRequest();

                    versionResponse = client.SendMessageAndWaitResponse<FiscalGetVersionInfoResponse>(request, TIMEOUT);
                    if (versionResponse == null || String.IsNullOrWhiteSpace(versionResponse.ErrorMessage) == false)
                    {
                        return null;
                    }
                    return versionResponse.ExResult == null ? string.Empty : versionResponse.ExResult;
                case ConnectionType.EMULATED:
                    return "EMULATED";
            }


            throw new NotSupportedException(ConType.ToString() + " is not supported");
        }
    }
}
