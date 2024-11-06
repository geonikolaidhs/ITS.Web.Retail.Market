using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ITS.POS.Tools.FormDesigner.Loader
{
    /// <summary>
    /// This service resolved the types and is required when using the
    /// CodeDomHostLoader
    /// </summary>
    public class TypeResolutionService : ITypeResolutionService
    {
        private Hashtable ht = new Hashtable();
        public TypeResolutionService()
        {
        }
        public Assembly GetAssembly(AssemblyName name)
        {
            return GetAssembly(name, true);
        }
        public Assembly GetAssembly(AssemblyName name, bool throwOnErrors)
        {
            return Assembly.GetAssembly(typeof(Form));
        }
        public string GetPathOfAssembly(AssemblyName name)
        {
            return null;
        }
        public Type GetType(string name)
        {
            name = name.Replace("+", ".");
            Type result =  GetType(name, true);
            if(result == null && name.Contains("ITS.POS.Client.Forms")==false && name.Contains("ITS.POS.Client")==true)
            {
                //backwards compatibility with frmMainBase namespace change
                result = GetType(name.Replace("ITS.POS.Client", "ITS.POS.Client.Forms"));
            }

            return result;
        }
        public Type GetType(string name, bool throwOnError)
        {
            return GetType(name, throwOnError, false);
        }


        protected String convertgeneric(String name)
        {
            if (!name.Contains("<"))
            {
                return name;
            }
            String genericParams = name.Substring(name.IndexOf('<')).Replace("<", string.Empty).Replace(">", string.Empty);
            String[] genericp = genericParams.Split(',');
            foreach (String s in genericp)
            {
                Type tt = GetType(s.Trim());
                if (tt == null)
                {
                    throw new Exception("Unrecognized type " + s);
                }
                name = name.Replace(s.Trim(), "[" + tt.FullName + "]");
            }

            int f = 1 + name.Where(g => g == ',').Count();
            String newname = name.Replace("<", "`" + f + "[").Replace(">", "]");
            return newname;
        }
        /// <summary>
        /// This method is called when dropping controls from the toolbox
        /// to the host that is loaded using CodeDomHostLoader. For
        /// simplicity we just go through System.Windows.Forms assembly
        /// </summary>
        public Type GetType(string namein, bool throwOnError, bool ignoreCase)
        {
            if (namein == "int")
            {
                return typeof(int);
            }
            if (namein == "long")
            {
                return typeof(long);
            }
            if (namein == "short")
            {
                return typeof(short);
            }
            if (namein == "string")
            {
                return typeof(string);
            }
            if (namein == "double")
            {
                return typeof(double);
            }
            if (namein == "float")
            {
                return typeof(float);
            }
            String name = convertgeneric(namein);
            if (ht.ContainsKey(name))
            {
                return (Type)ht[name];
            }
            Assembly winForms = Assembly.GetAssembly(typeof(Button));
            Type[] types = winForms.GetTypes();
            string typeName = String.Empty;
            foreach (Type type in types)
            {
                typeName = "system.windows.forms." + type.Name.ToLower();
                if (typeName == name.ToLower() || name.ToLower() == type.Name.ToLower())
                {
                    ht[name] = type;
                    return type;
                }
            }
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type typ = asm.GetType(name, false);
                if ( typ != null)
                {
                    ht[name] = typ;
                    return typ;
                }
            }
            return Type.GetType(name);
        }
        public void ReferenceAssembly(System.Reflection.AssemblyName name)
        {
        }
    }
}
