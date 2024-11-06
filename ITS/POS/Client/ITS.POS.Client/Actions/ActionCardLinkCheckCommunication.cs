using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Transactions;
using DevExpress.Xpo;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;

namespace ITS.POS.Client.Actions
{
    public class ActionCardLinkCheckCommunication : Action
    {
        public ActionCardLinkCheckCommunication(IPosKernel kernel) : base(kernel)
        {
        }

        public override eActions ActionCode
        {
            get { return eActions.CARDLINK_CHECK_COMMUNICATION; }
        }



        public override eMachineStatus ValidMachineStatuses
        {
            get
            {
                return
              eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED
              | eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT
              | eMachineStatus.SALE | eMachineStatus.UNKNOWN;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();
            CardlinkPaymentCreditDevice device = deviceManager.Devices.FirstOrDefault(x => x is CardlinkPaymentCreditDevice) as CardlinkPaymentCreditDevice;
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            if (device == null)
            {
                throw new POSException(POSClientResources.NO_PRIMARY_EDPS_FOUND);
            }
            formManager.ShowMessageBox(CardlinkLink.TestComunnication(device.Settings.Ethernet) ? POSClientResources.OK : POSClientResources.EDPS_COMMUNICATION_ERROR);
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
