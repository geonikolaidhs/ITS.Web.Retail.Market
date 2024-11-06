using Designer.ToolWindows;
using ITS.POS.Tools.FormDesigner.Host;
using ITS.POS.Tools.FormDesigner.Loader;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace ITS.POS.Tools.FormDesigner.Main
{
    /// <summary>
    /// This is the Shell that has the Toolbox, PropertyGrid, hosts Designers, etc.
    /// </summary>
    public partial class frmDesigner : Form
    {
        private int _formCount = 0;
        public HostSurfaceManager _hostSurfaceManager = null;
        private int _prevIndex = 0;
        private int _curIndex = 0;


        public frmDesigner()
        {
            InitializeComponent();
            CustomInitialize();
        }

        /// <summary>
        /// Adds custom services to the HostManager like TGoolbox, PropertyGrid, 
        /// SolutionExplorer.
        /// OutputWindow is added as a service. It is used by the HostSurfaceManager
        /// to write out to the OutputWindow. You can add any services
        /// you want.
        /// </summary>
        private void CustomInitialize()
        {
            _hostSurfaceManager = new HostSurfaceManager();
            _hostSurfaceManager.AddService(typeof(IToolboxService), toolbox1);
            //_hostSurfaceManager.AddService(typeof(ToolWindows.SolutionExplorer), solutionExplorer1);
            _hostSurfaceManager.AddService(typeof(OutputWindow), OutputWindow);
            //_hostSurfaceManager.AddService(typeof(System.Windows.Forms.PropertyGrid), propertyGrid1);
            _hostSurfaceManager.AddService(typeof(System.Windows.Forms.PropertyGrid), _hostSurfaceManager.PropertyGridHost.PropertyGrid);

            _hostSurfaceManager.PropertyGridHost.Parent = this.splitContainer2.Panel2;
            tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_SelectedIndexChanged);
        }

        private int CurrentDocumentsDesignIndex
        {
            get
            {
                string codeText;
                string designText;
                int index = 0;

                if (CurrentDocumentView == Strings.Design)
                {
                    return tabControl1.SelectedIndex;
                }
                else
                {
                    codeText = tabControl1.TabPages[tabControl1.SelectedIndex].Text.Trim();
                    designText = codeText.Replace(Strings.Code, Strings.Design);
                    foreach (TabPage tab in tabControl1.TabPages)
                    {
                        if (tab.Text == designText)
                        {
                            return index;
                        }
                        index++;
                    }
                }

                return -1;
            }
        }
        private int CurrentDocumentsCodeIndex
        {
            get
            {
                if (CurrentDocumentView == Strings.Code)
                {
                    return tabControl1.SelectedIndex;
                }
                int index = 0;

                string designText = tabControl1.TabPages[tabControl1.SelectedIndex].Text.Trim();
                string codeText = designText.Replace(Strings.Design, Strings.Code);

                foreach (TabPage tab in tabControl1.TabPages)
                {
                    if (tab.Text == codeText)
                    {
                        return index;
                    }
                    index++;
                }

                TabPage tabPage = new TabPage();

                tabPage.Text = codeText;
                tabPage.Tag = CurrentActiveDocumentLoaderType;
                tabControl1.Controls.Add(tabPage);

                RichTextBox codeEditor = new System.Windows.Forms.RichTextBox();

                codeEditor.BackColor = System.Drawing.SystemColors.Desktop;
                codeEditor.ForeColor = System.Drawing.Color.White;
                codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
                codeEditor.Font = new System.Drawing.Font("Verdana", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
                codeEditor.Location = new System.Drawing.Point(0, 0);
                codeEditor.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
                codeEditor.WordWrap = false;
                codeEditor.Size = new System.Drawing.Size(284, 247);
                codeEditor.TabIndex = 0;
                codeEditor.ReadOnly = true;
                codeEditor.Text = string.Empty;
                tabPage.Controls.Add(codeEditor);
                return tabControl1.TabPages.Count - 1;
            }
        }
        private HostControl CurrentDocumentsHostControl
        {
            get
            {
                return (HostControl)tabControl1.TabPages[CurrentDocumentsDesignIndex].Controls[0];
            }
        }
        private RichTextBox CurrentDocumentsCodeEditor
        {
            get
            {
                return (RichTextBox)tabControl1.TabPages[CurrentDocumentsCodeIndex].Controls[0];
            }
        }
        private LoaderType CurrentMenuSelectionLoaderType
        {
            get
            {
                return LoaderType.CodeDomDesignerLoader;
            }
        }
        private LoaderType CurrentActiveDocumentLoaderType
        {
            get
            {
                TabPage tabPage = tabControl1.TabPages[tabControl1.SelectedIndex];

                return (LoaderType)tabPage.Tag;
            }
        }
        private string CurrentDocumentView
        {
            get
            {
                TabPage tabPage = tabControl1.TabPages[tabControl1.SelectedIndex];

                if (tabPage.Text.Contains(Strings.Design))
                {
                    return Strings.Design;
                }
                else
                {
                    return Strings.Code;
                }
            }
        }


        private void cMenuItem1_Click(object sender, System.EventArgs e)
        {
            SwitchToCode(Strings.CS);
        }

        private void vBMenuItem_Click(object sender, System.EventArgs e)
        {
            SwitchToCode(Strings.VB);
        }

        private void jMenuItem1_Click(object sender, System.EventArgs e)
        {
            SwitchToCode(Strings.JS);
        }

        private void SwitchToCode(string context)
        {
            HostControl currentHostControl = CurrentDocumentsHostControl;
            RichTextBox codeEditor = CurrentDocumentsCodeEditor;
            codeEditor.Text = ((CodeDomHostLoader)currentHostControl.HostSurface.Loader).GetCode(context);

            int index = CurrentDocumentsCodeIndex;

            if (tabControl1.SelectedIndex != index)
            {
                _prevIndex = tabControl1.SelectedIndex;
                tabControl1.SelectedIndex = index;
                _curIndex = index;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentDocumentView == Strings.Design)
            {
                if (CurrentActiveDocumentLoaderType == LoaderType.CodeDomDesignerLoader)
                {
                    eMenuItem.Enabled = true;
                }
                else
                {
                    eMenuItem.Enabled = false;
                }
            }
            else
            {
                SwitchToCode(Strings.CS);
            }
        }

        /// <summary>
        /// Persist the code if the host is loaded using a BasicDesignerLoader
        /// </summary>
        private void saveMenuItem_Click(object sender, System.EventArgs e)
        {
            HostControl currentHostControl = CurrentDocumentsHostControl;
            (currentHostControl.HostSurface.Loader as CodeDomHostLoader).Save(false);
        }

        /// <summary>
        /// Open an xml file that was saved earlier
        /// </summary>
        private void openMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                string fileName = null;
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = "itsform";
                dlg.Filter = "ITS Form Files|*.itsform;*.itssform";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    fileName = dlg.FileName;
                }
                if (fileName == null)
                {
                    return;
                }
                _formCount++;
                string tempResourcesFileName = null;
                string tempCCUObjectStateFileName = null;
                HostControl hc = _hostSurfaceManager.GetNewHost(fileName, ref tempCCUObjectStateFileName, ref tempResourcesFileName);
                AddTabForNewHost("Form" + _formCount.ToString() + " - " + Strings.Design, hc);
            }
            catch
            {
                MessageBox.Show("Error in creating new host", "Shell Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveAsMenuItem_Click(object sender, System.EventArgs e)
        {
            HostControl currentHostControl = CurrentDocumentsHostControl;
            (currentHostControl.HostSurface.Loader as CodeDomHostLoader).Save(true);
        }

        /// <summary>
        /// If the host was loaded using a CodeDomDesignerLoader then we can run it
        /// </summary>
        private void runMenuItem_Click(object sender, System.EventArgs e)
        {
            HostControl currentHostControl = CurrentDocumentsHostControl;
            ((CodeDomHostLoader)currentHostControl.HostSurface.Loader).Run();
        }

        private void exitMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void AddTabForNewHost(string tabText, HostControl hc)
        {
            Toolbox.DesignerHost = hc.DesignerHost;
            TabPage tabpage = new TabPage(tabText);
            tabpage.Tag = CurrentMenuSelectionLoaderType;
            hc.Parent = tabpage;
            hc.Dock = DockStyle.Fill;
            tabControl1.TabPages.Add(tabpage);
            tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
            _hostSurfaceManager.ActiveDesignSurface = hc.HostSurface;
            if (CurrentActiveDocumentLoaderType == LoaderType.CodeDomDesignerLoader)
            {
                eMenuItem.Enabled = true;
            }
            else
            {
                eMenuItem.Enabled = false;
            }
            //solutionExplorer1.AddFileNode(tabText);
        }

        private void CreateForm(Type tp, String Filter)
        {
            try
            {
                SaveAll();
                _formCount++;
                SaveFileDialog dlg = new SaveFileDialog();

                dlg.Filter = Filter;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string fileName = dlg.FileName;
                    HostControl hc = _hostSurfaceManager.GetNewHost(tp, CurrentMenuSelectionLoaderType);

                    ISelectionService selectionService = (ISelectionService)(hc.DesignerHost.GetService(typeof(ISelectionService)));
                    selectionService.SelectionChanged += selectionService_SelectionChanged;


                    hc.FileName = fileName;

                    hc.TempCCUObjectStateFileName = Path.ChangeExtension(Path.GetTempFileName(), ".xml");
                    hc.TempResourcesFileName = Path.ChangeExtension(Path.GetTempFileName(), ".resources");

                    AddTabForNewHost("Form" + _formCount.ToString() + " - " + Strings.Design, hc);
                    CurrentDocumentsHostControl.FileName = fileName;
                    (CurrentDocumentsHostControl.HostSurface.Loader as CodeDomHostLoader).FileName = fileName;
                    (CurrentDocumentsHostControl.HostSurface.Loader as CodeDomHostLoader).TempCCUObjectStateFileName = hc.TempCCUObjectStateFileName;
                    (CurrentDocumentsHostControl.HostSurface.Loader as CodeDomHostLoader).TempResourcesFileName = hc.TempResourcesFileName;
                }
            }
            catch
            {
                MessageBox.Show("Error in creating new host", "Shell Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void selectionService_SelectionChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            IDesignerHost isurf = CurrentDocumentsHostControl.DesignerHost;
            if (null != isurf)
            {
                ISelectionService selectionService = null;
                selectionService = isurf.GetService(typeof(ISelectionService)) as ISelectionService;
                _hostSurfaceManager.PropertyGridHost.PropertyGrid.SelectedObject = selectionService.PrimarySelection;
            }
        }


        private void formMenuItem_Click(object sender, System.EventArgs e)
        {
            CreateForm(typeof(ITS.POS.Client.Forms.frmMainBase), "ITS Form Files|*.itsform;*.itssform");
        }


        /// <summary>
        /// Perform all the Edit menu options using the MenuCommandService
        /// </summary>
        private void PerformAction(string text)
        {
            if (CurrentDocumentView == Strings.Code)
            {
                MessageBox.Show("This is not in supported code view");
                return;
            }

            if (CurrentDocumentsHostControl == null)
            {
                return;
            }
            IMenuCommandService ims = CurrentDocumentsHostControl.HostSurface.GetService(typeof(IMenuCommandService)) as IMenuCommandService;

            try
            {
                switch (text)
                {
                    case "&Cut":
                        ims.GlobalInvoke(StandardCommands.Cut);
                        break;
                    case "C&opy":
                        ims.GlobalInvoke(StandardCommands.Copy);
                        break;
                    case "&Paste":
                        ims.GlobalInvoke(StandardCommands.Paste);
                        break;
                    case "&Undo":
                        ims.GlobalInvoke(StandardCommands.Undo);
                        break;
                    case "&Redo":
                        ims.GlobalInvoke(StandardCommands.Redo);
                        break;
                    case "&Delete":
                        ims.GlobalInvoke(StandardCommands.Delete);
                        break;
                    case "&Select All":
                        ims.GlobalInvoke(StandardCommands.SelectAll);
                        break;
                    case "&Lefts":
                        ims.GlobalInvoke(StandardCommands.AlignLeft);
                        break;
                    case "&Centers":
                        ims.GlobalInvoke(StandardCommands.AlignHorizontalCenters);
                        break;
                    case "&Rights":
                        ims.GlobalInvoke(StandardCommands.AlignRight);
                        break;
                    case "&Tops":
                        ims.GlobalInvoke(StandardCommands.AlignTop);
                        break;
                    case "&Middles":
                        ims.GlobalInvoke(StandardCommands.AlignVerticalCenters);
                        break;
                    case "&Bottoms":
                        ims.GlobalInvoke(StandardCommands.AlignBottom);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                OutputWindow.RichTextBox.Text += "Error in performing the action: " + text.Replace("&", string.Empty);
            }
        }

        private void ActionClick(object sender, EventArgs e)
        {
            PerformAction((sender as MenuItem).Text);
        }

        private void openMenuItem1_Click(object sender, EventArgs e)
        {
            runMenuItem_Click(sender, e);
        }

        private void abMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ITS POS Form Designer");
        }

        private class Strings
        {
            public const string Design = "Design";
            public const string Code = "Code";
            public const string Xml = "Xml";
            public const string CS = "C#";
            public const string JS = "J#";
            public const string VB = "VB";
        }

        private void Toolbox_Load(object sender, EventArgs e)
        {
        }

        private void fileMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            saveMenuItem.PerformClick();
            runMenuItem_Click(sender, e);
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            CreateForm(typeof(ITS.POS.Client.Forms.frmSupportingBase), "ITS Supporting Form Files|*.itssform");
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            int totalForms = this._formCount;
            if (totalForms == 1)
            {
                menuItem1_Click(sender, e);
            }
            else
            {
                HostControl mainForm = null, supportingForm = null, current = null;

                for (int i = 0; i < totalForms; i++)
                {
                    current = (HostControl)tabControl1.TabPages[i].Controls[0];

                    if (mainForm == null && (current.HostSurface.Loader as CodeDomHostLoader).RootType == typeof(ITS.POS.Client.Forms.frmMainBase))
                    {
                        mainForm = current;
                    }
                    if (supportingForm == null && (current.HostSurface.Loader as CodeDomHostLoader).RootType == typeof(ITS.POS.Client.Forms.frmSupportingBase))
                    {
                        supportingForm = current;
                    }
                    if (mainForm != null && supportingForm != null)
                    {
                        break;
                    }
                }
                (mainForm.HostSurface.Loader as CodeDomHostLoader).Save(false);
                (supportingForm.HostSurface.Loader as CodeDomHostLoader).Save(false);

                CompilerParameters cp = null;
                CodeCompileUnit ccu1 = null, ccu2 = null;
                String outputFilename = "";
                (mainForm.HostSurface.Loader as CodeDomHostLoader).PrepareBuild(ref cp, ref outputFilename, ref ccu1);
                (supportingForm.HostSurface.Loader as CodeDomHostLoader).PrepareBuild(ref cp, ref outputFilename, ref ccu2);

                CSharpCodeProvider cc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
                CompilerResults cr = cc.CompileAssemblyFromDom(cp, ccu1, ccu2);

                if (cr.Errors.HasErrors)
                {
                    string errors = string.Empty;

                    foreach (CompilerError error in cr.Errors)
                    {
                        errors += error.ErrorText + "\n";
                    }

                    MessageBox.Show(errors, "Errors during compile.");
                }
                else
                {
                    MessageBox.Show("Successful Build", "Build output", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }

        private void SaveAll()
        {
            for (int i = 0; i < _formCount; i++)
            {
                HostControl current = (HostControl)tabControl1.TabPages[i].Controls[0];
                (current.HostSurface.Loader as CodeDomHostLoader).Save(false);
            }
        }
    }
}
