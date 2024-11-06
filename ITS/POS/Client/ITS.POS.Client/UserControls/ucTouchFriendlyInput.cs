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
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Helpers;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using System.Runtime.InteropServices;
using ITS.POS.Client.Forms;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// A text edit that supports showing a touch pad when the application is in "Uses Touch" mode
    /// </summary>
    public class ucTouchFriendlyInput : ucTextEdit
    {

        public bool AutoHideTouchPad { get; set; }

        public ucTouchFriendlyInput()
        {
            this.GotFocus += ucTouchFriendlyInput_GotFocus;
            //this.LostFocus += HideTouchPad;
            this.KeyDown += ucTouchFriendlyInput_KeyDown;
            AutoHideTouchPad = true;
        }

        void ucTouchFriendlyInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (AutoHideTouchPad && (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)) //We assume that enter/tab moves to the next control, hides/changes the form or completes the input
            {
                HideTouchPad();
            }
        }

        void ucTouchFriendlyInput_GotFocus(object sender, System.EventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            if (config.UsesTouchScreen)
            {
                actionManager.GetAction(eActions.DISPLAY_TOUCH_PAD).Execute(new ActionDisplayTouchPadParams(this));
            }
        }

        public void HideTouchPad()
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            if (config.UsesTouchScreen)
            {
                try
                {
                    Control focusedControl = formManager.GetFocusedControl();
                    if (!(focusedControl is frmTouchPad))
                    {
                        actionManager.GetAction(eActions.HIDE_TOUCH_PAD).Execute();
                    }
                }
                catch (Exception ex)
                {
                    throw new POSException(ex.GetFullMessage());
                }
            }
        }


    }
}
