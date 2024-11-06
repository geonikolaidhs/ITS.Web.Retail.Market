using ITS.Common.Communication;
using ITS.Common.Utilities.Forms;
using ITS.POS.Fiscal.Common;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Fiscal.Service.RequestResponceLogging;
using Newtonsoft.Json;

namespace ITS.POS.Fiscal.Service.Listeners
{
    public class FiscalServerSetFiscalOnErrorListener : IMessageListener<FiscalSetOnErrorRequest>
    {
        FiscalServer server;
        public FiscalServerSetFiscalOnErrorListener(FiscalServer server)
        {
            this.server = server;
        }

        public IMessage OnMessageReceived(MessageListenerEventArgs args)
        {
            int currentMesssageID = ++Program.MessagesReceived;
            FiscalSetOnErrorRequest receivedMessage = (FiscalSetOnErrorRequest)args.Message;
            string fiscalRequestWasSendBy = "";
            Program.RequestResponseLogger.Log(currentMesssageID,
                                      fiscalRequestWasSendBy,
                                      RequestResponseConstants.DiSignService,
                                      JsonConvert.SerializeObject(receivedMessage, RequestResponseConstants.JSON_SERIALIZER_SETTINGS),
                                      string.Empty
                                     );

            try
            {
                this.server.Settings.FiscalOnError = (args.Message as FiscalSetOnErrorRequest).SetFiscalOnError;
                ConfigurationHelper.SaveSettingsFile(this.server.Settings, Common.SettingsFilePath);
                this.server.ReloadSettings = true;  //signals the UI to reload the settings;
                FiscalSetOnErrorResponse fiscalSetOnErrorResponse = new FiscalSetOnErrorResponse(eFiscalResponseType.SUCCESS,null);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          fiscalRequestWasSendBy,
                                          string.Empty,
                                          JsonConvert.SerializeObject(fiscalSetOnErrorResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                         );
                return fiscalSetOnErrorResponse;
            }
            catch(Exception ex)
            {
                FiscalSetOnErrorResponse fiscalSetOnErrorResponse = new FiscalSetOnErrorResponse(eFiscalResponseType.FAILURE,ex.Message);
                Program.RequestResponseLogger.Log(currentMesssageID,
                                          RequestResponseConstants.DiSignService,
                                          fiscalRequestWasSendBy,
                                          string.Empty,
                                          JsonConvert.SerializeObject(fiscalSetOnErrorResponse, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                         );
                return fiscalSetOnErrorResponse;
            }
        }
    }
}
