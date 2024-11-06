using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware;
using ITS.POS.Client.Forms;
using System.Windows.Forms;
using ITS.POS.Resources;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// For EAFDSS only. Requests the block range to print and then send a commands to the fiscal printer to print the blocks in that range.
    /// </summary>
    public class ActionFiscalPrinterPrintFiscalMemoryBlocks : Action
    {
        public ActionFiscalPrinterPrintFiscalMemoryBlocks(IPosKernel kernel) : base(kernel)
        {

        }


        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.SALE | eMachineStatus.PAUSE; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION3;
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.FISCAL_PRINTER_PRINT_FISCAL_MEMORY_BLOCKS; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override eFiscalMethod ValidFiscalMethods
        {
            get
            {
                return eFiscalMethod.ADHME;
            }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            FiscalPrinter printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();
            if (printer != null)
            {
                using (frmPrintFiscalMemoryBlocks form = new frmPrintFiscalMemoryBlocks(Kernel))
                {
                    DialogResult res = form.ShowDialog();
                    if (res == DialogResult.OK)
                    {

                        int fromBlock = form.FromBlock;
                        int toBlock = form.ToBlock;

                        if ((toBlock - fromBlock) >= 0)
                        {
                            printer.PrintFiscalMemoryBlocks(fromBlock, toBlock);
                        }
                        else
                        {
                            throw new POSException(POSClientResources.INVALID_FILTER);
                        }
                    }
                }
            }
            else
            {
                throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
            }
        }
    }
}
