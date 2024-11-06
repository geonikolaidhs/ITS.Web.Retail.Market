using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom;
using System.Collections;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Xml.Schema;

namespace POSLoader.WebServiceAutoGeneration
{
    public static class CustomProxyGenerator
    {
        private const string RootNamespace = "WS";
        private const string ArgTypeSuffix = "Arg";

        //private static void AddTrace(CodeTypeDeclaration typedecl, Hashtable enumTypes)
        //{
        //    foreach (CodeTypeMember mem in typedecl.Members)
        //    {
        //        if (mem is CodeMemberMethod && !(mem is CodeConstructor) && !IsAsyncMethod(mem.Name))
        //        {

        //            mem.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(TraceExtension).FullName));
        //        }
        //    }
        //}

        public static Assembly CreateAssembly(string url)
        {
            CodeCompileUnit ccu = ImportServiceDescription(url);

            //Hashtable enumTypes = new Hashtable();
            //
            //foreach (CodeTypeDeclaration typedecl in ccu.Namespaces[0].Types)
           // {
            //    if (typedecl.IsEnum)
            //        enumTypes[typedecl.Name] = typedecl;
            //}

            ArrayList webServices = new ArrayList();

            foreach (CodeTypeDeclaration typedecl in ccu.Namespaces[0].Types)
            {
                //AddTrace(typedecl, enumTypes);

                foreach (CodeTypeReference @ref in typedecl.BaseTypes)
                {
                    if (@ref.BaseType == "System.Web.Services.Protocols.SoapHttpClientProtocol" || @ref.BaseType == "Microsoft.Web.Services3.WebServicesClientProtocol")
                    {
                        webServices.Add(typedecl);
                        break;
                    }
                }
            }

            foreach (CodeTypeDeclaration svcdecl in webServices)
            {
                AddWebServiceMethodArgTypes(ccu.Namespaces[0], svcdecl);
            }

            return GenerateAssembly(ccu);
        }

        public static bool IsAsyncMethod(string methodName)
        {
            return methodName.StartsWith("Begin") || methodName.StartsWith("End");
        }

        public static string GetMethodArgType(string methodName)
        {
            return RootNamespace + "." + methodName + ArgTypeSuffix;
        }

        private static void AddWebServiceMethodArgTypes(CodeNamespace @namespace, CodeTypeDeclaration svcdecl)
        {
            foreach (CodeTypeMember mem in svcdecl.Members)
            {
                

                CodeMemberMethod method = mem as CodeMemberMethod;
                if (method ==null)
                    continue;

                if (method is CodeConstructor
                    || method is CodeTypeConstructor
                    || IsAsyncMethod(method.Name))
                {
                    continue;
                }

                CodeTypeDeclaration argtype = new CodeTypeDeclaration(method.Name + ArgTypeSuffix);

                CodeConstructor constructor = new CodeConstructor() { Attributes = MemberAttributes.Public };

                argtype.Members.Add(constructor);

                foreach (CodeParameterDeclarationExpression @param in method.Parameters)
                {
                    CodeMemberField newCodeMemberField = new CodeMemberField(@param.Type, @param.Name);
                    newCodeMemberField.Attributes =  MemberAttributes.Public;
                    argtype.Members.Add(newCodeMemberField);
                }

                @namespace.Types.Add(argtype);
            }
        }

        private static Assembly GenerateAssembly(CodeCompileUnit ccu)
        {
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {

                ICodeCompiler compiler = provider.CreateCompiler();

                CompilerParameters options = new CompilerParameters(
                    new string[] {
								 "System.dll",
								 "System.Drawing.dll",
								 "System.Design.dll",
								 "System.Data.dll",
								 "System.Web.Services.dll",
								 "System.Xml.dll",  
								// typeof(WebService).Assembly.Location,
                                 //typeof(WebServicesClientProtocol).Assembly.Location,
							 }, string.Empty, false);

                options.GenerateInMemory = false; // dynamic assemblies don't support xml serialization
                //using (System.IO.StreamWriter st = new System.IO.StreamWriter("c:\\test\\out.cs"))
                //{
                //    CodeGeneratorOptions op = new CodeGeneratorOptions();
                //    provider.GenerateCodeFromCompileUnit(ccu, st, op);
                //}
                CompilerResults results = compiler.CompileAssemblyFromDom(options, ccu);


                StringBuilder errors = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    if (!error.IsWarning)
                    {
                        errors.Append(error.ErrorText);
                        errors.Append(Environment.NewLine);
                    }
                }

                if (errors.Length > 0)
                    throw new Exception(errors.ToString());

                return Assembly.LoadFrom(results.PathToAssembly);
            }
        }
        

        private static CodeCompileUnit ImportServiceDescription(string url)
        {
            ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
            
            importer.ProtocolName = "Soap";
            importer.Style = ServiceDescriptionImportStyle.Client;


            using (DiscoveryClientProtocol dcc = new DiscoveryClientProtocol())
            {
                foreach (String ur in url.Split(';'))
                {
                    dcc.DiscoverAny(ur);
                }
                dcc.ResolveAll();



                foreach (object doc in dcc.Documents.Values)
                {
                    if (doc is ServiceDescription)
                        //importer.AddServiceDescription(doc as ServiceDescription);
                        importer.AddServiceDescription(doc as ServiceDescription, String.Empty, String.Empty);

                    else if (doc is XmlSchema)
                        importer.Schemas.Add(doc as XmlSchema);
                }

                if (importer.ServiceDescriptions.Count == 0)
                {
                    throw new Exception("No WSDL document was found at the url " + url);
                }

                CodeCompileUnit ccu = new CodeCompileUnit();

                ccu.Namespaces.Add(new CodeNamespace(RootNamespace));

                ServiceDescriptionImportWarnings warnings = importer.Import(ccu.Namespaces[0], ccu);

                if ((warnings & ServiceDescriptionImportWarnings.NoCodeGenerated) > 0)
                {
                    throw new Exception("No code generated");
                }

                //foreach (CodeTypeDeclaration type in ccu.Namespaces[0].Types)
                //{
                //    CodeTypeReference toRemove = null;
                //    foreach (CodeTypeReference baseType in type.BaseTypes)
                //    {
                //        if (baseType.BaseType == "System.Web.Services.Protocols.SoapHttpClientProtocol")
                //        {
                //            toRemove = baseType;
                //            break;
                //        }
                //    }
                //    if (toRemove != null)
                //    {
                //        type.BaseTypes.Remove(toRemove);
                //        type.BaseTypes.Add(new CodeTypeReference("Microsoft.Web.Services3.WebServicesClientProtocol"));
                //    }
                //}

                return ccu;
            }
        }
    }
}
