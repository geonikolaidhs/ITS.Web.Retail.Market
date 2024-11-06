using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Retail.Mobile
{
    public partial class AddOrReplace : Form
    {
        public AddOrReplace()
        {
            InitializeComponent();
        }

        private AddOrReplaceResult _result;

        public AddOrReplaceResult ResultChoice
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
            }
        }

        public enum AddOrReplaceResult
        {
            CANCEL =0,
            /// <summary>
            /// User chose to replace
            /// </summary>
            REPLACE = 1,
            /// <summary>
            /// User chose to add
            /// </summary>
            ADD = 2
        }

        private void btnreplace_Click(object sender, EventArgs e)
        {
            _result = AddOrReplaceResult.REPLACE;
            this.Close();
        }

        private void btnadd_Click(object sender, EventArgs e)
        {
            _result = AddOrReplaceResult.ADD;
            this.Close();
        }

        public static AddOrReplaceResult Execute()
        {
            using (AddOrReplace form = new AddOrReplace())
            {
                form.ResultChoice = AddOrReplaceResult.CANCEL;
                form.ShowDialog();
                return form.ResultChoice;
            }
        }

        private void AddOrReplace_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case ('1'):
                    _result = AddOrReplaceResult.REPLACE;
                    this.Close();
                    break;
                case ('2'):
                    _result = AddOrReplaceResult.ADD;
                    this.Close();
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void AddOrReplace_Load(object sender, EventArgs e)
        {
            btnreplace.Text = "[1] " +Resources.Resources.Replace;
            btnadd.Text = "[2] "+Resources.Resources.Add;
            this.Text = Resources.Resources.Quantity;
        }
    }
}