using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;
using System.Windows.Forms;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public class ucitsUseProformaButton : BaseActionButton
    {
        public ucitsUseProformaButton()
        {
            _button.Text = POSClientResources.PROFORMA;//"Prorfoma";
            //Button.Click += new System.EventHandler(OnButtonClick);
            Action = eActions.USE_PROFORMA;
            //this.ObserversNames.Add("ucInput");
        }

        public bool ShouldSerializeAction()
        {
            //DO NOT DELETE
            return false;
        }

        //protected override void OnButtonClick(object sender, EventArgs e)
        //{
        //    POSKeyMapping keyMap = new POSKeyMapping(SessionManager.MemorySettings) { KeyData = Keys.None, ActionCode = eActions.KEYMAPPINGS, NotificationType = eNotificationsTypes.ACTION };
        //    GlobalContext.GetAction(eActions.PUBLISH_KEY_PRESS).Execute(new ActionPublishKeyPressParams(keyMap));
        //    keyMap.Delete();
        //}

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button
            // 
            this._button.Size = new System.Drawing.Size(108, 41);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ucitsUseProformaButton";
            this.ResumeLayout(false);

        }
    }
}
