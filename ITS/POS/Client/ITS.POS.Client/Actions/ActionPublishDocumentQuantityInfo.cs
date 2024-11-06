using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Publishes the given document total quantity to the attached observers. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionPublishDocumentQuantityInfo : Action
    {
        public ActionPublishDocumentQuantityInfo(IPosKernel kernel) : base(kernel)
        {

        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            Notify(new NumberDisplayerParams((parameters as ActionPublishDocumentQuantityInfoParams).Qty, (parameters as ActionPublishDocumentQuantityInfoParams).Format));
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED;  }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.UNKNOWN;
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.PUBLISH_DOCUMENT_QUANTITY; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }
    }
}
