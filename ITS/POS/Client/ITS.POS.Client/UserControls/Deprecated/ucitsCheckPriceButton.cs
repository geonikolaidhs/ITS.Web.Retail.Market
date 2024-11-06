using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using System.Drawing;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;


namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class ucitsCheckPriceButton : BaseActionButton
    {
        public ucitsCheckPriceButton()
        {
            _button.Text = POSClientResources.CHECK_PRICE;//"CheckPrice";
            Action = eActions.CHECKPRICE;
            //this.ObserversNames.Add("ucInput");
        }

        public bool ShouldSerializeAction()
        {
            //DO NOT DELETE
            return false;
        }

        protected override void OnButtonClick(object sender, EventArgs e)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            POSKeyMapping keyMap = new POSKeyMapping(sessionManager.MemorySettings) { KeyData = Keys.None, ActionCode = eActions.CHECKPRICE, NotificationType = eNotificationsTypes.ACTION };
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.PUBLISH_KEY_PRESS).Execute(new ActionPublishKeyPressParams(keyMap));
            keyMap.Delete();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button
            // 
            this._button.Size = new System.Drawing.Size(108, 41);
            // 
            // ucitsCheckPriceButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ucitsCheckPriceButton";
            this.ResumeLayout(false);

        }

    }
}
