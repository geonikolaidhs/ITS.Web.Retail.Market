using System;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Displays the CheckPrice Form
    /// </summary>
    public class ActionDisplayCheckPrice : Action
    {

        public ActionDisplayCheckPrice(IPosKernel kernel) : base(kernel)
        {
        }

        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            frmCheckPrice ff = new frmCheckPrice(Kernel);
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ff.ShowDialog(appContext.MainForm);
            ff.Dispose();

        }

        public override eActions ActionCode
        {
            get { return eActions.CHECKPRICE; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT; }
        }
    }
}
