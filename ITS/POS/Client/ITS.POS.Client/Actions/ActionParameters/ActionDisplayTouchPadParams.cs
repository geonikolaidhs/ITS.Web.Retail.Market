using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.UserControls;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionDisplayTouchPadParams : ActionParams
    {
        public ucTouchFriendlyInput Input { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.DISPLAY_TOUCH_PAD; }
        }

        public ActionDisplayTouchPadParams(ucTouchFriendlyInput input)
        {
            this.Input = input;
        }
    }
}
