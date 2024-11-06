using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Master;
using ITS.POS.Client.Helpers;
using ITS.POS.Hardware;
using ITS.POS.Resources;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Gets the weight from the primary Scale device and calls "ActionAddItem" 
    /// with the given item code and the extracted weight as quantity
    /// </summary>
    public class ActionAddWeightedItem : Action
    {

        public ActionAddWeightedItem(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_ITEM_WEIGHTED; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT | eMachineStatus.SALE; }
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
            ActionAddWeightedItemParams actionParameters = parameters as ActionAddWeightedItemParams;
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            /*
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            

            Scale primaryScale = deviceManager.GetPrimaryDevice<Scale>();
            if (primaryScale == null)
            {
                formManager.ShowCancelOnlyMessageBox(POSClientResources.NO_PRIMARY_SCALE_FOUND);
            }
            else
            {
                decimal weight = primaryScale.ReadWeight();
                if (weight <= 0)
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_QUANTITY));
                }
                else
                {
                    actionManager.GetAction(eActions.ADD_ITEM).Execute(new ActionAddItemParams(actionParameters.ItemCode, weight) { CheckForWeightedItem = true }, true);
                }
            }*/
            actionManager.GetAction(eActions.ADD_ITEM).Execute(new ActionAddItemParams(actionParameters.ItemCode, 0) { CheckForWeightedItem = true,  ReadWeight = true}, true);
        }
    }
}
