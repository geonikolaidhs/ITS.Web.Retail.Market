using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Performs a slip print. Used if the payment method needs ratification. 
    /// For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionSlipPrint : Action
    {

        public ActionSlipPrint(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.SALE | eMachineStatus.PAUSE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.SLIP_PRINT; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            ActionSlipPrintParams castedParams = (ActionSlipPrintParams)parameters;
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();

            Device device = castedParams.Printer;
            if (device is FiscalPrinter)
            {
                FiscalPrinter printer = (FiscalPrinter)device;
                if (printer.HasSlipStation == false)
                {
                    formManager.ShowCancelOnlyMessageBox(POSClientResources.PRINTER_DOES_NOT_SUPPORT_SLIP_STATION);
                    return;
                }

                DialogResult dialogResult = DialogResult.OK;


                dialogResult = formManager.ShowMessageBox(POSClientResources.PLEASE_INSERT_DOCUMENT, MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }

                do
                {
                    try
                    {
                        DeviceResult result = printer.SlipStationPrint(castedParams.Lines.Select(x => new FiscalLine() { Value = x }).ToArray());
                        if (result != DeviceResult.SUCCESS)
                        {
                            throw new POSException("Station print failed");
                        }

                        dialogResult = DialogResult.OK;
                    }
                    catch (Exception ex)
                    {
                        dialogResult = formManager.ShowMessageBox(POSClientResources.SLIP_PRINT_FAILED + Environment.NewLine + ex.GetFullMessage(), MessageBoxButtons.RetryCancel);
                    }
                } while (dialogResult == DialogResult.Retry);

            }
            else if (device is Printer)
            {
                //TODO implement for normal slip printers 
            }
            else
            {
                throw new POSException("ActionSlipPrint call error. Device provided is not a Printer, Fiscal Printer or is null.");
            }
        }
    }
}
