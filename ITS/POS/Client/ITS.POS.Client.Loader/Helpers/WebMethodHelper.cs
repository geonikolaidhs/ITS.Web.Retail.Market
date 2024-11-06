using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace POSLoader.Helpers
{
	public static class WebMethodHelper
	{

		static readonly Dictionary<string, Type> alias = new Dictionary<string, Type>()
        {
            { "byte", typeof (byte)},
			{"byte[]",typeof(byte[])},
			{"bytes",typeof(byte[])},
            {"sbyte",typeof (sbyte) },
            {"short",typeof (short) },
            {"ushort",typeof (ushort) },
            {"int",typeof (int) },
            {"uint",typeof (uint) },
            {"long",typeof (long)},
            {"ulong",typeof (ulong) },
            {"float",typeof (float) },
            {"double",typeof (double) },
            {"decimal",typeof (decimal) },
            {"object",typeof (object) },
            {"bool",typeof (bool) },
            {"char",typeof (char) },
			{"char[]",typeof (char[]) },
            {"string",typeof (string) },
			{"date",typeof (DateTime) },
			{"dateTime",typeof (DateTime) },
        };

		private static Type GetTypeByAlias(string type)
		{
			return alias.ContainsKey(type) ? alias[type] : Type.GetType(type);
		}

        public static void AssignWebMethodParameters(ref object arguments, Dictionary<string, object> args)
        {
            // Parsing Parameters
            foreach (FieldInfo field_info in arguments.GetType().GetFields())
            {
                if (field_info.FieldType.IsArray)
                {
                    object args_field_info = args[field_info.Name];
                    if (args_field_info.GetType().IsArray && args_field_info.GetType().GetElementType()==field_info.FieldType.GetElementType())
                    {
                        arguments = Activator.CreateInstance(field_info.GetType(), new object[] { args.Count });
                        for (int i = 0; i <args.Count; i++)
                        {
                            if (args_field_info.GetType() == typeof(string))
                            {
                                ((object[])arguments)[i] = "";
                            }
                            else
                            {
                                ((object[])arguments)[i] = Activator.CreateInstance(args_field_info.GetType());
                            }
                        }
                    }
                }
                else if (field_info.FieldType.IsPrimitive)
                {
                    object args1 = args[field_info.Name];
                    if (args1.GetType().IsPrimitive)
                    {
                        field_info.SetValue(arguments, args[field_info.Name]);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (field_info.FieldType.IsClass)
                {
                    object ars = field_info.GetValue(arguments);
                    object args1 = args[field_info.Name];
                    if (args1 is Dictionary<string, object>)
                    {
                        AssignWebMethodParameters(ref ars, args1 as Dictionary<string, object>);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
        }

		private static String ReplaceRowArguments(String initalText, DataRow row)
		{
			return String.Format(initalText, row.ItemArray);
		}

		/// <summary>
		/// Function to get byte array from a object
		/// </summary>
		/// <param name="_Object">object to get byte array</param>
		/// <returns>Byte Array</returns>
		private static byte[] ObjectToByteArray(object _Object)
		{
			try
			{
				// create new memory stream
				System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream();

				// create new BinaryFormatter
				System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
							= new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

				// Serializes an object, or graph of connected objects, to the given stream.
				_BinaryFormatter.Serialize(_MemoryStream, _Object);

				// convert stream to byte array and return
				return _MemoryStream.ToArray();
			}
			catch (Exception _Exception)
			{
				// Error
				Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
			}

			// Error occured, return null
			return null;
		}

		private static object ChangeType(object value, Type conversionType)
		{
			if (conversionType == typeof(byte[]))
			{
				return WebMethodHelper.ObjectToByteArray(value);
			}
			else
			{
				try
				{
					return Convert.ChangeType(value, conversionType);
				}
				catch (Exception)
				{
					throw;
				}
			}
		}	

		

		public static XmlDocument CreateWebMethodResultFile(object result, object[] outputParameters, string outputFileName)
		{
			XmlDocument resultFile = new XmlDocument();


			string xslFileName = Path.GetFileNameWithoutExtension(outputFileName);
			resultFile.AppendChild(resultFile.CreateXmlDeclaration("1.0", "UTF-8", String.Empty));
			resultFile.AppendChild(resultFile.CreateProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"" + xslFileName + ".xsl\""));

			XmlNode resultRootNode = resultFile.AppendChild(resultFile.CreateElement("WebMethodResults"));
			if (result != null)
			{
				XmlNode resultNode = resultRootNode.AppendChild(resultFile.CreateElement("Result"));
				if (result.GetType().IsArray)
				{
					foreach (object arrayObj in (object[])result)
					{
						resultNode.AppendChild(WebMethodHelper.CreateResultPropertyNode(arrayObj, result.GetType().Name.Replace("[]",""),resultFile));
					}
				}
				else if (result.GetType().IsPrimitive || result.GetType() == typeof(string))
				{
					resultNode.InnerText = result.ToString();
					if (String.IsNullOrEmpty(resultNode.InnerText))
					{
						resultNode.InnerText = " ";
					}
				}
				else
				{
					foreach (PropertyInfo pi in result.GetType().GetProperties())
					{
                        object val = pi.GetValue(result, null);
                        if (val != null)
                        {
                            resultNode.AppendChild(WebMethodHelper.CreateResultPropertyNode(val, pi.Name, resultFile));
                        }
                        else
                        {
                            XmlNode nd = resultFile.CreateElement(pi.Name);
                            resultNode.AppendChild(nd);
                        }
					}
                    foreach (FieldInfo pi in result.GetType().GetFields())
                    {
                        object val = pi.GetValue(result);
                        if (val != null)
                        {
                            resultNode.AppendChild(WebMethodHelper.CreateResultPropertyNode(val, pi.Name, resultFile));
                        }
                        else
                        {
                            XmlNode nd = resultFile.CreateElement(pi.Name);
                            resultNode.AppendChild(nd);
                        }
                    }
				}
			}
			if (outputParameters != null)
			{
				foreach (object parameter in outputParameters)
				{
					resultRootNode.AppendChild(WebMethodHelper.CreateOutputParameterNode(parameter, resultFile));
				}
			}
			return resultFile;
		}

		private static XmlNode CreateOutputParameterNode(object outputParam, XmlDocument file)
		{
			string paramName = outputParam.GetType().GetProperty("Name").GetValue(outputParam, null).ToString();
			object paramValue = outputParam.GetType().GetProperty("Value").GetValue(outputParam, null);
			XmlNode rootNode = file.CreateElement(paramName);
			if(paramValue == null)
			{
				rootNode.InnerText = " ";
			}
			else if (paramValue.GetType().IsArray)
			{
				foreach (object obj in (object[])outputParam)
				{
					rootNode.AppendChild(WebMethodHelper.CreateResultPropertyNode(obj,paramValue.GetType().Name.Replace("[]",""), file));
				}
			}
            else if (paramValue.GetType().IsPrimitive || paramValue.GetType() == typeof(string) || paramValue.GetType() == typeof(decimal))
			{
				rootNode.InnerText = paramValue.ToString();
				if (String.IsNullOrEmpty(rootNode.InnerText))
				{
					rootNode.InnerText = " ";
				}
			}
			else
			{
				foreach (FieldInfo property in paramValue.GetType().GetFields())
				{
					XmlNode properyNode = rootNode.AppendChild(file.CreateElement(property.Name));
					properyNode.InnerText = property.GetValue(paramValue) == null ? "" : property.GetValue(paramValue).ToString();
					if(String.IsNullOrEmpty(properyNode.InnerText))
					{
						properyNode.InnerText = " ";
					}
				}
			}
			return rootNode;
		}

		private static XmlNode CreateResultPropertyNode(object propertyValue,string propertyName, XmlDocument file)
		{

			Type propertyType = propertyValue.GetType();
            string nodeName = propertyName;
			XmlNode rootNode = file.CreateElement(nodeName);

			if (propertyType.IsArray)
			{
				foreach (object obj in (object[])propertyValue)
				{
                    if (obj != null)
                    {
                        rootNode.AppendChild(WebMethodHelper.CreateResultPropertyNode(obj, propertyType.Name.Replace("[]", ""), file));
                    }
                    else
                    {
                        rootNode.AppendChild(file.CreateElement("Null"));
                    }
				}
			}
			else if (propertyType.IsPrimitive || propertyType == typeof(string) || propertyType == typeof(decimal))
			{
				rootNode.InnerText = propertyValue.ToString();
				if (String.IsNullOrEmpty(rootNode.InnerText))
				{
					rootNode.InnerText = " ";
				}
			}
			else
			{
				foreach (PropertyInfo property in propertyType.GetProperties())
				{
                    object propertyGetValue = property.GetValue(propertyValue, null);
                    if (propertyGetValue == null)
                    {
                        XmlNode nd = file.CreateElement(property.Name);
                        rootNode.AppendChild(nd);
                    }
                    else
                    {
                        rootNode.AppendChild(CreateResultPropertyNode(propertyGetValue,property.Name, file));
                    }

				}

                foreach (FieldInfo property in propertyType.GetFields())
                {
                    object propertyGetValue = property.GetValue(propertyValue);
                    if (propertyGetValue == null)
                    {
                        XmlNode nd = file.CreateElement(property.Name);
                        rootNode.AppendChild(nd);
                    }
                    else
                    {
                        rootNode.AppendChild(CreateResultPropertyNode(propertyGetValue, property.Name, file));
                    }
                }
			}
			return rootNode;
		}
	}
}
