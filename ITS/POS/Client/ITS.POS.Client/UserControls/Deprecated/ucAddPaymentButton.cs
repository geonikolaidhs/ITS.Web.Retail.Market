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
    [Obsolete("DO NOT CHANGE NAME OR NAMESPACE! Use AddTotalPaymentButton instead")]
    public partial class ucAddPaymentButton : ucButton
    {
        public string PaymentMethodCode { get; set; }


        public ucAddPaymentButton()
        {
            _button.Text = POSClientResources.ADD_PAYMENT;
            InitializeComponent();
        }

        private void AddPaymentButtonMethod_MouseClick(object sender, MouseEventArgs e)
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
                                decimal amount = 0;
                                if (decimal.TryParse((this.FindForm() as frmMainBase).InputText, out amount))
                                {
                                    actionManager.GetAction(eActions.ADD_PAYMENT).Execute(new ActionAddPaymentParams(paymentMethod, amount));
                                    (this.FindForm() as frmMainBase).ResetInputText();
                                }
                                else
                                {
                                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.INVALID_AMOUNT));
                                }
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
                Kernel.LogFile.InfoException("AddPaymentButton:Button_Click,Exception catched", ex);
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }
    }
}
