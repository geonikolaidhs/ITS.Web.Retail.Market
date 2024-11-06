using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo.DB;
using ITS.MobileAtStore.ObjectModel;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace ITS.MobileAtStore.Common
{
    public static class DataLayerSettings
    {
        public static Dictionary<DOC_TYPES, IDataLayer> DataLayers { get; private set; }

        public static bool CreateDictionary()
        {
            try
            {
                DataLayers = new Dictionary<DOC_TYPES, IDataLayer>();
                foreach (KeyValuePair<DOC_TYPES, ExportSettings> pair in Settings.ExportSettingsDictionary)
                {
                    if (pair.Value.ExportConnectionSettings != null && (pair.Value.ExportMode == ExportMode.DATABASE))// || pair.Value.ExportMode == ExportMode.BOTH))
                    {
                        ConnectionSettings set = pair.Value.ExportConnectionSettings;
                        String connectionString;
                        AutoCreateOption option = AutoCreateOption.DatabaseAndSchema;
                        connectionString = GetConnectionString(set, ref option);
                        if (String.IsNullOrEmpty(connectionString) == false)
                        {
                            XPDictionary dict = new ReflectionDictionary();
                            dict.GetDataStoreSchema(typeof(Header).Assembly);
                            DataLayers.Add(pair.Key, XpoDefault.GetDataLayer(connectionString, dict, option));

                            XPClassInfo headerClassInfo = dict.GetClassInfo(typeof(Header));
                            headerClassInfo.AddAttribute(new PersistentAttribute(Settings.HeaderPersistanceProperty));
                            XPClassInfo lineClassInfo = dict.GetClassInfo(typeof(Line));
                            lineClassInfo.AddAttribute(new PersistentAttribute(Settings.LinePersistanceProperty));
                        }
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public static String GetConnectionString(ConnectionSettings set, ref AutoCreateOption option)
        {
            option = AutoCreateOption.DatabaseAndSchema;
            String connectionString = "";
            switch (set.ConnectionMode)
            {
                case ConnectionMode.MYSQL:
                    connectionString = MySqlConnectionProvider.GetConnectionString(set.Server, set.Username, set.Password, set.DatabaseName);
                    break;
                case ConnectionMode.ORACLE:
                    connectionString = OracleConnectionProvider.GetConnectionString(set.Server, set.Username, set.Password);
                    option = AutoCreateOption.SchemaOnly;
                    break;
                case ConnectionMode.SQL_SERVER:
                    connectionString = MSSqlConnectionProvider.GetConnectionString(set.Server, set.Username, set.Password, set.DatabaseName);
                    break;
            }
            return connectionString;
        }
    }
}
