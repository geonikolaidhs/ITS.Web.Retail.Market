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
    public partial class ucDateFilterControl : ucBaseFilterControl
    {

        public ucDateFilterControl()
        {
            InitializeComponent();
            this.dateTimePicker1.Height = 40;
        }

        public override object GetControlValue()
        {
            return this.dateTimePicker1.EditValue;
        }

        public override void SetControlValue(object value)
        {
            try
            {
                if (value != null)
                    this.dateTimePicker1.EditValue = (DateTime)value;
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
            this.dateTimePicker1.Width = width;
            this.dateTimePicker1.Invalidate();
        }

        private void dateTimePicker1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_Click(object sender, EventArgs e)
        {
            if (!dateTimePicker1.IsPopupOpen)
            {
                dateTimePicker1.CancelPopup();
                dateTimePicker1.ShowPopup();
            }

        }
    }
}
