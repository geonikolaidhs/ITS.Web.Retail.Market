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

namespace ITS.POS.Client.UserControls
{
    public partial class ucNumberFilterControl : ucBaseFilterControl
    {

        public ucNumberFilterControl()
        {
            InitializeComponent();
            this.numericUpDown1.Height = 40;

        }

        public override object GetControlValue()
        {
            return this.numericUpDown1.Value;
        }

        public override void SetControlValue(object value)
        {
            try
            {
                if (value != null)
                    this.numericUpDown1.Value = (int)value;
            }
            catch (Exception ex) { }
        }



        public override void SetLabelText(string labelText)
        {
            this.controlLabel.Text = labelText;
        }

        public override void SetLabelColor(Color color)
        {
            this.controlLabel.ForeColor = color;
            this.controlLabel.Invalidate();
        }

        public override void SetLabelWidth(int width)
        {
            this.controlLabel.Width = width;
            this.controlLabel.Invalidate();
        }

        public override void SetControlWidth(int width)
        {
            this.numericUpDown1.Width = width;
            this.numericUpDown1.Invalidate();
        }

    }
}
