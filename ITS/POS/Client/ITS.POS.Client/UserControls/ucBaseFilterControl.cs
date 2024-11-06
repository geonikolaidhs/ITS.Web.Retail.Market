using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Client.Helpers;
using DevExpress.XtraLayout;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;
using System.Runtime.InteropServices;

namespace ITS.POS.Client.UserControls
{
    public partial class ucBaseFilterControl : XtraUserControl
    {


        public string ParameterName { get; set; }


        public bool Required { get; set; }

        public ucBaseFilterControl()
        {
            InitializeComponent();


        }



        public virtual void SetLabelText(string labelText)
        {

        }

        public virtual object GetControlValue()
        {
            return null;
        }

        public virtual void SetControlValue(object value)
        {

        }

        private void ucBaseFilterControl_Enter(object sender, EventArgs e)
        {

        }

        private void ucBaseFilterControl_Paint(object sender, PaintEventArgs e)
        {

        }

        public virtual void SetLabelColor(Color color)
        {

        }

        public virtual void SetLabelWidth(int width)
        {

        }

        public virtual void SetControlWidth(int width)
        {

        }
    }
}
