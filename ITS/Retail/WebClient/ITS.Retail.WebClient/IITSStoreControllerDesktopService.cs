using ITS.Retail.Common;
using ITS.Retail.Platform.Enumerations;
using POSCommandsLibrary;
using System.Collections.Generic;
using System.ServiceModel;

namespace ITS.Retail.WebClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IITSStoreControllerDesktopService" in both code and config file together.
    [ServiceContract]
    public interface IITSStoreControllerDesktopService
    {
        [OperationContract]
        bool SendPOSCommands(List<POSCommandDescription> commands);
        
        [OperationContract]
        bool SendMasterInfo(out eApplicationInstance appInstance, out string url);

        [OperationContract]
        DBType GetDbType();

        [OperationContract]
        long GetNowTicks();

        [OperationContract]
        ApplicationStatus GetApplicationStatus();
    }
}
