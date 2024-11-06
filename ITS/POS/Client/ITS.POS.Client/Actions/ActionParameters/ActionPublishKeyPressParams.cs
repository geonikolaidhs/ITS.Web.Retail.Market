using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionPublishKeyPressParams : ActionParams
    {
        public POSKeyMapping KeyPress;

        public override eActions ActionCode
        {
            get { return eActions.PUBLISH_KEY_PRESS; }
        }

        public ActionPublishKeyPressParams(POSKeyMapping keyPress)
        {
            this.KeyPress = keyPress;
        }
    }
}
