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
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using System.Text.RegularExpressions;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// A Numeric keypad, used for touch screens
    /// </summary>
    public partial class ucCashKeyPad : ucBaseControl
    {
        public event EventHandler<KeyNotifyEventArgs> KeyNotify;

        public ucCashKeyPad()
        {
            InitializeComponent();
            _ShowEnter = false;
            SetVisible();
            //this.btnEnter.Image = ITS.POS.Client.Properties.Resources.availibity_ok_32;
        }

        private void NotifyKey(Keys key, bool clearOldValues)
        {
            EventHandler<KeyNotifyEventArgs> handler = KeyNotify;
            if (handler != null)
            {
                handler(this, new UserControls.KeyNotifyEventArgs(key));
            }
        }


        private void btnBackspace_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.Back, false);
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
        private bool _ShowEnter;
        public bool ShowEnter
        {
            get { return _ShowEnter; }
            set
            {
                if (_ShowEnter != value)
                {
                    _ShowEnter = value;
                    SetVisible();
                }
            }
        }
        private void SetVisible()
        {
            if (ShowEnter == false)
            {
                btnEnter.Visible = false;
                btnMultiply.Visible = false;
                this.tlpMain.ColumnStyles[1].Width = 0;
            }
            else
            {
                btnEnter.Visible = true;
                btnMultiply.Visible = true;
                this.tlpMain.ColumnStyles[1].Width = 25;
            }
        }

        private void btnMultiply_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.Multiply, false);
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.Enter, false);
        }
    }
    public class KeyNotifyEventArgs : EventArgs
    {
        public KeyNotifyEventArgs(Keys Key)
        {
            this.Key = Key;
        }
        public Keys Key { get; set; }
    }

}
