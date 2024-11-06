using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Collections;
using ITS.MobileAtStore.ObjectModel;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.IO;
using ITS.MobileAtStore.Common.ApplicationExportSettings;

namespace ITS.MobileAtStore.Common
{
    public static class Settings 
    {
        public static ConnectionSettings ReadConnectionSettings { get; set; }
        public static ConnectionSettings StoreConnectionSettings { get; set; }
        public static PaddingSettings ProductPaddingSettings { get; set; }
        public static PaddingSettings CustomerPaddingSettings { get; set; }
        public static DatabaseViewSettings DatabaseViewSettings { get; set; }
        public static bool TraceSQL { get; set; }

        public static BarcodeDecodingSettings DecodingSettings { get; set; }
        public static Dictionary<DOC_TYPES, ExportSettings> ExportSettingsDictionary { get; set; }        
        public static List<FileExportLocation> FileExportSettings { get; set; }
        public static ReflexisExportSettings ReflexisExportSettings { get; set; }
        public static WRMExportSettings WRMExportSettings { get; set; }

        private static string _HeaderPersistanceProperty;
        public static string HeaderPersistanceProperty
        {
            get
            {
                return _HeaderPersistanceProperty;
            }
            set 
            {
                _HeaderPersistanceProperty = value;
                if (string.IsNullOrEmpty(_HeaderPersistanceProperty))
                {
                    _HeaderPersistanceProperty = "ITS_DATALOGGER_HEADER";
                }
            }
        }
        private static string _LinePersistanceProperty;
        public static string LinePersistanceProperty
        {
            get
            {
                return _LinePersistanceProperty;
            }
            set
            {
                _LinePersistanceProperty = value;
                if (string.IsNullOrEmpty(_LinePersistanceProperty))
                {
                    _LinePersistanceProperty = "ITS_DATALOGGER_LINE";
                }
            }
        }

        public static void LoadSettings(String filename)
        {
            DecodingSettings = new BarcodeDecodingSettings();
            ProductPaddingSettings = new PaddingSettings();
            ReadConnectionSettings = new ConnectionSettings();
            CustomerPaddingSettings = new PaddingSettings();
            ExportSettingsDictionary = new Dictionary<DOC_TYPES, ExportSettings>();
            DatabaseViewSettings = new DatabaseViewSettings();
            FileExportSettings = new List<FileExportLocation>();

            foreach (DOC_TYPES docType in Enum.GetValues(typeof(DOC_TYPES)).Cast <DOC_TYPES>())
            {
                ExportSettingsDictionary.Add(docType, new ExportSettings());
            }

            if (!File.Exists(filename))
            {
                ConfigurationHelper.SaveSettingsFileStatic(typeof(Settings), filename);
            }

            ConfigurationHelper.LoadSettingsStatic(typeof(Settings), filename);

            char[] trimChars = new char[] {'\r', '\n', ' ' };
            if (string.IsNullOrEmpty(HeaderPersistanceProperty))
            {
                HeaderPersistanceProperty = "ITS_DATALOGGER_HEADER";
            }
            HeaderPersistanceProperty = HeaderPersistanceProperty.TrimStart(trimChars).TrimEnd(trimChars);
            if (string.IsNullOrEmpty(LinePersistanceProperty))
            {
                LinePersistanceProperty = "ITS_DATALOGGER_LINE";
            }
            LinePersistanceProperty = LinePersistanceProperty.TrimStart(trimChars).TrimEnd(trimChars);
                        

            if (!DataLayerSettings.CreateDictionary())
            {
                //ViewBag.Error = "Παρακαλώ ελέγξτε τις ρυθμίσεις τις βάσεις εξαγωγής δεδομένων.";
            }


#if DEBUG
            TraceSQL = true;
#endif

        }

        public static void SetServerMapPath(string ServerMapPath)
        {
            if (Settings.ReflexisExportSettings != null)
            {
                string internalFolder = ServerMapPath.Split('\\').Last();
                Settings.ReflexisExportSettings.ServerMapPathRoot = ServerMapPath.Substring(0, ServerMapPath.LastIndexOf("\\"+internalFolder)); ;
            }
        }
        
    }
}
