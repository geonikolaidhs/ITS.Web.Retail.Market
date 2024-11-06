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
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Displays the WithdrawDeposit Form which in turn processes a deposit.
    /// </summary>
    public class ActionDisplayDeposit : Action
    {

        public ActionDisplayDeposit(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.DISPLAY_DEPOSIT; }
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
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            IFormManager FormManager = Kernel.GetModule<IFormManager>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (totalizersService.CheckIfMustIssueZ(AppContext.CurrentDailyTotals))
            {
                string errorMessage = String.Format(POSClientResources.YOU_MUST_ISSUE_Z, AppContext.CurrentDailyTotals.FiscalDate);
                FormManager.ShowCancelOnlyMessageBox(errorMessage);
                throw new POSException(errorMessage);
            }

            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            actionManager.GetAction(eActions.OPEN_DRAWER).Execute(new ActionOpenDrawerParams(deviceManager.GetPrimaryDevice<Drawer>(), false), true);
            using (frmWithdrawDeposit ff = new frmWithdrawDeposit(eOpenDrawerMode.DEPOSIT,this.Kernel))
            {
                ff.ShowDialog();
            }

        }

    }
}