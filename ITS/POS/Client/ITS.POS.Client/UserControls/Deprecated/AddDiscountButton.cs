using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using DevExpress.Data.Filtering;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Forms;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class AddDiscountButton : ucitsButton
    {
        public string DiscountTypeCode { get; set; }
        public decimal ValueOrPercentage { get; set; }


        public AddDiscountButton()
        {
            _button.Text = POSClientResources.DISCOUNT;
            InitializeComponent();
        }

        private void AddTotalPaymentButtonMethod_MouseClick(object sender, MouseEventArgs e)
        {
            Button_Click(sender, e);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                string inputText = appContext.MainForm.InputText.Trim();
                appContext.MainForm.ResetInputText();

                if (String.IsNullOrWhiteSpace(DiscountTypeCode))
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.DISCOUNT_TYPE_NOT_FOUND));
                    return;
                }

                DiscountType discountType = sessionManager.FindObject<DiscountType>(new BinaryOperator("Code", this.DiscountTypeCode));
                if (discountType != null)
                {
                    if (eMachineStatus.OPENDOCUMENT_PAYMENT != appContext.GetMachineStatus() && discountType.IsHeaderDiscount)
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.ACTION_FORBIDDEN));
                        return;
                    }
                    if (eMachineStatus.OPENDOCUMENT != appContext.GetMachineStatus() && discountType.IsHeaderDiscount == false)
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.ACTION_FORBIDDEN));
                        return;
                    }

                    decimal discount = 0;

                    if (this.ValueOrPercentage > 0)
                    {
                        discount = this.ValueOrPercentage;
                    }
                    else if (decimal.TryParse(inputText, out discount) == false)
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_AMOUNT));
                        return;
                    }

                    discount = discountType.eDiscountType == eDiscountType.PERCENTAGE ? (discount / 100) : discount;

                    if (discountType.IsHeaderDiscount)
                    {
                        actionManager.GetAction(eActions.ADD_DOCUMENT_DISCOUNT).Execute(new ActionAddDocumentDiscountParams(discount, discountType));
                    }
                    else
                    {
                        actionManager.GetAction(eActions.ADD_LINE_DISCOUNT).Execute(new ActionAddLineDiscountParams(discount, discountType,true));
                    }
                }
                else
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.DISCOUNT_TYPE_NOT_FOUND + " ('" + this.DiscountTypeCode + "')"), true);
                }

            }
            catch (Exception ex)
            {
                Kernel.LogFile.InfoException("AddTotalPaymentButton:Button_Click,Exception catched", ex);
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }
    }
}
