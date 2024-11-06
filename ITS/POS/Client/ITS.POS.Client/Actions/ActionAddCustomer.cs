using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations.ViewModel;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Displays the "Select Customer" form and after the user selects a customer, the internal action: ADD_CUSTOMER_INTERNAL is called
    /// </summary>
    public class ActionAddCustomer : Action
    {
        public ActionAddCustomer(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.OPENDOCUMENT; }
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
            using (frmSelectCustomer form = new frmSelectCustomer(this.Kernel))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Customer cust = form.SelectedCustomer;
                    string lookupCode = form.CustomerLookupCode;
                    Address address = form.SelectedAddress;
                    InsertedCustomerViewModel customerViewModel = form.CustomerViewModel;
                    IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                    actionManager.GetAction(eActions.ADD_CUSTOMER_INTERNAL).Execute(new ActionAddCustomerInternalParams(cust,lookupCode, address, customerViewModel));
                }
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_CUSTOMER; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }
    }
}
