using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
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
    ///  For EAFDSS only. Reprints old Z reports from the fiscal printer, either by date or by number
    /// </summary>
    public class ActionFiscalPrinterReprintZReports : Action
    {

        public ActionFiscalPrinterReprintZReports(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.SALE | eMachineStatus.PAUSE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.FISCAL_PRINTER_REPRINT_Z_REPORTS; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION3;
            }
        }

        public override bool RequiresParameters
        {
            get { return true; }
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
            ActionFiscalPrinterReprintZReportsParams castedParams = (ActionFiscalPrinterReprintZReportsParams)parameters;
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            FiscalPrinter printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();

            if (printer != null)
            {
                using (frmReprintZ form = new frmReprintZ(castedParams.UseDateFilter, this.Kernel))
                {
                    DialogResult res = form.ShowDialog();
                    if (res == DialogResult.OK)
                    {
                        if (castedParams.UseDateFilter)
                        {
                            DateTime fromZDateFilter = form.FromZDateFilter;
                            DateTime toZDateFilter = form.ToZDateFilter;

                            if ((toZDateFilter - fromZDateFilter).Ticks >= 0)
                            {
                                printer.ReprintZReportsDateToDate(fromZDateFilter, toZDateFilter,castedParams.Mode);
                            }
                            else
                            {
                                throw new POSException(POSClientResources.INVALID_FILTER);
                            }
                        }
                        else
                        {
                            int fromZFilter = form.FromZFilter;
                            int toZFilter = form.ToZFilter;

                            if ((toZFilter - fromZFilter) >=0)
                            {
                                printer.ReprintZReports(fromZFilter, toZFilter,castedParams.Mode);
                            }
                            else
                            {
                                throw new POSException(POSClientResources.INVALID_FILTER);
                            }
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
