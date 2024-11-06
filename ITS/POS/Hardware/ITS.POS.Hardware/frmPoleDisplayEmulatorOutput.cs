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

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Used by the pole display emulator. Displays the pole display's output.
    /// </summary>
    public partial class frmPoleDisplayEmulatorOutput : XtraForm
    {

        /// <summary>
        /// Makes the form unfocusable 
        /// </summary>
        /// <returns></returns>
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

        const long WS_EX_NOACTIVATE = 0x08000000L , S_EX_TOOLWINDOW = 0x00000080L;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;

                baseParams.ExStyle |= (int)(
                  WS_EX_NOACTIVATE |
                  S_EX_TOOLWINDOW);

                return baseParams;
            }
        }

        ////---------------------------

        public frmPoleDisplayEmulatorOutput()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void frmPoleDisplayEmulatorOutput_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Hide();
            //e.Cancel = true;
        }
    }
}
