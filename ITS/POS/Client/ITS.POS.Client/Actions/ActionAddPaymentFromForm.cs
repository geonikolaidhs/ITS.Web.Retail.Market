using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Opens up list of all payment methods in a SelectLookUpForm so that the user can select which payment method to apply. 
    /// Afterwards "ActionAddPayment" is called
    /// </summary>
    public class ActionAddPaymentFromForm : Action
    {

        public ActionAddPaymentFromForm(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT; }
        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_PAYMENT_FROM_FORM; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
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
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            IEnumerable<BaseObj> datasource = new XPCollection<PaymentMethod>(sessionManager.GetSession<PaymentMethod>());
            List<string> columns = new List<string> { "Description" };
            object selectedValue = datasource.FirstOrDefault() == null ? null : (object)datasource.FirstOrDefault().Oid;
            using (frmSelectLookup form = new frmSelectLookup(POSClientResources.SELECT_PAYMENT_METHOD, datasource, "Description", "Oid", columns, selectedValue,this.Kernel))
            {
                form.ShowDialog();

                if (form.DialogResult == System.Windows.Forms.DialogResult.OK && form.SelectedObject != null)
                {
                    actionManager.GetAction(eActions.ADD_PAYMENT).Execute(new ActionAddPaymentParams(form.SelectedObject as PaymentMethod, (parameters as ActionAddPaymentFromFormParams).Amount), true);
                }
            }
        }
    }
}
