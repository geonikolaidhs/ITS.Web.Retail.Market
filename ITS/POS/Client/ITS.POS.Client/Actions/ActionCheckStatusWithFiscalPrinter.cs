using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Micrelec.Fiscal;
using ITS.POS.Hardware.Wincor.Fiscal;
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
    /// Checks if the application's fiscal day status (Opened/Closed) is the same with the fiscal printer's. For internal use (not directly invoked by the user).
    /// </summary>
    public class ActionCheckStatusWithFiscalPrinter : Action
    {
        public ActionCheckStatusWithFiscalPrinter(IPosKernel kernel)
            : base(kernel)
        {

        }
        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.PAUSE | eMachineStatus.SALE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.CHECK_STATUS_WITH_FISCAL_PRINTER; }
        }

        public override eFiscalMethod ValidFiscalMethods
        {
            get
            {
                return eFiscalMethod.ADHME;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IActionManager actionManager = Kernel.GetModule<IActionManager>();
            ITotalizersService totalizerService = Kernel.GetModule<ITotalizersService>();
            IConfigurationManager configManager = Kernel.GetModule<IConfigurationManager>();

            FiscalPrinter primaryPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
            if (primaryPrinter == null)
            {
                throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
            }

            bool isApplicationDayOpen = appContext.GetMachineStatus() != eMachineStatus.CLOSED;
            primaryPrinter.ReadDeviceStatus();
            bool isFiscalPrinterDayOpen = primaryPrinter.FiscalStatus.DayOpen;

            bool ejDataPending = false;
            if (primaryPrinter is WincorFiscalPrinter)
            {
                ejDataPending = (primaryPrinter.FiscalStatus as WincorFiscalPrinterStatus).EJDataPending;
            }
            else if (primaryPrinter is MicrelecFiscalPrinter)
            {
                MicrelecFiscalPrinter micrelecPrinter = (MicrelecFiscalPrinter)primaryPrinter;
                ejDataPending = micrelecPrinter.ReadParameters() == MicrelecStatusCode.NOTTRANSFERFLASH;

            }

            if (isApplicationDayOpen != isFiscalPrinterDayOpen || ejDataPending == true)
            {
                bool shouldIssueZ = true;
                if (isFiscalPrinterDayOpen == false && isApplicationDayOpen == true && ejDataPending == false && primaryPrinter.FiscalDayStartedOnFirstReceipt == true)
                {
                    shouldIssueZ = totalizerService.GetNumberOfDocumentsInDailyTotals(appContext.CurrentDailyTotals, configManager.DefaultDocumentTypeOid) > 0;
                }
                if (shouldIssueZ)
                {
                    string errorMessage = String.Format(POSClientResources.FISCAL_PRINTER_STATUS_MISSMATCH, isFiscalPrinterDayOpen, isApplicationDayOpen);
                    DialogResult result = formManager.ShowMessageBox(errorMessage, System.Windows.Forms.MessageBoxButtons.OKCancel);
                    if (result == DialogResult.Cancel)
                    {
                        actionManager.GetAction(eActions.APPLICATION_EXIT).Execute();
                    }

                    ActionIssueZReportParams issueZActionParameters = null;

                    if (isApplicationDayOpen)
                    {
                        issueZActionParameters = new ActionIssueZReportParams(true, true);
                        actionManager.GetAction(eActions.SERVICE_FORCED_CANCEL_DOCUMENT).Execute(new ActionServiceForcedCancelDocumentParams(false), true, false);
                    }

                    if (isFiscalPrinterDayOpen || ejDataPending)
                    {
                        issueZActionParameters = new ActionIssueZReportParams(true, false);
                    }

                    actionManager.GetAction(eActions.ISSUE_Z).Execute(issueZActionParameters, true, false);
                }
            }

        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
