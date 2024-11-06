using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ITS.Retail.PriceChecker.Service
{
    public sealed class Settings
    {
        private static object objectlockCheck = new Object();

        private static volatile Settings instance;

        private static string applicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");
        private static string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        private String _Ip { get; set; }

        private Int32 _Port { get; set; }

        private String _StoreControllerURL { get; set; }

        private Encoding _FromEncoding { get; set; } = Encoding.Unicode;

        private Encoding _ToEncoding  { get; set; } = Encoding.UTF8;

        public Encoding FromEncoding
        {
            get
            {
                return _FromEncoding;
            }
            set
            {
                _FromEncoding = value;
            }
        }


        public Encoding ToEncoding
        {
            get
            {
                return _ToEncoding;
            }
            set
            {
                _ToEncoding = value;
            }
        }
        

        public String Ip
        {
            get
            {
                return _Ip;
            }
            set
            {
                _Ip = value;
            }
        }

        public Int32 Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
            }
        }


        public String StoreControllerURL
        {
            get
            {
                return _StoreControllerURL;
            }
            set
            {
                _StoreControllerURL = value;
            }
        }

        private Settings()
        {
            ReadSettings();
        }

        public static Settings getInstance()
        {

            if (instance == null)
            {
                lock (objectlockCheck)
                {
                    if (instance == null)
                    {
                        instance = new Settings();
                    }

                }
            }
            return instance;

        }

        private void ReadSettings()
        {
            string SettingsXmlPath = Path.GetDirectoryName(applicationPath) + "\\PriceCheckerService\\Settings.xml";
            Program.WriteToWindowsEventLog("search for Settings.xml at " + SettingsXmlPath, EventLogEntryType.Information);
            if (File.Exists(SettingsXmlPath))
            {
                XmlDocument xmlDocument = new XmlDocument();
                try
                {                   
                    xmlDocument.Load(SettingsXmlPath);
                    string ip = xmlDocument.GetElementsByTagName("Ip")[0].InnerXml.ToString();
                    string portString = xmlDocument.GetElementsByTagName("Port")[0].InnerXml.ToString();
                    string url = xmlDocument.GetElementsByTagName("StoreControllerURL")[0].InnerXml.ToString();               
                    int port;

                    if (!string.IsNullOrEmpty(ip))
                    {
                        _Ip = ip;
                    }

                    if (!string.IsNullOrEmpty(portString))
                    {
                        Int32.TryParse(portString, out port);
                        _Port = port;
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        _StoreControllerURL = url;
                    }

                    if (string.IsNullOrEmpty(_Ip) || string.IsNullOrEmpty(_StoreControllerURL) || string.IsNullOrEmpty(portString))
                    {
                        Program.WriteToWindowsEventLog("Null Ip,Url,Port", EventLogEntryType.Error);
                    }
                    else
                    {
                        Program.WriteToWindowsEventLog("Settings Loaded Succesfully", EventLogEntryType.Information);
                    }
                }
                catch (Exception ex)
                {
                    Program.WriteToWindowsEventLog("Error Loading Settings" + ex.Message, EventLogEntryType.Error);
                }

                try
                {
                    string fromEncoding = xmlDocument.GetElementsByTagName("EncodingFrom")[0].InnerXml.ToString();
                    string toEncoding = xmlDocument.GetElementsByTagName("EncodingTo")[0].InnerXml.ToString();

                    if (string.IsNullOrEmpty(fromEncoding))
                    {
                        Program.WriteToWindowsEventLog("Null fromEncoding so setting encoding to UNICODE", EventLogEntryType.Error);
                    }
                    else
                    {
                        _FromEncoding = Encoding.GetEncoding(fromEncoding);
                        Program.WriteToWindowsEventLog("fromEncoding was set to " + fromEncoding, EventLogEntryType.Information);
                    }

                    if (string.IsNullOrEmpty(toEncoding))
                    {
                        Program.WriteToWindowsEventLog("Null toEncoding so setting encoding to UTF-8", EventLogEntryType.Error);
                    }
                    else
                    {
                        _FromEncoding = Encoding.GetEncoding(fromEncoding);
                        Program.WriteToWindowsEventLog("toEncoding was set to " + toEncoding, EventLogEntryType.Information);
                    }


                }
                catch(Exception ex)
                {

                }
            }
            else
            {
                Program.WriteToWindowsEventLog("Settings File Not Found", EventLogEntryType.Error);
            }

        }
    }
}
