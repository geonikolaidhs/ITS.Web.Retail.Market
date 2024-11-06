using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Forms;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Resources;
using ITS.POS.Client.Exceptions;
using ITS.POS.Hardware;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Displays the WithdrawDeposit Form which in turn processes a withdrawal.
    /// </summary>
    public class ActionDisplayWithdrawal : Action
    {

        public ActionDisplayWithdrawal(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.DISPLAY_WITHDRAWAL; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION1;
            }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IActionManager actionManager = Kernel.GetModule<IActionManager>();

            if (totalizersService.CheckIfMustIssueZ(appContext.CurrentDailyTotals))
            {
                string errorMessage = String.Format(POSClientResources.YOU_MUST_ISSUE_Z, appContext.CurrentDailyTotals.FiscalDate);
                formManager.ShowCancelOnlyMessageBox(errorMessage);
                throw new POSException(errorMessage);
            }
            actionManager.GetAction(eActions.OPEN_DRAWER).Execute(new ActionOpenDrawerParams(deviceManager.GetPrimaryDevice<Drawer>(), false), true);
            using (frmWithdrawDeposit ff = new frmWithdrawDeposit(eOpenDrawerMode.WITHDRAW, this.Kernel))
            {
                ff.ShowDialog();
            }

        }

    }
}