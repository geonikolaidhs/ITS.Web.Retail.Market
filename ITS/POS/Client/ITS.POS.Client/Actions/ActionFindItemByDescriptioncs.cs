using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace ITS.POS.Client.Actions
{
    class ActionFindItemByDescription : Action
    {
        public ActionFindItemByDescription(IPosKernel kernel) : base(kernel)
        {
        }
        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            using (frmFindItemByDesc form = new frmFindItemByDesc(this.Kernel))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    form.Dispose();
                }

            }

        }
        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.OPENDOCUMENT; }
        }
        public override eActions ActionCode
        {
            get { return eActions.FIND_ITEM_BY_DESCRIPTION; }
        }
        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}