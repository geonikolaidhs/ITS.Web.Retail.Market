using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Forms;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using ITS.POS.Resources;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Starts a new shift by prompting the user for his/her credentials.
    /// </summary>
    public class ActionStartShift : Action
    {

        public ActionStartShift(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.START_SHIFT; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.DAYSTARTED; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {

            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                if (!deviceManager.HasDemoModeBeenSetupCorrectly(config.DemoMode, config.FiscalMethod, config.FiscalDevice))
                {
                    throw new Exception(POSClientResources.DEMO_MODE_ERROR);
                }

                if (appContext.GetMachineStatus() == eMachineStatus.DAYSTARTED)
                {
                    if (config.AsksForStartingAmount)
                    {
                        actionManager.GetAction(eActions.OPEN_DRAWER).Execute(new ActionOpenDrawerParams(deviceManager.GetPrimaryDevice<Drawer>(), false), true);
                    }
                    if (appContext.MainForm.Visible == true)
                    {
                        formManager.ShowForm<frmSplashScreen>(appContext.MainForm, true);
                    }
                    else if (appContext.SplashForm.Visible == true)
                    {
                        try
                        {
                            if (formManager.ShowForm<frmLogin>(appContext.SplashForm, false, true) == System.Windows.Forms.DialogResult.OK)
                            {
                                appContext.SplashForm.Close();
                                appContext.SplashForm = null;
                            }
                        }
                        catch (InvalidOperationException invalidOperationException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Kernel.LogFile.Info(ex, "Action Start Shift Error");
                            actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
                            formManager.ShowCancelOnlyMessageBox(ex.GetFullMessage());
                        }
                    }
                    actionManager.GetAction(eActions.SHOW_BLINKING_ERROR).Execute(new ActionShowBlinkingErrorParams(false));
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Kernel.LogFile.Info(invalidOperationException, "Action Start Shift Error InvalidOperationException");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(invalidOperationException.GetFullMessage()));
                formManager.ShowCancelOnlyMessageBox(invalidOperationException.GetFullMessage());
            }
        }
    }
}
