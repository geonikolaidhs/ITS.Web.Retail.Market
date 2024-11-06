using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ITS.Service.Guard
{
    public sealed class Settings
    {
        private static object objectlockCheck = new Object();

        private static volatile Settings instance;

        private static string applicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");
        private static string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        private String _ServiceName { get; set; } = "DiSign Service";

        private Int32 _Monitor { get; set; } = 0;

        private Int32 _Interval { get; set; } = 60;

        private Int32 _ServiceWaitTime { get; set; } = 50;

        private Int32 _CreateStartUpShortCut { get; set; } = 0;


        public TimeSpan ServiceWaitTime
        {
            get
            {
                return TimeSpan.FromSeconds(_ServiceWaitTime);
            }
            set
            {
                _ServiceWaitTime = (int)value.TotalSeconds;
            }
        }



        public String ServiceName
        {
            get
            {
                return _ServiceName;
            }
            set
            {
                _ServiceName = value;
            }
        }


        public Int32 Monitor
        {
            get
            {
                return _Monitor;
            }
            set
            {
                _Monitor = value;
            }
        }

        public Int32 CreateStartUpShortCut
        {
            get
            {
                return _CreateStartUpShortCut;
            }
            set
            {
                _CreateStartUpShortCut = value;
            }
        }

        public Int32 Interval
        {
            get
            {
                return _Interval;
            }
            set
            {
                _Interval = value;
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
            string SettingsXmlPath = Path.GetDirectoryName(applicationPath) + "\\ServiceGuard\\Config.xml";
            Program.WriteToWindowsEventLog("search for Config.xml at " + SettingsXmlPath, EventLogEntryType.Information);
            if (File.Exists(SettingsXmlPath))
            {
                XmlDocument xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.Load(SettingsXmlPath);
                    string servicename = xmlDocument.GetElementsByTagName("ServiceName")[0].InnerXml.ToString();
                    string serviceWaitTimeString = xmlDocument.GetElementsByTagName("ServiceName")[0].InnerXml.ToString();
                    string monitorString = xmlDocument.GetElementsByTagName("Monitor")[0].InnerXml.ToString();
                    string intervalString = xmlDocument.GetElementsByTagName("Interval")[0].InnerXml.ToString();
                    string createStartUpShortCutString = xmlDocument.GetElementsByTagName("CreateStartUpShortCut")[0].InnerXml.ToString();
                    int interval;
                    int monitor;
                    int serviceWaitTime;
                    int createStartUpShortCut;

                    if (!string.IsNullOrEmpty(servicename))
                    {
                        _ServiceName = servicename;
                    }

                    if (!string.IsNullOrEmpty(createStartUpShortCutString))
                    {
                        Int32.TryParse(createStartUpShortCutString, out createStartUpShortCut);
                        _CreateStartUpShortCut = createStartUpShortCut;
                    }

                    if (!string.IsNullOrEmpty(monitorString))
                    {
                        Int32.TryParse(monitorString, out monitor);
                        _Monitor = monitor;
                    }

                    if (!string.IsNullOrEmpty(intervalString))
                    {
                        Int32.TryParse(intervalString, out interval);
                        _Interval = interval;
                    }

                    if (!string.IsNullOrEmpty(serviceWaitTimeString))
                    {
                        Int32.TryParse(intervalString, out serviceWaitTime);
                        _ServiceWaitTime = serviceWaitTime;
                    }


                    if (string.IsNullOrEmpty(_Monitor.ToString()) || string.IsNullOrEmpty(_ServiceName) || string.IsNullOrEmpty(_Interval.ToString()))
                    {
                        Program.WriteToWindowsEventLog("Null _Monitor,_ServiceName,_Interval", EventLogEntryType.Error);
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
            }
            else
            {
                Program.WriteToWindowsEventLog("Config File Not Found at path ", EventLogEntryType.Error);
            }

        }
    }
}
