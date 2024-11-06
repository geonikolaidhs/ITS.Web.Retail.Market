using ITS.Common.Communication;
using ITS.POS.Fiscal.Common;
using ITS.POS.Fiscal.Common.Responses;
using ITS.POS.Fiscal.Service.RequestResponceLogging;
using ITS.POS.Fiscal.Service.Requests;
using ITS.POS.Hardware.Common;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace ITS.POS.Fiscal.Service.Listeners
{
    public class FiscalServerSignDocumentListener : IMessageListener<FiscalSignDocumentRequest>
    {

        FiscalServer server;
        FiscalServiceSettings settings;
        public FiscalServerSignDocumentListener(FiscalServer server, FiscalServiceSettings settings)
        {
            this.server = server;
            this.settings = settings;
        }

        static object lockingObject = new object();

        public IMessage OnMessageReceived(MessageListenerEventArgs args)
        {
            lock (lockingObject)
            {

                int currentMesssageID = ++Program.MessagesReceived;
                FiscalSignDocumentRequest receivedMessage = (FiscalSignDocumentRequest)args.Message;
                string fiscalRequestWasSendBy = "";
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          fiscalRequestWasSendBy,
                                          RequestResponseConstants.DiSignService,
                                          JsonConvert.SerializeObject(receivedMessage, RequestResponseConstants.JSON_SERIALIZER_SETTINGS),
                                          string.Empty
                                         );



                string output = "";


                if (settings.FiscalOnError)
                {
                    output = String.Format(settings.DeviceNotFoundMessage, settings.SerialNumber);
                    FiscalSignDocumentResponse fiscalSignDocumentResponse = new FiscalSignDocumentResponse(output, eFiscalResponseType.FISCAL_IS_ON_ERROR, 0);
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
                    FiscalSignDocumentResponse fiscalSignDocumentResponse = new FiscalSignDocumentResponse(AlgoboxNetESD.AlgoboxNetResult.CANNOT_FIND_THE_BOX.ToString(), eFiscalResponseType.FAILURE, (int)AlgoboxNetESD.AlgoboxNetResult.CANNOT_FIND_THE_BOX);
                    Program.RequestResponseLogger.Log(currentMesssageID,
                                              RequestResponseConstants.DiSignService,
                                              fiscalRequestWasSendBy,
                                              string.Empty,
                                              JsonConvert.SerializeObject(fiscalSignDocumentResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                             );
                    return fiscalSignDocumentResponse;
                }

                Program.Logger.Trace(string.Format("Signing {0}", receivedMessage.DocumentToSignOfficialFormat));
                String tempFile = Path.GetTempFileName();
                using (StreamWriter wr = new StreamWriter(tempFile, false, Encoding.GetEncoding(1253)))
                {
                    wr.WriteLine(receivedMessage.DocumentToSignPrintFormat);
                }
                try
                {
                    switch (this.server.Settings.FiscalDevice)
                    {
                        case Retail.Platform.Enumerations.eFiscalDevice.RBS_NET:
                            {
                                Program.RequestResponseLogger.Log(currentMesssageID,
                                                          RequestResponseConstants.DiSignService,
                                                          server.RbsSign.DeviceName,
                                                          "SignDocument()",
                                                          string.Empty
                                                         );
                                RBSSign.RBSSignResult result = this.server.RbsSign.SignDocument(this.settings.AbcFolder, tempFile, receivedMessage.DocumentToSignOfficialFormat, ref output);
                                Program.RequestResponseLogger.Log(currentMesssageID,
                                                          server.RbsSign.DeviceName,
                                                          RequestResponseConstants.DiSignService,
                                                          string.Empty,
                                                          result.ToString()
                                                         );

                                FiscalSignDocumentResponse response = null;

                                response = new FiscalSignDocumentResponse(output, (result != RBSSign.RBSSignResult.ERR_SUCCESS) ? eFiscalResponseType.FAILURE : eFiscalResponseType.SUCCESS, (int)result);
                                if (response.Result != eFiscalResponseType.SUCCESS)
                                {
                                    response.ErrorMessage = result.ToString() + ": " + RBSSign.GetResultInfo(result);
                                }
                                Program.Logger.Trace(string.Format("Result {0} {1}", result, receivedMessage.DocumentToSignOfficialFormat));
                                Program.RequestResponseLogger.Log(currentMesssageID,
                                                          RequestResponseConstants.DiSignService,
                                                          fiscalRequestWasSendBy,
                                                          string.Empty,
                                                          JsonConvert.SerializeObject(response, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                                         );
                                return response;
                            }
                        case Retail.Platform.Enumerations.eFiscalDevice.ALGOBOX_NETV2:
                            {
                                Program.RequestResponseLogger.Log(currentMesssageID, RequestResponseConstants.DiSignService, server.AlgoboxV2.DeviceName, "SignDocument()", string.Empty);
                                AlgoboxNetESDV2.AlgoboxNetResult result = this.server.AlgoboxV2.SignDocument(tempFile, receivedMessage.DocumentToSignOfficialFormat, ref output);
                                Program.RequestResponseLogger.Log(currentMesssageID, server.AlgoboxV2.DeviceName, RequestResponseConstants.DiSignService, string.Empty, result.ToString());
                                FiscalSignDocumentResponse response = null;

                                if ((int)result < -1 && (int)result > -5)  //special case, connection error, return success with DeviceNotFoundMessage
                                {
                                    output = String.Format(settings.DeviceNotFoundMessage, settings.SerialNumber);
                                    response = new FiscalSignDocumentResponse(output, eFiscalResponseType.SUCCESS, 0);
                                }
                                else
                                {
                                    response = new FiscalSignDocumentResponse(output, (result != AlgoboxNetESDV2.AlgoboxNetResult.SUCCESS) ? eFiscalResponseType.FAILURE : eFiscalResponseType.SUCCESS, (int)result);
                                    if (result == AlgoboxNetESDV2.AlgoboxNetResult.ISSUE_Z_24_HOURS_SINCE_LAST_Z)
                                    {
                                        response.Result = eFiscalResponseType.FISCAL_MUST_ISSUE_Z;
                                    }
                                    if (response.Result != eFiscalResponseType.SUCCESS)
                                    {
                                        response.ErrorMessage = result.ToString();
                                    }
                                }
                                return response;
                            }
                        default:
                            {
                                Program.RequestResponseLogger.Log(currentMesssageID,
                                                          RequestResponseConstants.DiSignService,
                                                          server.Algobox.DeviceName,
                                                          "SignDocument()",
                                                          string.Empty
                                                         );
                                AlgoboxNetESD.AlgoboxNetResult result = this.server.Algobox.SignDocument(tempFile, receivedMessage.DocumentToSignOfficialFormat, ref output);
                                Program.RequestResponseLogger.Log(currentMesssageID,
                                                          server.Algobox.DeviceName,
                                                          RequestResponseConstants.DiSignService,
                                                          string.Empty,
                                                          result.ToString()
                                                         );

                                FiscalSignDocumentResponse response = null;

                                if ((int)result < -1 && (int)result > -5)  //special case, connection error, return success with DeviceNotFoundMessage
                                {
                                    output = String.Format(settings.DeviceNotFoundMessage, settings.SerialNumber);
                                    response = new FiscalSignDocumentResponse(output, eFiscalResponseType.SUCCESS, 0);
                                }
                                else
                                {
                                    response = new FiscalSignDocumentResponse(output, (result != AlgoboxNetESD.AlgoboxNetResult.SUCCESS) ? eFiscalResponseType.FAILURE : eFiscalResponseType.SUCCESS, (int)result);
                                    if (result == AlgoboxNetESD.AlgoboxNetResult.ISSUE_Z_24_HOURS_SINCE_LAST_Z)
                                    {
                                        response.Result = eFiscalResponseType.FISCAL_MUST_ISSUE_Z;
                                    }
                                    if (response.Result != eFiscalResponseType.SUCCESS)
                                    {
                                        response.ErrorMessage = result.ToString();
                                    }
                                }
                                return response;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Program.Logger.Error(ex, "Error signing document");
                    FiscalSignDocumentResponse fiscalSignDocumentResponse = new FiscalSignDocumentResponse("", eFiscalResponseType.FAILURE, -4);
                    Program.RequestResponseLogger.Log(currentMesssageID,
                                              RequestResponseConstants.DiSignService,
                                              fiscalRequestWasSendBy,
                                              string.Empty,
                                              JsonConvert.SerializeObject(fiscalSignDocumentResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                             );
                    return fiscalSignDocumentResponse;
                }
                finally
                {
                    try
                    {
                        if (File.Exists(tempFile))
                        {
                            File.Delete(tempFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Program.Logger.Error(ex, "Cannot delete temp file");
                    }
                }
            }
        }
    }
}
