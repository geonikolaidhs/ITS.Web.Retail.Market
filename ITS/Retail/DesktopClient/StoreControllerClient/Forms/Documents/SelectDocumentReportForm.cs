using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class SelectDocumentReportForm : XtraLocalizedForm
    {
        private Guid documentGuid;
        public DocumentTypeCustomReport documentReport { get; set; }
        protected bool autoprint { get; set; }

        public SelectDocumentReportForm(IEnumerable<DocumentTypeCustomReport> documentReports, Guid 
             documentOid, bool autoPrint)
        {
            InitializeComponent();
            autoprint = autoPrint;
            documentReport = null;
            documentGuid = documentOid;
            this.lookUpEditDocumentReports.Properties.DataSource = documentReports;
            this.lookUpEditDocumentReports.Properties.Columns.Clear();
            this.lookUpEditDocumentReports.Properties.ValueMember = "This";
            this.lookUpEditDocumentReports.Properties.DisplayMember = "Description";
            this.lookUpEditDocumentReports.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            this.lookUpEditDocumentReports.DataBindings.Add("EditValue", this, "documentReport", true, DataSourceUpdateMode.OnPropertyChanged);

            LocaliseApplication();
            this.lookUpEditDocumentReports.EditValue = documentReports.FirstOrDefault();
        }

        private void LocaliseApplication()
        {
            this.Text = Resources.SelectReport;
            this.layoutControlItemReports.Text = Resources.Reports;
            simpleButtonOK.Text = Resources.Continue;
            simpleButtonCancel.Text = Resources.Cancel;
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButtonOK_Click(object sender, EventArgs e)
        {
            if(documentReport == null)
            {
                XtraMessageBox.Show(Resources.PleaseSelectARecord, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CustomReport report = documentReport.Report;
            string title, description;
            XtraReportBaseExtension xtraReport = ReportsHelper.GetXtraReport(report.Oid, Program.Settings.StoreControllerSettings.Owner, Program.Settings.CurrentUser,null, out title, out description);
            (xtraReport as SingleObjectXtraReport).ObjectOid = documentGuid;
            XtraReportBaseExtension dupls = ReportsHelper.DuplicateReport(xtraReport, documentReport.Duplicates);
            using (DocumentPrintForm documentPrintForm = new DocumentPrintForm(dupls, this.autoprint))
            {
                documentPrintForm.ShowDialog();
                this.Close();
            }
        }

        private void SelectDocumentReportForm_Shown(object sender, EventArgs e)
        {
            lookUpEditDocumentReports.Focus();
        }
    }
}
