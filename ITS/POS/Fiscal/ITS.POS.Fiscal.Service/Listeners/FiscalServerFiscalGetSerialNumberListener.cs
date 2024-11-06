using ITS.Common.Communication;
using ITS.Common.Utilities.Forms;
using ITS.POS.Fiscal.Common;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;
using ITS.POS.Fiscal.Service.RequestResponceLogging;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;

namespace ITS.POS.Fiscal.Service.Listeners
{
    public class FiscalServerFiscalGetSerialNumberListener : IMessageListener<FiscalGetSerialNumberRequest>
    {
        FiscalServer server;
        FiscalServiceSettings settings;
        public FiscalServerFiscalGetSerialNumberListener(FiscalServer server, FiscalServiceSettings settings)
        {
            this.server = server;
            this.settings = settings;
        }

        public IMessage OnMessageReceived(MessageListenerEventArgs args)
        {
            int currentMesssageID = ++Program.MessagesReceived;
            FiscalGetSerialNumberRequest receivedMessage = (FiscalGetSerialNumberRequest)args.Message;
            string fiscalRequestWasSendBy = "";
            Program.RequestResponseLogger.Log(currentMesssageID, fiscalRequestWasSendBy, RequestResponseConstants.DiSignService, JsonConvert.SerializeObject(receivedMessage, RequestResponseConstants.JSON_SERIALIZER_SETTINGS), string.Empty);

            if (server.Online == false)
            {
                FiscalGetSerialNumberResponse failureResponse = new FiscalGetSerialNumberResponse("", eFiscalResponseType.FAILURE);
                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, fiscalRequestWasSendBy, string.Empty, JsonConvert.SerializeObject(failureResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS));
                return failureResponse;
            }
            else if (server.Algobox != null)
            {
                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, this.server.Algobox.DeviceName, "GetSerialNumber()", string.Empty);
                String result = this.server.Algobox.GetSerialNumber();
                Program.RequestResponseLogger.Log(currentMesssageID, this.server.Algobox.DeviceName, RequestResponseConstants.DiSignService, string.Empty, result);
                if (result != null && settings.SerialNumber != result)
                {
                    settings.SerialNumber = result;
                    ConfigurationHelper.SaveSettingsFile(settings, Common.SettingsFilePath);
                }
                FiscalGetSerialNumberResponse successResponse = new FiscalGetSerialNumberResponse(result, eFiscalResponseType.SUCCESS);
                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, fiscalRequestWasSendBy, string.Empty, JsonConvert.SerializeObject(successResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS));
                return successResponse;
            }
            else if (server.AlgoboxV2 != null)
            {
                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, this.server.AlgoboxV2.DeviceName, "GetSerialNumber()", string.Empty);
                String result = this.server.AlgoboxV2.GetSerialNumber();
                Program.RequestResponseLogger.Log(currentMesssageID, this.server.AlgoboxV2.DeviceName, RequestResponseConstants.DiSignService, string.Empty, result);
                if (result != null && settings.SerialNumber != result)
                {
                    settings.SerialNumber = result;
                    ConfigurationHelper.SaveSettingsFile(settings, Common.SettingsFilePath);
                }
                FiscalGetSerialNumberResponse successResponse = new FiscalGetSerialNumberResponse(result, eFiscalResponseType.SUCCESS);
                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, fiscalRequestWasSendBy, string.Empty, JsonConvert.SerializeObject(successResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS));
                return successResponse;
            }
            else if (server.RbsSign != null)
            {
                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, this.server.RbsSign.DeviceName, "GetSerialNumber()", string.Empty);
                String result = this.server.RbsSign.GetSerialNumber(server.Settings.AbcFolder);
                Program.RequestResponseLogger.Log(currentMesssageID, this.server.RbsSign.DeviceName, RequestResponseConstants.DiSignService, string.Empty, result);

                if (result != null && settings.SerialNumber != result)
                {
                    settings.SerialNumber = result;
                    ConfigurationHelper.SaveSettingsFile(settings, Common.SettingsFilePath);
                }
                FiscalGetSerialNumberResponse successResponse = new FiscalGetSerialNumberResponse(result, eFiscalResponseType.SUCCESS);
                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, fiscalRequestWasSendBy, string.Empty, JsonConvert.SerializeObject(successResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS));
                return successResponse;
            }
            FiscalGetSerialNumberResponse response = new FiscalGetSerialNumberResponse("Not supported ", eFiscalResponseType.FAILURE);
            Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, fiscalRequestWasSendBy, string.Empty, JsonConvert.SerializeObject(response, RequestResponseConstants.JSON_SERIALIZER_SETTINGS));
            return response;
        }
    }
}
