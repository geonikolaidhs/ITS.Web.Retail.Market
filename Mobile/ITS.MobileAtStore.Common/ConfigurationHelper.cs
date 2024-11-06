using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using ITS.MobileAtStore.ObjectModel.Attributes;

namespace ITS.MobileAtStore.Common
{
    public interface IXmlSubitems { }

    public static class ConfigurationHelper
    {
        public static bool LoadSettingsStatic(Type staticType, string filePath)
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
                    IEnumerable<PropertyInfo> properties = staticType.GetProperties().Where(prop => prop.CanWrite);
                    List<string> propertiesNames = properties.Select(pr => pr.Name).ToList();
                        //Where(prop => prop.CanWrite && prop.GetCustomAttributes(typeof(IgnoreFieldFromView), true).Count() <= 0);
                    foreach (PropertyInfo propInfo in properties)
                    {
                        if (el[propInfo.Name] != null)
                        {
                            object value = el[propInfo.Name].InnerText;
                            if (typeof(IXmlSubitems).IsAssignableFrom(propInfo.PropertyType))
                            {
                                object objectValue = propInfo.GetValue(null, null);
                                if (objectValue == null)
                                {
                                    objectValue = Activator.CreateInstance(propInfo.PropertyType);
                                }
                                AssignN(propInfo.PropertyType, objectValue, el[propInfo.Name]);
                                value = objectValue;
                            }
                            else if (typeof(IDictionary).IsAssignableFrom(propInfo.PropertyType))
                            {
                                Type[] types = propInfo.PropertyType.GetGenericArguments();
                                Type keyType = types[0];
                                Type valueType = types[1];
                                object list = propInfo.GetValue(null, null);
                                if (list == null)
                                {
                                    list = Activator.CreateInstance(propInfo.PropertyType);
                                }
                                IDictionary ilist = (IDictionary)list;
                                ilist.Clear();
                                foreach (XmlElement child in el[propInfo.Name].ChildNodes)
                                {
                                    object key;// = Activator.CreateInstance(keyType);
                                    object keyvalue = Activator.CreateInstance(valueType);
                                    AssignN(valueType, keyvalue, child);

                                    string keyString = child.GetAttribute("Key");
                                    key = keyString;
                                    if (keyType.IsEnum)
                                    {
                                        key = Enum.Parse(keyType, keyString);
                                    }
                                    else if (keyType != typeof(String))
                                    {
                                        IEnumerable<MemberInfo> inf = propInfo.PropertyType.GetMember("Parse")
                                            .Where(g => (g as MethodInfo).GetParameters().Count() == 1 && 
                                                (g as MethodInfo).GetParameters()[0].ParameterType == typeof(String));
                                        if (inf.Count() == 1)
                                        {
                                            MethodInfo mi = inf.First() as MethodInfo;
                                            var v = mi.Invoke(null, new object[] { keyString });
                                            key = v;
                                        }
                                    }
                                    ilist.Add(key, keyvalue);
                                }
                                value = ilist;
                            }
                            else if (typeof(IList).IsAssignableFrom(propInfo.PropertyType))
                            {
                                object list = propInfo.GetValue(null, null);
                                if (list == null)
                                {
                                    list = Activator.CreateInstance(propInfo.PropertyType);
                                }
                                IList ilist = (IList)list;
                                ilist.Clear();
                                Type valueType = propInfo.PropertyType.GetGenericArguments()[0];
                                foreach (XmlElement child in el[propInfo.Name].ChildNodes)
                                {
                                    object childObj = Activator.CreateInstance(valueType);
                                    AssignN(valueType, childObj, child);
                                    ilist.Add(childObj);
                                }
                                value = ilist;
                            }
                            else if (propInfo.PropertyType != typeof(String) && propInfo.PropertyType.IsEnum == false)
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
                            try
                            {
                                propInfo.SetValue(null, value, null);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                    return true;
                }
                throw new Exception("Error in settings File");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static T LoadSettings<T>(string filePath)
        {
            return LoadSettings<T>(filePath, default(T));
        }
        public static T LoadSettings<T>(string filePath, T existingObject)
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
                    Assign<T>(obj, el);
                    return obj;
                }
                throw new Exception("Error in settings File");
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private static void AssignN(Type type, Object obj, XmlElement el)
        {
            MethodInfo method = typeof(ConfigurationHelper).GetMethod("Assign", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo generic = method.MakeGenericMethod(type);
            generic.Invoke(null, new object[] { obj, el });
            //.Invoke(null,obj, el);
        }

        private static void Assign<T>(T obj, XmlElement el)
        {
            foreach (PropertyInfo propInfo in typeof(T).GetProperties().Where(property=>property.CanWrite))
            {
                if (el[propInfo.Name] != null)
                {
                    object value = el[propInfo.Name].InnerText;
                    if (typeof(IXmlSubitems).IsAssignableFrom(propInfo.PropertyType))
                    {
                        object objectValue = propInfo.GetValue(obj, null);
                        if (objectValue == null)
                        {
                            objectValue = Activator.CreateInstance(propInfo.PropertyType);
                        }
                        AssignN(propInfo.PropertyType, objectValue, el[propInfo.Name]);
                        value = objectValue;
                    }
                    else if (typeof(IDictionary).IsAssignableFrom(propInfo.PropertyType))
                    {
                        Type [] types = propInfo.PropertyType.GetGenericArguments();
                        Type keyType = types[0];
                        Type valueType = types[1];                        
                    }
                    else if (typeof(IList).IsAssignableFrom(propInfo.PropertyType))
                    {
                        object list = propInfo.GetValue(obj, null);
                        if (list == null)
                        {
                            list = Activator.CreateInstance(propInfo.PropertyType);
                        }
                        IList ilist = (IList)list;
                        ilist.Clear();
                        Type valueType  = propInfo.PropertyType.GetGenericArguments()[0];
                        foreach (XmlElement child in el[propInfo.Name].ChildNodes)
                        {
                            object childObj = Activator.CreateInstance(valueType);
                            AssignN(valueType, childObj,child);
                            ilist.Add(childObj);
                        }
                        propInfo.SetValue(obj, ilist, null);
                        value = ilist;
                    }
                    else if (propInfo.PropertyType.IsEnum)
                    {
                        value = Enum.Parse(propInfo.PropertyType, el[propInfo.Name].InnerText);
                    }
                    else if(propInfo.PropertyType.Equals(typeof(char)))
                    {
                        string xmlValueString = el[propInfo.Name].InnerText;
                        if (String.IsNullOrEmpty(xmlValueString))
                        {
                            value = ' ';
                        }
                        else
                        {
                            value = xmlValueString[0];
                        }                        
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

                    try
                    {
                        propInfo.SetValue(obj, value, null);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }

        public static void SaveSettingsFileStatic(Type type, string filepath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement(type.Name);
            CreateSubElementStatic(type, xmlDoc, root);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
        }

        private static void CreateSubElement(object obj, XmlDocument xmlDoc, XmlElement root)
        {
            if (obj == null)
                return;
            foreach (PropertyInfo propInfo in obj.GetType().GetProperties())
            {
                BrowsableAttribute[] propsBrowsable = propInfo.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                if (propsBrowsable.Length == 1 && propsBrowsable[0].Browsable == false)
                {
                    continue;
                }
                XmlElement node = xmlDoc.CreateElement(propInfo.Name);
                if (typeof(IXmlSubitems).IsAssignableFrom(propInfo.PropertyType))
                {
                    CreateSubElement(propInfo.GetValue(obj, null), xmlDoc, node);
                }
                else if (typeof(IDictionary).IsAssignableFrom(propInfo.PropertyType))
                {
                    Type[] types = propInfo.PropertyType.GetGenericArguments();
                    Type keyType = types[0];
                    Type valueType = types[1];
                    IDictionary ilist = (IDictionary)propInfo.GetValue(obj, null);
                    foreach (var item in ilist)
                    {
                        if (item is DictionaryEntry)
                        {
                            DictionaryEntry entry = (DictionaryEntry)item;
                            XmlElement internalNode = xmlDoc.CreateElement(valueType.Name);
                            XmlAttribute attribute = xmlDoc.CreateAttribute("Key");
                            attribute.Value = entry.Key.ToString();
                            internalNode.Attributes.Append(attribute);
                            CreateSubElement(entry.Value, xmlDoc, internalNode);
                            node.AppendChild(internalNode);
                        }
                    }
                }
                else if (typeof(IList).IsAssignableFrom(propInfo.PropertyType))
                {
                    IList ilist = (IList)propInfo.GetValue(obj, null);
                    foreach (var item in ilist)
                    {
                        XmlElement internalNode = xmlDoc.CreateElement(item.GetType().Name);
                        CreateSubElement(item, xmlDoc, internalNode);
                        node.AppendChild(internalNode);
                    }
                }
                else
                {
                    try
                    {
                        object propertyValue = propInfo.GetValue(obj, null);
                        if (propertyValue == null)
                        {
                            node.InnerText = "";
                        }
                        else
                        {
                            node.InnerText = propertyValue.ToString();
                        }
                    }
                    catch
                    { 
                    }

                  
                }
                root.AppendChild(node);
            }
        }

        private static void CreateSubElementStatic(Type type, XmlDocument xmlDoc, XmlElement root)
        {
            foreach (PropertyInfo propInfo in type.GetProperties())
            {
                BrowsableAttribute[] propsBrowsable = propInfo.GetCustomAttributes(typeof(BrowsableAttribute), true) as BrowsableAttribute[];
                if (propsBrowsable.Length == 1 && propsBrowsable[0].Browsable == false)
                {
                    continue;
                }
                XmlElement node = xmlDoc.CreateElement(propInfo.Name);
                if (typeof(IXmlSubitems).IsAssignableFrom(propInfo.PropertyType))
                {
                    CreateSubElement(propInfo.GetValue(null, null), xmlDoc, node);
                }
                else if (typeof(IDictionary).IsAssignableFrom(propInfo.PropertyType))
                {
                    Type[] types = propInfo.PropertyType.GetGenericArguments();
                    Type keyType = types[0];
                    Type valueType = types[1];
                    IDictionary ilist = (IDictionary)propInfo.GetValue(null, null);
                    foreach (var item in ilist)
                    {
                        if (item is DictionaryEntry)
                        {
                            DictionaryEntry entry = (DictionaryEntry)item;
                            XmlElement internalNode = xmlDoc.CreateElement(valueType.Name);
                            XmlAttribute attribute = xmlDoc.CreateAttribute("Key");
                            attribute.Value = entry.Key.ToString();
                            internalNode.Attributes.Append(attribute);
                            CreateSubElement(entry.Value, xmlDoc, internalNode);
                            node.AppendChild(internalNode);
                        }
                    }
                }
                else if (typeof(IList).IsAssignableFrom(propInfo.PropertyType))
                {
                    IList ilist = (IList)propInfo.GetValue(null, null);
                    foreach (var item in ilist)
                    {
                        XmlElement internalNode = xmlDoc.CreateElement(item.GetType().Name);
                        CreateSubElement(item, xmlDoc, internalNode);
                        node.AppendChild(internalNode);
                    }
                }
                else
                {
                    node.InnerText = propInfo.GetValue(null, null).ToString();
                }
                root.AppendChild(node);
            }
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
                node.InnerText = propInfo.GetValue(settings, null).ToString();
                root.AppendChild(node);
            }
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
        }

    }
}
