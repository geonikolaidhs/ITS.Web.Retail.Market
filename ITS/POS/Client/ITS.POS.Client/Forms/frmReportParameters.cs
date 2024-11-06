using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.UserControls;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Master;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using DevExpress.XtraEditors;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Resources;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ITS.POS.Client.Forms
{
    public partial class frmReportParameters : XtraForm, IPOSForm
    {

        public IPosKernel Kernel { get; set; }

        public PosReport PosReport { get; set; }



        private IOposReportService ReportService { get; set; }

        private List<UserParameter> ReportParams { get; set; }

        public IAppContext appContext { get; set; }




        public frmReportParameters()
        {
            InitializeComponent();

        }

        public frmReportParameters(List<UserParameter> reportParameters, IPosKernel kernel, PosReport posReport, IOposReportService reportService, List<UserParameter> reportParams)
        {
            InitializeComponent();
            Initialize(kernel);
            PrepareForm(CreateReportControls(reportParameters));
            this.PosReport = posReport;
            this.ReportService = reportService;
            this.ReportParams = reportParams;
            appContext = Kernel.GetModule<IAppContext>();


        }

        public void Initialize(IPosKernel kernel)
        {
            this.Kernel = kernel;
        }



        public List<ucBaseFilterControl> CreateReportControls(List<UserParameter> reportParameters)
        {
            List<ucBaseFilterControl> controlsToAdd = new List<ucBaseFilterControl>();
            foreach (UserParameter rep in reportParameters)
            {
                ucBaseFilterControl filter;

                switch (rep.Type)
                {
                    case eUserParameterType.Text:
                        filter = new ucTextFilterControl();
                        break;
                    case eUserParameterType.Number:
                        filter = new ucNumberFilterControl();
                        break;
                    case eUserParameterType.Boolean:
                        filter = new ucBooleanFilterControl();
                        break;
                    case eUserParameterType.DateTime:
                        filter = new ucDateFilterControl();
                        break;
                    case eUserParameterType.LookUpEdit:
                        filter = new ucLookUpEditFilterControl(rep.Datasource);
                        break;
                    default:
                        filter = new ucTextFilterControl();
                        break;

                }

                filter.Required = rep.Required;
                filter.Width = 400;
                filter.Name = rep.ParameterName;
                filter.ParameterName = rep.ParameterName;
                filter.SetLabelText(rep.LabelText);
                filter.SetControlValue(rep.DefaultValue);
                controlsToAdd.Add(filter);
            }
            return controlsToAdd;

        }



        private void PrepareForm(List<ucBaseFilterControl> controlsToAdd)
        {

            System.Drawing.Rectangle screen = Screen.PrimaryScreen.WorkingArea;


            this.SuspendLayout();


            btnCancel.Text = Resources.POSClientResources.CANCEL;
            btnPrint.Text = Resources.POSClientResources.PRINT;
            btnPreview.Text = Resources.POSClientResources.PREVIEW;
            btnKeyboard.Text = Resources.POSClientResources.KEYBOARD;


            int numberOfRows = controlsToAdd.Count;
            this.Width = screen.Width;
            this.Height = screen.Height;
            this.Width = 1024;
            this.Height = 768;


            //title layout
            this.titleLayout.SuspendLayout();

            this.titleLayout.Height = 100;
            this.titleLayout.ResumeLayout();

            //controls layout
            this.filterLayout.SuspendLayout();
            this.filterLayout.Controls.Clear();
            this.filterLayout.RowStyles.Clear();
            this.filterLayout.ColumnStyles.Clear();
            this.filterLayout.ColumnCount = 1;

            for (int i = 0; i < controlsToAdd.Count; i++)
            {
                this.filterLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize, 90));
            }

            foreach (ucBaseFilterControl ctrl in controlsToAdd)
            {
                //ctrl.Width = screen.Width * 30 / 100;
                this.filterLayout.Controls.Add(ctrl);
            }
            this.filterLayout.ResumeLayout();

            btnCancel.Click += delegate (object sender, EventArgs e)
            {
                btnCancel.FindForm().DialogResult = DialogResult.Cancel;
                btnCancel.FindForm().Hide();
            };

            this.CancelButton = btnCancel;
            this.AcceptButton = btnPrint;
            this.ResumeLayout();


        }


        private bool ValidateControls()
        {
            bool validForm = true;
            bool controlIsFocused = false;

            List<ucBaseFilterControl> requiredControls = this.titleLayout.Controls.OfType<ucBaseFilterControl>().Where(x => x.Required == true).ToList();

            foreach (ucBaseFilterControl control in requiredControls)
            {
                bool invalidControl = false;
                if (control.GetControlValue() == null)
                {
                    validForm = false;
                    invalidControl = true;
                }

                if (control.GetControlValue() != null && string.IsNullOrEmpty(control.GetControlValue().ToString()))
                {
                    invalidControl = true;
                    validForm = false;
                }

                if (invalidControl)
                {
                    if (controlIsFocused == false)
                    {
                        control.Focus();
                        controlIsFocused = true;
                    }
                    control.SetLabelColor(Color.Red);

                }
                else
                {
                    control.SetLabelColor(Color.Gray);
                }

            }
            return validForm;

        }



        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (ValidateControls())
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {

            rtbReport.Text = string.Empty;
            rtbReport.Invalidate();
            if (ValidateControls())
            {
                object instance = ReportService.GetReportInstance(PosReport.Oid.ToString() + ".dll", "PosOposReport");
                if (instance != null)
                {
                    rtbReport.ReadOnly = true;
                    List<string> lines = ReportService.GetStringLinesResult(this, instance, ReportParams, appContext, this);
                    StringBuilder sb = new StringBuilder();
                    foreach (string line in lines)
                    {
                        sb.Append(line);
                        sb.Append(Environment.NewLine);
                    }
                    rtbReport.Text = sb.ToString();
                    rtbReport.EditValue = sb.ToString();
                }
                this.DialogResult = DialogResult.None;
            }
        }


        private void ToogleVirtualKeyboard()
        {
            var path64 = Path.Combine(Directory.GetDirectories(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "winsxs"), "amd64_microsoft-windows-osk_*")[0], "osk.exe");
            var path32 = @"C:\windows\system32\osk.exe";
            var path = (Environment.Is64BitOperatingSystem) ? path64 : path32;
            Process[] procs = Process.GetProcessesByName("osk");
            if (procs.Count() > 0)
                for (int i = 0; i < procs.Count(); i++)
                    procs[i].Kill();
            else
                Process.Start(path);

        }



        private void btnKeyboard_Click(object sender, EventArgs e)
        {
            try
            {
                ToogleVirtualKeyboard();
            }
            catch (Exception ex) { }
        }

        private void rtbReport_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
