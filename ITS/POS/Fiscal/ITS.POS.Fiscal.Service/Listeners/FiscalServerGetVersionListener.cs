using ITS.Common.Communication;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;
using ITS.POS.Fiscal.Service.RequestResponceLogging;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;

namespace ITS.POS.Fiscal.Service.Listeners
{
    public class FiscalServerGetVersionListener : IMessageListener<FiscalGetVersionInfoRequest>
    {
        FiscalServer server;
        public FiscalServerGetVersionListener(FiscalServer server)
        {
            this.server = server;
        }

        public IMessage OnMessageReceived(MessageListenerEventArgs args)
        {
            int currentMesssageID = ++Program.MessagesReceived;
            FiscalGetVersionInfoRequest receivedMessage = (FiscalGetVersionInfoRequest)args.Message;
            string fiscalRequestWasSendBy = "";
            Program.RequestResponseLogger.Log(currentMesssageID,
                                      fiscalRequestWasSendBy,
                                      RequestResponseConstants.DiSignService,
                                      JsonConvert.SerializeObject(receivedMessage, RequestResponseConstants.JSON_SERIALIZER_SETTINGS),
                                      string.Empty
                                     );

            if (server.Online == false)
            {
                FiscalGetVersionInfoResponse failureResponse = new FiscalGetVersionInfoResponse("Fiscal Box is on Error", eFiscalResponseType.FAILURE, -1000);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                      RequestResponseConstants.DiSignService,
                                      fiscalRequestWasSendBy,
                                      string.Empty,
                                      JsonConvert.SerializeObject(failureResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                     );
                return failureResponse;
            }
            else if (server.Algobox == null && server.AlgoboxV2 == null)
            {
                FiscalGetVersionInfoResponse failureResponse = new FiscalGetVersionInfoResponse("Not supported ", eFiscalResponseType.FAILURE, -1001);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                      RequestResponseConstants.DiSignService,
                                      fiscalRequestWasSendBy,
                                      string.Empty,
                                      JsonConvert.SerializeObject(failureResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                     );
                return failureResponse;
            }
            String exResult = "";
            FiscalGetVersionInfoResponse fiscalGetVersionInfoResponse = new FiscalGetVersionInfoResponse();
            if (server.Algobox != null)
            {
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          server.Algobox.DeviceName,
                                          "GetVersionInfo()",
                                          string.Empty
                                         );
                int result = (int)server.Algobox.GetVersionInfo(ref exResult);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          server.Algobox.DeviceName,
                                          RequestResponseConstants.DiSignService,
                                          string.Empty,
                                          result.ToString()
                                         );

                fiscalGetVersionInfoResponse = new FiscalGetVersionInfoResponse(exResult, result == 0 ? eFiscalResponseType.SUCCESS : eFiscalResponseType.FAILURE, result);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          fiscalRequestWasSendBy,
                                          string.Empty,
                                          JsonConvert.SerializeObject(fiscalGetVersionInfoResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                         );
            }
            else if (server.AlgoboxV2 != null)
            {
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          server.AlgoboxV2.DeviceName,
                                          "GetVersionInfo()",
                                          string.Empty
                                         );
                int result = (int)server.AlgoboxV2.GetVersionInfo(ref exResult);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          server.AlgoboxV2.DeviceName,
                                          RequestResponseConstants.DiSignService,
                                          string.Empty,
                                          result.ToString()
                                         );

                fiscalGetVersionInfoResponse = new FiscalGetVersionInfoResponse(exResult, result == 0 ? eFiscalResponseType.SUCCESS : eFiscalResponseType.FAILURE, result);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          fiscalRequestWasSendBy,
                                          string.Empty,
                                          JsonConvert.SerializeObject(fiscalGetVersionInfoResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                         );
            }
            return fiscalGetVersionInfoResponse;
        }
    }
}
