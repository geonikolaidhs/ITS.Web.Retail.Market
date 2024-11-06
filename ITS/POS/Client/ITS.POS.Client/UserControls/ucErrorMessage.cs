using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Model.Settings;
using System.Threading;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays non disruptive error messages
    /// </summary>
    [ChildsUseSameFont]
    public partial class ucErrorMessage : ucObserver, IObserverErrorMessenger
    {
        Type[] paramsTypes = new Type[] { typeof(ErrorMessengerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }


        private LabelControl lblMessage;
        public ucErrorMessage()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.SHOW_ERROR);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LabelControl LabelControl
        {
            get { return lblMessage; }
        }

        private void InitializeComponent()
        {
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AllowHtmlString = true;
            this.lblMessage.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblMessage.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblMessage.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblMessage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 0);
            this.lblMessage.TabIndex = 0;
            // 
            // ucErrorMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoSize = true;
            this.Controls.Add(this.lblMessage);
            this.Name = "ucErrorMessage";
            this.Size = new System.Drawing.Size(97, 31);
            this.Load += new System.EventHandler(this.ucErrorMessage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public void Update(ErrorMessengerParams parameters)
        {
            if (parameters != null && !String.IsNullOrEmpty(parameters.Message))
            {
                //System.Media.SystemSounds.Exclamation.Play();
                UtilsHelper.HardwareBeep();
            }
            try
            {
                //lblMessage.Text = parameters.Message;
                SetTextDelegate setTextDel = new SetTextDelegate(SetText);
                this.Invoke(setTextDel, new object[] { lblMessage, parameters.Message });
            }
            catch (System.Exception ex)
            {
                Kernel.LogFile.Info(ex, "ucErrorMessage:Update,Exception catched");
            }

        }

        private delegate void SetTextDelegate(LabelControl label, string text);

        private void SetText(LabelControl label, string text)
        {
            label.Text = text;
        }

        private void ucErrorMessage_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                this.lblMessage.Text = "ERROR MESSAGE AREA";
            }
            else
            {
                this.lblMessage.Text = "";
            }
        }
    }
}
