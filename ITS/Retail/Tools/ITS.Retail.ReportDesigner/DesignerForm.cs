using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using ITS.Retail.Common;
using DevExpress.Xpo;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using DevExpress.XtraReports.Design.Commands;

namespace ITS.Retail.ReportDesigner
{
    public partial class DesignerForm : Form
    {
        protected BindingSource BindingSource { get; set; }
        public UnitOfWork mainSession { get; protected set; }

        public DesignerForm()
        {
            DevExpress.Data.Helpers.IsDesignModeHelper.BypassDesignModeAlterationDetection = true;
            InitializeComponent();
            ReportDesignExtension.RegisterExtension(new RetailReportExtension(mainSession), "RetailExtension");
            reportDesigner1.DesignPanelLoaded += DesignPanelLoaded;
            reportDesigner1.AnyDocumentActivated += AnyDocumentActivated;
            btnQueryEditor.Enabled = false;
            btnExtraScripts.Enabled = false;
            reportDesigner1.AddCommandHandler(new ITSCommandHandler(null, reportDesigner1, this));
            this.barStaticItemVersion.Caption = GetType().Assembly.GetName().Version.ToString(4);
        }

        void AnyDocumentActivated(object sender, DevExpress.XtraBars.Docking2010.Views.DocumentEventArgs e)
        {
            btnQueryEditor.Enabled = true;
            btnExtraScripts.Enabled = true;
        }

        private void DesignPanelLoaded(object sender, DesignerLoadedEventArgs e)
        {

            btnQueryEditor.Enabled = true;
            btnExtraScripts.Enabled = true;
            XRDesignPanel panel = (XRDesignPanel)sender;

            reportDesigner1.AddCommandHandler(new ITSCommandHandler(panel, reportDesigner1, this));
            IToolboxService toolbox = e.DesignerHost.GetService(typeof(IToolboxService)) as IToolboxService;
            toolbox.AddToolboxItem(new ToolboxItem(typeof(XRPivotGridExtension)) { DisplayName = "Pivot Grid Web OLAP"});

            IMenuCommandService menuCommandService = e.DesignerHost.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            menuCommandService.RemoveCommand(menuCommandService.FindCommand(FieldListCommands.AddParameter));
            MenuCommand command = new MenuCommand(new EventHandler(AddParameter), FieldListCommands.AddParameter);
            command.Properties.Add("DesignerHost", e.DesignerHost);

            menuCommandService.AddCommand(command);

        }

        private void AddParameter(object sender, EventArgs e)
        {
            IDesignerHost designerHost = (sender as MenuCommand).Properties["DesignerHost"] as IDesignerHost;
            XtraReport report = designerHost.RootComponent as XtraReport;
            IComponentChangeService changeServ = designerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            ISelectionService selectionServ = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;

            string description = String.Format(DesignSR.Trans_Add, typeof(ReportParameterExtension).Name);
            DesignerTransaction transaction = designerHost.CreateTransaction(description);
            ReportParameterExtension parameter = new ReportParameterExtension();
            try
            {
                PropertyDescriptor property = DevExpress.XtraReports.Native.XRAccessor.GetPropertyDescriptor(report, XRComponentPropertyNames.Parameters);
                changeServ.OnComponentChanging(report, property);
                designerHost.AddToContainer(parameter, true);
                report.Parameters.Add(parameter);
                changeServ.OnComponentChanged(report, property, null, null);
            }
            finally
            {
                transaction.Commit();
            }
            selectionServ.SetSelectedComponents(new object[] { parameter });
        }

        private void btnQueryEditor_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (CodeEditorForm codeForm = new CodeEditorForm((reportDesigner1.ActiveDesignPanel.Report as XtraReportBaseExtension).LinqCode, EditorType.QUERY))
            {
                codeForm.ShowDialog();
                UpdateQueryCode(codeForm.Code);
            }
        }

        public void UpdateQueryCode(string newQuery)
        {
            XRDesignerHostBase host = (reportDesigner1.ActiveDesignPanel.GetService(typeof(IDesignerHost)) as XRDesignerHostBase);
            XtraReportBaseExtension report = reportDesigner1.ActiveDesignPanel.Report as XtraReportBaseExtension;

            reportDesigner1.ActiveDesignPanel.Report.DataSource = null;
            (reportDesigner1.ActiveDesignPanel.Report as XtraReportBaseExtension).LinqCode = newQuery;

            reportDesigner1.ActiveDesignPanel.Report.DataSource = (reportDesigner1.ActiveDesignPanel.Report as XtraReportBaseExtension).ModelProperty;
            reportDesigner1.ActiveDesignPanel.Invalidate();
            reportDesigner1.ActiveDesignPanel.Refresh();

            FieldListDockPanel fieldList = (FieldListDockPanel)xrDesignDockManager1[DesignDockPanelType.FieldList];
            fieldList.UpdateDataSource(host);
        }

        public class ITSCommandHandler : ICommandHandler
        {
            XRDesignPanel panel;
            XRDesignMdiController designer;
            DesignerForm form;

            public ITSCommandHandler(XRDesignPanel panel, XRDesignMdiController designer, DesignerForm form)
            {
                this.panel = panel;
                this.designer = designer;
                this.form = form;
            }

            public void HandleCommand(ReportCommand command,
            object[] args)
            {
                switch (command)
                {
                    case ReportCommand.NewReport:
                        AddNewReport(eReportType.General);
                        break;
                    case ReportCommand.NewReportWizard:
                        AddNewReport(eReportType.Document);
                        break;
                    case ReportCommand.SaveFile:
                        Save(false);
                        break;
                    case ReportCommand.SaveFileAs:
                        Save(true);
                        break;
                    case ReportCommand.OpenFile:
                        OpenFile();
                        break;
                }
            }

            public bool CanHandleCommand(ReportCommand command,
            ref bool useNextHandler)
            {
                useNextHandler = !(command == ReportCommand.SaveFile || command == ReportCommand.NewReport || command == ReportCommand.NewReportWizard ||
                    command == ReportCommand.SaveFileAs || command == ReportCommand.OpenFile);
                return !useNextHandler;
            }

            void Save(bool askFile)
            {
                if (askFile || string.IsNullOrWhiteSpace(panel.FileName))
                {
                    using (SaveFileDialog fld = new SaveFileDialog())
                    {
                        fld.Filter = "ITS report|*.itsreport";

                        DialogResult res = fld.ShowDialog();
                        if (res != DialogResult.OK)
                        {
                            return;
                        }
                        panel.FileName = fld.FileName;
                    }
                }
                (panel.Report as XtraReportBaseExtension).SaveEncrypted(panel.FileName);
                panel.ReportState = ReportState.Saved;
            }

            public void AddNewReport(eReportType reportType)
            {
                try
                {
                    XtraReportBaseExtension newXtraReport;
                    string defaultCode;
                    if (reportType == eReportType.Document)
                    {
                        newXtraReport = new SingleObjectXtraReport();
                        (newXtraReport as SingleObjectXtraReport).ObjectType = typeof(DocumentHeader);
                        newXtraReport.LinqCode = "";
                    }
                    else
                    {
                        newXtraReport = new XtraReportExtension() { Name = "New Report" };
                        defaultCode = newXtraReport.DefaultQueryCode;
                        defaultCode = defaultCode.Replace("{2}", "ItemsQuery"); //replace default return

                        defaultCode = defaultCode.Replace("{0}", "ITS").Replace("{1}", "ReportName");

                        using (CodeEditorForm codeForm = new CodeEditorForm(defaultCode, EditorType.QUERY))
                        {
                            codeForm.ShowDialog();
                            newXtraReport.LinqCode = codeForm.Code;
                        }
                    }

                    if (reportType == eReportType.Document || newXtraReport.compiledAssemblyLocation != null)
                    {

                        using (SelectTemplateForm form = new SelectTemplateForm())
                        {
                            form.ShowDialog();
                            string file = form.SelectedTemplate;
                            if (!String.IsNullOrWhiteSpace(file))
                            {
                                newXtraReport.LoadLayout(file);
                            }
                        }
                        ReportDesignExtension.AssociateReportWithExtension(newXtraReport, "RetailExtension");
                        designer.OpenReport(newXtraReport);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Report Canceled. A runtime exception occured:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, "Runtime Error");
                }

            }

            public void OpenFile()
            {
                using (OpenFileDialog fld = new OpenFileDialog())
                {
                    fld.Filter = "ITS report|*.itsreport";
                    fld.Multiselect = false;
                    DialogResult res = fld.ShowDialog();
                    if (res != DialogResult.OK)
                    {
                        return;
                    }
                    Type reportType = XtraReportBaseExtension.GetReportTypeFromFile(fld.FileName);
                    XtraReportBaseExtension xr = Activator.CreateInstance(reportType) as XtraReportBaseExtension;
                    xr.LoadEncrypted(fld.FileName);
                    designer.OpenReport(xr);
                }
            }
        }

        /// <summary>
        /// Not working
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadTemplate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (reportDesigner1.ActiveDesignPanel != null && (reportDesigner1.ActiveDesignPanel.Report as XtraReportBaseExtension) != null)
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
                    try
                    {
                        (reportDesigner1.ActiveDesignPanel.Report as XtraReportBaseExtension).LoadLayout(fld.FileName);                        
                        (reportDesigner1.ActiveDesignPanel.Report as XtraReportBaseExtension).Bands.Add(new GroupHeaderBand());
                        reportDesigner1.ActiveDesignPanel.Invalidate();
                        reportDesigner1.ActiveDesignPanel.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading template: " + Environment.NewLine + ex.Message);
                    }
                }
            }
        }

        private void scriptsCommandBarItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraReportBaseExtension report = reportDesigner1.ActiveDesignPanel == null ? null : (reportDesigner1.ActiveDesignPanel.Report as XtraReportBaseExtension);
            if (report != null)
            {
                using (CodeEditorForm codeForm = new CodeEditorForm(report.ExtraScriptsCode ?? report.DefaultExtraScriptsCode.Replace("{0}", "ITS").Replace("{1}", "ReportName"), EditorType.SCRIPTS))
                {
                    codeForm.ShowDialog();
                    (reportDesigner1.ActiveDesignPanel.Report as XtraReportBaseExtension).ExtraScriptsCode = codeForm.Code;
                }
            }
        }
    }
}
