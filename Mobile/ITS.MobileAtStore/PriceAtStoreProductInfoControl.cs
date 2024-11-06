using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenNETCF.Windows.Forms;
using System.IO;
using ITS.MobileAtStore.WRMMobileAtStore;

namespace ITS.MobileAtStore
{
    public partial class PriceAtStoreProductInfoControl : UserControl
    {
        public PriceAtStoreProductInfoControl()
        {
            InitializeComponent();
        }

        public delegate void KeyScanned(Char key);
        public event KeyScanned OnKeyScanned;

        private void PriceAtStoreProductInfoControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyScanned(e.KeyChar);
        }

        public void SetInfo(Product prod, Offer[] offers)
        {
            label1.Text = prod.Description;
        }
    }
}
