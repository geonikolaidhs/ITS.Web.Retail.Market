using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.ReportDesigner
{
    public partial class ReportTypeForm : Form
    {
        public eReportType ReportType { get; set; }
        //{
        //    get
        //    {
        //        eReportType reportType = eReportType.General;
        //        if(cmbBoxReportType.SelectedValue != null){
        //            reportType = (eReportType)cmbBoxReportType.SelectedValue;
        //        }
        //        return  reportType;
        //    }
        //}

        public ReportTypeForm()
        {
            InitializeComponent();
        }

        private void ReportTypeForm_Load(object sender, EventArgs e)
        {
            cmbBoxReportType.Items.AddRange( Enum.GetNames(typeof(eReportType)) );
            cmbBoxReportType.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ReportType = (eReportType)cmbBoxReportType.SelectedIndex;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
