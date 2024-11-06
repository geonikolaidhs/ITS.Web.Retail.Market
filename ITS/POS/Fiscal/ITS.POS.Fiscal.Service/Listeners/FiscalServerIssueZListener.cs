using ITS.Common.Communication;
using ITS.POS.Fiscal.Common;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;
using ITS.POS.Fiscal.Service.RequestResponceLogging;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;

namespace ITS.POS.Fiscal.Service.Listeners
{
    public class FiscalServerIssueZListener : IMessageListener<FiscalIssueZRequest>
    {
        FiscalServer server;
        FiscalServiceSettings settings;
        public FiscalServerIssueZListener(FiscalServer server, FiscalServiceSettings settings)
        {
            this.server = server;
            this.settings = settings;
        }

        public IMessage OnMessageReceived(MessageListenerEventArgs args)
        {
            string uploadErrorMessage = string.Empty;
            int currentMesssageID = ++Program.MessagesReceived;
            FiscalIssueZRequest receivedMessage = (FiscalIssueZRequest)args.Message;
            string fiscalRequestWasSendBy = "";
            Program.RequestResponseLogger.Log(currentMesssageID,
                                      fiscalRequestWasSendBy,
                                      RequestResponseConstants.DiSignService,
                                      JsonConvert.SerializeObject(receivedMessage, RequestResponseConstants.JSON_SERIALIZER_SETTINGS),
                                      string.Empty
                                     );


            if (settings.FiscalOnError)
            {
                string output = "";
                output = String.Format(settings.DeviceNotFoundMessage, settings.SerialNumber);
                FiscalSignDocumentResponse fiscalSignDocumentResponse = new FiscalSignDocumentResponse(output, eFiscalResponseType.FISCAL_IS_ON_ERROR, -2);
                fiscalSignDocumentResponse.ErrorMessage = output;
                Program.RequestResponseLogger.Log(currentMesssageID,
                                      RequestResponseConstants.DiSignService,
                                      fiscalRequestWasSendBy,
                                      string.Empty,
                                      JsonConvert.SerializeObject(fiscalSignDocumentResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                     );
                return fiscalSignDocumentResponse;
            }
            if (server.Online == false)
            {
                FiscalIssueZResponse failureResponse = new FiscalIssueZResponse("Fiscal Box is on Error", eFiscalResponseType.FAILURE, -1000);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                      RequestResponseConstants.DiSignService,
                                      fiscalRequestWasSendBy,
                                      string.Empty,
                                      JsonConvert.SerializeObject(failureResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                     );
                return failureResponse;
            }
            String exResult = "";
            if (server.Algobox != null)
            {
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          server.Algobox.DeviceName,
                                          "IssueZreport()",
                                          string.Empty
                                         );

                int result = (int)server.Algobox.IssueZreport(ref exResult);

                Program.RequestResponseLogger.Log(currentMesssageID,
                                          server.Algobox.DeviceName,
                                          RequestResponseConstants.DiSignService,
                                          string.Empty,
                                          result.ToString()
                                         );

                FiscalIssueZResponse fiscalIssueZResponse = new FiscalIssueZResponse(exResult, result == 0 ? eFiscalResponseType.SUCCESS : eFiscalResponseType.FAILURE, result);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                      RequestResponseConstants.DiSignService,
                                      fiscalRequestWasSendBy,
                                      string.Empty,
                                      JsonConvert.SerializeObject(fiscalIssueZResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                     );
                return fiscalIssueZResponse;
            }
            else if (server.AlgoboxV2 != null)
            {
                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, server.AlgoboxV2.DeviceName, "IssueZreport()", string.Empty);
                int result = (int)server.AlgoboxV2.IssueZreport(ref exResult, out uploadErrorMessage);
                Program.RequestResponseLogger.Log(currentMesssageID, server.AlgoboxV2.DeviceName, RequestResponseConstants.DiSignService, string.Empty, result.ToString());

                FiscalIssueZResponse fiscalIssueZResponse = new FiscalIssueZResponse(exResult, result == 0 ? eFiscalResponseType.SUCCESS : eFiscalResponseType.FAILURE, result, uploadErrorMessage);

                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, fiscalRequestWasSendBy, string.Empty,
                                                                            JsonConvert.SerializeObject(fiscalIssueZResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS));
                return fiscalIssueZResponse;
            }
            else if (server.RbsSign != null)
            {
                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, server.RbsSign.DeviceName, "IssueZreport()", string.Empty);
                int result = (int)server.RbsSign.IssueZreport(server.Settings.AbcFolder, out uploadErrorMessage);
                Program.RequestResponseLogger.Log(currentMesssageID, server.RbsSign.DeviceName, RequestResponseConstants.DiSignService, string.Empty, result.ToString());
                exResult = Hardware.Common.RBSSign.GetResultInfo((Hardware.Common.RBSSign.RBSSignResult)result);

                FiscalIssueZResponse fiscalIssueZResponse = new FiscalIssueZResponse(exResult, result == 0 ? eFiscalResponseType.SUCCESS : eFiscalResponseType.FAILURE, result, uploadErrorMessage);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                      RequestResponseConstants.DiSignService,
                                      fiscalRequestWasSendBy,
                                      string.Empty,
                                      JsonConvert.SerializeObject(fiscalIssueZResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                     );
                return fiscalIssueZResponse;
            }
            FiscalIssueZResponse incorrectSettingsResponse = new FiscalIssueZResponse("Incorrect settings", eFiscalResponseType.FAILURE, -1000);
            Program.RequestResponseLogger.Log(currentMesssageID,
                                      RequestResponseConstants.DiSignService,
                                      fiscalRequestWasSendBy,
                                      string.Empty,
                                      JsonConvert.SerializeObject(incorrectSettingsResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                     );
            return incorrectSettingsResponse;
        }
    }
}
