using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ITS.Licensing.ClientLibrary
{
    public partial class SerialNumberForm : Form
    {
        public SerialNumberForm()
        {
            InitializeComponent();
        }

        private void txtSerial2_TextChanged(object sender, EventArgs e)
        {
            if (txtSerial2.Text.IndexOf('-') >= 0)
                txtSerial2.Text = txtSerial2.Text.Replace("-", "");
            else if (txtSerial2.Text.Length >= 5)
            {
                txtSerial3.Focus();
                if (txtSerial2.Text.Length > 5)
                    txtSerial3.Text = txtSerial2.Text.Substring(5);
                txtSerial2.Text = txtSerial2.Text.Substring(0, 5);
            }
        }

        private void txtSerial3_TextChanged(object sender, EventArgs e)
        {
            if (txtSerial3.Text.IndexOf('-') >= 0)
                txtSerial3.Text = txtSerial3.Text.Replace("-", "");
            else if (txtSerial3.Text.Length >= 5)
            {
                txtSerial4.Focus();
                if (txtSerial3.Text.Length > 5)
                    txtSerial4.Text = txtSerial3.Text.Substring(5);
                txtSerial3.Text = txtSerial3.Text.Substring(0, 5);
            }
        }

        private void txtSerial1_TextChanged(object sender, EventArgs e)
        {
            if(txtSerial1.Text.IndexOf('-')>=0)
                txtSerial1.Text = txtSerial1.Text.Replace("-", "");
            else if (txtSerial1.Text.Length >= 5)
            {
                txtSerial2.Focus();
                if (txtSerial1.Text.Length > 5)
                    txtSerial2.Text = txtSerial1.Text.Substring(5);
                txtSerial1.Text = txtSerial1.Text.Substring(0, 5);
            }
        }

        public ITSLicense itslic;

        private void btnOK_Click(object sender, EventArgs e)
        {
            String newserial ="";
            if (txtSerial1.Text.Length != 5 || txtSerial2.Text.Length != 5 || txtSerial3.Text.Length != 5 || txtSerial4.Text.Length != 5)
            {
                MessageBox.Show("Δεν έχετε πληκτρολογήσει έγκυρο κλειδί");
                return;
            }
            newserial = txtSerial1.Text + txtSerial2.Text + txtSerial3.Text + txtSerial4.Text;
            if (!itslic.SetSerialNumber(newserial))
            {
                MessageBox.Show("Δεν έχετε πληκτρολογήσει έγκυρο κλειδί");
                return;
            }
            this.Close();
        }

        private void txtSerial4_TextChanged(object sender, EventArgs e)
        {
            if (txtSerial4.Text.IndexOf('-') >= 0)
                txtSerial4.Text = txtSerial4.Text.Replace("-", "");
            else if (txtSerial4.Text.Length > 5)
                txtSerial4.Text = txtSerial4.Text.Substring(0, 5);
        }

    }
}
