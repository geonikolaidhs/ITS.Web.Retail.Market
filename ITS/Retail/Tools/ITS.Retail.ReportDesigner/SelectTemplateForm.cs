using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.ReportDesigner
{
    public partial class SelectTemplateForm : Form
    {

        public string SelectedTemplate { get; set; }

        public SelectTemplateForm()
        {
            InitializeComponent();
            SelectedTemplate = null;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fld = new OpenFileDialog())
            {
                fld.Filter = "Xtra report|*.repx";
                fld.Multiselect = false;
                DialogResult res = fld.ShowDialog();
                if (res != DialogResult.OK)
                {
                    return;
                }
                else
                {
                    SelectedTemplate = fld.FileName;
                    this.Close();
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
