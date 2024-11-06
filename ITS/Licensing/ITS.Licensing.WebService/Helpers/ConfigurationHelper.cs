using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using ITS.Licensing.Web.AuxilliaryClasses;
using Common;

namespace ITS.Licensing.Web.Helpers
{
    public static class ConfigurationHelper
    {
        public static void ReadConfiguration(string config_file)
        {
            
            //string config_file = Server.MapPath("Configuration\\config.xml");
            if (File.Exists(config_file))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(config_file);
                XmlNodeList nodes = xmlDoc.GetElementsByTagName("sqlserver");
                foreach (XmlNode node in nodes)
                {
                    if (node.ParentNode.Name=="retaildb")
                    {
                        ConnectionSettings.RetailDB.sqlserver = node.InnerText;
                    }
                    if (node.ParentNode.Name == "licensedb")
                    {
                        ConnectionSettings.LicenseDB.sqlserver = node.InnerText;
                    }
                }

                nodes = xmlDoc.GetElementsByTagName("username");
                foreach (XmlNode node in nodes)
                {
                    if (node.ParentNode.Name == "retaildb")
                    {
                        ConnectionSettings.RetailDB.username = node.InnerText;
                    }
                    if (node.ParentNode.Name == "licensedb")
                    {
                        ConnectionSettings.LicenseDB.username = node.InnerText;
                    }
                }

                nodes = xmlDoc.GetElementsByTagName("pass");
                foreach (XmlNode node in nodes)
                {
                    if (node.ParentNode.Name == "retaildb")
                    {
                        ConnectionSettings.RetailDB.pass = node.InnerText;
                    }
                    if (node.ParentNode.Name == "licensedb")
                    {
                        ConnectionSettings.LicenseDB.pass = node.InnerText;
                    }
                }

                nodes = xmlDoc.GetElementsByTagName("database");
                foreach (XmlNode node in nodes)
                {
                    if (node.ParentNode.Name == "retaildb")
                    {
                        ConnectionSettings.RetailDB.database = node.InnerText;
                    }
                    if (node.ParentNode.Name == "licensedb")
                    {
                        ConnectionSettings.LicenseDB.database = node.InnerText;
                    }
                }

                nodes = xmlDoc.GetElementsByTagName("sqlprovider");
                foreach (XmlNode node in nodes)
                {
                    if (node.ParentNode.Name == "retaildb")
                    {
                        ConnectionSettings.RetailDB.sqlprovider = node.InnerText;
                    }
                    if (node.ParentNode.Name == "licensedb")
                    {
                        ConnectionSettings.LicenseDB.sqlprovider = node.InnerText;
                    }
                }


                XpoHelper.sqlserver = ConnectionSettings.RetailDB.sqlserver;
                XpoHelper.database = ConnectionSettings.RetailDB.database;
                XpoHelper.username = ConnectionSettings.RetailDB.username;
                XpoHelper.pass = ConnectionSettings.RetailDB.pass;
                //XpoHelper.sqlprovider = ConnectionSettings.RetailDB.sqlprovider;
/*
                using (XmlTextReader xmlReader = new XmlTextReader(config_file))
                {
                    //read through all the nodes
                    while (xmlReader.Read())
                    {
                        //the headlines we want are in the item nodes
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            switch (xmlReader.Name)
                            {
                                case "sqlserver":
                                    xmlReader.Read();
                                    XpoHelper.sqlserver = xmlReader.Value;
                                    break;
                                case "username":
                                     xmlReader.Read();
                                    XpoHelper.username= xmlReader.Value;
                                    break;
                                case "pass":
                                     xmlReader.Read();
                                    XpoHelper.pass = xmlReader.Value;
                                    break;
                                case "database":
                                     xmlReader.Read();
                                    XpoHelper.database = xmlReader.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
 */
            }
        }

    }
}