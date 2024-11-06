using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.WebService;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Versions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITS.Retail.Platform.Enumerations;
using System.Collections.Concurrent;
using ITS.Retail.Platform;
using ITS.POS.Client.Synchronization;
using System.IO;
using System.Xml;
using NLog;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Contains all the backgrounds threads and logic for synchronization.
    /// </summary>
    public class SynchronizationManager : ISynchronizationManager
    {
        public List<Guid> DocumentSeriesList { get; set; }
        public CustomThread UpdateStatusThread { get; set; }
        public CustomThread AutoFocusThread { get; set; }
        public CustomThread GetUpdatesThread { get; set; }
        public CustomThread PublishStatusThread { get; set; }
        public CustomThread PostSynchronizationInfoThread { get; set; }
        public PostThread PostTransactionsThread { get; set; }
        private IConfigurationManager ConfigurationManager { get; set; }
        private IAppContext AppContext { get; set; }
        private Logger LogFile { get; set; }
        private ISessionManager SessionManager { get; set; }
        private IActionManager ActionManager { get; set; }
        public bool UpdateKeyMappings { get; set; }

        public SynchronizationManager(IConfigurationManager configurationManager, IAppContext appContext, ISessionManager sessionManager, IActionManager actionManager, Logger logger, string entityUpdaterModesXmlPath)
        {
            ConfigurationManager = configurationManager;
            AppContext = appContext;
            LogFile = logger;
            SessionManager = sessionManager;
            ActionManager = actionManager;
            PublishStatusThread = new CustomThread(PublishStatusRunnable, ThreadType.PUBLISH_STATUS);

            DocumentSeriesList = new List<Guid>() { ConfigurationManager.DefaultDocumentSeriesOid,
                                                    ConfigurationManager.ProFormaInvoiceDocumentSeriesOid,
                                                    ConfigurationManager.WithdrawalDocumentSeriesOid,
                                                    ConfigurationManager.DepositDocumentSeriesOid,
                                                    ConfigurationManager.SpecialProformaDocumentSeriesOid

            };

            CriteriaOperator documentSeriesCriteria = CriteriaOperator.And(new BinaryOperator("eModule", eModule.POS), new NotOperator(new InOperator("Oid", DocumentSeriesList)));
            XPCollection<DocumentSeries> additionalDocumentSeries = new XPCollection<DocumentSeries>(sessionManager.GetSession<DocumentSeries>(), documentSeriesCriteria);
            DocumentSeriesList.AddRange(additionalDocumentSeries.Select(addSeries => addSeries.Oid));



            AutoFocusThread = new CustomThread(AutoFocusRunnable, ThreadType.AUTO_FOCUS);
            UpdateStatusThread = new CustomThread(UpdateStatusRunnable, ThreadType.UPDATE_STATUS);
            GetUpdatesThread = new CustomThread(GetUpdatesRunnable, ThreadType.GET_UPDATES);
            PostTransactionsThread = new PostThread(PostTransactionsRunnable, ThreadType.POST_TRANSACTIONS);
            PostSynchronizationInfoThread = new CustomThread(PostSynchronizationInfoRunnable, ThreadType.POST_VERSIONS);

            LoadEntityModes(entityUpdaterModesXmlPath);


        }

        public List<EntityMode> EntityModes { get; set; }

        private void LoadEntityModes(string entityUpdaterModesXmlPath)
        {
            EntityModes = new List<EntityMode>();
            if (File.Exists(entityUpdaterModesXmlPath))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(entityUpdaterModesXmlPath);
                XmlNodeList entityModes = xml.GetElementsByTagName("UpdaterMode");
                foreach (XmlNode entityMode in entityModes)
                {
                    if (entityMode["Entity"] != null && string.IsNullOrWhiteSpace(entityMode["Entity"].InnerText) == false)
                    {
                        if (entityMode["Mode"] != null && string.IsNullOrWhiteSpace(entityMode["Mode"].InnerText) == false)
                        {
                            try
                            {
                                Type entityType = UtilsHelper.GetEntityByName(entityMode["Entity"].InnerText.Trim());
                                eUpdaterMode mode = (eUpdaterMode)Enum.Parse(typeof(eUpdaterMode), entityMode["Mode"].InnerText.Trim());
                                EntityModes.Add(new EntityMode() { EntityType = entityType, Mode = mode });
                            }
                            catch //(Exception ex)
                            {
                            }
                        }
                    }
                }
            }
        }

        public readonly int WaitTimeAfterEachUpdate = 15000; //Wait time in milisecs before checking for updates again
        public readonly int WaitTimeAfterEachPost = 5000; //Wait time in milisecs before posting transactions again
        public readonly int WaitTimeAfterEachStatusPost = 40000; //Wait time in milisecs before posting status again
        public readonly int WaitTimeAfterEachUpdateStatus = 1000; //Wait time in milisecs before updating status again

        public readonly int LowEndModeWaitTimeAfterEachUpdate = 30000; //Wait time in milisecs before checking for updates again
        public readonly int LowEndModeWaitTimeAfterEachPost = 10000; //Wait time in milisecs before posting transactions again
        public readonly int LowEndModeWaitTimeAfterEachStatusPost = 60000; //Wait time in milisecs before posting status again
        public readonly int LowEndModeWaitTimeAfterEachUpdateStatus = 2000; //Wait time in milisecs before updating status again

        public readonly int WaitTimeAfterEachAutoFocus = 10000; //Wait time in milisecs before atempting to auto focus again
        public readonly int WaitTimeAfterEachPostSynchronizationInfo = 10000; //Wait time in milisecs before atempting to post versions again
        public readonly int InterruptTime = 900;            //Interval for thread sleep
        public readonly int WebServiceTimeout = 1000000;

        private volatile bool _serviceIsAlive;
        public bool ServiceIsAlive
        {
            get
            {
                return _serviceIsAlive;
            }
            set
            {
                _serviceIsAlive = value;
            }
        }

        private Type _downloadingType;
        public Type CurrentDownloadingType
        {
            get
            {
                return _downloadingType;
            }
        }

        private Type _uploadingType;
        public Type CurrentUploadingType
        {
            get
            {
                return _uploadingType;
            }
        }

        private eDownloadingStatus _downloadingStatus;
        public eDownloadingStatus CurrentDownloadingStatus
        {
            get
            {
                return _downloadingStatus;
            }
        }

        private eUploadingStatus _uploadingStatus;
        public eUploadingStatus CurrentUploadingStatus
        {
            get
            {
                return _uploadingStatus;
            }
        }

        private int _downloadingProgress;
        public int CurrentDownloadingProgress
        {
            get
            {
                return _downloadingProgress;
            }
        }

        private int _uploadingProgress;
        public int CurrentUploadingProgress
        {
            get
            {
                return _uploadingProgress;
            }
        }

        private ConcurrentDictionary<Type, bool> _entitiesForceDownloadFlags;
        public ConcurrentDictionary<Type, bool> EntitiesToForceDownload
        {
            get
            {
                return _entitiesForceDownloadFlags;
            }

        }

        private eUpdaterMode GetEntityUpdaterMode(Type type)
        {
            eUpdaterMode retValue = eUpdaterMode.AUTOMATIC;
            EntityMode entityMode = EntityModes.Where(x => x.EntityType == type).FirstOrDefault();
            if (entityMode != null)
            {
                retValue = entityMode.Mode;
            }

            return retValue;
        }



        public void Initialize()
        {
            List<Type> allEntities = new List<Type>();
            allEntities.AddRange(typeof(Item).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(BaseObj)) && x.GetCustomAttributes(typeof(NonPersistentAttribute), false).FirstOrDefault() == null));
            allEntities.AddRange(typeof(DocumentType).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(BaseObj)) && x.GetCustomAttributes(typeof(NonPersistentAttribute), false).FirstOrDefault() == null));
            _entitiesForceDownloadFlags = new ConcurrentDictionary<Type, bool>();
            foreach (Type entity in allEntities)
            {
                _entitiesForceDownloadFlags.AddOrUpdate(entity, false, (key, oldValue) => false);
            }
        }


        private void PublishStatusRunnable()
        {
            LocaleHelper.SetLocale(LocaleHelper.GetLanguageCode(ConfigurationManager.Locale), ConfigurationManager.CurrencySymbol, ConfigurationManager.CurrencyPattern);
            while (true)
            {
                try
                {
                    if (PublishStatusThread.IsAborted)
                    {
                        return;
                    }
                    ActionManager.GetAction(eActions.POST_STATUS).Execute();
                    //Moved to its own dedicated thread
                    //GlobalContext.GetAction(eActions.UPDATE_COMMUNICATION_STATUS).Execute(new ActionUpdateCommunicationStatusParams(ServiceIsAlive, DownloadingStatus, DownloadingType, UploadingStatus, UploadingType, DownloadingProgress, UploadingProgress, GlobalContext.GetMachineStatus()));
                    int waitTime = ConfigurationManager.EnableLowEndMode ? LowEndModeWaitTimeAfterEachStatusPost : WaitTimeAfterEachStatusPost;
                    PublishStatusThread.Sleep(waitTime);
                }
                catch (Exception ex)
                {
                    LogFile.Trace(ex, "PublishStatus Thread:Exception catched");
                }
            }
        }

        private void UpdateStatusRunnable()
        {
            LocaleHelper.SetLocale(LocaleHelper.GetLanguageCode(ConfigurationManager.Locale), ConfigurationManager.CurrencySymbol, ConfigurationManager.CurrencyPattern);
            while (true)
            {
                try
                {
                    if (UpdateStatusThread.IsAborted)
                    {
                        return;
                    }
                    ActionManager.GetAction(eActions.UPDATE_COMMUNICATION_STATUS).Execute(new ActionUpdateCommunicationStatusParams(ServiceIsAlive, CurrentDownloadingStatus, CurrentDownloadingType, CurrentUploadingStatus, CurrentUploadingType, CurrentDownloadingProgress, CurrentUploadingProgress, AppContext.GetMachineStatus()));
                    int waitTime = ConfigurationManager.EnableLowEndMode ? LowEndModeWaitTimeAfterEachUpdateStatus : WaitTimeAfterEachUpdateStatus;
                    UpdateStatusThread.Sleep(waitTime);
                }
                catch (Exception ex)
                {
                    LogFile.Trace(ex, "UpdateStatus Thread:Exception catched");
                }
            }
        }

        private void AutoFocusRunnable()
        {
            while (true)
            {
                try
                {
                    if (AutoFocusThread.IsAborted)
                    {
                        return;
                    }

                    Program.BringApplicationToFront();

                    AutoFocusThread.Sleep(WaitTimeAfterEachAutoFocus);
                }
                catch (Exception ex)
                {
                    LogFile.Trace(ex, "AutoFocus Thread: Exception catched");
                }
            }
        }

        private List<XPClassInfo> AllEntities = null;

        private void PostSynchronizationInfoRunnable()
        {
            while (true)
            {
                try
                {
                    if (PostSynchronizationInfoThread.IsAborted)
                    {
                        return;
                    }

                    if (AllEntities == null)
                    {
                        XPDictionary dcSettings = new ReflectionDictionary();
                        XPDictionary dcMaster = new ReflectionDictionary();
                        XPDictionary dcTransactions = new ReflectionDictionary();
                        dcSettings.GetDataStoreSchema(typeof(VatCategory).Assembly);
                        dcMaster.GetDataStoreSchema(typeof(Item).Assembly);
                        dcTransactions.GetDataStoreSchema(typeof(DocumentHeader).Assembly);

                        List<XPClassInfo> allClassesUnfiltered = new List<XPClassInfo>();
                        allClassesUnfiltered.AddRange(dcMaster.Classes.Cast<XPClassInfo>().Where(entity => entity.ClassType.Assembly == typeof(Item).Assembly));
                        allClassesUnfiltered.AddRange(dcSettings.Classes.Cast<XPClassInfo>().Where(entity => entity.ClassType.Assembly == typeof(VatCategory).Assembly));
                        allClassesUnfiltered.AddRange(dcTransactions.Classes.Cast<XPClassInfo>().Where(entity => entity.ClassType.Assembly == typeof(DocumentHeader).Assembly));

                        AllEntities = allClassesUnfiltered.Where(entity => entity.TableName != null &&
                                                                           entity.ClassType.GetCustomAttributes(typeof(SyncInfoIgnoreAttribute), false).Count() == 0).ToList();
                    }

                    using (UnitOfWork posUow = SessionManager.CreateSession(typeof(Model.Settings.POS)))
                    {

                        Model.Settings.POS currentTerminal = posUow.GetObjectByKey<Model.Settings.POS>(ConfigurationManager.CurrentTerminalOid);
                        string posName = currentTerminal == null ? ("POS " + ConfigurationManager.TerminalID) : currentTerminal.Name;
                        foreach (XPClassInfo entity in AllEntities)
                        {
                            string className = entity.ClassType.Name;
                            if (PostSynchronizationInfoThread.IsAborted)
                            {
                                return;
                            }

                            using (UnitOfWork uow = SessionManager.CreateSession(entity.ClassType))
                            using (UnitOfWork versionsUow = SessionManager.CreateSession(typeof(TableVersions)))
                            {
                                long databaseCount = (int)uow.Evaluate(entity.ClassType, CriteriaOperator.Parse("Count"), new BinaryOperator("IsActive", true));
                                long version = 0;
                                WebService.eSyncInfoEntityDirection direction;
                                if (entity.ClassType.Assembly == typeof(DocumentHeader).Assembly ||
                                   entity.ClassType == typeof(DocumentSequence) ||
                                   entity.ClassType == typeof(ZReportSequence))
                                {
                                    string tmpVersion = (string)uow.Evaluate(entity.ClassType, CriteriaOperator.Parse("Max(UpdatedOnTicksStr)"), null);

                                    ////Entity that goes upwards and is created at POS. Get version directly from DB table
                                    /*objects = new XPCollection(uow, entity.ClassType, null);
                                    objects.Sorting = new SortingCollection(new SortProperty("UpdatedOnTicksStr", DevExpress.Xpo.DB.SortingDirection.Descending));
                                    objects.TopReturnedObjects = 1;

                                    BaseObj newestObj = objects.Cast<BaseObj>().FirstOrDefault();//objects.Count >= 1 ? objects[0] as BaseObj : null;
                                    version = newestObj == null ? 0 : newestObj.UpdatedOnTicks;*/
                                    if (long.TryParse(tmpVersion, out version) == false)
                                    {
                                        version = 0;
                                    }

                                    direction = WebService.eSyncInfoEntityDirection.UP;
                                }
                                else
                                {
                                    DateTime versionFromVersionsTable = GetVersion(entity.ClassType, false, versionsUow);
                                    version = versionFromVersionsTable == Convert.ToDateTime("1/1/1900") ? 0 : versionFromVersionsTable.Ticks;
                                    direction = WebService.eSyncInfoEntityDirection.DOWN;
                                }
                                string tmpMinVersion = (string)uow.Evaluate(entity.ClassType, CriteriaOperator.Parse("Min(UpdatedOnTicksStr)"), null);
                                /*objects = new XPCollection(uow, entity.ClassType, null);
                                objects.Sorting = new SortingCollection(new SortProperty("UpdatedOnTicksStr", DevExpress.Xpo.DB.SortingDirection.Ascending));
                                objects.TopReturnedObjects = 1;
                                BaseObj oldestObj = objects.Cast<BaseObj>().FirstOrDefault();//objects.Count >= 1 ? objects[0] as BaseObj: null;*/
                                long minVersion;// = oldestObj == null ? 0 : oldestObj.UpdatedOnTicks;
                                if (long.TryParse(tmpMinVersion, out minVersion) == false)
                                {
                                    minVersion = 0;
                                }


                                using (POSUpdateService webService = new POSUpdateService())
                                {
                                    webService.Timeout = this.WebServiceTimeout;
                                    webService.Url = ConfigurationManager.StoreControllerWebServiceURL;
                                    string message;
                                    long serverVersion, serverCount;
                                    if (webService.PostSynchronizationInfo(ConfigurationManager.CurrentTerminalOid, posName, databaseCount, version, entity.ClassType.Name, direction, minVersion, out serverVersion, out serverCount, out message) == false)
                                    {
                                        LogFile.Error("PostVersions Thread: Server error posting pending changes info for \"" + entity.ClassType.Name + "\". Message: " + message);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogFile.Error(ex, "PostVersions Thread: Exception");
                }
                finally
                {
                    PostSynchronizationInfoThread.Sleep(WaitTimeAfterEachPostSynchronizationInfo);
                }
            }
        }

        public void GetDocumentSequences(IDocumentService documentService)
        {
            using (POSUpdateService webService = new POSUpdateService())
            {
                webService.Timeout = this.WebServiceTimeout;
                webService.Url = ConfigurationManager.StoreControllerWebServiceURL;


                if (ConfigurationManager.DocumentReports.Count > 0)
                {
                    try
                    {
                        CriteriaOperator crit = CriteriaOperator.And(new BinaryOperator("DocumentType", ConfigurationManager.DocumentReports.Where(x => x.CustomReportOid != null).FirstOrDefault().DocumentTypeOid));
                        StoreDocumentSeriesType sdst = SessionManager.GetSession<StoreDocumentSeriesType>().FindObject<StoreDocumentSeriesType>(crit);
                        if (sdst != null)
                        {
                            Guid CustomStoreDocumentSeriesGuid = sdst.DocumentSeries;
                            CriteriaOperator seriescrit = CriteriaOperator.And(new BinaryOperator("Oid", CustomStoreDocumentSeriesGuid), new BinaryOperator("IsCancelingSeries", false));
                            DocumentSeries ds = SessionManager.GetSession<DocumentSeries>().FindObject<DocumentSeries>(seriescrit);
                            if (ds != null)
                            {
                                Guid CustomDocumentSeriesGuid = ds.Oid;
                                DocumentSeriesList.Add(CustomDocumentSeriesGuid);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        string error = ex.GetFullMessage();
                    }

                }




                foreach (Guid documentSeries in DocumentSeriesList)
                {
                    try
                    {
                        if (documentService.SequenceExists(documentSeries))
                        {
                            continue;
                        }

                        string sequenceJson = null;
                        try
                        {
                            sequenceJson = webService.GetDocumentSequence(documentSeries);
                        }
                        catch (Exception ex)
                        {
                            LogFile.Info(ex, "Synchronizer.GetDocumentSequences() Web Service Error");
                            break;
                        }

                        JObject jsonItem = JObject.Parse(sequenceJson);
                        string key = jsonItem.Property("Oid").Value.ToString();
                        DocumentSequence sequence = SessionManager.GetObjectByKey<DocumentSequence>(Guid.Parse(key));

                        int posSequence = 0;
                        if (sequence != null)
                        {
                            posSequence = sequence.DocumentNumber;
                        }
                        int scSequence = int.Parse(jsonItem.Property("DocumentNumber").Value.ToString());
                        if (scSequence > posSequence ||
                            (posSequence == 0 && scSequence == 0))
                        {
                            if (sequence == null)
                            {
                                sequence = new DocumentSequence(SessionManager.GetSession<DocumentSequence>());
                            }
                            sequence.FromJson(sequenceJson, PlatformConstants.JSON_SERIALIZER_SETTINGS, true);
                            sequence.CreatedByDevice = ConfigurationManager.CurrentTerminalOid.ToString();
                            sequence.Save();
                        }

                        SessionManager.GetSession<DocumentSequence>().CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        LogFile.Info(ex, "Synchronizer.GetDocumentSequences() Other Error");
                    }
                }

            }
        }

        public void GetZReportSequences(IDocumentService documentService)
        {
            using (POSUpdateService webService = new POSUpdateService())
            {
                webService.Timeout = this.WebServiceTimeout;
                webService.Url = ConfigurationManager.StoreControllerWebServiceURL;
                List<Guid> posList = new List<Guid>() { ConfigurationManager.CurrentTerminalOid };

                foreach (Guid pos in posList)
                {
                    try
                    {
                        if (documentService.ZSequenceExists(pos)) continue;

                        string sequenceJson = null;
                        try
                        {
                            sequenceJson = webService.GetZReportSequence(pos);
                        }
                        catch (Exception ex)
                        {
                            LogFile.Info(ex, "Synchronizer.GetZReportSequences() Web Service Error");
                            break;
                        }

                        JObject jsonItem = JObject.Parse(sequenceJson);
                        string key = jsonItem.Property("Oid").Value.ToString();
                        ZReportSequence sequence = SessionManager.GetObjectByKey<ZReportSequence>(Guid.Parse(key));
                        int posSequence = 0;
                        if (sequence != null)
                        {
                            posSequence = sequence.ZReportNumber;
                        }
                        int scSequence = int.Parse(jsonItem.Property("ZReportNumber").Value.ToString());
                        if (scSequence > posSequence ||
                            (posSequence == 0 && scSequence == 0))
                        {
                            if (sequence == null)
                            {
                                sequence = new ZReportSequence(SessionManager.GetSession<ZReportSequence>());
                            }
                            sequence.FromJson(sequenceJson, PlatformConstants.JSON_SERIALIZER_SETTINGS, true);
                            sequence.CreatedByDevice = ConfigurationManager.CurrentTerminalOid.ToString();
                            sequence.Save();
                        }
                        SessionManager.GetSession<ZReportSequence>().CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        LogFile.Info(ex, "Synchronizer.GetDocumentSequences() Other Error");
                    }
                }

            }
        }

        public void CheckZReportSequenceNumber(Guid terminalOid, out int localNumber, out int remoteNumber)
        {
            localNumber = 0;
            remoteNumber = 0;
            using (POSUpdateService webService = new POSUpdateService())
            {
                webService.Timeout = this.WebServiceTimeout;
                webService.Url = ConfigurationManager.StoreControllerWebServiceURL;
                try
                {
                    string sequenceJson = null;
                    try
                    {
                        sequenceJson = webService.GetZReportSequence(terminalOid);
                    }
                    catch (Exception ex)
                    {
                        LogFile.Info(ex, "Synchronizer.CheckDocumentSequence() Web Service Error");
                        return;
                    }

                    JObject jsonItem = JObject.Parse(sequenceJson);
                    string key = jsonItem.Property("Oid").Value.ToString();
                    ZReportSequence currentObject = SessionManager.GetObjectByKey<ZReportSequence>(Guid.Parse(key));
                    if (currentObject == null)
                    {
                        LogFile.Info("Synchronizer.CheckDocumentSequence(): ZReportSequence for POS '" + terminalOid + "' does not exist");
                        return;
                    }

                    remoteNumber = int.Parse(jsonItem.Property("ZReportNumber").Value.ToString());
                    localNumber = currentObject.ZReportNumber;
                }
                catch (Exception ex)
                {
                    LogFile.Info(ex, "Synchronizer.CheckDocumentSequence() Other Error");
                    throw;
                }
            }
        }

        public void CheckDocumentSequenceNumber(Guid documentSeries, out int localNumber, out int remoteNumber)
        {
            localNumber = 0;
            remoteNumber = 0;
            using (POSUpdateService webService = new POSUpdateService())
            {
                webService.Timeout = this.WebServiceTimeout;
                webService.Url = ConfigurationManager.StoreControllerWebServiceURL;
                try
                {
                    string sequenceJson = null;
                    try
                    {
                        sequenceJson = webService.GetDocumentSequence(documentSeries);
                    }
                    catch (Exception ex)
                    {
                        LogFile.Info(ex, "Synchronizer.CheckDocumentSequence() Web Service Error");
                        return;
                    }

                    JObject jsonItem = JObject.Parse(sequenceJson);
                    string key = jsonItem.Property("Oid").Value.ToString();
                    DocumentSequence currentObject = SessionManager.GetObjectByKey<DocumentSequence>(Guid.Parse(key));
                    if (currentObject == null)
                    {
                        LogFile.Info("Synchronizer.CheckDocumentSequence(): DocumentSequence for DocumentSeries '" + documentSeries + "' does not exist");
                        return;
                    }

                    remoteNumber = int.Parse(jsonItem.Property("DocumentNumber").Value.ToString());
                    localNumber = currentObject.DocumentNumber;
                }
                catch (Exception ex)
                {
                    LogFile.Info(ex, "Synchronizer.CheckDocumentSequence() Other Error");
                    throw;
                }
            }
        }

        /// <summary>
        /// Pauses the thread as long as required
        /// </summary>
        private void WaitIfRequired(CustomThread thread)
        {
            switch (thread.Type)
            {
                case ThreadType.GET_UPDATES:
                    while (InterruptUpdates())
                    {
                        _downloadingStatus = eDownloadingStatus.PAUSED;
                        thread.Sleep(this.InterruptTime);
                    }
                    break;
                case ThreadType.POST_TRANSACTIONS:
                    while (InterruptTransactionsPost())
                    {
                        _uploadingStatus = eUploadingStatus.PAUSED;
                        thread.Sleep(this.InterruptTime);
                    }
                    break;
            }
        }

        /// <summary>
        /// Checks if the PostTransactions thread should stop working.
        /// </summary>
        /// <returns></returns>
        private bool InterruptTransactionsPost()
        {
            if (PostTransactionsThread.IsAborted)
            {
                return false;
            }

            if (PausedPostThread || AppContext.GetMachineStatus() == ITS.Retail.Platform.Enumerations.eMachineStatus.OPENDOCUMENT
              || AppContext.GetMachineStatus() == ITS.Retail.Platform.Enumerations.eMachineStatus.OPENDOCUMENT_PAYMENT)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the Updates thread should stop working.
        /// </summary>
        /// <returns></returns>
        private bool InterruptUpdates()
        {
            if (GetUpdatesThread.IsAborted)
            {
                return false;
            }

            if (AppContext.GetMachineStatus() == ITS.Retail.Platform.Enumerations.eMachineStatus.OPENDOCUMENT
              || AppContext.GetMachineStatus() == ITS.Retail.Platform.Enumerations.eMachineStatus.OPENDOCUMENT_PAYMENT
              || AppContext.DocumentsOnHold.Count > 0
              )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool PausedPostThread = false;

        public bool PostTransactionsAndPauseThread(int retryNo)
        {
            try
            {
                long startTime = DateTime.Now.Ticks;
                PostTransactionsThread.CompletedForcePost = PostTransactionsThread.StartedForcePost = false;
                PostTransactionsThread.ForceUpdate = true;
                long[] steps = 
#if DEBUG
                { 10*TimeSpan.TicksPerSecond, 20* TimeSpan.TicksPerSecond, 60*TimeSpan.TicksPerSecond };
#else
                { TimeSpan.TicksPerMinute, 5 * TimeSpan.TicksPerMinute, 30 * TimeSpan.TicksPerMinute };
#endif
                long stopTime = 0;// = DateTime.Now.Ticks + 30 * TimeSpan.TicksPerSecond;
                for (int i = 0; i < steps.Length && PostTransactionsThread.CompletedForcePost == false; i++)
                {
                    stopTime = DateTime.Now.Ticks + retryNo * steps[i];
                    do
                    {
                    } while (PostTransactionsThread.CompletedForcePost == false && DateTime.Now.Ticks < stopTime);
                }

                if (DateTime.Now.Ticks >= stopTime)
                {
                    LogFile.Error(string.Format("Synchronizer:PostTransactionsAndPauseThread,Post Transactions Thread timeout ({0} secs) wait reached. Transactions File will not be dettached",
                      (DateTime.Now.Ticks - startTime) / TimeSpan.TicksPerSecond));
                }

                PostTransactionsThread.ForceUpdate = false;
                PostTransactionsThread.Abort();
                PostTransactionsThread.WaitToEnd();

                return PostTransactionsThread.CompletedForcePost;
            }
            catch (System.Exception ex)
            {
                LogFile.Info(ex, "Synchronizer:PostTransactionsAndPauseThread,Exception catched");
                return false;
            }
        }

        public void ResumePostTransactionThread()
        {
            PostTransactionsThread = new PostThread(PostTransactionsRunnable, ThreadType.POST_TRANSACTIONS);
            PostTransactionsThread.Start();
        }

        /// <summary>
        /// Main function of the PostTransactionsThread
        /// </summary>
        private void PostTransactionsRunnable()
        {

            LocaleHelper.SetLocale(LocaleHelper.GetLanguageCode(ConfigurationManager.Locale), ConfigurationManager.CurrencySymbol, ConfigurationManager.CurrencyPattern);
            while (true)
            {

                if (PostTransactionsThread.ForceUpdate && !PostTransactionsThread.StartedForcePost)
                {
                    PostTransactionsThread.StartedForcePost = true;
                    PostTransactionsThread.CompletedForcePost = false;
                }
                WaitIfRequired(PostTransactionsThread); //Makes the thread wait if required
                if (PostTransactionsThread.IsAborted)//Aborts the thread if marked for abortion
                {
                    return;
                }


                //UtilsHelper.InitVersionsDB();
                XPDictionary dcTransactions = new ReflectionDictionary();

                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

                dcTransactions.GetDataStoreSchema(typeof(DocumentHeader).Assembly);

                List<Type> transactionTypesSorted = new List<Type>();
                transactionTypesSorted.Add(typeof(DailyTotals));
                transactionTypesSorted.Add(typeof(DocumentHeader));
                transactionTypesSorted.Add(typeof(ElectronicJournalFilePackage));
                transactionTypesSorted.Add(typeof(EdpsBatchTotal));
                transactionTypesSorted.Add(typeof(UserDailyTotalsCashCountDetail));

                List<Type> settingsTypesSorted = new List<Type>();
                settingsTypesSorted.Add(typeof(DocumentSequence));
                settingsTypesSorted.Add(typeof(ZReportSequence));


                //foreach (XPClassInfo itm in dcTransactions.Classes)
                foreach (Type type in transactionTypesSorted)
                {

                    using (UnitOfWork uow = SessionManager.CreateSession(type))
                    {

                        WaitIfRequired(PostTransactionsThread); //Makes the thread wait if required
                        if (PostTransactionsThread.IsAborted) //Aborts the thread if marked for abortion
                        {
                            return;
                        }

                        //if (itm.TableName == null) continue;
                        //if (itm.ClassType.IsAbstract) continue;
                        MethodInfo callMethod = typeof(SynchronizationManager).GetMethod("PostUpdates", flags);
                        if (callMethod == null)
                        {
                            continue;
                        }
                        Type[] args = new Type[] { type };
                        MethodInfo generic = callMethod.MakeGenericMethod(args);
                        bool success = (bool)generic.Invoke(this, new object[] { uow });
                        if (!success)
                        {
                            PostTransactionsThread.StartedForcePost = false;
                            break;
                        }
                    }
                }

                foreach (Type type in settingsTypesSorted)
                {

                    using (UnitOfWork uow = SessionManager.CreateSession(type))
                    {
                        WaitIfRequired(PostTransactionsThread); //Makes the thread wait if required
                        if (PostTransactionsThread.IsAborted) //Aborts the thread if marked for abortion
                        {
                            return;
                        }
                        //if (itm.TableName == null) continue;
                        //if (itm.ClassType.IsAbstract) continue;
                        MethodInfo callMethod = typeof(SynchronizationManager).GetMethod("PostUpdates", flags);
                        if (callMethod == null)
                        {
                            continue;
                        }
                        Type[] args = new Type[] { type };
                        MethodInfo generic = callMethod.MakeGenericMethod(args);
                        bool success = (bool)generic.Invoke(this, new object[] { uow });
                        if (!success)
                        {
                            PostTransactionsThread.StartedForcePost = false;
                            break;
                        }
                    }
                }

                _uploadingStatus = eUploadingStatus.IDLE;

                int waitTime = ConfigurationManager.EnableLowEndMode ? LowEndModeWaitTimeAfterEachPost : WaitTimeAfterEachPost;
                PostTransactionsThread.Sleep(waitTime);
                if (PostTransactionsThread.StartedForcePost)
                {
                    PostTransactionsThread.CompletedForcePost = true;
                }

            }
        }

        private bool PostUpdates<T>(UnitOfWork session) where T : BaseObj
        {
            _uploadingStatus = eUploadingStatus.CHECKING;
            _uploadingType = typeof(T);

            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentCulture = PlatformConstants.DefaultCulture;

            try
            {
                using (POSUpdateService webService = new POSUpdateService())
                {
                    webService.Timeout = this.WebServiceTimeout;
                    webService.Url = ConfigurationManager.StoreControllerWebServiceURL;

                    WaitIfRequired(PostTransactionsThread); //Makes the thread wait if required
                    if (PostTransactionsThread.IsAborted) //Aborts the thread if marked for abortion
                    {
                        return false;
                    }
                    _uploadingStatus = eUploadingStatus.CHECKING;

                    long latestVersion = webService.GetVersion(typeof(T).Name, ConfigurationManager.CurrentTerminalOid.ToString());
                    //_serviceIsAlive = true;

                    XPCollection<T> transactions;
                    if (typeof(T) == typeof(DocumentHeader))
                    {
                        transactions = new XPCollection<T>(session, CriteriaOperator.And(new BinaryOperator("UpdatedOnTicksStr", latestVersion.ToString(), BinaryOperatorType.Greater), new BinaryOperator("IsOpen", false)));
                    }
                    else if (typeof(T) == typeof(DocumentDetail))
                    {
                        transactions = new XPCollection<T>(session, CriteriaOperator.And(new BinaryOperator("UpdatedOnTicksStr", latestVersion.ToString(), BinaryOperatorType.Greater), new BinaryOperator("DocumentHeader.IsOpen", false)));
                    }
                    else if (typeof(T) == typeof(DailyTotals) || typeof(T) == typeof(ZReportSequence))
                    {
                        transactions = new XPCollection<T>(session);
                    }
                    else
                    {
                        transactions = new XPCollection<T>(session, new BinaryOperator("UpdatedOnTicksStr", latestVersion.ToString(), BinaryOperatorType.Greater));
                    }

                    IEnumerable<string> list = transactions.Where(p => p.GetType() == typeof(T)).OrderBy(p => p.UpdatedOnTicks).Take(10000)
                        .Select(p => p.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS));
                    if (list.Count() > 0)
                    {
                        _uploadingProgress = 0;
                        _uploadingStatus = eUploadingStatus.UPLOADING;

                        WaitIfRequired(PostTransactionsThread);
                        if (PostTransactionsThread.IsAborted)
                        {
                            return false;
                        }

                        _uploadingProgress = 100;
                        string jsonObject = JsonConvert.SerializeObject(list, PlatformConstants.JSON_SERIALIZER_SETTINGS);
                        bool bb = webService.PostData(typeof(T).Name, UtilsHelper.CompressLZMA(jsonObject), ConfigurationManager.CurrentTerminalOid.ToString(), ITS.POS.Client.WebService.eIdentifier.POS);
                    }
                }
            }
            catch (Exception ex)
            {
                //_serviceIsAlive = false;
                LogFile.Info(ex, "Synchronizer:PostUpdates,Exception catched");
                return false;
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);

            return true;
        }

        /// <summary>
        /// Main function of the GetUpdatesThread
        /// </summary>
        private void GetUpdatesRunnable()
        {
            LocaleHelper.SetLocale(LocaleHelper.GetLanguageCode(ConfigurationManager.Locale), ConfigurationManager.CurrencySymbol, ConfigurationManager.CurrencyPattern);
            while (true)
            {
                WaitIfRequired(GetUpdatesThread); //Makes the thread wait if required
                if (GetUpdatesThread.IsAborted) //Aborts the thread if marked for abortion
                {
                    return;
                }
                bool updateKeyMappings = false;
                bool updateApplicationSettings = false;
                try
                {
                    XPDictionary dcSettings = new ReflectionDictionary();
                    XPDictionary dcMaster = new ReflectionDictionary();

                    BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
                    dcSettings.GetDataStoreSchema(typeof(VatCategory).Assembly);
                    dcMaster.GetDataStoreSchema(typeof(Item).Assembly);
                    foreach (XPClassInfo itm in dcSettings.Classes)
                    {
                        if (GetUpdatesThread.IsAborted)
                        {
                            break;
                        }
                        if (itm.TableName == null)
                        {
                            continue;
                        }
                        //if (MustSkipEntity(itm.ClassType))
                        //{
                        //    continue;
                        //}
                        if (itm.ClassType == typeof(DocumentSequence) || itm.ClassType == typeof(ZReportSequence))
                        {
                            continue;
                        }//special case

                        using (UnitOfWork uow = SessionManager.CreateSession(itm.ClassType))
                        {
                            WaitIfRequired(GetUpdatesThread); //Makes the thread wait if required
                            if (GetUpdatesThread.IsAborted) //Aborts the thread if marked for abortion
                            {
                                return;
                            }

                            MethodInfo callMethod = typeof(SynchronizationManager).GetMethod("GetUpdates", flags);
                            if (callMethod == null)
                            {
                                continue;
                            }
                            Type[] args = new Type[] { itm.ClassType };
                            MethodInfo generic = callMethod.MakeGenericMethod(args);
                            int result = (int)generic.Invoke(this, new object[] { uow });

                            if (itm.ClassType == typeof(POSKeyMapping) && result > 0)
                            {
                                updateKeyMappings = true;
                            }
                            if (itm.ClassType == typeof(OwnerApplicationSettings) && result > 0)
                            {
                                updateApplicationSettings = true;
                            }
                            if (result >= 0 && GetEntityUpdaterMode(itm.ClassType) == eUpdaterMode.MANUAL)
                            {
                                EntitiesToForceDownload[itm.ClassType] = false;
                            }
                        }
                    }
                    foreach (XPClassInfo itm in dcMaster.Classes)
                    {
                        if (GetUpdatesThread.IsAborted)
                        {
                            break;
                        }
                        if (itm.TableName == null)
                        {
                            continue;
                        }
                        if (!itm.ClassType.IsSubclassOf(typeof(BasicObj)))
                        {
                            continue;
                        }
                        //if (MustSkipEntity(itm.ClassType))
                        //{
                        //    continue;
                        //}

                        using (UnitOfWork uow = SessionManager.CreateSession(itm.ClassType))
                        {

                            WaitIfRequired(GetUpdatesThread); //Makes the thread wait if required
                            if (GetUpdatesThread.IsAborted) //Aborts the thread if marked for abortion
                            {
                                return;
                            }

                            MethodInfo callMethod = typeof(SynchronizationManager).GetMethod("GetUpdates", flags);
                            if (callMethod == null)
                            {
                                continue;
                            }
                            Type[] args = new Type[] { itm.ClassType };
                            MethodInfo generic = callMethod.MakeGenericMethod(args);
                            int result = (int)generic.Invoke(this, new object[] { uow });

                            if (result < 0) //error
                            {
                                break;
                            }
                            else if (GetEntityUpdaterMode(itm.ClassType) == eUpdaterMode.MANUAL)
                            {
                                EntitiesToForceDownload[itm.ClassType] = false;
                            }
                        }
                    }
                    _downloadingStatus = eDownloadingStatus.IDLE;
                    GetUpdatesThread.ForceUpdate = false;
                }
                catch (Exception ex)
                {
                    LogFile.Info(ex, "Synchronizer:GetUpdatesRunnable,Exception catched");
                }

                if (updateKeyMappings)
                {
                    this.UpdateKeyMappings = true;
                }

                if (updateApplicationSettings)
                {
                    ConfigurationManager.ReloadApplicationSettings();
                }

                int waitTime = ConfigurationManager.EnableLowEndMode ? LowEndModeWaitTimeAfterEachUpdate : WaitTimeAfterEachUpdate;
                GetUpdatesThread.Sleep(waitTime);
            }

        }

        private DateTime GetVersion(Type type, bool allRecords, UnitOfWork versionsUow)
        {
            if (allRecords)
            {
                return Convert.ToDateTime("1/1/1900");
            }
            TableVersions obj = versionsUow.FindObject<TableVersions>(new BinaryOperator("EntityName", type.ToString()));
            if (obj != null)
            {
                return obj.Number;
            }
            return Convert.ToDateTime("1/1/1900");
        }

        private DateTime GetVersion<T>(bool allRecords, UnitOfWork versionsUow)
        {
            return GetVersion(typeof(T), allRecords, versionsUow);
        }

        private void SetVersion<T>(DateTime ver, UnitOfWork versionsUow)
        {
            SetVersion(typeof(T), ver, versionsUow);
        }

        private bool GetForceReload<T>(UnitOfWork versionsUow)
        {
            TableVersions obj = versionsUow.FindObject<TableVersions>(new BinaryOperator("EntityName", typeof(T).ToString(), BinaryOperatorType.Equal));
            if (obj != null)
            {
                return obj.ForceReload;
            }
            return true;
        }

        private void SetForceReload<T>(bool reload, UnitOfWork versionsUow)
        {
            TableVersions obj = versionsUow.FindObject<TableVersions>(new BinaryOperator("EntityName", typeof(T).ToString(), BinaryOperatorType.Equal));
            if (obj != null)
            {
                obj.ForceReload = reload;
                obj.Save();
                versionsUow.CommitChanges();
            }
        }

        public void SetVersion(Type type, DateTime ver, UnitOfWork versionsUow)
        {
            TableVersions obj = versionsUow.FindObject<TableVersions>(new BinaryOperator("EntityName", type.ToString(), BinaryOperatorType.Equal));
            if (obj == null)
            {
                obj = new TableVersions(versionsUow) { EntityName = type.ToString() };
            }
            obj.Number = ver;
            if (ver == DateTime.MinValue)
            {
                obj.ForceReload = true;
            }
            obj.Save();
            versionsUow.CommitChanges();
        }

        private int GetUpdates<T>(UnitOfWork session) where T : BasicObj
        {
            if (ConfigurationManager.CurrentTerminalOid == null)
            {
                return -1;
            }

            try
            {
                _downloadingStatus = eDownloadingStatus.CHECKING;
                _downloadingType = typeof(T);

                string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
                Thread.CurrentThread.CurrentCulture = PlatformConstants.DefaultCulture;

                string sUpdates;
                JObject jsonItem;
                int lines = 0;
                int totalCount = -1;
                T currentItem;

                using (POSUpdateService webService = new POSUpdateService())
                {
                    webService.Timeout = this.WebServiceTimeout;
                    webService.Url = ConfigurationManager.StoreControllerWebServiceURL;

                    int lastObjects = 0;
                    string lastObjectString = string.Empty;
                    using (UnitOfWork versionsUow = VersionsConnectionHelper.GetNewUnitOfWork())
                    {
                        bool forceReload = GetForceReload<T>(versionsUow);
                        while (true)
                        {

                            DateTime verUpdate = GetVersion<T>(false, versionsUow);
                            try
                            {
                                WaitIfRequired(GetUpdatesThread); //Makes the thread wait if required
                                if (GetUpdatesThread.IsAborted) //Aborts the thread if marked for abortion
                                {
                                    return -1;
                                }
                                int remainingRows;
                                sUpdates = webService.GetChanges(typeof(T).Name, verUpdate, ConfigurationManager.CurrentTerminalOid.ToString(), WebService.eIdentifier.POS, out remainingRows);
                                if (totalCount < 0)
                                {
                                    totalCount = remainingRows; //initialize totalCount
                                }

                                if (sUpdates == null)
                                {
                                    break; //service is alive but an exception occured
                                }
                            }
                            catch (Exception ex)
                            {
                                LogFile.Debug(ex, ex.Message);
                                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                                return -1;
                            }
                            try
                            {
                                sUpdates = Convert.ToString(UtilsHelper.DecompressLZMA(sUpdates));
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Decompress Error. Response: " + sUpdates, ex);
                            }
                            List<string> items = JsonConvert.DeserializeObject<List<string>>(sUpdates);

                            if (items.Count == 0)
                            {
                                break;
                            }

                            if (items.Count < 10000 && items.Count == lastObjects && items[0] == lastObjectString)
                            {
                                break;
                            }

                            lastObjectString = items[0];
                            lastObjects = items.Count;
                            _downloadingProgress = 0;

                            foreach (string itm in items)
                            {
                                WaitIfRequired(GetUpdatesThread); //Makes the thread wait if required
                                if (GetUpdatesThread.IsAborted) //Aborts the thread if marked for abortion
                                {
                                    return -1;
                                }

                                lines++;
                                _downloadingStatus = eDownloadingStatus.DOWNLOADING;
                                if (totalCount > 0)
                                {
                                    _downloadingProgress = (int)(lines / (decimal)totalCount * 100);
                                }
                                else
                                {
                                    _downloadingProgress = 0;
                                }

                                jsonItem = JObject.Parse(itm);
                                // Find if object exists
                                string key = jsonItem.Property("Oid").Value.ToString();
                                try
                                {
                                    currentItem = session.GetObjectByKey<T>(new Guid(key));
                                    if (currentItem == null)
                                    {
                                        string gcRecord = jsonItem.Property("GCRecord").Value.ToString();
                                        if (gcRecord == "null")
                                        {
                                            currentItem = (T)Activator.CreateInstance(typeof(T), new object[] { session });
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }


                                    if (currentItem.UpdatedOnTicks.ToString() == jsonItem.Property("UpdatedOnTicks").Value.ToString() && forceReload == false)
                                    {
                                        verUpdate = currentItem.UpdatedOn;
                                        continue;
                                    }
                                    currentItem.FromJson(jsonItem, PlatformConstants.JSON_SERIALIZER_SETTINGS, true);
                                    GetUpdatesOnSaving(currentItem);
                                    currentItem.Save();
                                    verUpdate = currentItem.UpdatedOn;
                                    if (lines % 1000 == 0 || GetUpdatesThread.IsAborted)
                                    {
                                        session.CommitChanges();
                                        SetVersion<T>(verUpdate, versionsUow);
                                        if (GetUpdatesThread.IsAborted)
                                        {
                                            break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogFile.Info(e, "Synchronizer:GetUpdates<" + typeof(T).Name + ">, Exception catched: ");
                                    session.RollbackTransaction();
                                }
                            }
                            session.CommitChanges();
                            SetVersion<T>(verUpdate, versionsUow);
                            if (GetUpdatesThread.IsAborted)
                            {
                                break;
                            }
                        }
                        SetForceReload<T>(false, versionsUow);
                    }
                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                    return lines;
                }
            }
            catch (Exception ex)
            {
                LogFile.Info(ex, "Synchronizer:GetUpdates<" + typeof(T).Name + ">,Exception catched");
                return -1;
            }
        }

        private void GetUpdatesOnSaving<T>(T currentItem) where T : BasicObj
        {
            if (currentItem is Customer)
            {
                GetUpdatesOnSavingCustomer(currentItem as Customer);
            }
            if (currentItem is Address)
            {
                GetUpdatesOnSavingAddress(currentItem as Address);
            }
        }

        private void GetUpdatesOnSavingAddress(Address address)
        {
            if (address == null)
            {
                return;
            }

            CriteriaOperator documentAddressCriteria = CriteriaOperator.And(new BinaryOperator("DenormalisedAddress", address.Oid),
                                                                             new BinaryOperator("BillingAddress", address.Oid, BinaryOperatorType.NotEqual)
                                                                           );

            XPCollection<DocumentHeader> documentHeaders = new XPCollection<DocumentHeader>(SessionManager.GetSession(typeof(DocumentHeader)), documentAddressCriteria);
            foreach (DocumentHeader documentHeader in documentHeaders)
            {
                documentHeader.BillingAddress = address.Oid;
                documentHeader.Save();
            }
            documentHeaders.Session.CommitTransaction();
        }

        private void GetUpdatesOnSavingCustomer(Customer customer)
        {
            if (customer == null)
            {
                return;
            }
            using (UnitOfWork traderUow = SessionManager.CreateSession(typeof(Trader)))
            {
                Trader trader = traderUow.GetObjectByKey<Trader>(customer.Trader);
                if (trader == null)
                {
                    return;
                }
                CriteriaOperator customerDocumentCriteria = CriteriaOperator.And(new BinaryOperator("Customer", customer.Oid, BinaryOperatorType.NotEqual),
                                                                                 new BinaryOperator("CustomerLookUpTaxCode", trader.TaxCode)
                                                                                 );
                using (UnitOfWork documentsSession = SessionManager.CreateSession(typeof(DocumentHeader)))
                {
                    XPCollection<DocumentHeader> documentHeaders = new XPCollection<DocumentHeader>(documentsSession, customerDocumentCriteria);
                    if (documentHeaders.Count > 0)
                    {
                        foreach (DocumentHeader documentHeader in documentHeaders)
                        {
                            documentHeader.Customer = customer.Oid;
                            documentHeader.CustomerCode = customer.Code;
                            documentHeader.CustomerName = customer.CompanyName;
                            documentHeader.Save();
                        }
                        documentHeaders.Session.CommitTransaction();
                    }
                }
            }
        }
    }
}
