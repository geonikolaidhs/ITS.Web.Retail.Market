using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// A button that cannot be focused.
    /// </summary>
    public partial class ucUnfocusableSimpleButton : SimpleButton
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

        public ucUnfocusableSimpleButton()
        {
            InitializeComponent();
        }
    }
}
