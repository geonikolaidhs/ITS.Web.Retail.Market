using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Helpers;
using ITS.POS.Hardware;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// For EAFDSS only. Opens a fiscal day only on the device
    /// </summary>
    public class ActionServiceStartDayFiscalPrinter : Action
    {
        public ActionServiceStartDayFiscalPrinter(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.PAUSE | eMachineStatus.SALE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.SERVICE_FORCED_START_DAY_FISCAL_PRINTER; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }


        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION3;
            }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {

            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();

            DialogResult result = DialogResult.OK;
            string message = POSClientResources.WARNING.ToUpperGR() + "!!!" + Environment.NewLine + POSClientResources.DO_NOT_PROCEED_IF_YOU_DONT_KNOW;
            result = formManager.ShowMessageBox(message, MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK && config.FiscalMethod == eFiscalMethod.ADHME)
            {
                FiscalPrinter printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                if (printer != null)
                {
                    try
                    {
                        printer.OpenFiscalDay(config.TerminalID);
                    }
                    catch
                    {
                    }
                }
            }

        }
    }
}
