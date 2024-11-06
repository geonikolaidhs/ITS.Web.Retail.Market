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
    public partial class InvMasterForm : Form
    {
        public InvMasterForm()
        {
            InitializeComponent();
        }

        private void btnInv_Click(object sender, EventArgs e)
        {
            using (InvProcessForm form = new InvProcessForm())
            {
                form.ShowDialog();
            }
        }

        private void btnControl_Click(object sender, EventArgs e)
        {
            using (InvControlForm form = new InvControlForm())
            {
                form.ShowDialog();
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InvMasterForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            if (e.KeyChar == (char)Keys.D1)
            {
                btnInv_Click(null, null);
            }
            else
                if (e.KeyChar == (char)Keys.D2)
                {
                    btnControl_Click(null, null);
                }
        }

        private void InvMasterForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
