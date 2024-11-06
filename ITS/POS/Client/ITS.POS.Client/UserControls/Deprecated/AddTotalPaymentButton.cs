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
    public partial class AddTotalPaymentButton : ucitsButton
    {
        public string PaymentMethodCode { get; set; }
        public decimal Amount { get; set; }


        public AddTotalPaymentButton()
        {
            _button.Text = POSClientResources.ADD_PAYMENT;
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
                if (!String.IsNullOrWhiteSpace(PaymentMethodCode))
                {
                    switch (appContext.GetMachineStatus())
                    {
                        case eMachineStatus.OPENDOCUMENT_PAYMENT:
                            PaymentMethod paymentMethod = sessionManager.FindObject<PaymentMethod>(new BinaryOperator("Code", PaymentMethodCode));
                            if (paymentMethod != null)
                            {
                                decimal amount;
                                decimal? finalAmount = null;
                                if(this.Amount > 0)
                                {
                                    finalAmount = this.Amount;
                                }
                                else if (decimal.TryParse(appContext.MainForm.InputText, out amount))
                                {
                                    finalAmount = amount;
                                }
                                else if (!String.IsNullOrWhiteSpace(appContext.MainForm.InputText))
                                {
                                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_AMOUNT));
                                    return;
                                }

                                actionManager.GetAction(eActions.ADD_TOTAL_PAYMENT).Execute(new ActionAddTotalPaymentParams(paymentMethod, finalAmount));
                                (this.FindForm() as frmMainBase).ResetInputText();
                            }
                            else
                            {
                                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.PAYMENT_METHOD_NOT_FOUND + " ('" + PaymentMethodCode + "')"));
                            }
                            break;
                    }
                }
                else
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.PAYMENT_METHOD_NOT_FOUND));
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
