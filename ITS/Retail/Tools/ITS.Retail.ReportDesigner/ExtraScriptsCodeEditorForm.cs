using DevExpress.Xpo;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.CodeCompletion;
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
    public partial class ExtraScriptsCodeEditorForm : Form
    {
        protected CodeTextEditor codeEditor;

        private CSharpCompletion completion;
        //private readonly ProjectContentRegistry _registry = new ProjectContentRegistry();

        public ExtraScriptsCodeEditorForm(String code)
        {
            
            InitializeComponent();
            
            codeEditor = new CodeTextEditor();
            codeEditor.FontFamily = new System.Windows.Media.FontFamily("Consolas");
            codeEditor.FontSize = 12;
            completion = new ICSharpCode.CodeCompletion.CSharpCompletion(new ScriptProvider());
            completion.AddAssembly(typeof(XPBaseObject).Assembly.Location);
            completion.AddAssembly(typeof(ITS.Retail.Model.Address).Assembly.Location);
            completion.AddAssembly(typeof(DevExpress.Data.AsyncListDataControllerHelper).Assembly.Location);
            completion.AddAssembly(typeof(DevExpress.Data.Linq.AttachedProperty).Assembly.Location);
            completion.AddAssembly(typeof(ITS.Retail.Platform.Enumerations.DeviceResultExtensions).Assembly.Location);

            codeEditor.Completion = completion;
            
            codeEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");


            codeEditor.ShowLineNumbers = true;

            this.wpfElementHost.Child = codeEditor;

            codeEditor.Document.FileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".cs";

            //if (oldCode == null)
            //{
            //    codeEditor.Text = DefaultCode.Replace("{0}", Company).Replace("{1}", reportName);
            //}
            //else
            //{
            codeEditor.Text = code;
            //}

        }


        public string Code
        {
            get
            {
                return codeEditor.Text;
            }
        }
    }
}
