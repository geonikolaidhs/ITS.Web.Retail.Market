using ITS.Common.Communication;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;
using ITS.POS.Fiscal.Service.RequestResponceLogging;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;

namespace ITS.POS.Fiscal.Service.Listeners
{
    public class FiscalServerCheckBoxSystemListener : IMessageListener<FiscalCheckSystemRequest>
    {
        FiscalServer server;
        public FiscalServerCheckBoxSystemListener(FiscalServer server)
        {
            this.server = server;
        }

        public IMessage OnMessageReceived(MessageListenerEventArgs args)
        {
            int currentMesssageID = ++Program.MessagesReceived;
            FiscalCheckSystemRequest receivedMessage = (FiscalCheckSystemRequest)args.Message;

            string fiscalRequestWasSendBy = "";
            Program.RequestResponseLogger.Log(currentMesssageID,
                                      fiscalRequestWasSendBy,
                                      RequestResponseConstants.DiSignService,
                                      JsonConvert.SerializeObject(receivedMessage, RequestResponseConstants.JSON_SERIALIZER_SETTINGS),
                                      string.Empty
                                      );

            if (server.Online == false)
            {
                FiscalCheckSystemResponse failureResponse = new FiscalCheckSystemResponse("Fiscal Box is on Error", eFiscalResponseType.FAILURE, -1000);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          fiscalRequestWasSendBy,
                                          String.Empty,
                                          JsonConvert.SerializeObject(failureResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                          );

                return failureResponse;
            }
            else if (server.Algobox == null && server.AlgoboxV2 == null)
            {
                FiscalCheckSystemResponse failureResponse = new FiscalCheckSystemResponse("Not supported ", eFiscalResponseType.FAILURE, -1001);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          fiscalRequestWasSendBy,
                                          String.Empty,
                                          JsonConvert.SerializeObject(failureResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                          );
                return failureResponse;
            }
            FiscalCheckSystemResponse fiscalCheckSystemResponse = new FiscalCheckSystemResponse();
            String explanation;
            short Result;
            if (server.Algobox != null)
            {
                server.Algobox.CheckSystem(out Result, out explanation);
                fiscalCheckSystemResponse = new FiscalCheckSystemResponse(explanation, Result == 0 ? eFiscalResponseType.SUCCESS : eFiscalResponseType.FAILURE, Result);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          fiscalRequestWasSendBy,
                                          String.Empty,
                                          JsonConvert.SerializeObject(fiscalCheckSystemResponse,
                                          RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                          );

            }
            else if (server.AlgoboxV2 != null)
            {
                server.AlgoboxV2.CheckSystem(out Result, out explanation);
                fiscalCheckSystemResponse = new FiscalCheckSystemResponse(explanation, Result == 0 ? eFiscalResponseType.SUCCESS : eFiscalResponseType.FAILURE, Result);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          fiscalRequestWasSendBy,
                                          String.Empty,
                                          JsonConvert.SerializeObject(fiscalCheckSystemResponse,
                                          RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                          );
            }
            return fiscalCheckSystemResponse;
        }
    }
}
