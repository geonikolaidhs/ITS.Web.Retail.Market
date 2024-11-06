using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using ITS.POS.Client.Helpers;
using ITS.Retail.Platform.Common.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using System.Media;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Performs an item return for the given item code, with the given quantity.
    /// </summary>
    public class ActionReturnItem : Action
    {

        public ActionReturnItem(IPosKernel kernel) : base(kernel)
        {
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT; }
        }

        public override eActions ActionCode
        {
            get { return eActions.RETURN_ITEM; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
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
            ActionReturnItemParams parcedParams = parameters as ActionReturnItemParams;

            decimal qty = (-1) * parcedParams.Quantity;
            string code = parcedParams.Code;
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IScannedCodeHandler scannedCodeHandler = Kernel.GetModule<IScannedCodeHandler>();

            using(IGetItemPriceForm form = new frmGetItemPrice(Kernel))
            {
                scannedCodeHandler.HandleScannedCode(form,config.GetAppSettings(),config.POSSellsInactiveItems, code, qty, false, true, parcedParams.FromScanner, false);
            }
        }


    }
}
