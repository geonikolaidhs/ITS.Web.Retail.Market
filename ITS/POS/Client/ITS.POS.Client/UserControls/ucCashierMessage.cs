using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using DevExpress.XtraEditors;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays any message shown to the Cachier's pole display
    /// </summary>
    public partial class ucCashierMessage : ucObserver, IObserverSimpleMessenger 
    {
        public ucCashierMessage()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.CASHIER_POLE_DISPLAY_MESSAGE);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl MessageLabel
        {
            get
            {
                return this.lblMessage;
            }
        }

        Type[] paramsTypes = new Type[] { typeof(MessengerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }


        public void Update(MessengerParams parameters)
        {
            //if (parameters != null && String.IsNullOrEmpty(parameters.Message))
            //{
            //    System.Media.SystemSounds.Exclamation.Play();
            //}
            if (this.ParentForm==null || !this.ParentForm.IsHandleCreated)
            {
                return;
            }
            try
            {
                SetTextDelegate setTextDel = new SetTextDelegate(SetText);
                this.Invoke(setTextDel, new object[] { lblMessage, parameters.Message });
            }
            catch (System.Exception ex)
            {
                Kernel.LogFile.Info(ex, "ucCashierMessage:Update,Exception catched");
            }

        }

        private delegate void SetTextDelegate(LabelControl label, string text);

        private void SetText(LabelControl label, string text)
        {
            label.Text = text;
        }

        private void ucCashierMessage_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                this.lblMessage.Text = "MESSAGE AREA";
            }
            else
            {
                this.lblMessage.Text = "";
            }
        }
    }
}
