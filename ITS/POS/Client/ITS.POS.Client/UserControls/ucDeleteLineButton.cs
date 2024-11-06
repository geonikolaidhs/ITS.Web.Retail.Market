using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Cancels a document detail or a document payment
    /// </summary>
    public partial class ucDeleteLineButton : ucActionButton
    {
        public ucDeleteLineButton()
        {
            InitializeComponent();
            this.ButtonText = POSClientResources.CANCEL_LINE;//"Delete Line";
        }


        public bool ShouldSerializeAction()
        {
            //DO NOT DELETE
            return false;
        }


        protected override void OnButtonClick(object sender, EventArgs e)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            if(appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT)
            {
                if (appContext.CurrentDocumentLine != null)
                {
                    actionManager.GetAction(eActions.DELETE_ITEM).Execute(new ActionDeleteItemParams(appContext.CurrentDocumentLine,true));
                }
            }
            else if(appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
            {
                if (appContext.CurrentDocumentPayment != null)
                {
                    actionManager.GetAction(eActions.DELETE_PAYMENT).Execute(new ActionDeletePaymentParams(appContext.CurrentDocumentPayment));
                }
            }

        }
    }
}
