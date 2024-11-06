using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;

namespace ITS.POS.Tools.FormDesigner.Loader
{
/// <summary>
    /// This is used by the CodeDomHostLoader to generate the CodeCompleUnit
    /// </summary>
    /// <remarks>
    /// An internal class to generate all required components for the first form in the code compile unit
    /// </remarks>
    internal class CodeGen
    {
        private CodeCompileUnit codeCompileUnit;
        private CodeNamespace ns;
        private CodeTypeDeclaration myDesignerClass = new CodeTypeDeclaration();
        private CodeMemberMethod initializeComponent = new CodeMemberMethod();
        private IDesignerHost host;
        private IComponent root;

        /// <summary>
        /// This function generates the default CodeCompileUnit template
        /// </summary>
        public CodeCompileUnit GetCodeCompileUnit(IDesignerHost host)
        {

            this.host = host;
            IDesignerHost idh = (IDesignerHost)this.host.GetService(typeof(IDesignerHost));
            root = idh.RootComponent;

            //*

            ns = new CodeNamespace("ITS.POS.Client.Forms");
            myDesignerClass = new CodeTypeDeclaration();
            initializeComponent = new CodeMemberMethod();

            CodeCompileUnit code = new CodeCompileUnit();
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            ns.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));
            ns.Imports.Add(new CodeNamespaceImport("System.Linq"));
            code.Namespaces.Add(ns);

            myDesignerClass = new CodeTypeDeclaration(root.Site.Name);
            myDesignerClass.BaseTypes.Add(host.RootComponent.GetType());

            ns.Types.Add(myDesignerClass);

            CodeConstructor con = new CodeConstructor();

            con.Attributes = MemberAttributes.Public;
            con.Statements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "InitializeComponent")));
            myDesignerClass.Members.Add(con);

            initializeComponent.Name = "InitializeComponent";
            initializeComponent.Attributes = MemberAttributes.Private;
            initializeComponent.ReturnType = new CodeTypeReference(typeof(void));
            myDesignerClass.Members.Add(initializeComponent);
            codeCompileUnit = code;
            return codeCompileUnit;
            //*/
        }

    }
}
