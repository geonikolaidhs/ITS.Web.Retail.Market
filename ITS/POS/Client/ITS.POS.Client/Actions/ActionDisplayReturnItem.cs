using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Displays the ReturnItem form, which in turn processes an item return
    /// </summary>
    public class ActionDisplayReturnItem : Action
    {
        public ActionDisplayReturnItem(IPosKernel kernel) : base(kernel)
        {
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT; }
        }

        public override eActions ActionCode
        {
            get {  return eActions.DISPLAY_RETURN_ITEM_FORM; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION1;
            }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IFormManager FormManager = Kernel.GetModule<IFormManager>();
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            FormManager.ShowForm<frmReturnItem>(AppContext.MainForm, false, true, true);
        }
    }
}
