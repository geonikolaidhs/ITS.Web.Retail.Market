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
    /// Publishes the give line's quantity to the attached observers. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionPublishLineQuantityInfo : Action
    {
        public ActionPublishLineQuantityInfo(IPosKernel kernel) : base(kernel)
        {

        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            Notify(new NumberDisplayerParams((parameters as ActionPublishLineQuantityInfoParams).Qty, (parameters as ActionPublishLineQuantityInfoParams).Format));
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
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
            get { return eActions.PUBLISH_LINE_QUANTITY_INFO; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }
    }
}
