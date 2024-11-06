using System;
using System.Collections.Generic;

using System.Data;
using System.Linq;


using System.Windows.Forms;

using ITS.Retail.Common;
using System.IO;
using ICSharpCode.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Model;

namespace ITS.Retail.ReportDesigner
{
    public enum EditorType
    {
        QUERY,
        SCRIPTS
    }

    public partial class CodeEditorForm : Form
    {
        protected CodeTextEditor codeEditor;

        private CSharpCompletion completion;

        public EditorType EditorType {get; set;}

        public CodeEditorForm(String code,EditorType formType)
        {
            
            InitializeComponent();
            this.EditorType = formType;

            codeEditor = new CodeTextEditor();
            codeEditor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
            codeEditor.FontSize = 12;
            switch (formType)
            {
                case ReportDesigner.EditorType.QUERY:
                    button2.Text = "Preview Results";
                    completion = new ICSharpCode.CodeCompletion.CSharpCompletion(new QueryUsingProvider());
                    break;
                case ReportDesigner.EditorType.SCRIPTS:
                    button2.Text = "Validate";
                    completion = new ICSharpCode.CodeCompletion.CSharpCompletion(new ScriptsUsingProvider());
                    break;
            }
            completion.AddAssembly(typeof(Customer).Assembly.Location);
            completion.AddAssembly(typeof(XPBaseObject).Assembly.Location);
            completion.AddAssembly(typeof(ContainsOperator).Assembly.Location);
            completion.AddAssembly(typeof(XpoHelper).Assembly.Location);
            completion.AddAssembly(typeof(Enumerable).Assembly.Location);
            completion.AddAssembly(typeof(System.ComponentModel.ArrayConverter).Assembly.Location);
            completion.AddAssembly(typeof(decimal).Assembly.Location);
            completion.AddAssembly(typeof(DevExpress.Data.Linq.LinqInstantFeedbackSource).Assembly.Location);
            completion.AddAssembly(typeof(DevExpress.Data.ListSortInfo).Assembly.Location);
            completion.AddAssembly(typeof(DevExpress.XtraPrinting.BarCodeBrick).Assembly.Location);
            completion.AddAssembly(typeof(DevExpress.Xpo.Metadata.CanGetClassInfoByTypeEventArgs).Assembly.Location);
            completion.AddAssembly(typeof(DevExpress.XtraReports.BandKindAttribute).Assembly.Location);
            completion.AddAssembly(typeof(System.Linq.Expressions.BinaryExpression).Assembly.Location);
            completion.AddAssembly(typeof(System.Drawing.Bitmap).Assembly.Location);
            completion.AddAssembly(typeof(ITS.Retail.Platform.Enumerations.DeviceResultExtensions).Assembly.Location);
            completion.AddAssembly(typeof(ITS.Retail.Common.ExtraReportScripts).Assembly.Location);

            codeEditor.Completion = completion;
            
            codeEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");

            codeEditor.ShowLineNumbers = true;

            this.wpfElementHost.Child = codeEditor;

            codeEditor.Document.FileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".cs";

            codeEditor.Text = code;
        }


        public string Code
        {
            get
            {
                return codeEditor.Text;
            }
        }

        public bool TryCompile(String code, out List<CompilerMessage> Warnings, out List<CompilerMessage> Errors, out String compiledAssembly)
        {
            return ApplicationDomainUtility.CompileInSeparateApplicationDomain(code, out Warnings, out Errors, out compiledAssembly);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                button2.Enabled = false;
                List<CompilerMessage> warn;
                List<CompilerMessage> erro;
                String asmb;

                if (TryCompile(this.Code, out warn, out erro, out asmb))
                {
                    if (this.EditorType == ReportDesigner.EditorType.QUERY)
                    {
                        PreviewResults(asmb);
                    }
                }

                asmb = null;
                GC.Collect();

                DataTable dt = new DataTable();

                dt.Columns.Add("Type");
                dt.Columns.Add("Line");
                dt.Columns.Add("Column");
                dt.Columns.Add("Description");

                foreach (CompilerMessage ce in erro)
                {
                    dt.Rows.Add("Error", ce.Line, ce.Column, ce.ErrorText);
                }

                grdWarningError.DataSource = dt;
                grdWarningError.Columns[3].Width = grdWarningError.Width - 400;
            }
            finally
            {
                button2.Enabled = true;
            }
        }
        private void PreviewResults(String asmb)
        {
            DataTable result;
            try
            {
                ApplicationDomainUtility.ExecuteInSeparateApplicationDomain(asmb, out result);
                using (DataPreview prv = new DataPreview(result))
                {
                    prv.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A runtime exception occured:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, "Runtime Error");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }


    class QueryUsingProvider : ICSharpScriptProvider
    {
        public string GetUsing()
        {
            return "" +
                "using System; " +
                "using System.Collections.Generic; " +
                "using System.Linq; " +
                "using System.Text; ";
        }


        public string GetVars()
        {
            return null;
        }
    }

    class ScriptsUsingProvider : ICSharpScriptProvider
    {
        public string GetUsing()
        {
            return  "" +
                    "using System; " +
                    "using System.Collections.Generic; " +
                    "using System.Linq; " +
                    "using System.Text; ";
        }

        public string GetVars()
        {
            return null;
        }
    }
}
