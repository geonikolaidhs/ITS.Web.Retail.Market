using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace ITS.POS.Client.Forms
{
    public partial class frmTouchPad : XtraForm
    {
        //Makes the form unfocusable 
        //--------------------------
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

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

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams; // Retrieve the normal parameters.
                cp.Style = 0x40000000 | 0x4000000; // WS_CHILD | WS_CLIPSIBLINGS
                cp.ExStyle &= 0x00080000; // WS_EX_LAYERED
                cp.Parent = GetDesktopWindow(); // Make "GetDesktopWindow()" from your own namespace.
                return cp;
            }
        }
        //---------------------------

        public frmTouchPad()
        {
            InitializeComponent();
        }


    }
}
