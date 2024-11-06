using ITS.POS.Fiscal.GUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Fiscal.GUI
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            lblTitleShort.Text = Resources.FISCAL_SERVICE_NAME;
            lblTitleFull.Text = Resources.FISCAL_SERVICE_NAME_FULL;
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {

        }
    }
}
