using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Actions.ActionParameters;


namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// The touch pad that is used when the application is in "Uses Touch" mode
    /// </summary>
    public partial class ucTouchPad : ucBaseControl
    {
        //Makes the control unfocusable 
        //-----------------------------------

        private const int WM_MOUSEACTIVATE = 0x0021, MA_NOACTIVATE = 0x0003;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_NOACTIVATE;
                return;
            }
            base.WndProc(ref m);
        }
        //------------------------------------


        public ucTouchFriendlyInput TouchFriendlyInput { get; set; }

        public ucTouchPad()
        {
            InitializeComponent();
        }

        string decodeKey(Keys key)
        {
            if (key == Keys.Enter)
            {
                return "{ENTER}";
            }
            else if (key == Keys.Back)
            {
                return "{BACKSPACE}";
            }
            else if (key == Keys.Escape)
            {
                return "{ESC}";
            }
            else
            {
                return UtilsHelper.KeyboardDecode(key);
            }
        }

        private void NotifyKey(Keys key, bool clearOldValues)
        {
            if (TouchFriendlyInput != null)
            {
                TouchFriendlyInput.Focus();
                Application.DoEvents();
                if (key == Keys.Decimal)
                {
                    SendKeys.SendWait(".");
                }
                else
                {
                    SendKeys.SendWait(decodeKey(key));
                }
                //TouchFriendlyInput.PressKey(key);
            }
        }

        private void btnDoubleZero_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D0, false);
            NotifyKey(Keys.D0, false);
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.Enter, true);
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.Back, false);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //NotifyKey(Keys.Multiply, false);
            if (this.FindForm() != null)
            {
                this.FindForm().Hide();
            }
        }

        private void btnDecimalDigit_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.Decimal, false);
        }

        private void btnZero_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D0, false);
        }

        private void btnOne_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D1, false);
        }

        private void btnTwo_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D2, false);
        }

        private void btnThree_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D3, false);
        }

        private void btnFour_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D4, false);
        }

        private void btnFive_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D5, false);
        }

        private void btnSix_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D6, false);
        }

        private void btnSeven_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D7, false);
        }

        private void btnEight_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D8, false);
        }

        private void btnNine_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D9, false);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.Escape, false);
        }


    }
}
