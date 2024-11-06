using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DevExpress.Xpo;
using System.IO;
using ITS.Retail.Common;
using DevExpress.Web.Mvc;
using System.Collections.Concurrent;
using ITS.Retail.Model;
using System.Xml;
using DevExpress.Data.Filtering;
using ITS.Retail.WebClient.Helpers;
using System.Reflection;
using System.Web.Optimization;
using DevExpress.Web.Internal;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.AuxillaryClasses;
using System.Web.Http;
using ITS.Retail.WebClient.Providers;
using StackExchange.Profiling;
using StackExchange.Profiling.Storage;
using StackExchange.Profiling.Mvc;
using System.Threading.Tasks;
using System.Threading;
using ITS.Licensing.Client.Core.Implementations;
using System.Runtime.InteropServices;
using ITS.Retail.WebClient.Licensing;
using ITS.Licensing.Client.Services.Proxies.LicensingService;
using ITS.Licensing.Client.Services.Proxies;
using ITS.Licensing.Common.Implementations;
using ITS.Licensing.Client.Core;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WRM.Kernel.Interface;
using ITS.Retail.WebClient.App_Start;
using ITS.Retail.WRM.Kernel;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Diagnostics;
using ITS.Retail.WebClient.ViewModel;
using ITS.Retail.Model.NonPersistant;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Timers;

namespace ITS.Retail.WebClient
{
    public class MvcApplication : HttpApplication
    {
        private static byte[] RGBKey = { 185, 45, 138, 86, 180, 73, 50, 31, 73, 26, 53, 32, 2, 143, 64, 108, 214, 198, 77, 246, 219, 201, 128, 120 };
        private static byte[] RGBIV = { 57, 193, 117, 41, 127, 122, 176, 58 };
        private const string CONFIGURATION_FOLDER = "~/Configuration/";
        private const string CONFIGURATION_FILE = "wrm.config";
        public static int RetailMasterPostInterval = 30000;
        public static int RetailMasterUpdateInterval = 30000;
        public static int RetailMasterServiceTimeout = 300000;

        public static readonly string IMPORT_IS_RUNNING = "Import Thread is Running";
        public static readonly string TEMP_FOLDER = System.Web.HttpContext.Current.Server.MapPath("~/Temp/");
        public static readonly string LICENSE_FILE = System.Web.HttpContext.Current.Server.MapPath(CONFIGURATION_FOLDER + "license.its");

        public static readonly string LICENSE_SERVICE_NAME = "LicensingService.svc";
        public static readonly string LICENSE_SERVICE_USERNAME = "its";
        public static readonly string LICENSE_SERVICE_PASSWORD = "1t$ervices";

        public static ActiveUsersValidator ActiveUsersValidator = new ActiveUsersValidator();

        private static IWRMKernel WRMKernel { get; set; }
        public static IWRMLogModule WRMLogModule { get; set; }

#if _RETAIL_DUAL || _RETAIL_WEBCLIENT
        public static ApplicationStatus Status = ApplicationStatus.ONLINE;
#else
        public static ApplicationStatus Status = ApplicationStatus.OFFLINE_VIA_ERROR;
#endif

#if _LICENSED
        public static readonly bool USES_LICENSE = true;
#else
        public static readonly bool USES_LICENSE = true;
#endif

        public static bool EnableMiniProfiler { get; set; }

        public static int UpdaterBatchSize { get; set; }

        public static string OLAPConnectionString { get; set; }

        public static int DataTransferVerificationThreadSleepTimeInMilliSeconds { get; set; }
        public static int DataTransferVerificationThreadPastHoursToCheck { get; set; }
        public static int DataTransferToPOSVerificationThreadSleepTimeInMilliSeconds { get; set; }
        public static int DataTransferToPOSVerificationThreadPastHoursToCheck { get; set; }
        public static int ApplicationLogDayRange { get; set; }

        /* Lic Addition Start */
        private static string LicenseServerURL { get; set; }

        private static DateTime LastLicenseCheck;

        public static ITS.Licensing.Enumerations.LicenseStatus LicenseStatus { get; private set; }

        public static void SetLicenseStatus(ITS.Licensing.Enumerations.LicenseStatus licenseStatus)
        {
            LicenseStatus = licenseStatus;
        }

        public static LicenseManager LicenseManager { get; private set; }

        public static LicenseStatusViewModel LicenseStatusViewModel
        {
            get
            {
                LicenseInfo licenseInfo = MvcApplication.ReadStoredLicenseInfo();
                LicenseStatusViewModel licenseStatusViewModel = new LicenseStatusViewModel()
                {
                    StartDate = licenseInfo.StartDate,
                    EndDate = licenseInfo.EndDate,
                    CurrentUsers = ActiveUsersValidator.NumberOfActiveUsers,
                    MaxUsers = licenseInfo.MaxUsers,
                    MaxPeripheralUsers = licenseInfo.MaxPeripheralsUsers,
                    MaxTabletSmartPhoneUsers = licenseInfo.MaxTabletSmartPhoneUsers,
                    LicenseType = licenseInfo.LicenseType,
                    GreyZoneDays = licenseInfo.GreyZoneDays,
                    DaysToAlertBeforeExpiration = licenseInfo.DaysToAlertBeforeExpiration
                };
                return licenseStatusViewModel;
            }
        }

        public static string LicenseInfo
        {
            get
            {
                LicenseInfo licenseInfo = MvcApplication.ReadStoredLicenseInfo();
                string licenseInfoStr = String.Format("{0}:{1}",
                                                        Resources.EndDate, licenseInfo.EndDate.ToShortDateString()
                                                     );
                return licenseInfoStr;
            }
        }

        public ElapsedEventArgs e;

        public static LicenseInfo ReadStoredLicenseInfo()
        {
            CriteriaOperator licenseInfoCriteria;
            LicenseInfo licenseInfo = null;
            switch (ApplicationInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    return LicenseManager.ReadStoredLicenseInfo();
                case eApplicationInstance.RETAIL:
                    licenseInfoCriteria = CriteriaOperator.And(new BinaryOperator("Description", Platform.PlatformConstants.HEADQUARTERS),
                                                                               new BinaryOperator("Server", Guid.Empty)
                                                                             );//RetailHelper.ApplyOwnerCriteria

                    licenseInfo = LicenseManager.ReadStoredLicenseInfo();
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        LicenseUserDistribution licenseUserDistribution = uow.FindObject<LicenseUserDistribution>(licenseInfoCriteria);
                        if (licenseUserDistribution == null)
                        {
                            return licenseInfo;
                        }
                        else
                        {
                            ServerLicenseInfo serverLicenseInfo = licenseUserDistribution.ServerLicenseInfo;
                            licenseInfo.MaxUsers = serverLicenseInfo.MaxConnectedUsers;
                            licenseInfo.MaxPeripheralsUsers = serverLicenseInfo.MaxPeripheralsUsers;
                            licenseInfo.MaxTabletSmartPhoneUsers = serverLicenseInfo.MaxTabletSmartPhoneUsers;
                            return licenseInfo;
                        }
                        return licenseInfo;
                    }

                case eApplicationInstance.STORE_CONTROLER:
                    licenseInfoCriteria = new BinaryOperator("Server", StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid);
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        LicenseUserDistribution licenseUserDistribution = uow.FindObject<LicenseUserDistribution>(licenseInfoCriteria);
                        if (licenseUserDistribution == null)
                        {
                            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
                            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                            using (RetailWebClient.POSUpdateService.POSUpdateService webService = new RetailWebClient.POSUpdateService.POSUpdateService())
                            {
                                webService.Timeout = MvcApplication.RetailMasterServiceTimeout;
                                webService.Url = StoreControllerAppiSettings.MasterServiceURL;

                                long verUpdate = DateTime.MinValue.Ticks;
                                try
                                {
                                    int totalCount;
                                    string sUpdates = webService.GetChanges(typeof(LicenseUserDistribution).Name, new DateTime(verUpdate), StoreControllerAppiSettings.StoreControllerOid.ToString(), RetailWebClient.POSUpdateService.eIdentifier.STORECONTROLLER, out totalCount);
                                    if (sUpdates == null)
                                    {
                                        throw new Exception(String.Format("Could not find LicenseUserDistribution for store with Code {0} and Oid {1}",
                                                                          StoreControllerAppiSettings.CurrentStore.Code,
                                                                          StoreControllerAppiSettings.StoreControllerOid.ToString()
                                                                         )
                                                           );
                                    }
                                    if (sUpdates == MvcApplication.IMPORT_IS_RUNNING)
                                    {
                                        throw new Exception(String.Format("Could not find LicenseUserDistribution for store with Code {0} and Oid {1} because import is running",
                                                                          StoreControllerAppiSettings.CurrentStore.Code,
                                                                          StoreControllerAppiSettings.StoreControllerOid.ToString()
                                                                         )
                                                           );
                                    }
                                    sUpdates = Convert.ToString(CompressionHelper.DecompressLZMA(sUpdates));
                                    List<string> licenseUserDistributions = JsonConvert.DeserializeObject<List<string>>(sUpdates);
                                    if (licenseUserDistributions.Count == 0)
                                    {
                                        throw new Exception(String.Format("Got empty responce for LicenseUserDistribution for store with Code {0} and Oid {1}",
                                                                          StoreControllerAppiSettings.CurrentStore.Code,
                                                                          StoreControllerAppiSettings.StoreControllerOid.ToString()
                                                                         )
                                                            );
                                    }
                                    else if (licenseUserDistributions.Count > 1)
                                    {
                                        throw new Exception(String.Format("Got to many responces for LicenseUserDistribution for store with Code {0} and Oid {1}",
                                                                          StoreControllerAppiSettings.CurrentStore.Code,
                                                                          StoreControllerAppiSettings.StoreControllerOid.ToString()
                                                                         )
                                                            );
                                    }
                                    else//Counted ONLY ONE ANSWER
                                    {
                                        JObject jObject = JObject.Parse(licenseUserDistributions.First());
                                        licenseUserDistribution = new LicenseUserDistribution(uow);
                                        string error;
                                        licenseUserDistribution.FromJson(jObject, Platform.PlatformConstants.JSON_SERIALIZER_SETTINGS, true, true, out error);
                                        if (String.IsNullOrWhiteSpace(error) == false)
                                        {
                                            throw new Exception(error);
                                        }
                                        licenseUserDistribution.Save();
                                        XpoHelper.CommitTransaction(uow);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    throw;
                                }
                                finally
                                {
                                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                                }
                            }
                        }

                        ServerLicenseInfo serverLicenseInfo = licenseUserDistribution.ServerLicenseInfo;
                        LicenseInfo storeControllerlicenseInfo = new LicenseInfo()
                        {
                            StartDate = serverLicenseInfo.StartDate,
                            EndDate = serverLicenseInfo.EndDate,
                            MaxUsers = serverLicenseInfo.MaxConnectedUsers,
                            MaxPeripheralsUsers = serverLicenseInfo.MaxPeripheralsUsers,
                            MaxTabletSmartPhoneUsers = serverLicenseInfo.MaxTabletSmartPhoneUsers,
                            DaysToAlertBeforeExpiration = serverLicenseInfo.DaysToAlertBeforeExpiration,
                            GreyZoneDays = serverLicenseInfo.GreyZoneDays
                        };

                        return storeControllerlicenseInfo;
                    }
                    return licenseInfo;

            }
            return licenseInfo;
        }

        /* Lic Addition End */

        private static bool _IsImportRunning;
        public static bool IsImportRunning
        {
            get
            {
                return _IsImportRunning;
            }
            set
            {
                _IsImportRunning = value;
            }
        }

        private static Assembly GetWebClientAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        public static Guid GetAssemblyID()
        {
            GuidAttribute attribute = (GuidAttribute)GetWebClientAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return Guid.Parse(attribute.Value);
        }

        public static string GetMachineID()
        {
            MachineIDGenerator machineIDGenerator = new MachineIDGenerator();
            return machineIDGenerator.GenerateMachineID();
        }

        public static Version GetVersion()
        {
            return GetWebClientAssembly().GetName().Version;
        }

        private void RegisterThemes()
        {
            string[] filePaths = Directory.GetFiles(Server.MapPath("~/Themes"), "*.dll");
            foreach (string dll in filePaths)
            {
                Assembly asm = Assembly.LoadFile(dll);
                IEnumerable<Type> types = asm.GetTypes().Where(g => g.IsSubclassOf(typeof(ThemesProvider)));
                if (types.Count() > 0)
                {
                    foreach (Type type in types)
                    {
                        MethodInfo meth = type.GetMethod("Register");
                        meth.Invoke(null, null);
                    }
                }
            }
        }

        public static ConcurrentDictionary<string, DateTime> ConnectedtUsers { get; set; }
        public static ConcurrentDictionary<Guid, DateTime> UsersOnline { get; set; }
        public static DateTime lastGarbageCollection;

        protected WRMSettings ReadSettings()
        {
            string configurationFilePath = Server.MapPath(CONFIGURATION_FOLDER + CONFIGURATION_FILE);

            using (TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                using (Stream fileStream = File.Open(configurationFilePath, FileMode.Open) as Stream)
                {
                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, cryptoServiceProvider.CreateDecryptor(RGBKey, RGBIV), CryptoStreamMode.Read))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(WRMSettings));
                        WRMSettings wrmSettings = xmlSerializer.Deserialize(cryptoStream) as WRMSettings;
                        return wrmSettings;
                    }
                }
            }
        }

        protected void SaveSettings()
        {
            string configurationFilePath = Server.MapPath(CONFIGURATION_FOLDER + CONFIGURATION_FILE);
#if DEBUG
            WRMSettings wrmSettings = new WRMSettings() { LicenseServerURL = "http://localhost/ITS.Licensing.Web" };
#else
            WRMSettings wrmSettings = new WRMSettings() { LicenseServerURL = "http://www.its.net.gr/its.license" };
#endif

            using (TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                using (Stream fileStream = File.Open(configurationFilePath, FileMode.OpenOrCreate, FileAccess.Write) as Stream)
                {
                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, cryptoServiceProvider.CreateEncryptor(RGBKey, RGBIV), CryptoStreamMode.Write))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(WRMSettings));
                        xmlSerializer.Serialize(cryptoStream, wrmSettings);
                    }
                }
            }
        }

        protected void PrepareDatabaseConnection()
        {
            string configurationXMLFile = Server.MapPath("~/Configuration/config.xml");
            if (File.Exists(configurationXMLFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configurationXMLFile);

                XmlNode settingsNode = doc.SelectSingleNode("settings");
                XmlNode xmlNode = settingsNode.SelectSingleNode("sqlserver");
                if (xmlNode != null)
                {
                    XpoHelper.sqlserver = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("username");
                if (xmlNode != null)
                {
                    XpoHelper.username = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("pass");
                if (xmlNode != null)
                {
                    XpoHelper.pass = xmlNode.InnerText.Trim();
                }
                xmlNode = settingsNode.SelectSingleNode("database");
                if (xmlNode != null)
                {
                    XpoHelper.database = xmlNode.InnerText.Trim();
                }

                xmlNode = settingsNode.SelectSingleNode("DisableCache");
                bool disableCache = false;
                if (xmlNode != null && bool.TryParse(xmlNode.InnerText.Trim(), out disableCache) == true)
                {
                    XpoHelper.DisableCache = disableCache;
                }
                else
                {
                    XpoHelper.DisableCache = false;
                }

                xmlNode = settingsNode.SelectSingleNode("dbtype");
                if (xmlNode != null)
                {
                    try
                    {
                        XpoHelper.databasetype = (DBType)Enum.Parse(typeof(DBType), xmlNode.InnerText.Trim());
                    }
                    catch
                    {
                        XpoHelper.databasetype = DBType.SQLServer;
                    }
                }

                BridgeHelper.SMTPHost = "";
                XmlNode RetailBridgeEmailSettingsNode = doc.SelectSingleNode("SMTPHost/RetailBridgeEmailSettings");
                if (RetailBridgeEmailSettingsNode != null)
                {
                    BridgeHelper.SMTPHost = RetailBridgeEmailSettingsNode.InnerText;
                }

                BridgeHelper.Port = "";
                XmlNode PortNode = settingsNode.SelectSingleNode("SMTPHost/RetailBridgeEmailSettings/Port");
                if (PortNode != null)
                {
                    BridgeHelper.Port = PortNode.InnerText;
                }


                BridgeHelper.EmailFrom = "";
                XmlNode FromNode = settingsNode.SelectSingleNode("SMTPHost/RetailBridgeEmailSettings/From");
                if (FromNode != null)
                {
                    BridgeHelper.EmailFrom = FromNode.InnerText;
                }

                XmlNode UsernameNode = settingsNode.SelectSingleNode("SMTPHost/RetailBridgeEmailSettings/Username");
                BridgeHelper.EmailUsername = "";
                if (UsernameNode != null)
                {
                    BridgeHelper.EmailUsername = UsernameNode.InnerText;
                }


                XmlNode PasswordNode = settingsNode.SelectSingleNode("SMTPHost/RetailBridgeEmailSettings/Password");
                BridgeHelper.EmailPassword = "";
                if (PasswordNode != null)
                {
                    BridgeHelper.EmailPassword = PasswordNode.InnerText;
                }

                XmlNode DomainNode = settingsNode.SelectSingleNode("SMTPHost/RetailBridgeEmailSettings/Domain");
                BridgeHelper.Domain = "";
                if (DomainNode != null)
                {
                    BridgeHelper.Domain = DomainNode.InnerText;
                }

                XmlNode EnableSSLNode = settingsNode.SelectSingleNode("SMTPHost/RetailBridgeEmailSettings/EnableSSL");
                BridgeHelper.EnableSSL = false;
                if (EnableSSLNode != null)
                {
                    bool enableSSL;
                    Boolean.TryParse(EnableSSLNode.InnerText, out enableSSL);
                    BridgeHelper.EnableSSL = enableSSL;
                }

                XmlNode IISCacheNode = settingsNode.SelectSingleNode("IISCache");
                if (IISCacheNode != null)
                {
                    XpoHelper.IISCache = long.Parse(IISCacheNode.InnerText);
                }

                EnableMiniProfiler = false;
                XmlNode EnableMiniProfilerNode = settingsNode.SelectSingleNode("EnableMiniProfiler");
                if (EnableMiniProfilerNode != null && !string.IsNullOrEmpty(EnableMiniProfilerNode.InnerText))
                {
                    bool value;
                    if (bool.TryParse(EnableMiniProfilerNode.InnerText, out value))
                    {
                        EnableMiniProfiler = value;
                    }
                }

                UpdaterBatchSize = 10000;
                XmlNode UpdaterBatchSizeNode = settingsNode.SelectSingleNode("UpdaterBatchSize");
                if (UpdaterBatchSizeNode != null && !string.IsNullOrEmpty(UpdaterBatchSizeNode.InnerText))
                {
                    int value;
                    if (int.TryParse(UpdaterBatchSizeNode.InnerText, out value))
                    {
                        UpdaterBatchSize = value;
                    }
                }

                OLAPConnectionString = null;
                XmlNode OLAPConnectionStringNode = settingsNode.SelectSingleNode("OLAPConnectionString");
                if (OLAPConnectionStringNode != null && !string.IsNullOrEmpty(OLAPConnectionStringNode.InnerText))
                {
                    OLAPConnectionString = OLAPConnectionStringNode.InnerText;
                }

                long databaseVersion = VersionHelper.GetMigrationVersion();
                if (BasicObj.schemaVersion != VersionHelper.GetMigrationVersion())
                {
                    throw new Exception(Resources.DatabaseVersionMismatch + Environment.NewLine + "Expected Version:" + BasicObj.schemaVersion + Environment.NewLine + "Database Version:" + databaseVersion);
                }

                if (XpoHelper.IISCache < 1)
                {
                    XpoHelper.IISCache = -1;
                }

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    ApplicationSettings application_settings = uow.FindObject<ApplicationSettings>(null);
                    AppiSettings.ReadSettings(application_settings);
                }

                XmlNode dataTransferNode = settingsNode.SelectSingleNode("DataTransferVerificationThreadSleepTime");
                int dataTransferVerificationThreadSleepTimeInMilliSeconds;
                if (dataTransferNode != null
                    && !string.IsNullOrWhiteSpace(dataTransferNode.InnerText)
                    && int.TryParse(dataTransferNode.InnerText, out dataTransferVerificationThreadSleepTimeInMilliSeconds)
                    )
                {
                    DataTransferVerificationThreadSleepTimeInMilliSeconds = dataTransferVerificationThreadSleepTimeInMilliSeconds; ;
                }
                else
                {
                    DataTransferVerificationThreadSleepTimeInMilliSeconds = 3600 * 1000;
                }

                XmlNode dataTransferNodePastHoursToCheck = settingsNode.SelectSingleNode("DataTransferVerificationThreadPastHoursToCheck");
                int transferNodePastHoursToCheck;
                if (dataTransferNodePastHoursToCheck != null
                    && !string.IsNullOrWhiteSpace(dataTransferNodePastHoursToCheck.InnerText)
                    && int.TryParse(dataTransferNodePastHoursToCheck.InnerText, out transferNodePastHoursToCheck)
                    )
                {
                    DataTransferVerificationThreadPastHoursToCheck = transferNodePastHoursToCheck; ;
                }
                else
                {
                    DataTransferVerificationThreadPastHoursToCheck = 48;
                }

                XmlNode dataTransferToPOSNode = settingsNode.SelectSingleNode("DataTransferVerificationToPOSThreadSleepTime");
                int dataTransferToPOSVerificationThreadSleepTimeInMilliSeconds;
                if (dataTransferToPOSNode != null
                    && !string.IsNullOrWhiteSpace(dataTransferToPOSNode.InnerText)
                    && int.TryParse(dataTransferToPOSNode.InnerText, out dataTransferToPOSVerificationThreadSleepTimeInMilliSeconds))
                {
                    DataTransferToPOSVerificationThreadSleepTimeInMilliSeconds = dataTransferToPOSVerificationThreadSleepTimeInMilliSeconds; ;
                }
                else
                {
                    DataTransferToPOSVerificationThreadSleepTimeInMilliSeconds = 10000;
                }

                XmlNode dataTransferToPOSastHoursToCheck = settingsNode.SelectSingleNode("DataTransferVerificationToPOSPastHoursToCheck");
                int transferToPOSVerificationPastHoursToCheck;
                if (dataTransferToPOSastHoursToCheck != null
                    && !string.IsNullOrWhiteSpace(dataTransferToPOSastHoursToCheck.InnerText)
                    && int.TryParse(dataTransferToPOSastHoursToCheck.InnerText, out transferToPOSVerificationPastHoursToCheck))
                {
                    DataTransferToPOSVerificationThreadPastHoursToCheck = transferToPOSVerificationPastHoursToCheck; ;
                }
                else
                {
                    DataTransferToPOSVerificationThreadPastHoursToCheck = 24;
                }

                ApplicationLogDayRange = 30;
                XmlNode ApplicationLogDayRangeNode = settingsNode.SelectSingleNode("ApplicationLogDayRange");
                if (ApplicationLogDayRangeNode != null && !string.IsNullOrEmpty(ApplicationLogDayRangeNode.InnerText))
                {
                    int value;
                    if (int.TryParse(ApplicationLogDayRangeNode.InnerText, out value))
                    {
                        ApplicationLogDayRange = value;
                    }
                }
                if (USES_LICENSE && ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
                {
                    //XmlNode licenseServerURLNode = settingsNode.SelectSingleNode("LicenseServerURL");

                    //if (licenseServerURLNode == null)
                    //{
                    //    throw new Exception("Initialization Error. LicenseServerURL is not defined in the config file");
                    //}
                    //LicenseServerURL = licenseServerURLNode.InnerText;

                    //Uri dummyUri;
                    //if (Uri.TryCreate(LicenseServerURL, UriKind.RelativeOrAbsolute, out dummyUri) == false)
                    //{
                    //    throw new Exception("Initialization Error. LicenseServerURL is not a valid url");
                    //}
                    //Debugger.Launch();
                    //SaveSettings();throw new Exception("end");
                    WRMSettings wrmSettings = ReadSettings();
                    LicenseServerURL = wrmSettings.LicenseServerURL;
                }
                if (ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
                {
                    string storeControllerSettingsPath = "StoreControllerSettings/";
                    XmlNode currentStoreNode = settingsNode.SelectSingleNode(storeControllerSettingsPath + "StoreControllerCommandInterval");
                    int ScCommandInterval;

                    if (currentStoreNode == null || string.IsNullOrEmpty(currentStoreNode.InnerText) || !int.TryParse(currentStoreNode.InnerText, out ScCommandInterval))
                    {
                        ScCommandInterval = 30 * 1000;
                    }

                    MvcApplication.StoreControllerCommandInterval = ScCommandInterval;

                }
                if (ApplicationInstance == eApplicationInstance.STORE_CONTROLER || ApplicationInstance == eApplicationInstance.DUAL_MODE)
                {
                    string storeControllerSettingsPath = "StoreControllerSettings/";
                    XmlNode currentStoreNode = settingsNode.SelectSingleNode(storeControllerSettingsPath + "CurrentStore");
                    if (currentStoreNode == null || string.IsNullOrEmpty(currentStoreNode.InnerText))
                    {
                        throw new Exception("Initialization Error. CurrentStore (Oid) is not defined in the config file");
                    }
                    else
                    {
                        Guid storeGuid;
                        Guid.TryParse(currentStoreNode.InnerText, out storeGuid);
                        StoreControllerAppiSettings.CurrentStoreOid = storeGuid;
                    }

                    XmlNode storeIDNode = settingsNode.SelectSingleNode(storeControllerSettingsPath + "ID");
                    if (storeIDNode == null || string.IsNullOrEmpty(storeIDNode.InnerText))
                    {
                        throw new Exception("Initialization Error. ID (int) is not defined in the config file");
                    }
                    else
                    {
                        int id = -1;
                        int.TryParse(storeIDNode.InnerText, out id);
                        StoreControllerAppiSettings.ID = id;
                    }

                    XmlNode MasterURLnode = settingsNode.SelectSingleNode(storeControllerSettingsPath + "MasterURL");
                    if (MasterURLnode == null || string.IsNullOrEmpty(MasterURLnode.InnerText))
                    {
                        throw new Exception("Initialization Error. MasterURL is not defined in the config file");
                    }
                    else
                    {
                        StoreControllerAppiSettings.MasterURL = MasterURLnode.InnerText;
                    }


                    XmlNode defaultCustomerNode = settingsNode.SelectSingleNode(storeControllerSettingsPath + "DefaultCustomer");

                    if (defaultCustomerNode == null)
                    {
                        throw new Exception("Initialization Error. DefaultCustomer (Oid) is not defined in the config file");
                    }
                    else
                    {
                        Guid customerGuid;
                        Guid.TryParse(defaultCustomerNode.InnerText, out customerGuid);
                        StoreControllerAppiSettings.DefaultCustomerOid = customerGuid;
                    }

                    XmlNode storeControllerNode = settingsNode.SelectSingleNode(storeControllerSettingsPath + "StoreController");

                    if (storeControllerNode == null || string.IsNullOrEmpty(storeControllerNode.InnerText))
                    {
                        throw new Exception("Initialization Error. StoreController (Oid) is not defined in the config file");
                    }
                    else
                    {
                        StoreControllerAppiSettings.StoreControllerOid = Guid.Parse(storeControllerNode.InnerText);
                    }
                }
            }
            else
            {
                throw new Exception("Configuration File " + configurationXMLFile + " not found");
            }
        }

        /* Lic Addition Start */
        public static DateTime lastLicCheck;
        public static bool isRegistered;
        /* Lic Addition End */
        public static string baseTheme;

#if _RETAIL_STORECONTROLLER
        public const eApplicationInstance ApplicationInstance = eApplicationInstance.STORE_CONTROLER;
        public const LicenseServerInstance LicenseServerInstanceType = LicenseServerInstance.STORE_CONTROLLER;
#elif _RETAIL_DUAL
        public const eApplicationInstance ApplicationInstance = eApplicationInstance.DUAL_MODE;
        public const LicenseServerInstance LicenseServerInstanceType = LicenseServerInstance.MASTER_OR_DUAL;
#else
        public const eApplicationInstance ApplicationInstance = eApplicationInstance.RETAIL;
        public const LicenseServerInstance LicenseServerInstanceType = LicenseServerInstance.MASTER_OR_DUAL;
#endif

        protected void Application_Start()
        {
#if DEBUG
            //System.Diagnostics.Debugger.Launch();
#endif
            //RouteTable.Routes.MapHubs();
            ModelBinders.Binders.DefaultBinder = new RetailModelBinder();
            IsImportRunning = false;
            GC.KeepAlive(WRMLogModule);

            AreaRegistration.RegisterAllAreas();
            RegisterThemes();
#if _RETAIL_WEBCLIENT || _RETAIL_DUAL

            RetailBridgeService.Initialize();
#endif
            POSUpdateService.Initialize();

            ConnectedtUsers = new ConcurrentDictionary<string, DateTime>();
            UsersOnline = new ConcurrentDictionary<Guid, DateTime>();

            lastGarbageCollection = DateTime.Now;

            PrepareDatabaseConnection();

            KernelConfig.RegisterKernel();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            IViewEngine razorEngine = new RazorViewEngine() { FileExtensions = new string[] { "cshtml" } };
            if (EnableMiniProfiler)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(razorEngine));
            }
            else
            {
                ViewEngines.Engines.Add(razorEngine);
            }


            if (!Directory.Exists(TEMP_FOLDER))
            {
                Directory.CreateDirectory(TEMP_FOLDER);
            }

            if (ApplicationHelper.IsStoreControllerInstance())
            {
                Updater.ServerRoot = this.Server.MapPath("~");
                Updater.StoreControllerCommandThread.Start();
                Updater.GetUpdatesThread.Start();
                Updater.PostRecordsThread.Start();
                Updater.PostSyncInfoThread.Start();
                Updater.pauseEvent.PauseEventPostRecordsThread = new ManualResetEvent(true);
                Updater.pauseEvent.PauseEventPostSyncInfoThread = new ManualResetEvent(false);
                Updater.pauseEvent.PauseEventStartUpdateThread = new ManualResetEvent(true);
                Updater.pauseEvent.ExceptionThrownPostRecordsThread = Updater.pauseEvent.ExceptionThrownPostSyncInfoThread = Updater.pauseEvent.ExceptionThrownStartUpdateThread = false;
                //Updater.DataTransferToPOSVerificationThread.Start();//TODO Enable thread on POS when READY!!!
                Updater.DataTransferVerificationThread.Start();//TODO start the thread in ALL Application Instances
            }


            if (MvcApplication.USES_LICENSE && MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
            {
                string serviceURL = MvcApplication.LicenseServerURL.TrimEnd('/') + "/" + MvcApplication.LICENSE_SERVICE_NAME;
                LicensingServiceClient service = ServiceProxyFactory.CreateProxy<LicensingServiceClient>(serviceURL);
                service.ClientCredentials.UserName.UserName = MvcApplication.LICENSE_SERVICE_USERNAME;
                service.ClientCredentials.UserName.Password = MvcApplication.LICENSE_SERVICE_PASSWORD;

                NLogger logger = new NLogger(NLog.LogManager.GetLogger("ITS.Retail"));//TODO Remove this and update License Library
                LicenseManager = LicenseManagerFactory.CreateLicenseManager(MvcApplication.LICENSE_FILE, service, logger);
                LicenseManager.OnUpdateLicense += RetailHelper.SetLicenseUserDistribution;
                try
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        int currentUsersCount = ActiveUsersValidator.NumberOfActiveUsers;
                        int currentPeripheralUsersCount = ActiveUsersValidator.NumberOfActivePeripheralUsers;
                        int currentTabletSmartPhoneUsersCount = ActiveUsersValidator.NumberOfActiveTabletSmartPhoneUsers;
                        string machineID = MvcApplication.GetMachineID();
                        Guid assemblyID = MvcApplication.GetAssemblyID();
                        Version currentVersion = MvcApplication.GetVersion();
                        GetLicenseStatusResult licenseStatusResult = MvcApplication.LicenseManager.GetLicenseStatus(currentUsersCount,
                                                                                                                    currentPeripheralUsersCount,
                                                                                                                    currentTabletSmartPhoneUsersCount,
                                                                                                                    machineID,
                                                                                                                    assemblyID,
                                                                                                                    currentVersion
                                                                                                                    );
                        MvcApplication.LicenseStatus = licenseStatusResult.LicenseStatus;
                    }
                }
                catch (Exception ex)
                {
                    WRMLogModule.Log(ex, "Error geting license status during initialization", KernelLogLevel.Error);
                    MvcApplication.LicenseStatus = ITS.Licensing.Enumerations.LicenseStatus.Undefined;
                }
            }

            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    WRMApplicationSettings wrmApplicationSettings = uow.FindObject<WRMApplicationSettings>(null);
                    if (wrmApplicationSettings == null)
                    {
                        wrmApplicationSettings = new WRMApplicationSettings(uow);
                    }
                    wrmApplicationSettings.ApplicationInstanceInteger = (int)MvcApplication.ApplicationInstance;
                    wrmApplicationSettings.Save();
                    uow.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                WRMLogModule.Log(ex, "Error while reading application instance info", KernelLogLevel.Error);
            }

            Task reportTask = new Task(() =>
            {
                try
                {
                    PreloadReports();
                }
                catch (Exception e)
                {
                    WRMLogModule.Log(e);
                }
            });
            Task ApplicationLogClearTask = new Task(() =>
            {
                try
                {
                    System.Timers.Timer scheduleTimer = new System.Timers.Timer();
                    scheduleTimer.Interval = 1000 * 60 * 60 * 24; // run every 24 hours
                    scheduleTimer.Elapsed += new ElapsedEventHandler(ApplicationLogClear);
                    scheduleTimer.Start();
                    //ApplicationLogClear();
                }
                catch (Exception e)
                {
                    WRMLogModule.Log(e);
                }
            });


            reportTask.Start();
            ApplicationLogClearTask.Start();

            BasicObj.ExecuteActionTypes += this.ExecuteActionTypes;

            WRMLogModule.Log("Application Start", KernelLogLevel.Info);
        }

        private void ApplicationLogClear(object sender, ElapsedEventArgs e)
        {
            while (true)
            {
                try
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        Debug.WriteLine(ApplicationLogDayRange);
                        long DaysRangeOnTicks = (DateTime.UtcNow - TimeSpan.FromDays(ApplicationLogDayRange)).Ticks;
                        XPCollection<ApplicationLog> appLogs = new XPCollection<ApplicationLog>(uow, new BinaryOperator("CreatedOnTicks", DaysRangeOnTicks, BinaryOperatorType.LessOrEqual));

                        foreach (ApplicationLog applog_item in appLogs)
                        {
                            if (applog_item.Oid != Guid.Empty)
                            {
                                uow.Delete(appLogs);
                                uow.CommitTransaction();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WRMLogModule.Log(ex, "Error clearing Application Log files ", kernelLogLevel: KernelLogLevel.Info);
                }
            }
        }

        private void PreloadReports()
        {
            if (ApplicationInstance != eApplicationInstance.RETAIL)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    XPCollection<CustomReport> reports = new XPCollection<CustomReport>(uow, new BinaryOperator("ObjectType", "DocumentHeader"));
                    CompanyNew owner = uow.GetObjectByKey<CompanyNew>(StoreControllerAppiSettings.CurrentStore.Owner.Oid);
                    string title, descr;
                    Dictionary<XtraReportBaseExtension, string> xreportList = reports.
                        Select(x => new
                        {
                            Description = x.Description,
                            Report = ReportsHelper.GetXtraReport(x.Oid, owner, null, null, out title, out descr)
                        }).Where(x => x.Report != null)
                            .ToDictionary(customReport => customReport.Report
                        , x => x.Description);
                    foreach (KeyValuePair<XtraReportBaseExtension, string> pair in xreportList)
                    {
                        XtraReportBaseExtension xreport = pair.Key;
                        try
                        {
                            xreport.CreateDocument();
                        }
                        catch (Exception exception)
                        {
                            //string error = exception.GetFullMessage();
                            WRMLogModule.Log(exception, "Error preloading report " + pair.Value, "PreloadReports", kernelLogLevel: KernelLogLevel.Info);
                        }

                    }
                }
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();
            WRMLogModule.Log(exc, "Unhandled Exception", KernelLogLevel.Error);
            try
            {
                //if (exc.GetType() == typeof(HttpException))
                //{
                Server.Transfer("~/Error/Error500");
                //}
                //Server.ClearError();
                //Server.Transfer("~/Error/Error500");
            }
            catch (Exception exception)
            {
                //Server.Transfer("~/Error/Error500");
            }
        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (MvcApplication.EnableMiniProfiler)
            {
                MiniProfiler.Start();
            }
            if (!MvcApplication.USES_LICENSE)
            {
                isRegistered = true;
            }
            if (MvcApplication.USES_LICENSE)
            {
                if (MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER && MvcApplication.LicenseManager != null)
                {
                    TimeSpan sp = new TimeSpan(DateTime.Now.Ticks - MvcApplication.LastLicenseCheck.Ticks);
                    if (sp.TotalSeconds > 1800 + new Random().Next() % 180) //ελέγχουμε περίπου κάθε 30 λεπτά εάν η εφαρμογή έχει έγκυρο license.
                    {
                        try
                        {
                            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                            {
                                int currentUsersCount = ActiveUsersValidator.NumberOfActiveUsers;
                                int currentPeripheralUsersCount = ActiveUsersValidator.NumberOfActivePeripheralUsers;
                                int currentTabletSmartPhoneUsersCount = ActiveUsersValidator.NumberOfActiveTabletSmartPhoneUsers;
                                string machineID = MvcApplication.GetMachineID();
                                Guid assemblyID = MvcApplication.GetAssemblyID();
                                Version currentVersion = MvcApplication.GetVersion();

                                lock (LICENSE_FILE) // lock for threads trying to access LICENSE_FILE simultaneously                                           
                                {
                                    GetLicenseStatusResult result = MvcApplication.LicenseManager.GetLicenseStatus(currentUsersCount,
                                                                                                               currentPeripheralUsersCount,
                                                                                                               currentTabletSmartPhoneUsersCount,
                                                                                                               machineID,
                                                                                                               assemblyID,
                                                                                                               currentVersion
                                                                                                               );
                                    MvcApplication.LicenseStatus = result.LicenseStatus;
                                }
                            }

                            MvcApplication.LastLicenseCheck = DateTime.Now;
                        }
                        catch (Exception ex)
                        {
                            WRMLogModule.Log(ex, "Error updating License Status", KernelLogLevel.Error);
                        }
                    }
                }
                else
                {
                    MvcApplication.LicenseStatus = LicenseStatusViewModel.LicenseStatusComputed;
                }

                if (MvcApplication.LicenseStatus == ITS.Licensing.Enumerations.LicenseStatus.Invalid)
                {
                    if (!RequestIsAllowed())
                    {
                        HttpContext.Current.Response.StatusCode = 402;
                        HttpApplication httpApplication = sender as HttpApplication;
                        httpApplication.CompleteRequest();
                    }
                }
            }
        }

        private bool RequestIsAllowed()
        {
            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            if (routeData != null)
            {
                if (MvcApplication.ApplicationInstance != eApplicationInstance.STORE_CONTROLER)
                {
                    return true;
                }

            }
            else
            {
                //Physical File
                if (File.Exists(this.Request.PhysicalPath)
                    && this.Request.PhysicalPath.EndsWith("zip", StringComparison.InvariantCultureIgnoreCase) == false
                    && this.Request.PhysicalPath.EndsWith("cab", StringComparison.InvariantCultureIgnoreCase) == false
                    && this.Request.PhysicalPath.EndsWith("svc", StringComparison.InvariantCultureIgnoreCase) == false
                    && this.Request.PhysicalPath.EndsWith("asmx", StringComparison.InvariantCultureIgnoreCase) == false
                    )
                {
                    return true;
                }
                else //Web service call
                {

                }
            }
            return false;
        }

        internal static void ValidateStoreControllerLicense()
        {
            CriteriaOperator licenseInfoCriteria = new BinaryOperator("Server", StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid);
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                LicenseUserDistribution licenseUserDistribution = uow.FindObject<LicenseUserDistribution>(licenseInfoCriteria);
                ServerLicenseInfo serverLicenseInfo = licenseUserDistribution.ServerLicenseInfo;
                serverLicenseInfo.EndDate = DateTime.Today;
                serverLicenseInfo.GreyZoneDays = 0;
                licenseUserDistribution.SetInfo(serverLicenseInfo);
                licenseUserDistribution.UpdatedOnTicks++;
                licenseUserDistribution.UpdatedOnTicks--;
                licenseUserDistribution.Save();
                XpoHelper.CommitChanges(uow);
            }
        }

        internal static void InvalidateStoreControllerLicense()
        {
            CriteriaOperator licenseInfoCriteria = new BinaryOperator("Server", StoreControllerAppiSettings.CurrentStore.StoreControllerSettings.Oid);
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                LicenseUserDistribution licenseUserDistribution = uow.FindObject<LicenseUserDistribution>(licenseInfoCriteria);
                ServerLicenseInfo serverLicenseInfo = licenseUserDistribution.ServerLicenseInfo;
                serverLicenseInfo.EndDate = DateTime.Today.AddDays(-1);
                serverLicenseInfo.GreyZoneDays = 0;
                licenseUserDistribution.SetInfo(serverLicenseInfo);
                licenseUserDistribution.UpdatedOnTicks++;
                licenseUserDistribution.UpdatedOnTicks--;
                licenseUserDistribution.Save();
                XpoHelper.CommitChanges(uow);
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            MiniProfiler.Stop();
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            DevExpressHelper.Theme = "ITSTheme1";
        }
        protected void ExecuteActionTypes(IEnumerable<ActionTypeEntity> actionTypeEntities, Guid objectOid, Type objectType)
        {
            new Task(() =>
            {
                Thread.Sleep(AppiSettings.ActionTypeTriggerSleepTime);
                try
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        BasicObj basicObject = (BasicObj)(uow.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, objectType, new BinaryOperator("Oid", objectOid)));

                        foreach (ActionTypeEntity actionTypeEntity in ((DocumentHeader)basicObject).DocumentType.ActionTypeEntities.Where(actTypeEnt => actTypeEnt.ActionType.UpdateMode == ActionTypeHelper.GetUpdaterMode(ApplicationInstance)))
                        {
                            MethodInfo methodInfo = objectType.GetMethod("ShouldExecuteActionTypes");
                            if (methodInfo != null && (bool)methodInfo.Invoke(null, new object[] { basicObject, actionTypeEntity }))
                            {
                                List<BasicObj> detailEntities = ActionTypeHelper.GetActivityObjects(basicObject, actionTypeEntity.ActionType.Category);
                                foreach (BasicObj detailObject in detailEntities)
                                {
                                    VariableValues variableValues = actionTypeEntity.Execute(detailObject, uow);
                                    IEnumerable<VariableActionType> updatingFieldVariables = actionTypeEntity.ActionType.VariableActionTypes.Where(varActType => !String.IsNullOrEmpty(varActType.Variable.TargetFieldName));

                                    if (updatingFieldVariables.Count() > 0)
                                    {
                                        foreach (VariableActionType updVariable in updatingFieldVariables)
                                        {
                                            ActionTypeHelper.VariableUpdateDataField(basicObject, updVariable, variableValues);
                                        }
                                    }
                                    variableValues.Save();
                                }
                            }
                        }
                        XpoHelper.CommitTransaction(uow);
                    }
                }
                catch (Exception exception)
                {
                    MvcApplication.WRMLogModule.Log(exception, kernelLogLevel: KernelLogLevel.Info);
                }
            }).Start();
        }





        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public static string ConnectionString
        {
            get { return "Data Source = " + HttpContext.Current.Server.MapPath("~/Log/TestMiniProfiler.sqlite"); }
        }

        public static int StoreControllerCommandInterval
        {
            get;
            private set;
        }

        /// <summary>
        /// Customize aspects of the MiniProfiler.
        /// </summary>
        private void InitProfilerSettings()
        {

            // a powerful feature of the MiniProfiler is the ability to share links to results with other developers.
            // by default, however, long-term result caching is done in HttpRuntime.Cache, which is very volatile.
            // 
            // let's rig up serialization of our profiler results to a database, so they survive app restarts.

            // Setting up a MultiStorage provider. This will store results in the HttpRuntimeCache (normally the default) and in SqlLite as well.
            SqliteMiniProfilerStorage sqlite = new SqliteMiniProfilerStorage(ConnectionString);
            MultiStorageProvider multiStorage = new MultiStorageProvider(
                new HttpRuntimeCacheStorage(new TimeSpan(1, 0, 0)),
                sqlite);

            if (File.Exists(ConnectionString) == false)
            {
                sqlite.RecreateDatabase("create table RouteHits(RouteName,HitCount,unique(RouteName))");

            }

            MiniProfiler.Settings.Storage = multiStorage;
            MiniProfiler.Settings.CustomUITemplates = "~/Views/Profiler";
            MiniProfiler.Settings.PopupRenderPosition = RenderPosition.BottomRight;
            MiniProfiler.Settings.ShowControls = true;



            // different RDBMS have different ways of declaring sql parameters - SQLite can understand inline sql parameters just fine
            // by default, sql parameters won't be displayed
            MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();

            // these settings are optional and all have defaults, any matching setting specified in .RenderIncludes() will
            // override the application-wide defaults specified here, for example if you had both:
            //    MiniProfiler.Settings.PopupRenderPosition = RenderPosition.Right;
            //    and in the page:
            //    @MiniProfiler.RenderIncludes(position: RenderPosition.Left)
            // then the position would be on the left that that page, and on the right (the app default) for anywhere that doesn't
            // specified position in the .RenderIncludes() call.
            //MiniProfiler.Settings.PopupRenderPosition = RenderPosition.Right; // defaults to left
            //MiniProfiler.Settings.PopupMaxTracesToShow = 50;                  // defaults to 15
            //MiniProfiler.Settings.RouteBasePath = "~/profiler";               // e.g. /profiler/mini-profiler-includes.js

            // MiniProfiler.Settings.ShowControls = true;
            MiniProfiler.Settings.StackMaxLength = 256;          // default is 120 characters
            MiniProfiler.Settings.PopupRenderPosition = RenderPosition.BottomRight;

            // because profiler results can contain sensitive data (e.g. sql queries with parameter values displayed), we
            // can define a function that will authorize clients to see the json or full page results.
            // we use it on http://stackoverflow.com to check that the request cookies belong to a valid developer.
            /*MiniProfiler.Settings.Results_Authorize = request =>
            {
                return false ;
            };

            // the list of all sessions in the store is restricted by default, you must return true to allow it
            MiniProfiler.Settings.Results_List_Authorize = request =>
            {
                // you may implement this if you need to restrict visibility of profiling lists on a per request basis 
                return false; // all requests are kosher
            };*/
        }

        /// <summary>
        /// Is true if and only if application instance allows users (with the right Role)
        /// to edit Prices
        /// </summary>
        public static bool ApplicationInstanceAllowsPriceEdit
        {
            get
            {
                return (ApplicationInstance == eApplicationInstance.RETAIL
                        || ApplicationInstance == eApplicationInstance.DUAL_MODE
                       )
                     || (ApplicationInstance == eApplicationInstance.STORE_CONTROLER
                         && Status == ApplicationStatus.ONLINE
                        );
            }
        }
    }
}
