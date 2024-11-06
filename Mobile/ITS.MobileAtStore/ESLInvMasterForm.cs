using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ITS.MobileAtStore.ObjectModel;
using ITS.Common.Utilities.Compact;
using ITS.Common.Keyboards.Compact;
using ITS.MobileAtStore.Properties;

namespace ITS.MobileAtStore
{
    public partial class ESLInvMasterForm : Form
    {
        public ESLInvMasterForm()
        {
            InitializeComponent();
        }

        private void btnPrepareStore_Click(object sender, EventArgs e)
        {
            using (ESLInvProcessForm form = new ESLInvProcessForm(ESLInvProcessForm.Modes.PREPARE_STORE))
            {
                form.ShowDialog();
            }
        }

        private void btnESLInv_Click(object sender, EventArgs e)
        {
            using (ESLInvProcessForm form = new ESLInvProcessForm(ESLInvProcessForm.Modes.PROCESS_INV))
            {
                form.ShowDialog();
            }
        }

        private void btnControl_Click(object sender, EventArgs e)
        {
            using (ESLInvControlForm form = new ESLInvControlForm())
            {
                form.ShowDialog();
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ESLInvMasterForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.D1)
            {
                btnPrepareStore_Click(null, null);
            }
            else
                if (e.KeyChar == (char)Keys.D2)
                {
                    btnESLInv_Click(null, null);
                }
                else
                    if (e.KeyChar == (char)Keys.D3)
                    {
                        btnControl_Click(null, null);
                    }
        }

        private void ESLInvMasterForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
