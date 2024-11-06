using ITS.Retail.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DevExpress.Xpo;
using System.Xml.Serialization;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common;

namespace ITS.Retail.MigrationTool
{
    [XmlRoot(ElementName = "settings", Namespace = "")]
    public class WebConfigurationSettings : BasicViewModel
    {

        public WebConfigurationSettings()
        {
            this.RetailBridgeEmailSettings = new EmailSettings();
            this.StoreControllerSettings = new StoreControllerSettingsVM();
            this.UpdaterBatchSize = 10000;
            this.LicenseServerURL = "http://www.its.net.gr/LicensingManager";
        }


        [XmlElement(ElementName = "sqlserver")]
        public string SqlServer
        {
            get
            {
                return _SqlServer;
            }
            set
            {
                SetPropertyValue("SqlServer", ref _SqlServer, value);
            }
        }

        [XmlElement(ElementName = "username")]
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                SetPropertyValue("Username", ref _Username, value);
            }
        }

        [XmlElement(ElementName = "pass")]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                SetPropertyValue("Password", ref _Password, value);
            }
        }

        [XmlElement(ElementName = "database")]
        public string Database
        {
            get
            {
                return _Database;
            }
            set
            {
                SetPropertyValue("Database", ref _Database, value);
            }
        }

        [XmlElement(ElementName = "dbtype")]
        public DBType DatabaseType
        {
            get
            {
                return _DatabaseType;
            }
            set
            {
                SetPropertyValue("DatabaseType", ref _DatabaseType, value);
            }
        }

        public string LicenseServerURL
        {
            get
            {
                return _LicenseServerURL;
            }
            set
            {
                SetPropertyValue("LicenseServerURL", ref _LicenseServerURL, value);
            }
        }


        //public bool DisableCache
        //{
        //    get
        //    {
        //        return _DisableCache;
        //    }
        //    set
        //    {
        //        SetPropertyValue("DisableCache", ref _DisableCache, value);
        //    }
        //}


        public StoreControllerSettingsVM StoreControllerSettings
        {
            get
            {
                return _StoreControllerSettings;
            }
            set
            {
                SetPropertyValue("StoreControllerSettings", ref _StoreControllerSettings, value);
            }
        }


        public EmailSettings RetailBridgeEmailSettings
        {
            get
            {
                return _RetailBridgeEmailSettings;
            }
            set
            {
                SetPropertyValue("RetailBridgeEmailSettings", ref _RetailBridgeEmailSettings, value);
            }
        }


        public int IISCache
        {
            get
            {
                return _IISCache;
            }
            set
            {
                SetPropertyValue("IISCache", ref _IISCache, value);
            }
        }


        public bool EnableMiniProfiler
        {
            get
            {
                return _EnableMiniProfiler;
            }
            set
            {
                SetPropertyValue("EnableMiniProfiler", ref _EnableMiniProfiler, value);
            }
        }


        public int UpdaterBatchSize
        {
            get
            {
                return _UpdaterBatchSize;
            }
            set
            {
                SetPropertyValue("UpdaterBatchSize", ref _UpdaterBatchSize, value);
            }
        }


        [XmlIgnore]
        public eCultureInfo Locale
        {
            get
            {
                return _Locale;
            }
            set
            {
                SetPropertyValue("Locale", ref _Locale, value);
            }
        }

        [XmlIgnore]
        public eApplicationInstance ApplicationInstance
        {
            get
            {
                return _ApplicationInstance;
            }
            set
            {
                SetPropertyValue("ApplicationInstance", ref _ApplicationInstance, value);
                Notify("RetailMaster");
                Notify("RetailStoreController");
                Notify("RetailDual");
            }
        }

        [XmlIgnore]
        public bool RetailMaster
        {
            get
            {
                return eApplicationInstance.RETAIL == ApplicationInstance;
            }
            set
            {
                ApplicationInstance = eApplicationInstance.RETAIL;
            }
        }

        [XmlIgnore]
        public bool RetailStoreController
        {
            get
            {
                return eApplicationInstance.STORE_CONTROLER == ApplicationInstance;
            }
            set
            {
                ApplicationInstance = eApplicationInstance.STORE_CONTROLER;
            }
        }

        [XmlIgnore]
        public bool RetailDual
        {
            get
            {
                return eApplicationInstance.DUAL_MODE == ApplicationInstance;
            }
            set
            {
                ApplicationInstance = eApplicationInstance.DUAL_MODE;
            }
        }


        [XmlIgnore]
        public string OlapServer
        {
            get
            {
                return _OlapServer;
            }
            set
            {
                SetPropertyValue("OlapServer", ref _OlapServer, value);
            }
        }

        [XmlIgnore]
        public string OlapUsername
        {
            get
            {
                return _OlapUsername;
            }
            set
            {
                SetPropertyValue("OlapUsername", ref _OlapUsername, value);
            }
        }

        [XmlIgnore]
        public string OlapPassword
        {
            get
            {
                return _OlapPassword;
            }
            set
            {
                SetPropertyValue("OlapPassword", ref _OlapPassword, value);
            }
        }

        public string OLAPConnectionString
        {
            get
            {
                return string.Format("provider = MSOLAP.6;data source = {0};User Id = {1};Password = {2};initial catalog = WRM_Analysis;cube name = WRM",
                    OlapServer, OlapUsername, OlapPassword);
            }
            set
            {
                string constring = value;
                string[] values = constring.Split(';');
                foreach (string param in values)
                {
                    string[] paramKeys = param.Split('=');
                    string key = paramKeys[0].Trim();
                    string keyvalue = paramKeys[1].Trim();
                    if (string.Compare(key, "data source", true) == 0)
                    {
                        OlapServer = keyvalue;
                    }
                    else if (string.Compare(key, "User Id", true) == 0)
                    {
                        OlapUsername = keyvalue;
                    }
                    else if (string.Compare(key, "Password", true) == 0)
                    {
                        OlapPassword = keyvalue;
                    }
                }
            }
        }

        // Fields...        

        private eApplicationInstance _ApplicationInstance;
        private bool _EnableMiniProfiler;
        private bool _DisableCache;
        private int _UpdaterBatchSize;
        private int _IISCache;
        private EmailSettings _RetailBridgeEmailSettings;
        private StoreControllerSettingsVM _StoreControllerSettings;
        private DBType _DatabaseType;
        private string _Database;
        private string _Password;
        private string _Username;
        private string _SqlServer;
        private string _OlapServer;
        private string _OlapUsername;
        private string _OlapPassword;
        private eCultureInfo _Locale;
        private string _LicenseServerURL;

    }

    public class StoreControllerSettingsVM : BasicViewModel
    {
        // Fields...
        private Guid _StoreController;
        private string _WebPassword;
        private string _WebUsername;
        private Guid _DefaultCustomer;
        private Guid _CurrentStore;
        private string _ID;
        private string _MasterURL;

        public string MasterURL
        {
            get
            {
                return _MasterURL;
            }
            set
            {
                SetPropertyValue("MasterURL", ref _MasterURL, value);
            }
        }


        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                SetPropertyValue("ID", ref _ID, value);
            }
        }


        public Guid CurrentStore
        {
            get
            {
                return _CurrentStore;
            }
            set
            {
                SetPropertyValue("CurrentStore", ref _CurrentStore, value);
            }
        }


        public Guid DefaultCustomer
        {
            get
            {
                return _DefaultCustomer;
            }
            set
            {
                SetPropertyValue("DefaultCustomer", ref _DefaultCustomer, value);
            }
        }


        [XmlIgnore]
        public string WebUsername
        {
            get
            {
                return _WebUsername;
            }
            set
            {
                SetPropertyValue("WebUsername", ref _WebUsername, value);
            }
        }

        [XmlIgnore]
        public string WebPassword
        {
            get
            {
                return _WebPassword;
            }
            set
            {
                SetPropertyValue("WebPassword", ref _WebPassword, value);
            }
        }


        public Guid StoreController
        {
            get
            {
                return _StoreController;
            }
            set
            {
                SetPropertyValue("StoreController", ref _StoreController, value);
            }
        }
    }

    public class EmailSettings : BasicViewModel
    {
        // Fields...
        private bool _EnableSSL;
        private int _Port;
        private string _Domain;
        private string _Password;
        private string _Username;
        private string _From;
        private string _SMTPHost;

        public string SMTPHost
        {
            get
            {
                return _SMTPHost;
            }
            set
            {
                SetPropertyValue("SMTPHost", ref _SMTPHost, value);
            }
        }

        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                SetPropertyValue("Port", ref _Port, value);
            }
        }

        public string From
        {
            get
            {
                return _From;
            }
            set
            {
                SetPropertyValue("From", ref _From, value);
            }
        }

        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                SetPropertyValue("Username", ref _Username, value);
            }
        }

        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                SetPropertyValue("Password", ref _Password, value);
            }
        }

        public string Domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                SetPropertyValue("Domain", ref _Domain, value);
            }
        }

        public bool EnableSSL
        {
            get
            {
                return _EnableSSL;
            }
            set
            {
                SetPropertyValue("EnableSSL", ref _EnableSSL, value);
            }
        }

    }
}
