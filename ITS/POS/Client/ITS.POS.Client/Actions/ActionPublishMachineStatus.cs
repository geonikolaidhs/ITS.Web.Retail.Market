using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Synchronization;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Publishes info about the application's status to the attached observers. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionPublishMachineStatus : Action
    {

        public ActionPublishMachineStatus(IPosKernel kernel) : base(kernel)
        {
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.PAUSE | eMachineStatus.SALE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.PUBLISH_MACHINE_STATUS; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.UNKNOWN;
            }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            ActionPublishMachineStatusParams castedParams = parameters as ActionPublishMachineStatusParams;
            ISynchronizationManager SynchronizationManager = Kernel.GetModule<ISynchronizationManager>();
            Notify(new StatusDisplayerParams(SynchronizationManager.ServiceIsAlive, SynchronizationManager.CurrentDownloadingStatus, SynchronizationManager.CurrentDownloadingType, SynchronizationManager.CurrentUploadingStatus,
                SynchronizationManager.CurrentUploadingType, SynchronizationManager.CurrentDownloadingProgress, SynchronizationManager.CurrentUploadingProgress, castedParams.MachineStatus));
        }
    }
}
