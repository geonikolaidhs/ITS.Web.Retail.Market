using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ITS.MobileAtStore
{
    public enum AddOrReplaceResult
    {
        /// <summary>
        /// User chose to replace
        /// </summary>
        REPLACE = 0,
        /// <summary>
        /// User chose to add
        /// </summary>
        ADD = 1
    }

    public partial class AddOrReplace : Form
    {
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

        public AddOrReplace()
        {
            InitializeComponent();
            this.Focus();
        }

        public static AddOrReplaceResult Execute()
        {
            using (AddOrReplace form = new AddOrReplace())
            {
                form.ShowDialog();
                return form.ResultChoice;
            }
        }
        private void AddOrReplace_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case ('1'):
                    btnReplace_Click(null,null);
                    break;
                case ('2'):
                    btnAdd_Click(null, null);
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }


        private void btnReplace_Click(object sender, EventArgs e)
        {
            _result = AddOrReplaceResult.REPLACE;
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _result = AddOrReplaceResult.ADD;
            this.Close();
        }

        private void AddOrReplace_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
