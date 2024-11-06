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
using ITS.POS.Resources;

using DevExpress.XtraEditors.Controls;

namespace ITS.POS.Client.UserControls
{
    public partial class ucLookUpEditFilterControl : ucBaseFilterControl
    {

        public Dictionary<object, object> Datasource { get; set; }

        public ucLookUpEditFilterControl()
        {
            InitializeComponent();
        }
        public ucLookUpEditFilterControl(Dictionary<object, object> datasource)
        {
            InitializeComponent();
            this.dropDown.Height = 40;
            this.Datasource = new Dictionary<object, object>();
            this.Datasource = datasource;
            this.dropDown.Properties.DataSource = datasource;
            this.dropDown.Properties.Columns.Add(new LookUpColumnInfo("Key", POSClientResources.VALUE));
            this.dropDown.Properties.DisplayMember = "Key";
            this.dropDown.Properties.ValueMember = "Value";
        }

        public override object GetControlValue()
        {
            return this.dropDown.EditValue;
        }

        public override void SetControlValue(object value)
        {
            try
            {
                if (value != null)
                {
                    this.dropDown.EditValue = (bool)value;
                    this.Invalidate();
                }
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
            this.dropDown.Width = width;
            this.dropDown.Invalidate();
        }


    }
}
