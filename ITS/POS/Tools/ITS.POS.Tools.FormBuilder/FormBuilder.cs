using Ionic.Zip;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace ITS.POS.Tools.FormBuilder
{
    public class FormBuilder
    {


        protected string MainFormFileName { get; set; }
        protected string SecondaryFormFileName { get; set; }
        protected string TempFolder { get; set; }
        protected StreamWriter Logger { get; set; }

        private List<string> TemporaryFiles { get; set; }

        public FormBuilder(string mainFormFileName, string secondaryFormFileName, string tempFolder, StreamWriter logger)
        {
            MainFormFileName = mainFormFileName;
            SecondaryFormFileName = secondaryFormFileName;
            TempFolder = tempFolder;
            if (String.IsNullOrWhiteSpace(TempFolder))
            {
                TempFolder = Path.GetTempPath();
            }

            TemporaryFiles = new List<string>();
            this.Logger = logger;
        }

        public string BuildAll(out int exitCode)
        {
            try
            {
                this.Logger.AutoFlush = true;
                exitCode = 0;
                CompilerParameters cp = new CompilerParameters();

                string outputFilename = "";
                if (String.IsNullOrWhiteSpace(MainFormFileName) == false)
                {
                    outputFilename = Path.ChangeExtension(MainFormFileName, ".dll");
                }
                else if (String.IsNullOrWhiteSpace(SecondaryFormFileName) == false)
                {
                    outputFilename = Path.ChangeExtension(SecondaryFormFileName, ".dll");
                }
                else
                {
                    string message = "No file specified";
                    this.Logger.WriteLine(DateTime.Now + " - " + message);
                    return message;
                }

                cp.OutputAssembly = outputFilename;

                CodeCompileUnit ccu1 = null;

                if (String.IsNullOrWhiteSpace(MainFormFileName) == false)
                {
                    ccu1 = GetCodeCompileUnit(MainFormFileName, cp);
                }

                CodeCompileUnit ccu2 = null;

                if (String.IsNullOrWhiteSpace(SecondaryFormFileName) == false)
                {
                    ccu2 = GetCodeCompileUnit(SecondaryFormFileName, cp);
                }

                CSharpCodeProvider cc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
                CompilerResults cr = null;
                if (ccu2 != null && ccu1 != null)
                {
                    cr = cc.CompileAssemblyFromDom(cp, ccu1, ccu2);
                }
                else if (ccu1 != null)
                {
                    cr = cc.CompileAssemblyFromDom(cp, ccu1);
                }
                else if (ccu2 != null)
                {
                    cr = cc.CompileAssemblyFromDom(cp, ccu2);
                }
                else
                {
                    exitCode = -1;
                    string message = "No file to compile";
                    this.Logger.WriteLine(DateTime.Now + " - " + message);
                    return (message);
                }

                this.CleanTemporaryFiles();

                if (cr.Errors.HasErrors)
                {
                    string errors = string.Empty;

                    foreach (CompilerError error in cr.Errors)
                    {
                        errors += error.ErrorText + "\n";
                    }

                    exitCode = -2;
                    this.Logger.WriteLine(DateTime.Now + " - " + errors);
                    return errors;
                }
                else
                {
                    string message = "Successful build";
                    this.Logger.WriteLine(DateTime.Now + " - " + message);
                    return message;
                }

            }
            catch (Exception ex)
            {
                if (Logger != null)
                {
                    Logger.WriteLine(DateTime.Now + " - " + Program.GetFullMessage(ex));
                }
                exitCode = -3;
                return ex.Message;
            }
        }

        private void CleanTemporaryFiles()
        {
            foreach (string file in TemporaryFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }

            TemporaryFiles.Clear();
        }

        private Assembly GetAssembly(string path)
        {
            string name = Path.GetFileName(path);
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (File.Exists(currentDirectory + "\\" + name))
            {
                return Assembly.LoadFile(currentDirectory + "\\" + name);
            }

            if (File.Exists(path))
            {
                return Assembly.LoadFile(path);
            }

            return null;
        }

        private CodeCompileUnit GetCodeCompileUnit(string fileName, CompilerParameters cp)
        {
            string tempCCUObjectStateFileName = null;
            string tempResourceFileName = null;
            string newTempFolder = this.TempFolder.TrimEnd('\\') + "\\" + Guid.NewGuid() + "\\";
            string className = "";

            Directory.CreateDirectory(newTempFolder);

            if (fileName.EndsWith("itsform") || fileName.EndsWith("itssform"))
            {
                ReadOptions opts = new ReadOptions();
                using (Ionic.Zip.ZipFile zip = ZipFile.Read(fileName, new ReadOptions()))
                {

                    ZipEntry entry = zip.Entries.Where(x => Path.GetExtension(x.FileName) == ".xml").FirstOrDefault();
                    entry.Extract(newTempFolder, ExtractExistingFileAction.OverwriteSilently);
                    tempCCUObjectStateFileName = newTempFolder + entry.FileName;

                    try
                    {
                        //backwards compatibility fixes

                        XmlDocument document = new XmlDocument();
                        document.Load(tempCCUObjectStateFileName);
                        var classNameNode = document["CodeNamespace"]["namespaces"]["a:anyType"]["classes"]["a:anyType"]["name"];
                        className = classNameNode.InnerText;

                        var baseClassNode = document["CodeNamespace"]["namespaces"]["a:anyType"]["classes"]["a:anyType"]["baseTypes"]["a:anyType"]["baseType"];

                        var namespaceNode = document["CodeNamespace"]["namespaces"]["a:anyType"]["name"];

                        //backwards compatibility for frmMainBase namespace change
                        baseClassNode.InnerText = baseClassNode.InnerText.Replace("ITS.POS.Client.frmMainBase", "ITS.POS.Client.Forms.frmMainBase")
                                                                         .Replace("ITS.POS.Client.frmSupportingBase", "ITS.POS.Client.Forms.frmSupportingBase");

                        namespaceNode.InnerText = namespaceNode.InnerText.Contains("ITS.POS.Client.Forms") == false
                            ? namespaceNode.InnerText.Replace("ITS.POS.Client", "ITS.POS.Client.Forms")
                            : namespaceNode.InnerText;

                        document.Save(tempCCUObjectStateFileName);
                    }
                    catch
                    {

                    }

                    entry = zip.Entries.Where(x => Path.GetExtension(x.FileName) == ".resources").FirstOrDefault();
                    entry.Extract(newTempFolder, ExtractExistingFileAction.OverwriteSilently);
                    tempResourceFileName = newTempFolder + entry.FileName.Replace("frmMainBase", className).Replace("frmSupportingBase", className)
                        .Replace("ITS.POS.Client." + className, "ITS.POS.Client.Forms." + className);  //backwards compatibility for resources
                    File.Move(newTempFolder + entry.FileName, tempResourceFileName);

                }
            }
            else
            {
                Console.WriteLine("Only .itsform and .itssform are supported");
                return null;
            }

            CodeCompileUnit ccu = null;

            IEnumerable<Type> types = typeof(CodeNamespace).Assembly.GetTypes().Where(g => g.FullName.Contains("System.CodeDom") && !g.FullName.Contains("System.CodeDom.Compiler") && !g.FullName.Contains("Collection") && !g.IsInterface);
            IEnumerable<Type> reftype = typeof(System.Reflection.TypeAttributes).Assembly.GetTypes().Where(g => g.FullName.Contains("System.Reflection.TypeAttributes") && !g.IsInterface && !g.FullName.Contains("Reflection.Cer"));
            IEnumerable<Type> knownTypesList = types.Union(reftype);

            using (FileStream sr = new FileStream(tempCCUObjectStateFileName, FileMode.Open, FileAccess.Read))
            {
                DataContractSerializer ddd = new DataContractSerializer(typeof(CodeNamespace), knownTypesList, int.MaxValue, true, true, new InvalidEnumContractSurrogate(typeof(MemberAttributes)));
                object ob = ddd.ReadObject(sr);
                ccu = (CodeCompileUnit)ob;
                AddReferencedAssemblies(cp, ccu);

                cp.IncludeDebugInformation = true;
                cp.GenerateExecutable = false;
                if (File.Exists(tempResourceFileName))
                {
                    cp.EmbeddedResources.Add(tempResourceFileName);
                }
            }

            TemporaryFiles.Add(tempCCUObjectStateFileName);
            TemporaryFiles.Add(tempResourceFileName);

            return ccu;
        }

        private void AddReferencedAssemblies(CompilerParameters cp, CodeCompileUnit ccu)
        {
            var list = ccu.ReferencedAssemblies.Cast<string>().ToList();
            ccu.ReferencedAssemblies.Clear();
            foreach (String asm in list)
            {
                Assembly asm2 = GetAssembly(asm);
                if(asm2==null)
                {
                    throw new Exception(string.Format("Assembly {0} not found", asm));
                }
                ccu.ReferencedAssemblies.Add(asm2.Location);
                foreach (AssemblyName asm3 in asm2.GetReferencedAssemblies())
                {
                    if (asm3.CodeBase != null)
                    {
                        GetAssembly(asm3.CodeBase);
                    }
                }
            }

            foreach (String asm in ccu.ReferencedAssemblies)
            {
                Assembly asm2 = GetAssembly(asm);
                foreach (AssemblyName asm3 in asm2.GetReferencedAssemblies())
                {
                    Assembly asm4 = Assembly.Load(asm3);
                    if (asm4.Location != null)
                    {
                        if (cp.ReferencedAssemblies.Contains(asm4.Location) == false)
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
        }

    }
}
