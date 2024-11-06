using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware.Common;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    public class ActionEdpsCheckCommunication : Action
    {
        public ActionEdpsCheckCommunication(IPosKernel kernel)
            : base(kernel)
        {
            
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return 
                eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED
                | eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT 
                | eMachineStatus.SALE | eMachineStatus.UNKNOWN; }
        }

        public override eActions ActionCode
        {
            get { return eActions.EDPS_CHECK_COMMUNICATION; }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();
            EdpsPaymentCreditDevice edpsDevice = deviceManager.Devices.FirstOrDefault(x => x is EdpsPaymentCreditDevice) as EdpsPaymentCreditDevice;
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            if(edpsDevice == null)
            {
                throw new POSException(POSClientResources.NO_PRIMARY_EDPS_FOUND);
            }
            formManager.ShowMessageBox(edpsDevice.TestCommunication() ? POSClientResources.OK: POSClientResources.EDPS_COMMUNICATION_ERROR);
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
