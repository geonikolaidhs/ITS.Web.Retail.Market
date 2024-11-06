using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenNETCF.Windows.Forms;
using System.IO;

namespace ITS.MobileAtStore
{
    public partial class PriceAtStoreEnterCodeControl : UserControl
    {
        public PriceAtStoreEnterCodeControl()
        {
            InitializeComponent();
            string file = Application2.StartupPath +  "\\PriceAtStoreImages\\top_label.gif";
            if (File.Exists(file))
                upperLogo.Image = new Bitmap(file);
            string bottomFile = Application2.StartupPath + "\\PriceAtStoreImages\\ITS-Logo.jpg";
            if (File.Exists(bottomFile))
                bottomLogo.Image = new Bitmap(bottomFile);
        }

        //private void txtInputCode_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        try
        //        {
        //            txtInputCode.Enabled = false;
        //            OnCodeScanned(txtInputCode.Text.Trim());
        //            txtInputCode.Focus();
        //        }
        //        finally
        //        {
        //            txtInputCode.Enabled = true;
        //        }
        //    }
        //}

        public delegate void KeyScanned(Char key);
        public event KeyScanned OnKeyScanned;

        private void PriceAtStoreEnterCodeControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyScanned(e.KeyChar);
        }
    }
}
