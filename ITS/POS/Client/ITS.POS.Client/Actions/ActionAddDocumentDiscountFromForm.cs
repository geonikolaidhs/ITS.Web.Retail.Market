using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using ITS.POS.Client.Forms;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Opens up list of all document discounts in a SelectLookUpForm so that the user can select which discount to apply. 
    /// Afterwards "ActionAddDocumentDiscount" is called
    /// </summary>
    public class ActionAddDocumentDiscountFromForm : Action
    {
        public ActionAddDocumentDiscountFromForm(IPosKernel kernel) : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT_PAYMENT; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_DOCUMENT_DISCOUNT_FROM_FORM; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            ActionAddDocumentDiscountFromFormParams castedParams = parameters as ActionAddDocumentDiscountFromFormParams;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (appContext.CurrentDocument != null)
            {
                IEnumerable<BaseObj> datasource = new XPCollection<DiscountType>(sessionManager.GetSession<DiscountType>(), new BinaryOperator("IsHeaderDiscount", true), new SortProperty("Code", DevExpress.Xpo.DB.SortingDirection.Ascending));
                List<string> columns = new List<string> { "Description" };
                object selectedValue = datasource.FirstOrDefault() == null ? null : (object)datasource.FirstOrDefault().Oid;
                using (frmSelectLookup form = new frmSelectLookup(POSClientResources.SELECT_DISCOUNT, datasource, "Description", "Oid", columns, selectedValue, Kernel))
                {
                    form.ShowDialog();

                    if (form.DialogResult == System.Windows.Forms.DialogResult.OK && form.SelectedObject != null)
                    {
                        decimal discount = (form.SelectedObject as DiscountType).eDiscountType == eDiscountType.PERCENTAGE ? (castedParams.ValueOrPercentage / 100) : castedParams.ValueOrPercentage;
                        actionManager.GetAction(eActions.ADD_DOCUMENT_DISCOUNT).Execute(new ActionAddDocumentDiscountParams(discount, form.SelectedObject as DiscountType), true);
                    }
                }
            }

        }
    }
}
