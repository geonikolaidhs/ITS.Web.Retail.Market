using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Helpers;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmPause : frmInputFormBase
    {
        public frmPause(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();

            this.CanBeClosedByUser = false;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            this.lblUser.Text = appContext.CurrentUser.POSUserName;
            this.lblTitle.Text = POSClientResources.PAUSED.ToUpperGR();
        }

        private void tePassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter && (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9) && e.KeyCode != Keys.Back)
            {
                //only numbers are allowed
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                if (tePassword.Text == appContext.CurrentUser.POSPassword)
                {
                    this.Close();
                }
                else
                {
                    UtilsHelper.HardwareBeep();
                    tePassword.Text = "";
                }
            }
        }

    }
}
