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

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// A Numeric keypad, used for touch screens
    /// </summary>
    public partial class ucKeyPadUnbind : ucBaseControl
    {
        public ucKeyPadUnbind()
        {
            InitializeComponent();
            //this.btnEnter.Image = ITS.POS.Client.Properties.Resources.availibity_ok_32;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableLayoutPanel TableLayoutPanel
        {
            get
            {
                return this.tableLayoutPanel1;
            }
        }



        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonZero
        {
            get
            {
                return this.btnZero;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonOne
        {
            get
            {
                return this.btnOne;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonTwo
        {
            get
            {
                return this.btnTwo;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonThree
        {
            get
            {
                return this.btnThree;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonFour
        {
            get
            {
                return this.btnFour;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonFive
        {
            get
            {
                return this.btnFive;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonSix
        {
            get
            {
                return this.btnSix;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonSeven
        {
            get
            {
                return this.btnSeven;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonEight
        {
            get
            {
                return this.btnEight;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonNine
        {
            get
            {
                return this.btnNine;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonBackspace
        {
            get
            {
                return this.btnBackspace;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonDecimal
        {
            get
            {
                return this.btnDecimalDigit;
            }
        }





        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Font ButtonsFont
        {
            get
            {
                return btnBackspace.Font;
            }
            set
            {
                //btnDoubleZero.Font = value;
                btnBackspace.Font = value;
                btnDecimalDigit.Font = value;
                btnEight.Font = value;
                //btnFactor.Font = value;
                btnFive.Font = value;
                btnFour.Font = value;
                btnNine.Font = value;
                btnOne.Font = value;
                btnSeven.Font = value;
                btnSix.Font = value;
                btnThree.Font = value;
                btnTwo.Font = value;
                btnZero.Font = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Color ButtonsBackColor
        {
            get
            {
                return btnBackspace.BackColor;
            }
            set
            {
                //btnDoubleZero.BackColor = value;
                btnBackspace.BackColor = value;
                btnDecimalDigit.BackColor = value;
                btnEight.BackColor = value;
                //btnFactor.BackColor = value;
                btnFive.BackColor = value;
                btnFour.BackColor = value;
                btnNine.BackColor = value;
                btnOne.BackColor = value;
                btnSeven.BackColor = value;
                btnSix.BackColor = value;
                btnThree.BackColor = value;
                btnTwo.BackColor = value;
                btnZero.BackColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Color ButtonsForeColor
        {
            get
            {
                return btnBackspace.ForeColor;
            }
            set
            {
                //btnDoubleZero.ForeColor = value;
                btnBackspace.ForeColor = value;
                btnDecimalDigit.ForeColor = value;
                btnEight.ForeColor = value;
                //btnFactor.ForeColor = value;
                btnFive.ForeColor = value;
                btnFour.ForeColor = value;
                btnNine.ForeColor = value;
                btnOne.ForeColor = value;
                btnSeven.ForeColor = value;
                btnSix.ForeColor = value;
                btnThree.ForeColor = value;
                btnTwo.ForeColor = value;
                btnZero.ForeColor = value;
            }
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
            // if (TouchFriendlyFilter != null)
            // {

            // TouchFriendlyFilter.Focus();


            if (key == Keys.Decimal)
            {
                SendKeys.SendWait(".");
            }
            else
            {
                SendKeys.SendWait(decodeKey(key));
            }
            //TouchFriendlyInput.PressKey(key);
            //  }
        }

        private void btnDoubleZero_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.D0, false);
            NotifyKey(Keys.D0, false);
        }

        //private void btnEnter_Click(object sender, EventArgs e)
        //{

        //    btnEnter.FindForm().DialogResult = DialogResult.OK;
        //}

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.Back, false);
        }

        private void btnFactor_Click(object sender, EventArgs e)
        {
            NotifyKey(Keys.Multiply, false);
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


    }
}
