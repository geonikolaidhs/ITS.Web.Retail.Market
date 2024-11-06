using ITS.POS.Client.Actions.ActionParameters;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;

namespace ITS.POS.Client.Actions
{
    public class ActionPrinterFeed : Action
    {
        IDeviceManager deviceManager;

        public ActionPrinterFeed(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get
            {
                return eActions.PRINTER_FEED;
            }
        }

        public override bool RequiresParameters
        {
            get
            {
                return false;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get
            {
                return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.OPENDOCUMENT | eMachineStatus.SALE | eMachineStatus.UNKNOWN;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            deviceManager = this.Kernel.GetModule<IDeviceManager>();
            deviceManager.Devices.Where(x => x is Printer).Cast<Printer>().ToList().ForEach(x=> x.Print("\r\n\r\n\r\n\r\n\r\n"));
        }
    }
}
