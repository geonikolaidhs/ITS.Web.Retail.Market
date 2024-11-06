using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Sets the application into the "PAUSE" state. The user must enter their credentials to unlock the application
    /// </summary>
    public class ActionPause : Action
    {
        public ActionPause(IPosKernel kernel) : base(kernel)
        {
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get
            {
                return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.OPENDOCUMENT |
              eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.PAUSE | eMachineStatus.SALE | eMachineStatus.UNKNOWN;
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.PAUSE; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions)
        {
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            eMachineStatus previousStatus = AppContext.GetMachineStatus();
            try
            {
                AppContext.SetMachineStatus(eMachineStatus.PAUSE);
                if (config.UseSliderPauseForm == true)
                {
                    using (frmPauseSlider form = new frmPauseSlider(this.Kernel, config.PauseFormImages))
                    {
                        form.ShowDialog();
                    }
                }
                else
                {
                    using (frmPause form = new frmPause(this.Kernel))
                    {
                        form.ShowDialog();
                    }
                }

            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex, "Error at pausing");
            }
            finally
            {
                AppContext.SetMachineStatus(previousStatus);
            }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
