using ITS.Common.Communication;
using ITS.POS.Fiscal.Common;
using ITS.POS.Fiscal.Common.Requests;
using ITS.POS.Fiscal.Common.Responses;
using ITS.POS.Fiscal.Service.RequestResponceLogging;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Service.Listeners
{
    public class FiscalServerGetOnlineListener : IMessageListener<FiscalGetOnlineRequest>
    {
        FiscalServer server;
        public FiscalServerGetOnlineListener(FiscalServer server)
        {
            this.server = server;
        }

        public IMessage OnMessageReceived(MessageListenerEventArgs args)
        {
            int currentMesssageID = ++Program.MessagesReceived;
            FiscalGetOnlineRequest receivedMessage = (FiscalGetOnlineRequest)args.Message;
            string fiscalRequestWasSendBy = "";
            Program.RequestResponseLogger.Log(currentMesssageID,
                                      fiscalRequestWasSendBy,
                                      RequestResponseConstants.DiSignService,
                                      JsonConvert.SerializeObject(receivedMessage, RequestResponseConstants.JSON_SERIALIZER_SETTINGS),
                                      string.Empty
                                     );


            bool reloadSettings = this.server.ReloadSettings;
            this.server.ReloadSettings = false; //Signals the UI to reload the settings. One-time
            FiscalGetOnlineResponse fiscalGetOnlineResponse = new FiscalGetOnlineResponse(server.Online ? eFiscalResponseType.SUCCESS : eFiscalResponseType.FAILURE, reloadSettings);
            Program.RequestResponseLogger.Log(currentMesssageID,
                                      RequestResponseConstants.DiSignService,
                                      fiscalRequestWasSendBy,                                      
                                      string.Empty,
                                      JsonConvert.SerializeObject(receivedMessage, RequestResponseConstants.JSON_SERIALIZER_SETTINGS)
                                     );
            return fiscalGetOnlineResponse;
        }
    }
}
