using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Notifies all the appropriate UI components about the application's communication status
    /// </summary>
    public class ActionUpdateCommunicationStatus : Action
    {
        public ActionUpdateCommunicationStatus(IPosKernel kernel) : base(kernel)
        {
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            ActionUpdateCommunicationStatusParams param = parameters as ActionUpdateCommunicationStatusParams;

            Notify(new StatusDisplayerParams(param.IsConnected,param.DownloadingStatus,param.DownloadingType,param.UploadingStatus,param.UploadingType,param.DownloadProgress,param.UploadProgress,param.MachineStatus));
        }

        public override eActions ActionCode
        {
            get { return eActions.UPDATE_COMMUNICATION_STATUS; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.UNKNOWN;
            }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }
    }
}
