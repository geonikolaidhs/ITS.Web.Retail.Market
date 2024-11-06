using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPublishMachineStatusParams : ActionParams
    {
        public eMachineStatus MachineStatus {get; protected set;}

        public override Retail.Platform.Enumerations.eActions ActionCode
        {
            get { return eActions.PUBLISH_MACHINE_STATUS; }
        }

        public ActionPublishMachineStatusParams(eMachineStatus machineStatus)
        {
            this.MachineStatus = machineStatus;
        }

    }
}
