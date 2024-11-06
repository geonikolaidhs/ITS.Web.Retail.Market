using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Actions
{
    public class ActionCancelNotIncludedItems : Action
    {
        public ActionCancelNotIncludedItems(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions = false)
        {
            
        }

        public override eActions ActionCode
        {
            get { return eActions.CANCEL_NOT_INCLUDED_ITEMS; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
