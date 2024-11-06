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
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class ucitsKeyPad : BaseControl//ucitsObservable
    {
        public ucitsKeyPad()
        {
            InitializeComponent();
            //this.ObserversNames.Add("ucInput");
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
        public SimpleButton ButtonDoubleZero
        {
            get
            {
                return this.btnDoubleZero;
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleButton ButtonEnter
        {
            get
            {
                return this.btnEnter;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Font ButtonsFont
        {
            get
            {
                return btnDoubleZero.Font;
            }
            set
            {
                btnDoubleZero.Font = value;
                btnBackspace.Font = value;
                btnDecimalDigit.Font = value;
                btnEight.Font = value;
                btnFactor.Font = value;
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
                return btnDoubleZero.BackColor;
            }
            set
            {
                btnDoubleZero.BackColor = value;
                btnBackspace.BackColor = value;
                btnDecimalDigit.BackColor = value;
                btnEight.BackColor = value;
                btnFactor.BackColor = value;
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
                return btnDoubleZero.ForeColor;
            }
            set
            {
                btnDoubleZero.ForeColor = value;
                btnBackspace.ForeColor = value;
                btnDecimalDigit.ForeColor = value;
                btnEight.ForeColor = value;
                btnFactor.ForeColor = value;
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Font EnterButtonFont
        {
            get
            {
                return btnEnter.Font;
            }
            set
            {
                btnEnter.Font = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Color EnterBackColor
        {
            get
            {
                return btnEnter.BackColor;
            }
            set
            {
                btnEnter.BackColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Color EnterForeColor
        {
            get
            {
                return btnEnter.ForeColor;
            }
            set
            {
                btnEnter.ForeColor = value;
            }
        }

        private void NotifyKey(Keys key, bool clearOldValues)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            POSKeyMapping keyMap = new POSKeyMapping(sessionManager.MemorySettings) { KeyData = key, ActionCode = eActions.NONE, NotificationType = eNotificationsTypes.KEY };
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.PUBLISH_KEY_PRESS).Execute(new ActionPublishKeyPressParams(keyMap));
            keyMap.Delete();

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
