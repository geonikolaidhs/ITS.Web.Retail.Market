using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;

namespace ITS.Common.Utilities.Forms
{
    public static class ConfigurationHelper
    {
        public static bool LoadSettingsStatic(Type staticType, string filePath, bool askAndSaveIfError)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);
                XmlNodeList nodelist = xmlDoc.GetElementsByTagName(staticType.Name);
                if (nodelist.Count == 1)
                {
                    //T obj = Activator.CreateInstance<T>();
                    XmlElement el = nodelist[0] as XmlElement;
                    foreach (PropertyInfo propInfo in staticType.GetProperties())
                    {
                        if (el[propInfo.Name] != null)
                        {
                            object value = el[propInfo.Name].InnerText;
                            if (propInfo.PropertyType != typeof(String) && propInfo.PropertyType.IsEnum == false)
                            {
                                IEnumerable<MemberInfo> inf = propInfo.PropertyType.GetMember("Parse").Where(g => (g as MethodInfo).GetParameters().Count() == 1 && (g as MethodInfo).GetParameters()[0].ParameterType == typeof(String));
                                if (inf.Count() == 1)
                                {
                                    MethodInfo mi = inf.First() as MethodInfo;
                                    var v = mi.Invoke(null, new object[] { value });
                                    value = v;
                                }
                            }
                            else if (propInfo.PropertyType.IsEnum)
                            {
                                value = Enum.Parse(propInfo.PropertyType, el[propInfo.Name].InnerText);
                            }
                            propInfo.SetValue(null, value, null);
                        }
                    }
                    return true;
                }
                throw new Exception("Error in settings File");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                if (askAndSaveIfError)
                {
                    return AskUserAndSaveFileStatic(staticType, filePath);
                }
                throw ex;
            }
            catch (System.IO.FileLoadException ex)
            {
                if (askAndSaveIfError)
                {
                    return AskUserAndSaveFileStatic(staticType, filePath);
                }
                throw ex;
            }

        }

        public static T LoadSettings<T>(string filePath, bool askAndSaveIfError, T existingObject = default(T))
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);
                XmlNodeList nodelist = xmlDoc.GetElementsByTagName(typeof(T).Name);
                if (nodelist.Count == 1)
                {

                    T obj;
                    if (existingObject == null)
                        obj = Activator.CreateInstance<T>();
                    else
                        obj = existingObject;

                    XmlElement el = nodelist[0] as XmlElement;
                    foreach (PropertyInfo propInfo in typeof(T).GetProperties())
                    {
                        if (el[propInfo.Name] != null)
                        {
                            object value = el[propInfo.Name].InnerText;
                            if (propInfo.PropertyType.IsEnum)
                            {
                                value = Enum.Parse(propInfo.PropertyType, el[propInfo.Name].InnerText);
                            }
                            else if (propInfo.PropertyType != typeof(String))
                            {
                                IEnumerable<MemberInfo> inf = propInfo.PropertyType.GetMember("Parse").Where(g => (g as MethodInfo).GetParameters().Count() == 1 && (g as MethodInfo).GetParameters()[0].ParameterType == typeof(String));
                                if (inf.Count() == 1)
                                {
                                    MethodInfo mi = inf.First() as MethodInfo;
                                    var v = mi.Invoke(null, new object[] { value });
                                    value = v;
                                }
                            }

                            propInfo.SetValue(obj, value, null);
                        }
                    }
                    return obj;
                }
                throw new Exception("Error in settings File");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                if (askAndSaveIfError)
                {
                    return AskUserAndSaveNewFile<T>(filePath);
                }
                throw ex;
            }
            catch (System.IO.FileLoadException ex)
            {
                if (askAndSaveIfError)
                {
                    return AskUserAndSaveNewFile<T>(filePath);
                }
                throw ex;
            }

        }

        public static void SaveSettingsFileStatic(Type type, string filepath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement(type.Name);
            foreach (PropertyInfo propInfo in type.GetProperties())
            {
                BrowsableAttribute[] propsBrowsable = propInfo.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                if (propsBrowsable.Length == 1 && propsBrowsable[0].Browsable == false)
                {
                    continue;
                }
                XmlElement node = xmlDoc.CreateElement(propInfo.Name);
                object value = propInfo.GetValue(null, null);
                if (value != null)
                {
                    node.InnerText = value.ToString();

                    root.AppendChild(node);
                }
            }
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
        }

        public static void SaveSettingsFile<T>(T settings, string filepath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDeclaration);
            XmlElement root = xmlDoc.CreateElement(typeof(T).Name);
            foreach (PropertyInfo propInfo in typeof(T).GetProperties())
            {
                BrowsableAttribute[] propsBrowsable = propInfo.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                if (propsBrowsable.Length == 1 && propsBrowsable[0].Browsable == false)
                {
                    continue;
                }
                XmlElement node = xmlDoc.CreateElement(propInfo.Name);
                node.InnerText = propInfo.GetValue(settings, null) == null ? "" : propInfo.GetValue(settings, null).ToString();
                root.AppendChild(node);
            }
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
        }


        public static T AskUserAndSaveNewFile<T>(string filePath)
        {

            using (ITSInputForm frm = new ITSInputForm(typeof(T), Activator.CreateInstance(typeof(T))))
            {
                frm.ShowDialog();
                T settingsObject = (T)frm.ReturnValue;
                SaveSettingsFile(settingsObject, filePath);
                return settingsObject;
            }

        }

        public static T AskUserAndSaveFile<T>(string filePath, T existing)
        {

            using (ITSInputForm frm = new ITSInputForm(typeof(T), existing))
            {
                frm.ShowDialog();
                T settingsObject = (T)frm.ReturnValue;
                SaveSettingsFile(settingsObject, filePath);
                return settingsObject;
            }

        }
        public static bool AskUserAndSaveFileStatic(Type type, string filePath)
        {
            using (ITSInputForm frm = new ITSInputForm(type, null))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    SaveSettingsFileStatic(type, filePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
    }
}

namespace ITS.Common.Attributes
{
    public class LookupAttribute : Attribute
    {
        public Type type { get; set; }
        public String Method { get; set; }
    }
}
