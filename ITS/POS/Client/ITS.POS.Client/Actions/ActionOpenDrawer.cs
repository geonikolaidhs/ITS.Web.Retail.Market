using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.POS.Hardware;
using System.Threading.Tasks;
using ITS.POS.Model.Master;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Opens the given drawer's tray
    /// </summary>
    public class ActionOpenDrawer : Action
    {
        public ActionOpenDrawer(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.OPEN_DRAWER; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.OPENDOCUMENT; }
        }

        private object lockObject = new object();

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            Drawer drawer = (parameters as ActionOpenDrawerParams).Drawer; //(GlobalContext.DeviceManager.GetPrimaryDevice<Drawer>() as Drawer);
            bool increaseTotalizor = (parameters as ActionOpenDrawerParams).IncreaseTotalizor;
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();

            if (drawer != null)
            {
                DeviceResult result = drawer.OpenDrawer();

                if (result == DeviceResult.SUCCESS)
                {
                    if (increaseTotalizor && appContext.CurrentUserDailyTotals != null && appContext.CurrentDailyTotals != null)
                    {
                        totalizersService.IncreaseDrawer(appContext.CurrentUser, appContext.CurrentDailyTotals, appContext.CurrentUserDailyTotals,
                            sessionManager.GetObjectByKey<Store>(config.CurrentStoreOid), sessionManager.GetObjectByKey<Model.Settings.POS>(config.CurrentTerminalOid));
                    }
                }
                else
                {
                    string errorMessage = POSClientResources.ERROR + ": " + POSClientResources.FAIL_OPEN_DRAWER + ". " + result.ToLocalizedString();
                    appContext.MainForm.Invoke((MethodInvoker)delegate()
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(errorMessage));
                    });
                }
            }
        }
    }
}
