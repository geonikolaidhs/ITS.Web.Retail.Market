using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml;

using Microsoft.CSharp;
using Microsoft.VisualBasic;
using SevenZip.Compression.LZMA;
using ITS.POS.Client;
using ITS.POS.Client.Forms;



namespace ITS.POS.Tools.FormDesigner.Loader
{
    /// <summary>
    /// Inherits from CodeDomDesignerLoader. It can generate C# or VB code
    /// for a HostSurface. This loader does not support parsing a 
    /// C# or VB file.
    /// </summary>
    public class CodeDomHostLoader : CodeDomDesignerLoader
    {
        private CSharpCodeProvider _csCodeProvider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
        private CodeCompileUnit codeCompileUnit = null;
        private CodeGen cg = null;
        private TypeResolutionService _trs = null;
        private string executable;

        public string FileName = String.Empty;
        public string TempCCUObjectStateFileName = string.Empty;
        public string TempResourcesFileName = String.Empty;

        private Type root;
        private IDesignerLoaderHost host;
        private static readonly Attribute[] propertyAttributes = new Attribute[] {
            DesignOnlyAttribute.No
        };

        public IDesignerLoaderHost GetLoaderHost()
        {
            return LoaderHost;
        }

        public CodeDomHostLoader()
        {
            _trs = new TypeResolutionService();            
            //host.AddService(typeof(ITypeResolutionService), _trs);
            root = typeof(Form);
        }

        public CodeDomHostLoader(Type rootType)
        {
            _trs = new TypeResolutionService();
           // host.AddService(typeof(ITypeResolutionService), _trs);
            root = rootType;
        }

        public CodeDomHostLoader(String filename)
        {
            _trs = new TypeResolutionService();
            //host.AddService(typeof(ITypeResolutionService), _trs);
            FileName = filename;
        }

        protected override ITypeResolutionService TypeResolutionService
        {
            get
            {
                return _trs;
            }
        }

        protected override CodeDomProvider CodeDomProvider
        {
            get
            {
                return _csCodeProvider;
            }
        }


        /// <summary>
        /// Bootstrap method - loads a blank Form
        /// </summary>
        /// <returns></returns>
        protected override CodeCompileUnit Parse()
        {
            CodeCompileUnit ccu = null;
            cg = new CodeGen();
            DesignSurface ds = new DesignSurface();
            
            if (string.IsNullOrEmpty(FileName))
            {
                IDesignerHost idh = (IDesignerHost)ds.GetService(typeof(IDesignerHost));
                idh.AddService(typeof(ITypeResolutionService), this.TypeResolutionService);
                bool hasloadedran = false;
                AssemblyName rootAN = root.Assembly.GetName();

                ds.BeginLoad(root);

                if (idh.RootComponent.GetType() == typeof(frmMainBase))
                {
                    idh.RootComponent.Site.Name = "frmMainCustom";
                }
                else if (idh.RootComponent.GetType() == typeof(frmSupportingBase))
                {
                    idh.RootComponent.Site.Name = "frmSupportingCustom";
                }

                ccu =  cg.GetCodeCompileUnit(idh);
                AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                for (int i = 0; i < names.Length; i++)
                {
                    Assembly assembly = Assembly.Load(names[i]);
                    ccu.ReferencedAssemblies.Add(assembly.Location);
                    if (rootAN == names[i])
                    {
                        hasloadedran = true;
                    }
                }
                if (!hasloadedran)
                {
                    ccu.ReferencedAssemblies.Add(root.Assembly.Location);
                }
            }
            else
            {
                using (FileStream sr = new FileStream(this.TempCCUObjectStateFileName, FileMode.Open, FileAccess.Read))
                {
                    IEnumerable<Type> types = typeof(CodeNamespace).Assembly.GetTypes().Where(g => g.FullName.Contains("System.CodeDom") && !g.FullName.Contains("System.CodeDom.Compiler") && !g.FullName.Contains("Collection") && !g.IsInterface);


                    IEnumerable<Type> reftype = typeof(System.Reflection.TypeAttributes).Assembly.GetTypes().Where(g => g.FullName.Contains("System.Reflection.TypeAttributes") && !g.IsInterface && !g.FullName.Contains("Reflection.Cer"));
                    IEnumerable<Type> knownTypesList = types.Union(reftype);

                    DataContractSerializer ddd = new DataContractSerializer(typeof(CodeNamespace), knownTypesList, int.MaxValue, true, true, new InvalidEnumContractSurrogate(typeof(MemberAttributes)));
                    object ob = ddd.ReadObject(sr);
                    ccu = (CodeCompileUnit)ob;
                    var list = ccu.ReferencedAssemblies.Cast<string>().ToList();
                    ccu.ReferencedAssemblies.Clear();
                    foreach (String asm in list)
                    {
                        Assembly asm2 = GetAssembly(asm);
                        ccu.ReferencedAssemblies.Add(asm2.Location);
                        foreach (AssemblyName asm3 in asm2.GetReferencedAssemblies())
                        {
                            if (asm3.CodeBase != null)
                            {
                                GetAssembly(asm3.CodeBase);
                            }
                        }
                    }
                }
            }
            codeCompileUnit = ccu;
            return ccu;
        }

        private Assembly GetAssembly(string path)
        {
            if (File.Exists(path))
            {
                return Assembly.LoadFile(path);
            }

            string name = Path.GetFileName(path);
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (File.Exists(currentDirectory + "\\" + name))
            {
                return Assembly.LoadFile(currentDirectory + "\\" + name);
            }

            return null;
        }

        /// <summary>
        /// When the Loader is Flushed this method is called. The base class
        /// (CodeDomDesignerLoader) creates the CodeCompileUnit. We
        /// simply cache it and use this when we need to generate code from it.
        /// To generate the code we use CodeProvider.
        /// </summary>
        protected override void Write(CodeCompileUnit unit)
        {
            codeCompileUnit = unit;
        }

        protected override void OnEndLoad(bool successful, ICollection errors)
        {
            base.OnEndLoad(successful, errors);
            if (errors != null)
            {
                IEnumerator ie = errors.GetEnumerator();
                while (ie.MoveNext())
                {
                    System.Diagnostics.Trace.WriteLine(ie.Current.ToString());
                }
            }
        }

        /// <summary>
        /// Flushes the host and returns the updated CodeCompileUnit
        /// </summary>
        /// <returns></returns>
        public CodeCompileUnit GetCodeCompileUnit()
        {
            Flush();
            return codeCompileUnit;
        }


        /// <summary>
        /// This method writes out the contents of our designer in C# and VB.
        /// It generates code from our codeCompileUnit using CodeRpovider
        /// </summary>
        public string GetCode(string context)
        {

            string mainClass = LoaderHost.RootComponentClassName;

            if (File.Exists(this.TempResourcesFileName))
            {
                File.Copy(this.TempResourcesFileName, Path.GetTempPath() + mainClass + ".resources", true);
            }
            string resourceFile = Path.GetTempPath() + mainClass + ".resources";
            executable = Path.ChangeExtension(FileName, ".dll");
            ResourcesDesigner rs = LoaderHost.GetService(typeof(IResourceService)) as ResourcesDesigner;
            rs.Filename = resourceFile;


            Flush();

            CodeGeneratorOptions o = new CodeGeneratorOptions();

            o.BlankLinesBetweenMembers = true;
            o.BracingStyle = "C";
            o.ElseOnClosing = false;
            o.IndentString = "    ";
            if (context == "C#")
            {
                StringWriter swCS = new StringWriter();
                CSharpCodeProvider cs = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });

                cs.GenerateCodeFromCompileUnit(codeCompileUnit, swCS, o);
                string code = swCS.ToString();
                swCS.Close();
                return code;
            }
            else
            {
                if (context == "VB")
                {
                    StringWriter swVB = new StringWriter();
                    VBCodeProvider vb = new VBCodeProvider();

                    vb.GenerateCodeFromCompileUnit(codeCompileUnit, swVB, o);
                    string code = swVB.ToString();
                    swVB.Close();
                    return code;
                }
            }
            return String.Empty;
        }
        public Type RootType
        {
            get
            {
                return LoaderHost.RootComponent.GetType();
            }
        }
        public bool PrepareBuild(ref CompilerParameters cp, ref String outputFilename, ref CodeCompileUnit ccu)
        {
            string mainClass = LoaderHost.RootComponentClassName;
            if (File.Exists(this.TempResourcesFileName))
            {
                File.Copy(this.TempResourcesFileName, Path.GetTempPath() + mainClass + ".resources", true);
            }
            string resourceFile = Path.GetTempPath() + mainClass + ".resources";
            executable = Path.ChangeExtension(FileName, ".dll");
            ResourcesDesigner rs = LoaderHost.GetService(typeof(IResourceService)) as ResourcesDesigner;
            rs.Filename = resourceFile;
            Flush();

            if(String.IsNullOrWhiteSpace(outputFilename))
            {
                outputFilename = executable;
            }
            else
            {
                String otherFn = Path.GetFileNameWithoutExtension(outputFilename);
                String thisFn = Path.GetFileNameWithoutExtension(executable);
                String newFn = otherFn + thisFn;
                outputFilename = Path.GetDirectoryName(outputFilename) + "\\" + newFn + ".dll";
            }
            if (outputFilename != null)
            {
                if (cp == null)
                {
                    cp = new CompilerParameters();
                }
                foreach (String asm in codeCompileUnit.ReferencedAssemblies)
                {
                    Assembly asm2 = GetAssembly(asm);
                    foreach (AssemblyName asm3 in asm2.GetReferencedAssemblies())
                    {
                        Assembly asm4 = Assembly.Load(asm3);
                        if (asm4.Location != null)
                        {
                            if (cp.ReferencedAssemblies.Contains(asm4.Location)==false)
                            {
                                cp.ReferencedAssemblies.Add(asm4.Location);
                            }                            
                        }
                    }
                }

                AssemblyName[] assemblyNames = Assembly.GetEntryAssembly().GetReferencedAssemblies();

                foreach (AssemblyName an in assemblyNames)
                {
                    Assembly assembly = Assembly.Load(an);
                    if (cp.ReferencedAssemblies.Contains(assembly.Location) == false)
                    {
                        cp.ReferencedAssemblies.Add(assembly.Location);
                    }
                }

                cp.IncludeDebugInformation = true;
                cp.GenerateExecutable = false;
                cp.OutputAssembly = outputFilename;
                if (File.Exists(rs.Filename))
                {
                    cp.EmbeddedResources.Add(rs.Filename);
                }

                ccu = this.codeCompileUnit;
            }

            return false;
        }

        /// <summary>
        /// Called when we want to build an executable. Returns true if we succeeded.
        /// </summary>
        public bool Build()
        {
            string mainClass = LoaderHost.RootComponentClassName;

            if (File.Exists(this.TempResourcesFileName))
            {
                File.Copy(this.TempResourcesFileName, Path.GetTempPath() + mainClass + ".resources", true);
            }
            string resourceFile = Path.GetTempPath() + mainClass+".resources";
            executable = Path.ChangeExtension(FileName, ".dll");
            ResourcesDesigner rs = LoaderHost.GetService(typeof(IResourceService)) as ResourcesDesigner;
            rs.Filename = resourceFile;
            
            Flush();


            if (executable != null)
            {
                CompilerParameters cp = new CompilerParameters();
                foreach (String asm in codeCompileUnit.ReferencedAssemblies)
                {
                    Assembly asm2 = GetAssembly(asm);
                    foreach (AssemblyName asm3 in asm2.GetReferencedAssemblies())
                    {
                        Assembly asm4 = Assembly.Load(asm3);
                        if (asm4.Location!= null)
                        {
                            cp.ReferencedAssemblies.Add(asm4.Location);
                            //Assembly.LoadFile(asm3.CodeBase);
                        }
                    }
                }

                AssemblyName[] assemblyNames = Assembly.GetEntryAssembly().GetReferencedAssemblies();

                foreach (AssemblyName an in assemblyNames)
                {
                    Assembly assembly = Assembly.Load(an);
                    cp.ReferencedAssemblies.Add(assembly.Location);
                }
                

                cp.GenerateExecutable = false;
                cp.IncludeDebugInformation = true;
                cp.OutputAssembly = executable;
                if (File.Exists(rs.Filename))
                {
                    cp.EmbeddedResources.Add(rs.Filename);
                }

                cp.MainClass = mainClass;

                CSharpCodeProvider cc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
                CompilerResults cr = cc.CompileAssemblyFromDom(cp, codeCompileUnit);

                if (cr.Errors.HasErrors)
                {
                    string errors = string.Empty;

                    foreach (CompilerError error in cr.Errors)
                    {
                        errors += error.ErrorText + "\n";
                    }

                    MessageBox.Show(errors, "Errors during compile.");
                }

                return !cr.Errors.HasErrors;
            }

            return false;
        }

        /// <summary>
        /// Here we build the executable and then run it. We make sure to not start
        /// two of the same process.
        /// </summary>
        public void Run()
        {
            if (Build())
            {
                MessageBox.Show("Successful Build", "Build output", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Unsuccessful Build", "Build output", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }


        public void Save(bool forceFilePrompt)
        {
            try
            {
                int filterIndex = 1;

                if (host == null)
                {
                    host = LoaderHost;

                }
                if ((String.IsNullOrEmpty(FileName)) || forceFilePrompt)
                {
                    SaveFileDialog dlg = new SaveFileDialog();

                    dlg.DefaultExt = "itsform";
                    dlg.Filter = "ITS Form Files|*.itsform";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        FileName = dlg.FileName;
                        filterIndex = dlg.FilterIndex;
                    }
                }

                foreach (Control control in host.Container.Components.OfType<Control>())
                {
                    control.Left++;
                    control.Left--;
                }

                this.Modified = true;
                
                ResourcesDesigner rs = host.GetService(typeof(IResourceService)) as ResourcesDesigner;
                //string mainClass = "ITS.POS.Client.frmMainCustom";
                string resourceFile = this.TempResourcesFileName;
                rs.Filename = resourceFile;
                Flush();

                if (FileName != null)
                {
                    if(File.Exists(FileName))
                    {
                        File.Move(FileName,FileName+"_");
                    }

                    using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(FileName))
                    {
                        
                        using (FileStream writer = new FileStream(TempCCUObjectStateFileName, FileMode.Create))
                        {
                            IEnumerable<Type> types = typeof(CodeNamespace).Assembly.GetTypes().Where(g => g.FullName.Contains("System.CodeDom") && !g.FullName.Contains("System.CodeDom.Compiler") && !g.FullName.Contains("Collection") && !g.IsInterface);
                            IEnumerable<Type> reftype = typeof(System.Reflection.TypeAttributes).Assembly.GetTypes().Where(g => g.FullName.Contains("System.Reflection.TypeAttributes") && !g.IsInterface && !g.FullName.Contains("Reflection.Cer"));
                            IEnumerable<Type> knownTypesList = types.Union(reftype);

                            DataContractSerializer ddd = new DataContractSerializer(typeof(CodeNamespace), knownTypesList, int.MaxValue, true, true, new InvalidEnumContractSurrogate(typeof(MemberAttributes)));
                            ddd.WriteObject(writer, codeCompileUnit);
                        }
                        zip.AddFile(TempCCUObjectStateFileName,"\\");
                        if (File.Exists(TempResourcesFileName))
                        {
                            string newResourceFile = Path.GetTempPath() + host.RootComponent.GetType().FullName + ".resources";
                            if (File.Exists(newResourceFile))
                            {
                                File.Delete(newResourceFile);
                            }

                            File.Copy(TempResourcesFileName, newResourceFile);
                            zip.AddFile(newResourceFile, "\\");
                            zip.Save();
                            File.Delete(newResourceFile);
                        }
                        else
                        {
                            zip.Save();
                        }
                        

                    }

                    if (File.Exists(FileName + "_"))
                    {
                        File.Delete(FileName + "_");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during save: " + ex.ToString());
            }
        }
        public class InvalidEnumContractSurrogate : IDataContractSurrogate
        {
            private HashSet<Type> typelist;

            /// <summary>
            /// Create new Data Contract Surrogate to handle the specified Enum type
            /// </summary>
            /// <param name="type">Enum Type</param>
            public InvalidEnumContractSurrogate(Type type)
            {
                typelist = new HashSet<Type>();
                if (!type.IsEnum)
                {
                    throw new ArgumentException(type.Name + " is not an enum", "type");
                }
                typelist.Add(type);
            }

            /// <summary>
            /// Create new Data Contract Surrogate to handle the specified Enum types
            /// </summary>
            /// <param name="types">IEnumerable of Enum Types</param>
            public InvalidEnumContractSurrogate(IEnumerable<Type> types)
            {
                typelist = new HashSet<Type>();
                foreach (var type in types)
                {
                    if (!type.IsEnum)
                    {
                        throw new ArgumentException(type.Name + " is not an enum", "type");
                    }
                    typelist.Add(type);
                }
            }



            public Type GetDataContractType(Type type)
            {
                if (typelist.Contains(type))
                {
                    return typeof(int);
                }
                return type;
            }

            public object GetObjectToSerialize(object obj, Type targetType)
            {
                if (typelist.Contains(obj.GetType()))
                {
                    return (int)obj;
                }
                return obj;
            }

            public object GetDeserializedObject(object obj, Type targetType)
            {
                if (typelist.Contains(targetType))
                {
                    return Enum.ToObject(targetType, obj);
                }
                return obj;
            }

            public void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
            {
            }

            public object GetCustomDataToExport(Type clrType, Type dataContractType)
            {
                return null;
            }

            public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
            {
                return null;
            }

            public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
            {
                return null;
            }

            public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
            {
                return typeDeclaration;
            }
        }
    }
}
