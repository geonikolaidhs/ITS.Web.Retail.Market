using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Displays the FindItem form, which finds a line in the current open document by item code.
    /// </summary>
    public class ActionDisplayFindItem : Action
    {

        public ActionDisplayFindItem(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.FIND_ITEM; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            using (frmFindItem ff = new frmFindItem(this.Kernel))
            {
                foreach (IObserver observer in this.Observers)
                {
                    ff.Attach(observer);
                }
                ff.ShowDialog();
            }
        }

    }
}
