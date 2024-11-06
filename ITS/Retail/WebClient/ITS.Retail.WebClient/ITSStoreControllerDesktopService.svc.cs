using ITS.Retail.Common;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers;
using POSCommandsLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace ITS.Retail.WebClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ITSStoreControllerDesktopService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ITSStoreControllerDesktopService.svc or ITSStoreControllerDesktopService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ITSStoreControllerDesktopService : IITSStoreControllerDesktopService
    {
        public bool SendPOSCommands(List<POSCommandDescription> commands)
        {
            bool commandsSend = false;
            try
            {
                foreach (POSCommandsLibrary.POSCommandDescription command in commands)
                {
                    if (command.POSOid == Guid.Empty)
                    {
                        string errorMessage;
                        POSHelper.ReloadScales(XpoHelper.GetNewUnitOfWork(), out errorMessage, StoreControllerAppiSettings.CurrentStore);
                    }
                    else
                    {
                        foreach (SerializableTuple<ePosCommand, string> tuple in command.POSCommandSet.Commands)
                        {
                            //ConcurrentDictionary<Guid, POSCommandSet> posCommandParameters = MvcApplication.posCommandParameters;
                            POSHelper.AddPosCommand(command.POSOid, tuple.Item1, tuple.Item2);
                            //MvcApplication.posCommandParameters = posCommandParameters;
                        }
                    }
                }
                commandsSend = true;
            }
            catch (Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
                commandsSend = false;
            }

            return commandsSend;
        }

        public bool SendMasterInfo(out eApplicationInstance appInstance, out string url)
        {
            appInstance = eApplicationInstance.STORE_CONTROLER;
            url = null;
            try
            {
                appInstance = MvcApplication.ApplicationInstance;
                url = StoreControllerAppiSettings.MasterServiceURL;
                return true;
            }
            catch (Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
                return false;
            }
        }

        public DBType GetDbType()
        {
            return XpoHelper.databasetype;
        }

        public long GetNowTicks()
        {
            return DateTime.Now.Ticks;
        }

        public ApplicationStatus GetApplicationStatus()
        {
            return MvcApplication.Status;
        }
    }
}
