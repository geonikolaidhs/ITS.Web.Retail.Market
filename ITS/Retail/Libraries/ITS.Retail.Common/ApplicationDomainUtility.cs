using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using Microsoft.CSharp;
using System.Security.Cryptography;


namespace ITS.Retail.Common
{

    public static class ApplicationDomainUtility
    {
        // Allow a test to modify static fields in an independent appdomain so that
        // other tests will not be affected.]

        public static bool CompileInSeparateApplicationDomain(string codeToRun, out List<CompilerMessage> Warn, out List<CompilerMessage> Err, out string assemblyLocation)
        {
            return CompileInSeparateApplicationDomain(new AppDomainSetup(), codeToRun, out Warn, out Err, out assemblyLocation);
        }

        public static bool CompileInSeparateApplicationDomain(AppDomainSetup setup, string codeToRun, out List<CompilerMessage> Warn, out List<CompilerMessage> Err, out string assemblyLocation)
        {
            var dir = Path.GetDirectoryName(typeof(ApplicationDomainUtility).Assembly.CodeBase).Replace("file:\\", "");
            setup.PrivateBinPath = dir;
            setup.ApplicationBase = dir;
            setup.ApplicationName = "ITS.AppDomainUtility";
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = setup.ApplicationBase;
            setup.CachePath = Path.Combine(Path.GetTempPath(), setup.ApplicationName);
            bool value = false;
            AppDomain appDomain = null;
            try
            {
                appDomain = AppDomain.CreateDomain(setup.ApplicationName, null, setup);
                CompilationDomainHelper helper = appDomain.CreateInstanceAndUnwrap(typeof(ApplicationDomainUtility).Assembly.FullName, typeof(CompilationDomainHelper).FullName) as CompilationDomainHelper;
                List<CompilerMessage> Warn2, Err2;
                value = helper.Run(codeToRun, out Warn2, out Err2, out assemblyLocation);
                Warn = new List<CompilerMessage>();
                Err = new List<CompilerMessage>();
                foreach (CompilerMessage m in Warn)
                {
                    Warn.Add(new CompilerMessage() { Column = m.Column, ErrorNumber = m.ErrorNumber, ErrorText = m.ErrorText, File = m.File, IsWarning = m.IsWarning, Line = m.Line });
                }
                foreach (CompilerMessage m in Err2)
                {
                    Err.Add(new CompilerMessage() { Column = m.Column, ErrorNumber = m.ErrorNumber, ErrorText = m.ErrorText, File = m.File, IsWarning = m.IsWarning, Line = m.Line });
                }
            }
            finally
            {
                if (appDomain != null)
                {
                    AppDomain.Unload(appDomain);
                }
            }
            return value;
        }

        public static void ExecuteInSeparateApplicationDomain(string assemblyToLoad, out DataTable result)
        {
            ExecuteInSeparateApplicationDomain(new AppDomainSetup(), assemblyToLoad, out result);
        }

        public static void ExecuteInSeparateApplicationDomain(AppDomainSetup setup, string assemblyToLoad, out DataTable result)
        {
            var dir = Path.GetDirectoryName(typeof(ApplicationDomainUtility).Assembly.CodeBase).Replace("file:\\", "");
            setup.PrivateBinPath = dir;
            setup.ApplicationBase = dir;
            setup.ApplicationName = Guid.NewGuid().ToString();
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = setup.ApplicationBase;
            setup.CachePath = Path.Combine(Path.GetTempPath(), setup.ApplicationName);

            AppDomain appDomain = AppDomain.CreateDomain(setup.ApplicationName, null, setup);
            try
            {
                LinqExpressionDomainHelper helper = appDomain.CreateInstanceAndUnwrap(typeof(ApplicationDomainUtility).Assembly.FullName, typeof(LinqExpressionDomainHelper).FullName) as LinqExpressionDomainHelper;

                helper.sqlserver = XpoHelper.sqlserver;
                helper.dbtype = XpoHelper.databasetype;
                helper.database = XpoHelper.database;
                helper.username = XpoHelper.username;
                helper.pass = XpoHelper.pass;
                helper.Run(assemblyToLoad);
                result = helper.result.Copy();
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
        }


        public class LinqExpressionDomainHelper : MarshalByRefObject
        {
            [field: NonSerialized()]
            public DataTable result;
            public string sqlserver, database, username, pass;
            public DBType dbtype;
            public void Run(string assebmly)
            {

                XpoHelper.sqlserver = sqlserver;
                XpoHelper.databasetype = dbtype;

                XpoHelper.database = database;
                XpoHelper.username = username;
                XpoHelper.pass = pass;

                AbstractLinqQuery ModelQuery;
                Assembly assembly = Assembly.LoadFile(assebmly);
                var types = assembly.GetTypes().Where(g => g.IsSubclassOf(typeof(AbstractLinqQuery)));
                result = null;
                if (types.Count() == 1)
                {
                    ModelQuery = Activator.CreateInstance(types.First()) as AbstractLinqQuery;
                    IQueryable result1 = ModelQuery.MainQuerySet();
                    result = new DataTable();
                    Type tp = result1.ElementType;
                    List<PropertyInfo> Properties = new List<PropertyInfo>();
                    List<FieldInfo> Fields = new List<FieldInfo>();
                    foreach (PropertyInfo pInfo in tp.GetProperties())
                    {
                        if (pInfo.CanRead && pInfo.PropertyType.IsSubclassOf(typeof(IEnumerable)) == false)
                        {
                            result.Columns.Add(pInfo.Name);
                            Properties.Add(pInfo);
                        }
                    }
                    foreach (FieldInfo fInfo in tp.GetFields())
                    {
                        if (fInfo.Attributes == FieldAttributes.Public && fInfo.FieldType.IsSubclassOf(typeof(IEnumerable)) == false)
                        {
                            result.Columns.Add(fInfo.Name);
                            Fields.Add(fInfo);
                        }
                    }
                    foreach (object o in result1)
                    {
                        DataRow row = result.NewRow();
                        foreach (PropertyInfo pInfo in Properties)
                        {
                            row[pInfo.Name] = pInfo.GetValue(o, null);
                        }
                        foreach (FieldInfo fInfo in Fields)
                        {
                            row[fInfo.Name] = fInfo.GetValue(o);
                        }
                        result.Rows.Add(row);
                        if (result.Rows.Count > 1000)
                            break;
                    }
                }
            }
        }

        public class CompilationDomainHelper : MarshalByRefObject
        {
            public bool Run(string code, out List<CompilerMessage> Warnings, out List<CompilerMessage> Errors, out string compiledAssembly)
            {
                Warnings = new List<CompilerMessage>();
                Errors = new List<CompilerMessage>();
                string filename = GenerateFileName(code);
                string fullfilename = Path.GetTempPath() + "\\temp" + filename + ".dll";
                if (File.Exists(fullfilename))
                {
                    compiledAssembly = fullfilename;
                    return true;
                }
                CSharpCodeProvider cscp = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });

                CodeSnippetCompileUnit cscu = new CodeSnippetCompileUnit(code);
                CompilerParameters cp = new CompilerParameters();

                cp.ReferencedAssemblies.Add(typeof(Customer).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(XPBaseObject).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(ContainsOperator).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(XpoHelper).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(Enumerable).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(System.ComponentModel.ArrayConverter).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(decimal).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(DevExpress.Data.Linq.LinqInstantFeedbackSource).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(DevExpress.Data.ListSortInfo).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(DevExpress.XtraPrinting.BarCodeBrick).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(DevExpress.Xpo.Metadata.CanGetClassInfoByTypeEventArgs).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(DevExpress.XtraReports.BandKindAttribute).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(System.Linq.Expressions.BinaryExpression).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(System.Drawing.Bitmap).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(Platform.Enumerations.DeviceResultExtensions).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(ExtraReportScripts).Assembly.Location);
                cp.ReferencedAssemblies.Add(typeof(Platform.Kernel.Model.IPersistentObject).Assembly.Location);
                cp.ReferencedAssemblies.Add("System.Data.dll");
                cp.ReferencedAssemblies.Add("System.Xml.dll");
                cp.ReferencedAssemblies.Add("System.Xml.Linq.dll");

                cp.IncludeDebugInformation = false;
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = false;
                cp.OutputAssembly = fullfilename;

                CompilerResults results = cscp.CompileAssemblyFromDom(cp, cscu);


                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError er in results.Errors)
                    {
                        CompilerMessage msg = new CompilerMessage() { Column = er.Column, ErrorNumber = er.ErrorNumber, ErrorText = er.ErrorText, File = er.FileName, Line=er.Line.ToString() };
                        if (er.IsWarning)
                        {
                            Warnings.Add(msg);

                        }
                        else
                        {
                            Errors.Add(msg);
                        }
                    }
                }
                if (Errors.Count == 0)
                {
                    compiledAssembly = results.CompiledAssembly.Location;
                    return true;
                }
                compiledAssembly = null;
                return false;
            }

            private string GenerateFileName(string code)
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    var originalBytes = Encoding.Default.GetBytes(code);
                    var encodedBytes = md5.ComputeHash(originalBytes);
                    return BitConverter.ToString(encodedBytes).Replace("-", "");
                }
            }
        }
    }
}
