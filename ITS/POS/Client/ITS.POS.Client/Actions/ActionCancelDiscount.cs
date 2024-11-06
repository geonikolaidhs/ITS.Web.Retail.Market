using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Forms;
using ITS.POS.Resources;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Cancels a discount (line or document depending on machine status) after user confirmation. 
    /// If more than one discount is currently applied, a SelectLookup form is used so that the user can choose a discount.
    /// </summary>
    public class ActionCancelDiscount : Action
    {
        public ActionCancelDiscount(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT; }
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
            get { return eActions.CANCEL_DISCOUNT; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }


        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            OwnerApplicationSettings appSettings = config.GetAppSettings();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            DocumentDetail currentDetail = appContext.CurrentDocumentLine;

            if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT && currentDetail != null)
            {
                DocumentDetailDiscount discountToDelete = null;
                int customDiscountsCount = currentDetail.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM).Count();
                if (customDiscountsCount == 0)
                {
                    //do nothing
                    return;
                }
                else if (customDiscountsCount == 1)
                {
                    discountToDelete = currentDetail.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.CUSTOM);
                }
                else
                {

                    IEnumerable<BaseObj> datasource = currentDetail.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM);
                    List<string> columns = new List<string> { "DisplayName", "Value" };
                    object selectedValue = datasource.FirstOrDefault() == null ? null : (object)datasource.FirstOrDefault().Oid;
                    using (frmSelectLookup form = new frmSelectLookup(POSClientResources.SELECT_DISCOUNT, datasource, "DisplayName", "Oid", columns, selectedValue, this.Kernel))
                    {
                        form.ShowDialog();
                        if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                        {
                            discountToDelete = form.SelectedObject as DocumentDetailDiscount;
                        }
                    }
                }

                if (discountToDelete != null)
                {
                    DialogResult result = formManager.ShowMessageBox(discountToDelete.DisplayName + " - " + POSClientResources.DELETE_DISCOUNT_QUESTION, MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        currentDetail.DocumentDetailDiscounts.Remove(discountToDelete);
                        DeleteDiscountTransactionCoupon(appContext, discountToDelete);
                        discountToDelete.Delete();

                        DocumentHeader header = currentDetail.DocumentHeader;
                        documentService.RecalculateDocumentDetail(currentDetail, header);
                        documentService.RecalculateDocumentCosts(header, false);
                        actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(header));
                        actionManager.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(currentDetail, true, true));


                        appContext.CurrentDocument.Save();
                        sessionManager.FillDenormalizedFields(appContext.CurrentDocumentLine);
                        appContext.CurrentDocumentLine.Save();
                        sessionManager.CommitTransactionsChanges();
                    }
                }

            }
            else if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
            {
                DocumentHeader header = appContext.CurrentDocument;
                if (header.DocumentDiscountType != Guid.Empty)
                {
                    DialogResult result = formManager.ShowMessageBox("\"" + POSClientResources.DOCUMENT_DISCOUNT + "\"" + Environment.NewLine
                                                                    + POSClientResources.DELETE_DISCOUNT_QUESTION, MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {

                        header.DocumentDiscountType = Guid.Empty;
                        header.DocumentDiscountPercentage = 0;
                        header.DocumentDiscountPercentagePerLine = 0;
                        header.DocumentDiscountAmount = 0;

                        foreach (DocumentDetail detail in header.DocumentDetails)
                        {
                            DocumentDetailDiscount docDiscount = detail.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.DOCUMENT);
                            if (docDiscount != null)
                            {
                                detail.DocumentDetailDiscounts.Remove(docDiscount);
                                //DeleteDiscountTransactionCoupon(appContext, docDiscount);
                                docDiscount.Delete();
                            }
                        }
                        TransactionCoupon transactionCoupon = appContext.CurrentDocument.TransactionCoupons
                            .FirstOrDefault(transCoupon => 
                                transCoupon.IsCanceled == false
                            && transCoupon.DocumentDetailDiscount == null 
                            && transCoupon.DocumentPayment == null);

                        if (transactionCoupon != null)
                        {
                            transactionCoupon.IsCanceled = true;
                            transactionCoupon.Save();
                        }

                        documentService.RecalculateDocumentCosts(header, true);

                        actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(header));
                        appContext.CurrentDocument.Save();
                        sessionManager.CommitTransactionsChanges();
                    }
                }
            }
        }

        private static void DeleteDiscountTransactionCoupon(IAppContext appContext, DocumentDetailDiscount discountToDelete)
        {
            TransactionCoupon transactionCoupon = appContext.CurrentDocument.TransactionCoupons
                .Where(transCoupon =>
                           transCoupon.IsCanceled == false
                        && transCoupon.DocumentDetailDiscount != null
                        && transCoupon.DocumentDetailDiscount.Oid == discountToDelete.Oid
                      )
                .FirstOrDefault();

            if (transactionCoupon != null)
            {
                transactionCoupon.IsCanceled = true;
                transactionCoupon.Save();
            }
        }
    }
}
