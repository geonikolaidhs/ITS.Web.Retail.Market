using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Helper class that handles the loading and resolving of extra modules, like custom forms and specific device dll implementations
    /// </summary>
    public static class DynamicModules
    {
        private static List<Assembly> loadedAssemblies;
        private static Dictionary<String, Assembly> mapping;

        public static String AssembliesPath { get; set; }

        /// <summary>
        /// Loads the module assemblies in the current AppDomain
        /// </summary>
        public static void LoadAssemblies()
        {
            if (loadedAssemblies == null)
            {
                loadedAssemblies = new List<Assembly>();
            }
            if (mapping == null)
            {
                mapping = new Dictionary<String, Assembly>();
            }
            //Scan Module Folder
            if (DynamicModules.AssembliesPath == null)
            {
                DynamicModules.AssembliesPath = Assembly.GetEntryAssembly().Location + "\\Modules";
            }
            if (!Directory.Exists(AssembliesPath))
                return;
            foreach (String file in Directory.GetFiles(AssembliesPath,"*.dll"))
            {
                try
                {
                    Assembly asm = Assembly.LoadFile(file);
                    loadedAssemblies.Add(asm);
                }
                catch (Exception )
                {
                    //int r = 0;
                }

            }
        }

        /// <summary>
        /// Gets all the loaded modules' types
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetTypes()
        {
            List<Type> types = new List<Type>();
            foreach (Assembly am in loadedAssemblies)
            {
                types.AddRange(am.GetTypes());
            }
            return types;
        }

        /// <summary>
        /// Gets a module's type by name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetType(String type)
        {
            if (type.IndexOf("ITS") < 0)
            {
                type = "ITS.POS.Hardware." + type;
            }
            Assembly asm = WhichAssembly(type);
            return (asm ==null)?null:asm.GetType(type, false);
        }


        private static Assembly WhichAssembly(String type)
        {

            if (mapping.ContainsKey(type))
                return mapping[type];
            foreach (Assembly am in loadedAssemblies)
            {
                try
                {
                    Type t = am.GetType(type, false);
                    if (t != null)
                    {
                        mapping[type] = am;
                        return am;
                    }
                }
                catch (Exception )
                {
                }
            }
            return null;
        }

    }
}
