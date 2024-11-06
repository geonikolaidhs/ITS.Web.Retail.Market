using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ITS.Retail.Platform.Tests.Common
{
    public static class TestSettings
    {
        public const string ConfigurationXMLFile = "settings.xml";
        public static string SqlServer { get; private set; }
        public static string Username { get; private set; }
        public static string Password { get; private set; }
        public static string MasterDatabaseInstallBat { get; private set; }
        public static string StoreControllerDatabaseInstallBat { get; private set; }
        public static string DualDatabaseInstallBat { get; private set; }
        public static string MasterDatabase { get; private set; }
        public static string StoreControllerDatabase { get; private set; }
        public static string DualDatabase { get; private set; }

        static TestSettings()
        {
            if (File.Exists(ConfigurationXMLFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigurationXMLFile);

                XmlNode settingsNode = doc.SelectSingleNode("settings");
                XmlNode xmlNode = settingsNode.SelectSingleNode("sqlserver");
                if (xmlNode != null)
                {
                    SqlServer = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("username");
                if (xmlNode != null)
                {
                    Username = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("pass");
                if (xmlNode != null)
                {
                    Password = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("MasterDatabaseInstallBat");
                if (xmlNode != null)
                {
                    MasterDatabaseInstallBat = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("StoreControllerDatabaseInstallBat");
                if (xmlNode != null)
                {
                    StoreControllerDatabaseInstallBat = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("DualDatabaseInstallBat");
                if (xmlNode != null)
                {
                    DualDatabaseInstallBat = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("MasterDatabase");
                if (xmlNode != null)
                {
                    MasterDatabase = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("StoreControllerDatabase");
                if (xmlNode != null)
                {
                    StoreControllerDatabase = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("DualDatabase");
                if (xmlNode != null)
                {
                    DualDatabase = xmlNode.InnerText.Trim();
                }
            }
        }
    }
}
