using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.UserControls
{
    public partial class CustomKeyPressButton : ucitsButton
    {
        
        [Browsable(false)]
        [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]  //do not delete for backwards compartibility
        public Keys KeyData {get; set;}

        public string Characters { get; set; }


        public CustomKeyPressButton()
        {
            InitializeComponent();
            this._button.Click += OnButtonClick;
            //this.Action = Retail.Platform.Enumerations.eActions.PUBLISH_KEY_PRESS;
        }

        //public bool ShouldSerializeAction()
        //{
        //    //DO NOT DELETE
        //    return false;
        //}

        protected void OnButtonClick(object sender, EventArgs e)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            appContext.MainForm.AppendInputText(Characters);
            //POSKeyMapping keyMap = new POSKeyMapping(SessionManager.MemorySettings) { KeyData = KeyData, ActionCode = eActions.NONE, NotificationType = eNotificationsTypes.KEY };
            //GlobalContext.GetAction(eActions.PUBLISH_KEY_PRESS).Execute(new ActionPublishKeyPressParams(keyMap));
            //keyMap.Delete();
        }
    }
}
