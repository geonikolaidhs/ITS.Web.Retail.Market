using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays the application's logo. Clicking the logo provides access to the POS menu
    /// </summary>
    public partial class ucLogo : ucBaseControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PictureBox ImageBox
        {
            get
            {
                return this.Image;
            }
            set
            {
                this.Image = value;
            }
        }

        public ucLogo()
        {
            InitializeComponent();
        }

        private void ucLogo_Click(object sender, EventArgs e)
        {
            using (frmMenu menu = new frmMenu(this.Kernel))
            {
                menu.ShowDialog();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ucLogo_Click(sender, e);
        }
    }
}
