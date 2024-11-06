using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POSKeyboardTool
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            //string line = "KeyCode=" + Enum.GetName(typeof(Keys), e.KeyCode) + "  ScanCode=" + ScanCodeHelper.GetScanCode(e.KeyCode).ToString() + "  KeyData=" + ((int)e.KeyData).ToString() + "  Modifiers=" + e.Modifiers.ToString() + Environment.NewLine;
            //e.Handled = true;
            //txtOutput.AppendText(line);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "(*.txt)|*.txt";
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, txtOutput.Text);
            }
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            txtOutput.SelectionFont = new Font(txtOutput.SelectionFont, FontStyle.Bold);
            txtOutput.AppendText("KeyCode=");
            txtOutput.SelectionFont = new Font(txtOutput.SelectionFont, FontStyle.Regular);
            txtOutput.AppendText(Enum.GetName(typeof(Keys), e.KeyCode).PadRight(12));
            txtOutput.SelectionFont = new Font(txtOutput.SelectionFont, FontStyle.Bold);
            txtOutput.AppendText("ScanCode=");
            txtOutput.SelectionFont = new Font(txtOutput.SelectionFont, FontStyle.Regular);
            txtOutput.AppendText(ScanCodeHelper.GetScanCode(e.KeyCode).ToString().PadRight(5));
            txtOutput.SelectionFont = new Font(txtOutput.SelectionFont, FontStyle.Bold);
            txtOutput.AppendText("KeyData=");
            txtOutput.SelectionFont = new Font(txtOutput.SelectionFont, FontStyle.Regular);
            txtOutput.AppendText(((int)e.KeyData).ToString().PadRight(8));
            txtOutput.SelectionFont = new Font(txtOutput.SelectionFont, FontStyle.Bold);
            txtOutput.AppendText("Modifiers=");
            txtOutput.SelectionFont = new Font(txtOutput.SelectionFont, FontStyle.Regular);
            txtOutput.AppendText(e.Modifiers.ToString().PadRight(15));
            txtOutput.AppendText(Environment.NewLine);
        }

        private void frmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtOutput.SelectedText) == false)
            {
                Clipboard.SetText(txtOutput.SelectedText.Trim());
            }
        }
    }
}
