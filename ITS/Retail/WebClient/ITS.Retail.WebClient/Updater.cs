using System;
using System.Collections.Generic;
using System.Linq;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Xpo.Metadata;
using System.Reflection;
using DevExpress.Xpo;
using System.Threading;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform;
using ImageMagick;
using System.Drawing;
using DevExpress.Xpo.DB;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using System.IO;
using System.Web;
using ITS.Retail.WebClient.AuxillaryClasses;
using System.Diagnostics;

namespace ITS.Retail.WebClient
{
    public static class Updater
    {
        public static string ServerRoot;
        private static bool StopThreads = false;
        private static bool? _ThreadIsRunnable = null;
        public static bool ThreadIsRunnable
        {
            get
            {
                if (MvcApplication.LicenseStatus == ITS.Licensing.Enumerations.LicenseStatus.Invalid)
                {
                    return false;
                }
                if (_ThreadIsRunnable.HasValue == false)
                {
                    LoadOnlineStatusFromFile();
                }
                return _ThreadIsRunnable.Value;
            }
            set
            {
                _ThreadIsRunnable = value;
                SaveOnlineStatusFromFile();
            }
        }

        public static Thread StoreControllerCommandThread = new Thread(GetStoreControllerCommandsRunnable);
        public static Thread GetUpdatesThread = new Thread(StartUpdate);
        public static Thread PostRecordsThread = new Thread(PostRecordsRunnable);
        public static Thread PostSyncInfoThread = new Thread(PostSyncInfoThreadRunnable);
        private static readonly int UpdateWaitTime = 6000;
        private static readonly int SyncInfoWaitTime = 30000;
        private static string ONLINE_FILE_NAME = "StoreControllerStatus.json";
        public static Thread DataTransferVerificationThread = new Thread(DataTransferVerification);
        //public static Thread DataTransferToPOSVerificationThread = new Thread(DataTransferToPOSVerification);
        public static ThreadHandleEvent pauseEvent = new ThreadHandleEvent();

        private static void LoadOnlineStatusFromFile()
        {
            _ThreadIsRunnable = true;
            if (File.Exists(ServerRoot.TrimEnd('\\') + "\\Configuration\\" + ONLINE_FILE_NAME))
            {
                string jsonObject = File.ReadAllText(ServerRoot.TrimEnd('\\') + "\\Configuration\\" + ONLINE_FILE_NAME);
                JObject jobject = JObject.Parse(jsonObject);
                if (jobject.Property("Online") != null && jobject.Property("Online").Value.Value<bool>() == false)
                {
                    _ThreadIsRunnable = false;
                }
            }
            else
            {
                ThreadIsRunnable = true;
            }
        }

        private static void SaveOnlineStatusFromFile()
        {
            //throw new NotImplementedException();
            object toWrite = new { Online = ThreadIsRunnable };
            File.WriteAllText(ServerRoot.TrimEnd('\\') + "\\Configuration\\" + ONLINE_FILE_NAME, JsonConvert.SerializeObject(toWrite));
        }

        public static void StartUpdate()
        {
            while (true)
            {
                try
                {
                    pauseEvent.PauseEventStartUpdateThread.WaitOne();
                }
                catch (Exception exception)
                {
                    MvcApplication.WRMLogModule.Log(exception, exception.GetFullMessage(), "StartUpdate", kernelLogLevel: KernelLogLevel.Error);
                }
                //bool shouldUpdate = true;
                if (ThreadIsRunnable)
                {
                    try
                    {
                        using (RetailWebClient.POSUpdateService.POSUpdateService service = new RetailWebClient.POSUpdateService.POSUpdateService())
                        {
                            service.Url = StoreControllerAppiSettings.MasterServiceURL;
                            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
                            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                            {
                                IEnumerable<XPClassInfo> classes = GetClasses(eUpdateDirection.MASTER_TO_STORECONTROLLER);
                                foreach (XPClassInfo itm in classes)
                                {
                                    try
                                    {
                                        if (StopThreads)
                                        {
                                            return;
                                        }
                                        MethodInfo callMethod = typeof(Updater).GetMethod("GetUpdates", flags);
                                        if (callMethod == null)
                                        {
                                            continue;
                                        }
                                        Type[] args = new Type[] { itm.ClassType };
                                        MethodInfo generic = callMethod.MakeGenericMethod(args);
                                        string result = generic.Invoke(null, new object[] { uow }) as string;
                                        if (result == MvcApplication.IMPORT_IS_RUNNING)
                                        {
                                            MvcApplication.WRMLogModule.Log(null, "Updater Paused", "UpdaterThread", itm.ClassType.Name, kernelLogLevel: KernelLogLevel.Debug);
                                            break;
                                        }
                                        if (!String.IsNullOrEmpty(result))
                                        {
                                            MvcApplication.WRMLogModule.Log(new Exception(result), ResourcesLib.Resources.AnErrorOccurred, "UpdaterThread", itm.ClassType.Name, kernelLogLevel: KernelLogLevel.Error);
                                            break;
                                        }
                                        GC.Collect();
                                    }
                                    catch (Exception ex)
                                    {
                                        MvcApplication.WRMLogModule.Log(ex, ResourcesLib.Resources.AnErrorOccurred, "UpdaterThread", itm.ClassType.Name, kernelLogLevel: KernelLogLevel.Error);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exex)
                    {

                        MvcApplication.WRMLogModule.Log(exex, ResourcesLib.Resources.AnErrorOccurred, "UpdaterThread", kernelLogLevel: KernelLogLevel.Error);
                        pauseEvent.ExceptionThrownStartUpdateThread = true;
                    }
                }
                if (StopThreads)
                {
                    return;
                }
                Thread.Sleep(UpdateWaitTime);
            }
        }

        private static IEnumerable<XPClassInfo> GetClasses(eUpdateDirection direction)
        {
            IEnumerable<XPClassInfo> xp = XpoHelper.dict.Classes.Cast<XPClassInfo>();

            IEnumerable<XPClassInfo> first = xp.Where(g => g.HasAttribute("UpdaterAttribute")).Where(g => (g.Attributes.Where(x => x is UpdaterAttribute).First() as UpdaterAttribute).Permissions.HasFlag(direction)).
                OrderBy(g => (g.Attributes.Where(x => x is UpdaterAttribute).First() as UpdaterAttribute).Order);
            return first;
        }

        private static long GetVersion(Type type, bool allRecords)
        {
            if (allRecords)
            {
                return 0;
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(new BinaryOperator("EntityName", type.ToString(), BinaryOperatorType.Equal));
                if (obj != null)
                {
                    return obj.Number;
                }
                return 0;
            }
        }

        private static long GetVersion<T>(bool allRecords)
        {
            return GetVersion(typeof(T), allRecords);
        }

        private static bool GetForceReload<T>()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(new BinaryOperator("EntityName", typeof(T).ToString(), BinaryOperatorType.Equal));
                if (obj != null)
                {
                    return obj.ForceReload;
                }
                return true;
            }
        }

        private static void SetForceReload<T>(bool reload)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(new BinaryOperator("EntityName", typeof(T).ToString(), BinaryOperatorType.Equal));
                if (obj != null)
                {
                    obj.ForceReload = reload;
                    obj.Save();
                    XpoHelper.CommitChanges(uow);
                }
            }
        }

        private static void SetVersion<T>(long ver)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TableVersion obj = uow.FindObject<TableVersion>(new BinaryOperator("EntityName", typeof(T).ToString(), BinaryOperatorType.Equal));
                if (obj == null)
                {
                    obj = new TableVersion(uow) { EntityName = typeof(T).ToString() };
                }
                obj.Number = ver;
                obj.Save();
                XpoHelper.CommitChanges(uow);
            }
        }

        private static string GetUpdates<T>(UnitOfWork session) where T : BasicObj
        {
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            string sUpdates;
            JObject jsonItem;
            int lines = 0;
            T currentItem;


            using (RetailWebClient.POSUpdateService.POSUpdateService webService = new RetailWebClient.POSUpdateService.POSUpdateService())
            {
                webService.Timeout = MvcApplication.RetailMasterServiceTimeout;
                webService.Url = StoreControllerAppiSettings.MasterServiceURL;
                int lastObjects = 0;
                bool forceReload = GetForceReload<T>();
                while (true)
                {
                    long verUpdate = GetVersion<T>(false);
                    try
                    {
                        int totalCount;
                        sUpdates = webService.GetChanges(typeof(T).Name, new DateTime(verUpdate), StoreControllerAppiSettings.StoreControllerOid.ToString(), RetailWebClient.POSUpdateService.eIdentifier.STORECONTROLLER, out totalCount);
                        if (sUpdates == null)
                        {
                            break; //service is alive but an exception occured
                        }
                        if (sUpdates == MvcApplication.IMPORT_IS_RUNNING)
                        {
                            return sUpdates;
                        }
                    }
                    catch (Exception ex)
                    {
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                        return ex.Message;
                    }

                    sUpdates = Convert.ToString(CompressionHelper.DecompressLZMA(sUpdates));
                    List<string> items = JsonConvert.DeserializeObject<List<string>>(sUpdates);

                    if (items.Count == 0)
                    {
                        break;
                    }
                    if (items.Count < 10000 && items.Count == lastObjects)
                    {
                        break;
                    }


                    lastObjects = items.Count;
                    string typeName = typeof(T).Name;
                    foreach (string itm in items)
                    {
                        lines++;
                        jsonItem = JObject.Parse(itm);

                        // Find if object  exists
                        string key = jsonItem.Property("Oid").Value.ToString();
                        try
                        {

                            Guid oid = Guid.Empty;
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
                                oid = (currentItem as BaseObj).Oid;
                            }


                            if (currentItem.UpdatedOnTicks.ToString() == jsonItem.Property("UpdatedOnTicks").Value.ToString() && forceReload == false)
                            {
                                verUpdate = currentItem.UpdatedOnTicks;
                                continue;
                            }
                            string error;
                            bool jsonResult = currentItem.FromJson(jsonItem, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, true, out error);
                            if (jsonResult == false)
                            {
                                session.RollbackTransaction();
                                return error;
                            }

                            GetUpdatesOnSaving(currentItem);
                            currentItem.Save();
                            verUpdate = currentItem.UpdatedOnTicks;

                            if (lines % 1000 == 0 || currentItem.GetMemberValue("GCRecord") != null)
                            {
                                try
                                {
                                    XpoHelper.CommitChanges(session);
                                    SetVersion<T>(verUpdate);
                                }
                                catch (Exception ex)
                                {
                                    session.RollbackTransaction();
                                    return ex.Message;
                                }

                            }
                        }
                        catch (Exception exception)
                        {
                            string exceptionsError = exception.GetFullMessage();
                        }
                    }
                    try
                    {
                        XpoHelper.CommitChanges(session);
                        SetVersion<T>(verUpdate);
                    }
                    catch (Exception ex)
                    {
                        session.RollbackTransaction();
                        return ex.Message;
                    }

                }
                //Turn off the force reload flag
                SetForceReload<T>(false);
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                return null;
            }
        }

        /// <summary>
        /// Implement extra OnSaving logic that must not be in the Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentItem"></param>
        private static void GetUpdatesOnSaving<T>(T currentItem)
        {
            if (typeof(T) == typeof(Item))
            {
                GetUpdatesOnSavingItem(currentItem);
            }
            else if (typeof(T) == typeof(Coupon))
            {
                GetUpdatesOnSavingCoupon(currentItem);
            }
            else if (typeof(T) == typeof(Customer))
            {
                GetUpdatesOnSavingCustomer(currentItem);
            }
        }

        private static void GetUpdatesOnSavingCustomer<T>(T currentItem)
        {
            Customer customer = currentItem as Customer;
            try
            {
                switch (MvcApplication.ApplicationInstance)
                {
                    case eApplicationInstance.DUAL_MODE:
                        break;
                    case eApplicationInstance.RETAIL:
                        break;
                    case eApplicationInstance.STORE_CONTROLER:
                        CriteriaOperator customerCriteria = CriteriaOperator.And(new BinaryOperator("Customer", customer.Oid, BinaryOperatorType.NotEqual),
                                                                                 new BinaryOperator("CustomerLookUpTaxCode", customer.Trader.TaxCode),
                                                                                 new NotOperator(new NullOperator("DenormalizedCustomer"))
                                                                                );
                        XPCollection<DocumentHeader> documentHeaders = new XPCollection<DocumentHeader>(customer.Session, customerCriteria);
                        foreach (DocumentHeader documentHeader in documentHeaders)
                        {
                            if (string.IsNullOrWhiteSpace(documentHeader.DenormalizedCustomer) == false)
                            {
                                documentHeader.Customer = customer;
                                documentHeader.CustomerCode = customer.Code;
                                documentHeader.CustomerName = customer.CompanyName;
                                documentHeader.Save();
                                documentHeader.ProcessedDenormalizedCustomer = documentHeader.DenormalizedCustomer;
                                documentHeader.DenormalizedCustomer = null;
                            }
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                MvcApplication.WRMLogModule.Log(exception, "GetUpdatesOnSavingCustomer: An error has occurred trying to Update Customer " + customer.Code,
                    "UpdaterThread", kernelLogLevel: KernelLogLevel.Error);
            }
        }

        private static void GetUpdatesOnSavingItem<T>(T currentItem)
        {
            Item item = currentItem as Item;
            try
            {
                if (item.ImageLarge != null)
                {
                    using (MagickImage imageLarge = new MagickImage(new Bitmap(item.ImageLarge)))
                    {
                        item.ImageMedium = ItemHelper.PrepareImage(imageLarge, 300, 300);
                        item.ImageSmall = ItemHelper.PrepareImage(imageLarge, 150, 150);
                    }
                }
            }
            catch (Exception ex)
            {
                MvcApplication.WRMLogModule.Log(ex, "An error has occurred trying to convert the image of item " + item.Code,
                    "UpdaterThread", kernelLogLevel: KernelLogLevel.Error);
            }
        }

        private static void GetUpdatesOnSavingCoupon<T>(T currentItem)
        {
            Coupon coupon = currentItem as Coupon;
            switch (MvcApplication.ApplicationInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    break;
                case eApplicationInstance.RETAIL:
                    break;
                case eApplicationInstance.STORE_CONTROLER:
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        GeneratedCoupon generatedCoupon = uow.FindObject<GeneratedCoupon>(new BinaryOperator("Code", coupon.Code));
                        if (generatedCoupon != null)
                        {
                            generatedCoupon.Delete();
                            XpoHelper.CommitChanges(uow);
                        }
                    }
                    break;
            }
        }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private static bool PostUpdates<T>(UnitOfWork session) where T : BasicObj
        {
            if (StoreControllerAppiSettings.CurrentStore == null)
            {
                return false;//Throw up
            }
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            try
            {
                using (RetailWebClient.POSUpdateService.POSUpdateService webService = new RetailWebClient.POSUpdateService.POSUpdateService())
                {
                    webService.Timeout = MvcApplication.RetailMasterServiceTimeout;
                    webService.Url = StoreControllerAppiSettings.MasterServiceURL;

                    //********** POS POSTS
                    XPCollection<Model.POS> posses = new XPCollection<Model.POS>(session);
                    List<string> posIDS = posses.Select(pos => pos.Oid.ToString()).ToList();

                    foreach (string posID in posIDS)
                    {
                        long posLatestVersion = webService.GetVersion(typeof(T).Name, posID);

                        CriteriaOperator posRestrictionsCrop = typeof(T).GetMethod("GetUpdaterCriteria", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                                .Invoke(null, new object[] {
                                                    eUpdateDirection.STORECONTROLLER_TO_MASTER, StoreControllerAppiSettings.CurrentStore.Owner,
                                                    StoreControllerAppiSettings.CurrentStore, "" }
                                                ) as CriteriaOperator;

                        CriteriaOperator posCrop = CriteriaOperator.And(posRestrictionsCrop,
                            new BinaryOperator("UpdatedOnTicks", posLatestVersion, BinaryOperatorType.GreaterOrEqual),
                            new BinaryOperator("CreatedByDevice", posID),
                            CriteriaOperator.Parse("IsExactType(This,?)", typeof(T).FullName),
                            CriteriaOperator.Or(new OperandProperty("GCRecord").IsNotNull(), new OperandProperty("GCRecord").IsNull())
                            );

                        XPCollection<T> posRecords = new XPCollection<T>(session, posCrop, new SortProperty("UpdatedOnTicks", SortingDirection.Ascending));
                        posRecords.TopReturnedObjects = MvcApplication.UpdaterBatchSize;
                        List<string> posList = posRecords.Select(p => p.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, eUpdateDirection.STORECONTROLLER_TO_MASTER)).ToList();
                        if (posList.Count > 0)
                        {
                            webService.PostData(typeof(T).Name, CompressionHelper.CompressLZMA(JsonConvert.SerializeObject(posList, PlatformConstants.JSON_SERIALIZER_SETTINGS)), posID, RetailWebClient.POSUpdateService.eIdentifier.STORECONTROLLER);
                        }
                    }
                    ////*************

                    ////************* STORE CONTROLLER POST
                    long latestVersion = webService.GetVersion(typeof(T).Name, StoreControllerAppiSettings.StoreControllerOid.ToString());

                    CriteriaOperator restriciontsCrop = typeof(T).GetMethod("GetUpdaterCriteria", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                            .Invoke(null, new object[] {
                                                    eUpdateDirection.STORECONTROLLER_TO_MASTER, StoreControllerAppiSettings.CurrentStore.Owner,
                                                    StoreControllerAppiSettings.CurrentStore, ""
                                            }) as CriteriaOperator;

                    CriteriaOperator crop = CriteriaOperator.And(restriciontsCrop,
                        new BinaryOperator("UpdatedOnTicks", latestVersion, BinaryOperatorType.GreaterOrEqual),
                        CriteriaOperator.Or(new BinaryOperator("CreatedByDevice", StoreControllerAppiSettings.StoreControllerOid.ToString()),
                                            new BinaryOperator("CreatedByDevice", ""),
                                            new NullOperator("CreatedByDevice")),
                        CriteriaOperator.Parse("IsExactType(This,?)", typeof(T).FullName),
                        CriteriaOperator.Or(new OperandProperty("GCRecord").IsNotNull(), new OperandProperty("GCRecord").IsNull())
                        );

                    XPCollection<T> records = new XPCollection<T>(session, crop, new SortProperty("UpdatedOnTicks", SortingDirection.Ascending));

                    //IEnumerable<Guid> resendObjectOids = new XPCollection<ResendObjectRequest>(session, CriteriaOperator.And(new BinaryOperator("EntityName", typeof(T).FullName),
                    //    new BinaryOperator("TargetDeviceOid",Guid.Empty))).Select(obj => obj.EntityOid);

                    records.AddRange(new XPCollection<T>(session, new JoinOperand("ResendObjectRequest",
                                                                  CriteriaOperator.And(new OperandProperty("^.Oid") == new OperandProperty("EntityOid"),
                                                                                       new BinaryOperator("EntityName", typeof(T).FullName),
                                                                                       new BinaryOperator("TargetDeviceOid", Guid.Empty)))));

                    records.TopReturnedObjects = MvcApplication.UpdaterBatchSize;
                    List<string> list = records.Select(p => p.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, eUpdateDirection.STORECONTROLLER_TO_MASTER)).ToList();
                    if (list.Count > 0)
                    {
                        webService.PostData(typeof(T).Name, CompressionHelper.CompressLZMA(JsonConvert.SerializeObject(list, PlatformConstants.JSON_SERIALIZER_SETTINGS)), StoreControllerAppiSettings.StoreControllerOid.ToString(), RetailWebClient.POSUpdateService.eIdentifier.STORECONTROLLER);
                    }
                    //delete 
                }
            }
            catch (Exception ex)
            {
                MvcApplication.WRMLogModule.Log(ex, "An error has occurred during posting " + typeof(T).Name, "UpdaterPostThread",
                        kernelLogLevel: KernelLogLevel.Error);
                return false;
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);

            return true;
        }

        private static void PostRecordsRunnable()
        {
            while (true)
            {
                try
                {
                    pauseEvent.PauseEventPostRecordsThread.WaitOne();
                }
                catch (Exception exception)
                {
                    MvcApplication.WRMLogModule.Log(exception, exception.GetFullMessage(), "PostRecordsRunnable", kernelLogLevel: KernelLogLevel.Error);
                }
                //bool shouldUpdate = true;
                if (ThreadIsRunnable)
                {
                    try
                    {
                        XPDictionary records = new ReflectionDictionary();

                        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

                        records.GetDataStoreSchema(typeof(DocumentHeader).Assembly);

                        IEnumerable<XPClassInfo> recordsTypesSorted = GetClasses(eUpdateDirection.STORECONTROLLER_TO_MASTER);

                        foreach (XPClassInfo classInfo in recordsTypesSorted)
                        {
                            if (StopThreads)
                            {
                                return;
                            }
                            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                            {
                                MethodInfo callMethod = typeof(Updater).GetMethod("PostUpdates", flags);
                                if (callMethod == null)
                                {
                                    continue;
                                }
                                Type[] args = new Type[] { classInfo.ClassType };
                                MethodInfo generic = callMethod.MakeGenericMethod(args);
                                bool success = (bool)generic.Invoke(null, new object[] { uow });
                                if (!success)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        pauseEvent.ExceptionThrownPostRecordsThread = true;
                    }
                }

                Thread.Sleep(MvcApplication.RetailMasterPostInterval);

            }
        }

        private static Dictionary<Type, UpdaterAttribute> AllEntities = null;
        public static void PostSyncInfoThreadRunnable()
        {
            while (true)
            {
                try
                {
                    pauseEvent.PauseEventPostSyncInfoThread.WaitOne();
                }
                catch (Exception exception)
                {
                    MvcApplication.WRMLogModule.Log(exception, exception.GetFullMessage(), "PostSyncInfoThreadRunnable", kernelLogLevel: KernelLogLevel.Error);
                }
                try
                {

                    if (MvcApplication.LicenseStatus == ITS.Licensing.Enumerations.LicenseStatus.Invalid)
                    {
                        Thread.Sleep(SyncInfoWaitTime);
                        continue;
                    }

                    if (AllEntities == null)
                    {
                        List<Type> pseudoAbstractClasses = new List<Type>() { typeof(CategoryNode),
                                                                              typeof(PromotionApplicationRule),
                                                                              typeof(PromotionExecution) ,
                                                                              typeof(PromotionResult) };

                        AllEntities =

                        typeof(Item).Assembly.GetTypes().Where
                        (x =>
                                pseudoAbstractClasses.Contains(x) == false &&
                                x.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() != null &&
                                (
                                    (MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL &&
                                        (
                                            (x.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() as UpdaterAttribute).Permissions.HasFlag(eUpdateDirection.POS_TO_STORECONTROLLER) ||
                                            (x.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() as UpdaterAttribute).Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_POS)
                                        )
                                    )
                                    ||
                                    (MvcApplication.ApplicationInstance != eApplicationInstance.DUAL_MODE &&
                                        (
                                            (x.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() as UpdaterAttribute).Permissions.HasFlag(eUpdateDirection.MASTER_TO_STORECONTROLLER) ||
                                            (x.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() as UpdaterAttribute).Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_MASTER)
                                        )
                                    )
                                )
                        ).ToDictionary(key => key, value => value.GetCustomAttributes(typeof(UpdaterAttribute), false).FirstOrDefault() as UpdaterAttribute);
                    }


                    string scName = "StoreController " + StoreControllerAppiSettings.ID;
                    foreach (KeyValuePair<Type, UpdaterAttribute> entity in AllEntities)
                    {
                        pauseEvent.PauseEventPostSyncInfoThread.WaitOne();
                        if (StopThreads)
                        {
                            return;
                        }
                        using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                        {
                            long databaseCount = (int)uow.Evaluate(entity.Key, CriteriaOperator.Parse("Count"), new BinaryOperator("IsActive", true));
                            long version = 0;
                            RetailWebClient.POSUpdateService.eSyncInfoEntityDirection direction;
                            XPCollection objects = null;
                            if (entity.Value.Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_MASTER))
                            {
                                ////Entity that goes upwards and is created at SC or POS. Get version directly from DB table
                                //objects = new XPCollection(uow, entity.Key, null);
                                //objects.Sorting = new SortingCollection(new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Descending));
                                //objects.TopReturnedObjects = 1;
                                //BaseObj newestObj = objects.Cast<BaseObj>().FirstOrDefault();
                                //long version1 = newestObj == null ? 0 : newestObj.UpdatedOnTicks;
                                direction = RetailWebClient.POSUpdateService.eSyncInfoEntityDirection.UP;
                                version = GetVersion(entity.Key, false);
                                //string tmpVersion = (string)uow.Evaluate(entity.Key, CriteriaOperator.Parse("Max(UpdatedOnTicks)"), null);
                                //if (long.TryParse(tmpVersion, out version) == false)
                                //{
                                //    version = 0;
                                //}
                            }
                            else
                            {
                                ////Downwards directed entity
                                version = GetVersion(entity.Key, false);
                                direction = RetailWebClient.POSUpdateService.eSyncInfoEntityDirection.DOWN;
                            }

                            //objects = new XPCollection(uow, entity.Key, null);
                            //objects.Sorting = new SortingCollection(new SortProperty("UpdatedOnTicks", DevExpress.Xpo.DB.SortingDirection.Ascending));
                            //objects.TopReturnedObjects = 1;
                            //BaseObj oldestObj = objects.Cast<BaseObj>().FirstOrDefault();
                            //long minVersion = oldestObj == null ? 0 : oldestObj.UpdatedOnTicks;
                            long minVersion = 0;
                            object tmpMinVersion = uow.Evaluate(entity.Key, CriteriaOperator.Parse("Min(UpdatedOnTicks)"), null);
                            if (tmpMinVersion != null)
                            {
                                try
                                {
                                    minVersion = (long)tmpMinVersion;
                                }
                                catch (Exception exception)
                                {
                                    MvcApplication.WRMLogModule.Log(exception, exception.GetFullMessage(), KernelLogLevel.Error);
                                }
                            }


                            using (RetailWebClient.POSUpdateService.POSUpdateService webService = new RetailWebClient.POSUpdateService.POSUpdateService())
                            {
                                webService.Timeout = MvcApplication.RetailMasterServiceTimeout;
                                webService.Url = StoreControllerAppiSettings.MasterServiceURL;
                                string message;
                                long serverVersion, serverCount;
                                if (webService.PostSynchronizationInfo(StoreControllerAppiSettings.StoreControllerOid, scName, databaseCount, version, entity.Key.Name, direction, minVersion, out serverVersion, out serverCount, out message) == false)
                                {
                                    MvcApplication.WRMLogModule.Log(null, "An error has occurred during posting Sync Info" + entity.Key.Name,
                                       "UpdaterPostThread", result: "PostVersions Thread: Server error posting pending changes info for \"" + entity.Key.Name + "\". Message: " + message, kernelLogLevel:
                                       KernelLogLevel.Warn);
                                }

                                ////Create Current SC Sync info
                                SynchronizationInfo scSyncInfo = uow.FindObject<SynchronizationInfo>(CriteriaOperator.And(new BinaryOperator("EntityName", entity.Key.Name),
                                                                                                                 new BinaryOperator("DeviceOid", StoreControllerAppiSettings.StoreControllerOid.ToString())));
                                if (scSyncInfo == null)
                                {
                                    scSyncInfo = new SynchronizationInfo(uow);
                                    scSyncInfo.EntityName = entity.Key.Name;
                                    scSyncInfo.DeviceOid = StoreControllerAppiSettings.StoreControllerOid;
                                    scSyncInfo.DeviceType = eIdentifier.STORECONTROLLER;
                                }

                                scSyncInfo.DeviceName = scName;
                                scSyncInfo.DeviceEntityCount = databaseCount;
                                scSyncInfo.DeviceVersion = version;
                                scSyncInfo.DeviceMinVersion = minVersion;
                                scSyncInfo.SyncInfoEntityDirection = (eSyncInfoEntityDirection)direction;
                                scSyncInfo.LastUpdate = DateTime.Now;
                                /////

                                ////Create Master Sync info
                                SynchronizationInfo masterSyncInfo = uow.FindObject<SynchronizationInfo>(CriteriaOperator.And(new BinaryOperator("EntityName", entity.Key.Name),
                                                                                 new BinaryOperator("DeviceType", eIdentifier.MASTER)));
                                if (masterSyncInfo == null)
                                {
                                    masterSyncInfo = new SynchronizationInfo(uow);
                                    masterSyncInfo.EntityName = entity.Key.Name;
                                    masterSyncInfo.DeviceType = eIdentifier.MASTER;
                                }

                                masterSyncInfo.DeviceName = "Master";
                                masterSyncInfo.DeviceEntityCount = serverCount;
                                masterSyncInfo.DeviceVersion = serverVersion;
                                masterSyncInfo.DeviceMinVersion = 0;
                                masterSyncInfo.SyncInfoEntityDirection = (eSyncInfoEntityDirection)direction;
                                masterSyncInfo.LastUpdate = DateTime.Now;
                                ////

                                XpoHelper.CommitChanges(uow);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MvcApplication.WRMLogModule.Log(ex, "An error occurred during posting Sync Info", "PostSyncInfoThreadRunnable",
                         kernelLogLevel: KernelLogLevel.Error);
                    pauseEvent.ExceptionThrownPostSyncInfoThread = true;
                }
                finally
                {
                    Thread.Sleep(SyncInfoWaitTime);
                }
                if (StopThreads)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// GetStoreControllerCommandsRunnable
        /// 
        /// Execute Commands for StoreController
        /// </summary>
        private static void GetStoreControllerCommandsRunnable()
        {
            while (true)
            {
                Thread.Sleep(MvcApplication.StoreControllerCommandInterval);

                using (RetailWebClient.POSUpdateService.POSUpdateService storeControlerUpdateService = new RetailWebClient.POSUpdateService.POSUpdateService())
                {
                    storeControlerUpdateService.Timeout = MvcApplication.RetailMasterServiceTimeout;
                    storeControlerUpdateService.Url = StoreControllerAppiSettings.MasterServiceURL;
                    string commands = string.Empty;
                    try
                    {
                        commands = storeControlerUpdateService.GetStoreControllerCommands(StoreControllerAppiSettings.StoreControllerOid, ThreadIsRunnable);
                        MvcApplication.Status = ThreadIsRunnable ? ApplicationStatus.ONLINE : ApplicationStatus.OFFLINE_VIA_COMMAND;
                        if (MvcApplication.LicenseStatus == ITS.Licensing.Enumerations.LicenseStatus.Invalid)
                        {
                            MvcApplication.ValidateStoreControllerLicense();
                        }
                    }
                    catch (System.Net.WebException webexception)
                    {
                        string exceptionMessage = webexception.GetFullMessage();
                        if (webexception.Response is System.Net.HttpWebResponse &&
                            ((System.Net.HttpWebResponse)webexception.Response).StatusCode == System.Net.HttpStatusCode.PaymentRequired)
                        {
                            MvcApplication.InvalidateStoreControllerLicense();

                            continue;
                        }
                        MvcApplication.Status = ApplicationStatus.OFFLINE_VIA_ERROR;
                        continue;
                    }
                    catch (Exception exception)
                    {

                        string exceptionMessage = exception.GetFullMessage();
                        MvcApplication.Status = ApplicationStatus.OFFLINE_VIA_ERROR;
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(commands) == false)
                    {
                        using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                        {
                            string[] commandSet = JsonConvert.DeserializeObject<string[]>(commands);
                            foreach (string command in commandSet)
                            {
                                JObject commandObject = JObject.Parse(command);
                                Guid key = new Guid(commandObject.Property("Oid").Value.Value<string>());
                                StoreControllerCommand persistentCommand = uow.GetObjectByKey<StoreControllerCommand>(key);
                                if (persistentCommand == null)
                                {
                                    persistentCommand = new StoreControllerCommand(uow);
                                    persistentCommand.Oid = key;
                                    persistentCommand.StoreController = uow.GetObjectByKey<StoreControllerSettings>(new Guid(commandObject.Property("StoreController").Value.Value<string>()));
                                }
                                persistentCommand.Command = (eStoreControllerCommand)commandObject.Property("Command").Value.Value<Int64>();
                                persistentCommand.CommandParameters = commandObject.Property("CommandParameters").Value.Value<string>();
                                persistentCommand.Save();
                            }
                            uow.CommitChanges();

                            XPCollection<StoreControllerCommand> scCommands = new XPCollection<StoreControllerCommand>(uow);
                            scCommands.OrderBy(x => x.CreatedOnTicks);
                            foreach (StoreControllerCommand cmd in scCommands)
                            {
                                switch (cmd.Command)
                                {
                                    case eStoreControllerCommand.OFF_LINE:
                                        MvcApplication.Status = ApplicationStatus.OFFLINE_VIA_COMMAND;
                                        ThreadIsRunnable = false;
                                        SaveOnlineStatusFromFile();
                                        break;
                                    case eStoreControllerCommand.ON_LINE:
                                        MvcApplication.Status = ApplicationStatus.ONLINE;
                                        ThreadIsRunnable = true;
                                        SaveOnlineStatusFromFile();
                                        break;
                                    case eStoreControllerCommand.RESTART:
                                        StopThreads = true;

                                        Thread.Sleep(1000);
                                        GetUpdatesThread.Abort();
                                        PostRecordsThread.Abort();
                                        PostSyncInfoThread.Abort();

                                        uow.Delete(scCommands);
                                        uow.CommitChanges();
                                        HttpRuntime.UnloadAppDomain();
                                        return;
                                    default:
                                        break;
                                }
                            }
                            uow.Delete(scCommands);
                            uow.CommitChanges();
                        }
                    }
                }

            }
        }

        public static void DataTransferVerification()
        {
            eUpdateDirection updateDirection = eUpdateDirection.MASTER_TO_STORECONTROLLER;
            CompanyNew owner = null;
            Store store = null;
            Model.POS pos = null;
            bool correctSettingsLoaded = false;
            while (!correctSettingsLoaded)
            {
                try
                {
                    Thread.Sleep(MvcApplication.DataTransferVerificationThreadSleepTimeInMilliSeconds);
                    switch (MvcApplication.ApplicationInstance)
                    {
                        case eApplicationInstance.DUAL_MODE:
                            updateDirection = eUpdateDirection.STORECONTROLLER_TO_POS;
                            owner = StoreControllerAppiSettings.Owner;
                            store = StoreControllerAppiSettings.CurrentStore;
                            break;
                        case eApplicationInstance.RETAIL:
                            updateDirection = eUpdateDirection.MASTER_TO_STORECONTROLLER;
                            //owner = StoreControllerAppiSettings.Owner; //TODO fill data
                            //store = StoreControllerAppiSettings.CurrentStore; //TODO fill data
                            break;
                        case eApplicationInstance.STORE_CONTROLER:
                            updateDirection = eUpdateDirection.STORECONTROLLER_TO_MASTER;
                            owner = StoreControllerAppiSettings.Owner;
                            store = StoreControllerAppiSettings.CurrentStore;
                            break;
                        default:
                            throw new NotImplementedException(string.Format(ResourcesLib.Resources.UknownApplicationInstance, updateDirection.ToString()));
                    }
                    correctSettingsLoaded = true;
                    break;
                }
                catch (Exception ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                }
            }

            while (true)
            {
                try
                {
                    GetObjectsForVerification(updateDirection, owner, store, pos, MvcApplication.DataTransferVerificationThreadPastHoursToCheck);
                }
                catch (Exception exception)
                {
                    MvcApplication.WRMLogModule.Log(exception, "DataTransferVerification Thread", KernelLogLevel.Error);
                }
                Thread.Sleep(MvcApplication.DataTransferVerificationThreadSleepTimeInMilliSeconds);
            }
        }

        //
        // Temporarily Inactive
        //
        //public static void DataTransferToPOSVerification()
        //{
        //    eUpdateDirection updateDirection = eUpdateDirection.STORECONTROLLER_TO_POS;
        //    CompanyNew owner = StoreControllerAppiSettings.Owner;
        //    Store store = StoreControllerAppiSettings.CurrentStore;
        //    Model.POS pos = null;

        //    while (true)
        //    {
        //        Thread.Sleep(MvcApplication.DataTransferToPOSVerificationThreadSleepTimeInMilliSeconds);
        //        try
        //        {
        //            GetObjectsForVerification(updateDirection, owner, store, pos, MvcApplication.DataTransferToPOSVerificationThreadPastHoursToCheck);
        //        }
        //        catch (Exception exception)
        //        {
        //            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
        //            {
        //                ApplicationLog logEvent = new ApplicationLog(uow);
        //                logEvent.Result = "An error has occurred during Data Transfer Verification";
        //                logEvent.Controller = "DataTransferToPOSVerification";
        //                logEvent.Error = "DataTransferToPOSVerification Thread: Exception" + exception.GetFullMessage() + Environment.NewLine + exception.GetFullStackTrace();
        //                logEvent.Save();
        //                XpoHelper.CommitChanges(uow);
        //            }
        //        }               
        //    }
        //}

        public static void GetObjectsForVerification(eUpdateDirection direction, CompanyNew owner, Store store, Model.POS pos, int hoursToLookBack)
        {
            using (RetailWebClient.POSUpdateService.POSUpdateService posUpdateService = new RetailWebClient.POSUpdateService.POSUpdateService())
            {
                posUpdateService.Timeout = MvcApplication.RetailMasterServiceTimeout;
                posUpdateService.Url = StoreControllerAppiSettings.MasterServiceURL;

                IEnumerable<XPClassInfo> recordsTypesSorted = GetClasses(direction);
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    foreach (XPClassInfo classInfo in recordsTypesSorted)
                    {
                        Type classType = classInfo.ClassType;

                        CriteriaOperator updaterCriteria = classType
                                                   .GetMethod("GetUpdaterCriteria", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                                   .Invoke(null,
                                                           new object[] { direction, owner, store, pos == null ? Guid.Empty.ToString() : pos.Oid.ToString() }
                                                    ) as CriteriaOperator;

                        long ticksToLookBack = DateTime.Now.AddHours(-hoursToLookBack).Ticks;
                        CriteriaOperator criteria = CriteriaOperator.Or(new JoinOperand("ResendObjectRequest", CriteriaOperator.And(new OperandProperty("EntityOid") == new OperandProperty("^.Oid"),
                                                                                       new BinaryOperator("EntityName", classType.FullName))),
                                                                                       CriteriaOperator.And(updaterCriteria,
                                                                         new BinaryOperator("UpdatedOnTicks", ticksToLookBack, BinaryOperatorType.GreaterOrEqual)
                                                                        ));

                        XPCursor records = new XPCursor(uow, classInfo, criteria, new SortProperty("UpdatedOnTicks", SortingDirection.Ascending));
                        records.PageSize = 100;

                        List<ObjectSignature> objectsToVerify = new List<ObjectSignature>();
                        foreach (BasicObj basicObject in records)
                        {
                            objectsToVerify.Add(new ObjectSignature(basicObject.Oid, basicObject.ObjectSignature, basicObject.UpdatedOnTicks));
                        }

                        string zippedInfo = ZipLZMA.CompressLZMA(JsonConvert.SerializeObject(objectsToVerify, PlatformConstants.JSON_SERIALIZER_SETTINGS));
                        string objectsRequestedToBeRetransfered = posUpdateService.GetNotTransferedObjects(zippedInfo, classType.FullName);


                        List<Guid> objectsOidsToRetransfer = JsonConvert.DeserializeObject<List<Guid>>(ZipLZMA.DecompressLZMA(objectsRequestedToBeRetransfered) as string,
                                                                                                        PlatformConstants.JSON_SERIALIZER_SETTINGS
                                                                                                      );
                        CriteriaOperator typeCriteria = CriteriaOperator.And(new BinaryOperator("EntityName", classType.FullName),
                                                                               new BinaryOperator("UpdateDirection", direction)
                                                                            );

                        //Clear successed objects
                        IEnumerable<Guid> successfullyTransferedOids = objectsToVerify.Where(objToVerify => objectsOidsToRetransfer.Contains(objToVerify.Oid) == false).Select(objToVerify => objToVerify.Oid);
                        foreach (Guid transferedOid in successfullyTransferedOids)
                        {
                            ResendObjectRequest deleteResendObjectRequest = uow.FindObject<ResendObjectRequest>(CriteriaOperator.And(typeCriteria,
                                                                                                                                 new BinaryOperator("EntityOid", transferedOid)
                                                                                                                               ));
                            if (deleteResendObjectRequest != null)
                            {
                                deleteResendObjectRequest.Delete();
                            }
                        }
                        XpoHelper.CommitTransaction(uow);

                        //Save resendRequests
                        objectsOidsToRetransfer.ForEach(objetOidToRetransfer =>
                        {
                            //Check if a request already exists
                            ResendObjectRequest resendObjectRequest = uow.FindObject<ResendObjectRequest>(CriteriaOperator.And(typeCriteria,
                                                                                                                                 new BinaryOperator("EntityOid", objetOidToRetransfer)
                                                                                                                               ));
                            if (resendObjectRequest == null)
                            {
                                resendObjectRequest = new ResendObjectRequest(uow)
                                {
                                    EntityName = classType.FullName,
                                    EntityOid = objetOidToRetransfer,
                                    UpdateDirection = direction,
                                    RequestedOnTicks = DateTime.Now.Ticks
                                };
                                resendObjectRequest.Save();
                            }
                        });
                        XpoHelper.CommitTransaction(uow);
                    }
                }
            }
        }
        public static void Pause(string ThreadName)
        {
            switch (ThreadName)
            {
                case "StartUpdate":
                    if (pauseEvent.PauseEventStartUpdateThread != null)
                    {
                        pauseEvent.PauseEventStartUpdateThread.Reset();
                    }
                    break;
                case "PostSyncInfoThread":
                    if (pauseEvent.PauseEventPostSyncInfoThread != null)
                    {
                        pauseEvent.PauseEventPostSyncInfoThread.Reset();
                    }
                    break;
                case "PostRecordsThread":
                    if (pauseEvent.PauseEventPostRecordsThread != null)
                    {
                        pauseEvent.PauseEventPostRecordsThread.Reset();
                    }
                    break;
            }
        }

        public static void Resume(string ThreadName)
        {
            switch (ThreadName)
            {
                case "StartUpdate":
                    if (pauseEvent.PauseEventStartUpdateThread != null)
                    {
                        pauseEvent.ExceptionThrownStartUpdateThread = false;
                        pauseEvent.PauseEventStartUpdateThread.Set();
                    }
                    break;
                case "PostSyncInfoThread":
                    if (pauseEvent.PauseEventPostSyncInfoThread != null)
                    {
                        pauseEvent.ExceptionThrownPostSyncInfoThread = false;
                        pauseEvent.PauseEventPostSyncInfoThread.Set();
                    }
                    break;
                case "PostRecordsThread":
                    if (pauseEvent.PauseEventPostRecordsThread != null)
                    {
                        pauseEvent.ExceptionThrownPostRecordsThread = false;
                        pauseEvent.PauseEventPostRecordsThread.Set();
                    }
                    break;
            }
        }

    }
}
