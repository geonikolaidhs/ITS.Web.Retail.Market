using System;
using System.Collections.Generic;
using System.Linq;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Model.Master;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Adds the given line discount to the current Document Detail
    /// </summary>
    public class ActionAddLineDiscount : Action
    {
        public ActionAddLineDiscount(IPosKernel kernel)
            : base(kernel)
        {
        }

        public override eActions ActionCode
        {
            get { return eActions.ADD_LINE_DISCOUNT; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.OPENDOCUMENT; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            DocumentDetail detail = appContext.CurrentDocumentLine;
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (detail == null)
            {
                throw new POSException(POSClientResources.SELECT_LINE);
            }

            if (detail.Item == null)
            {
                throw new POSException(POSClientResources.SELECT_LINE);
            }

            if (detail.IsReturn)
            {
                throw new POSException(POSClientResources.CANNOT_ADD_DISCOUNT_TO_RETURN);
            }

            if (detail.IsCanceled)
            {
                throw new POSException(POSClientResources.CANNOT_ADD_DISCOUNT_TO_CANCELED_LINE);
            }

            if (detail.IsTax || detail.DoesNotAllowDiscount)
            {
                throw new POSException(POSClientResources.INVALID_DISCOUNT);
            }

            ActionAddLineDiscountParams castedParameters = (parameters as ActionAddLineDiscountParams);
            DiscountType discountType = castedParameters.DiscountType;
            if (discountType.eDiscountType == eDiscountType.PERCENTAGE)
            {
                decimal percentage = castedParameters.ValueOrPercentage;
                if (percentage > 1 || percentage < 0)
                {
                    throw new POSException(POSClientResources.INVALID_DISCOUNT);
                }
            }
            else
            {
                decimal value = castedParameters.ValueOrPercentage;
                DocumentType docType = sessionManager.GetObjectByKey<DocumentType>(detail.DocumentHeader.DocumentType);
                decimal amountToCheck = docType.IsForWholesale ? detail.NetTotal : detail.GrossTotal;
                if (value > amountToCheck || value < 0)
                {
                    throw new POSException(POSClientResources.INVALID_DISCOUNT);
                }
            }

            DocumentDetailDiscount discount = documentService.CreateOrUpdateCustomDiscount(discountType, castedParameters.ValueOrPercentage, detail);

            XPCollection<DiscountTypeField> discountTypeFields = new XPCollection<DiscountTypeField>(sessionManager.GetSession<DiscountTypeField>(), new BinaryOperator("DiscountType", discountType.Oid));

            Dictionary<Guid, object> dictionary = new Dictionary<Guid, object>();

            if (castedParameters.CouponViewModel != null && discountTypeFields.Select(x => x.FieldName).Contains(castedParameters.CouponViewModel.PropertyName))
            {
                dictionary.Add(discountTypeFields.FirstOrDefault(x => x.FieldName == castedParameters.CouponViewModel.PropertyName).Oid, castedParameters.CouponViewModel.DecodedString);
            }
            IEnumerable<DiscountTypeField> remainingFields = discountTypeFields.Where(x => dictionary.ContainsKey(x.Oid) == false);
            if (remainingFields.Count() > 0)
            {
                using (Form form = formManager.CreateCustomFieldsInputForm(remainingFields))
                {
                    form.ShowDialog();
                    actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(new string[] { "" }));

                    if (form.DialogResult == DialogResult.OK)
                    {
                        foreach (Control control in form.Controls)
                        {
                            if (discountTypeFields.Where(x => x.Oid.ToString() == control.Name).Count() > 0)
                            {
                                if (control is LookUpEdit)
                                {
                                    dictionary.Add(Guid.Parse(control.Name), (control as LookUpEdit).EditValue);
                                }
                                else
                                {
                                    dictionary.Add(Guid.Parse(control.Name), control.Text);
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            foreach (KeyValuePair<Guid, object> pair in dictionary)
            {
                DiscountTypeField matchedField = discountTypeFields.Where(x => x.Oid == pair.Key).FirstOrDefault();
                object convertedValue = Convert.ChangeType(pair.Value, typeof(DocumentPayment).GetProperty(matchedField.FieldName).PropertyType);
                discount.GetType().GetProperty(matchedField.FieldName).SetValue(discount, convertedValue, null);
            }

            DocumentHeader header = detail.DocumentHeader;
            if (castedParameters.CouponViewModel != null)
            {
                documentService.CreateTransactionCoupon(header, castedParameters.CouponViewModel, discount);
            }
            detail.DocumentDetailDiscounts.Add(discount);
            documentService.RecalculateDocumentDetail(detail, header);
            documentService.RecalculateDocumentCosts(header, false);
            actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(header));
            actionManager.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(detail, true, true));

            appContext.CurrentDocument.Save();
            sessionManager.FillDenormalizedFields(appContext.CurrentDocumentLine);
            appContext.CurrentDocumentLine.Save();
            sessionManager.CommitTransactionsChanges();
        }
    }
}