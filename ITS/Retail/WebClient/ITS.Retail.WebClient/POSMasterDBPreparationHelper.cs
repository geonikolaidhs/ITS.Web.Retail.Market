using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
using System.IO;
using ITS.POS.Model.Master;
using System.Collections;
using Ionic.Zip;
using System.Threading;
using System.Data;
using System.Data.SQLite;
using ITS.Retail.Platform.Enumerations;
using System.Globalization;

namespace ITS.Retail.WebClient
{
    internal class CustomSqliteConnectionProvider : SQLiteConnectionProvider
    {
        public CustomSqliteConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
            : base(connection, autoCreateOption)
        {
        }

        public override void CreateIndex(DBTable table, DBIndex index)
        {
            //base.CreateIndex(table, index);
        }

        public new static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect)
        {
            IDbConnection connection = new SQLiteConnection(connectionString);
            objectsToDisposeOnDisconnect = new IDisposable[] { connection };
            return CreateProviderFromConnection(connection, autoCreateOption);
        }

        public new static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption)
        {
            if (connection is SQLiteConnection)
            {
                return new CustomSqliteConnectionProvider(connection, autoCreateOption);
            }
            else
            {
                return null;
            }
        }

        public new static string GetConnectionString(string database)
        {
            string pgIntConnString = SQLiteConnectionProvider.GetConnectionString(database);
            string pgFinConnString = pgIntConnString.Replace(SQLiteConnectionProvider.XpoProviderTypeString, CustomSqliteConnectionProvider.XpoProviderTypeString);
            return pgFinConnString;
        }

        public new static void Register()
        {
            try
            {
                DataStoreBase.RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
            }
            catch (ArgumentException e)
            {
                string errorMessage = e.GetFullMessage();
            }

        }

        public override string ComposeSafeConstraintName(string constraintName)
        {
            return base.ComposeSafeConstraintName(constraintName).ToUpper(Thread.CurrentThread.CurrentCulture);
        }

        public new const string XpoProviderTypeString = "CustomSqlite";

    }

    public static class POSMasterDBPreparationHelper
    {
        private readonly static object lockObject = new object();
        private readonly static object lockObjectGeneral = new object();

        private static IDisposable[] _objectsToDisposeOnDisconnect;
        public static IDisposable[] ObjectsToDisposeOnDisconnect
        {
            get
            {
                return _objectsToDisposeOnDisconnect;
            }
            set
            {
                _objectsToDisposeOnDisconnect = value;
            }
        }

        static volatile IDataLayer fDataLayer;
        static IDataLayer DataLayer
        {
            get
            {
                if (fDataLayer == null)
                {
                    lock (lockObject)
                    {
                        if (fDataLayer == null)
                        {
                            fDataLayer = GetDataLayer();
                        }
                    }
                }
                return fDataLayer;
            }
        }

        public static Session GetNewSession()
        {
            return new Session(DataLayer);
        }

        public static UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }
        private static IDataLayer GetDataLayer()
        {
            CustomSqliteConnectionProvider.Register();
            XpoDefault.Session = null;
            XPDictionary dict = new ReflectionDictionary();
            IDataStore store = XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.DatabaseAndSchema, out _objectsToDisposeOnDisconnect);
            dict.GetDataStoreSchema(typeof(ITS.POS.Model.Master.Item).Assembly);

            DataCacheRoot cacheRoot = new DataCacheRoot(store);
            DataCacheConfiguration dcc = new DataCacheConfiguration();
            DataCacheNode cacheNode1 = new DataCacheNode(cacheRoot);
            IDataLayer dl = new ThreadSafeDataLayer(dict, cacheNode1);
            return dl;
        }
        public static string ConnectionString = @"CustomSqlite=SQLite;Data Source=PosMaster";

        public static bool IsProcessing
        {
            get
            {
                return _IsProcessing;
            }
        }

        private static volatile bool _IsProcessing;

        public static string PreparePOSMaster(Guid Store, string path)
        {
            string POSMasterFile = path + "\\PosMaster";
            string POSVersionFile = null;//the filename is the output of the relevant function call
            string zipFile = path + "\\POSDatabase.zip";
            //MvcApplication.Log.Info("POSMasterDBPreparationHelper PreparePOSMaster Start");
            MvcApplication.WRMLogModule.Log("POSMasterDBPreparationHelper PreparePOSMaster Start",KernelLogLevel.Info);
            try
            {
                _IsProcessing = true;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!File.Exists(POSMasterFile))
                {
                    using (FileStream fs = File.Create(POSMasterFile))
                    {

                    }
                }

                lock (lockObjectGeneral)
                {
                    // Step 1 Prepare master database connection
                    ConnectionString = @"XpoProvider=CustomSqlite;Data Source=" + POSMasterFile;

                    //  Step 2 Clear old connection (if any)
                    if (fDataLayer != null)
                    {
                        fDataLayer.Dispose();
                        fDataLayer = null;
                    }

                    //Step 3 Type mapping
                    Dictionary<Type, Type> mappingDictionary = new Dictionary<Type, Type>();

                    
                    mappingDictionary[typeof(ITS.POS.Model.Master.Barcode)] = typeof(ITS.Retail.Model.Barcode);
                    mappingDictionary[typeof(ITS.POS.Model.Master.Trader)] = typeof(ITS.Retail.Model.Trader);
                    mappingDictionary[typeof(ITS.POS.Model.Master.Customer)] = typeof(ITS.Retail.Model.Customer);
                    mappingDictionary[typeof(ITS.POS.Model.Master.CustomerStorePriceList)] = typeof(ITS.Retail.Model.CustomerStorePriceList);
                    mappingDictionary[typeof(ITS.POS.Model.Master.Item)] = typeof(ITS.Retail.Model.Item);
                    mappingDictionary[typeof(ITS.POS.Model.Master.ItemAnalyticTree)] = typeof(ITS.Retail.Model.ItemAnalyticTree);
                    mappingDictionary[typeof(ITS.POS.Model.Master.ItemCategory)] = typeof(ITS.Retail.Model.ItemCategory);
                    mappingDictionary[typeof(ITS.POS.Model.Master.ItemBarcode)] = typeof(ITS.Retail.Model.ItemBarcode);
                    mappingDictionary[typeof(ITS.POS.Model.Master.LinkedItem)] = typeof(ITS.Retail.Model.LinkedItem);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PriceCatalog)] = typeof(ITS.Retail.Model.PriceCatalog);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PriceCatalogDetail)] = typeof(ITS.Retail.Model.PriceCatalogDetail);
                    mappingDictionary[typeof(ITS.POS.Model.Master.Store)] = typeof(ITS.Retail.Model.Store);
                    mappingDictionary[typeof(ITS.POS.Model.Master.StorePriceList)] = typeof(ITS.Retail.Model.StorePriceList);
                    mappingDictionary[typeof(ITS.POS.Model.Master.CompanyNew)] = typeof(ITS.Retail.Model.CompanyNew);
                    mappingDictionary[typeof(ITS.POS.Model.Master.Address)] = typeof(ITS.Retail.Model.Address);
                    mappingDictionary[typeof(ITS.POS.Model.Master.CustomerAnalyticTree)] = typeof(ITS.Retail.Model.CustomerAnalyticTree);
                    mappingDictionary[typeof(ITS.POS.Model.Master.CustomerCategory)] = typeof(ITS.Retail.Model.CustomerCategory);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PriceCatalogPromotion)] = typeof(ITS.Retail.Model.PriceCatalogPromotion);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionCustomerApplicationRule)] = typeof(ITS.Retail.Model.PromotionCustomerApplicationRule);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionDocumentApplicationRule)] = typeof(ITS.Retail.Model.PromotionDocumentApplicationRule);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionItemApplicationRule)] = typeof(ITS.Retail.Model.PromotionItemApplicationRule);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionItemCategoryApplicationRule)] = typeof(ITS.Retail.Model.PromotionItemCategoryApplicationRule);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionApplicationRuleGroup)] = typeof(ITS.Retail.Model.PromotionApplicationRuleGroup);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionDocumentExecution)] = typeof(ITS.Retail.Model.PromotionDocumentExecution);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionItemExecution)] = typeof(ITS.Retail.Model.PromotionItemExecution);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionItemCategoryExecution)] = typeof(ITS.Retail.Model.PromotionItemCategoryExecution);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionPrintResult)] = typeof(ITS.Retail.Model.PromotionPrintResult);
                    mappingDictionary[typeof(ITS.POS.Model.Master.Promotion)] = typeof(ITS.Retail.Model.Promotion);
                    mappingDictionary[typeof(ITS.POS.Model.Master.Address)] = typeof(ITS.Retail.Model.Address);
                    mappingDictionary[typeof(ITS.POS.Model.Master.Phone)] = typeof(ITS.Retail.Model.Phone);
                    mappingDictionary[typeof(ITS.POS.Model.Master.SupplierNew)] = typeof(ITS.Retail.Model.SupplierNew);
                    mappingDictionary[typeof(ITS.POS.Model.Master.ItemExtraInfo)] = typeof(ITS.Retail.Model.ItemExtraInfo);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PriceCatalogPolicyDetail)] = typeof(ITS.Retail.Model.PriceCatalogPolicyDetail);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PriceCatalogPolicy)] = typeof(ITS.Retail.Model.PriceCatalogPolicy);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PriceCatalogPolicyPromotion)] = typeof(ITS.Retail.Model.PriceCatalogPolicyPromotion);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PriceCatalogDetailTimeValue)] = typeof(ITS.Retail.Model.PriceCatalogDetailTimeValue);
                    mappingDictionary[typeof(ITS.POS.Model.Master.TaxOffice)] = typeof(ITS.Retail.Model.TaxOffice);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionPriceCatalogApplicationRule)] = typeof(ITS.Retail.Model.PromotionPriceCatalogApplicationRule);
                    mappingDictionary[typeof(ITS.POS.Model.Master.PromotionPriceCatalogExecution)] = typeof(ITS.Retail.Model.PromotionPriceCatalogExecution);


                    //Step 4 Perform the object transfer
                    foreach (KeyValuePair<Type, Type> pair in mappingDictionary)
                    {
                        {
                            using (UnitOfWork sourceUow = XpoHelper.GetNewUnitOfWork())
                            {
                                TransferObjects(pair.Value, pair.Key, sourceUow, Store, eUpdateDirection.STORECONTROLLER_TO_POS);
                            }
                        }
                    }

                    Dictionary<string, string> versionValues = new Dictionary<string, string>();
                    using (UnitOfWork POSMasterUow = GetNewUnitOfWork())
                    {
                        foreach (Type typ in mappingDictionary.Keys)
                        {
                            XPCollection objs = new XPCollection(POSMasterUow, typ, null, new SortProperty("UpdatedOnTicks", SortingDirection.Descending));
                            objs.TopReturnedObjects = 1;
                            if (objs.Count == 1)
                            {
                                versionValues[typ.ToString()] = (objs[0] as ITS.POS.Model.Settings.BasicObj).UpdatedOnTicks.ToString();
                            }
                        }
                    }
                    if (versionValues.Count > 0)
                    {
                        POSVersionFile = POSVersionDBPreparationHelper.PreparePOSVersion(Store, path, versionValues);
                    }
                    else
                    {
                        throw new Exception(ResourcesLib.Resources.AnErrorOccurred);
                    }


                }

                //Create Zip File
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }

                if (fDataLayer != null)
                {
                    fDataLayer.Dispose();
                }
                fDataLayer = null;
                foreach (IDisposable disp in _objectsToDisposeOnDisconnect)
                {
                    disp.Dispose();
                }
                // Step 1 Prepare master database connection
                ConnectionString = @"XpoProvider=SQLite;Data Source=" + POSMasterFile;


                using (ZipFile zip = new ZipFile(zipFile))
                {
                    Stream stream = null;
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        // Byte[] POSMasterContent = File.ReadAllBytes(POSMasterFile);
                        stream = File.OpenRead(POSMasterFile);
                        ZipEntry z = zip.AddEntry("POSMaster", stream);
                        Byte[] POSVersionContent = File.ReadAllBytes(POSVersionFile);
                        zip.AddEntry("POSVersions", POSVersionContent);
                    }
                    zip.Save();
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                    }
                }

            }
            catch (Exception e)
            {
                //MvcApplication.Log.Error(e, "POSMasterDBPreparationHelper: PreparePOSMaster");
                MvcApplication.WRMLogModule.Log(e);
                //Logger.Error("POSMasterDBPreparationHelper", "PreparePOSMaster", e.Message, e);
                return e.Message;
            }
            finally
            {
                _IsProcessing = false;

                if (fDataLayer != null)
                {
                    fDataLayer.Dispose();
                }
                fDataLayer = null;
                foreach (IDisposable disp in _objectsToDisposeOnDisconnect)
                {
                    disp.Dispose();
                }

                if (File.Exists(POSMasterFile))
                {
                    File.Delete(POSMasterFile);
                }

                if (File.Exists(POSVersionFile))
                {
                    File.Delete(POSVersionFile);
                }

            }

            //MvcApplication.Log.Info("POSMasterDBPreparationHelper: PreparePOSMaster End");
            MvcApplication.WRMLogModule.Log("POSMasterDBPreparationHelper: PreparePOSMaster End",KernelLogLevel.Info);
            return zipFile;
        }

        private static Type[] numericTypes = new Type[] { typeof(int), typeof(short), typeof(long), typeof(float), typeof(double) };
        
        public static void TransferObjects(Type sourceType, Type destinationType, UnitOfWork sourceUow, Guid store, eUpdateDirection updateDirection)
        {
            // Step 1 Property mapping
            UnitOfWork destinationUow = GetNewUnitOfWork();
            PropertyInfo[] sourceProperties = sourceType.GetProperties();
            PropertyInfo[] destinationProperties = destinationType.GetProperties();
            Dictionary<PropertyInfo, PropertyInfo> propertyDictionary = sourceProperties.Where(g => g.CanWrite == true && g.Name != "Session" && g.Name != "ClassInfo" && g.Name != "IsLoading" &&
                 (g.GetCustomAttributes(typeof(Model.UpdaterIgnoreFieldAttribute), false).Count() == 0 ||
                (g.GetCustomAttributes(typeof(Model.UpdaterIgnoreFieldAttribute), false)
                 .FirstOrDefault() as Model.UpdaterIgnoreFieldAttribute).IgnoreWhenDirection.HasFlag(updateDirection) == false) &&
                g.GetCustomAttributes(typeof(MemberDesignTimeVisibilityAttribute), false).Count() == 0 && destinationProperties.Where(x => x.Name == g.Name).Count() > 0).
                ToDictionary(g => g, g => destinationProperties.Where(x => x.Name == g.Name).First());

            Model.Store Store = sourceUow.GetObjectByKey<Model.Store>(store);
            Model.POS firstPos = sourceUow.FindObject<Model.POS>(new BinaryOperator("Store.Oid", store));

            CriteriaOperator crop = sourceType.GetMethod("GetUpdaterCriteria", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, new object[] { eUpdateDirection.STORECONTROLLER_TO_POS, Store.Owner, Store, firstPos == null ? Guid.Empty.ToString() : firstPos.Oid.ToString() }) as CriteriaOperator;

            XPCursor cursor = new XPCursor(sourceUow, sourceType, CriteriaOperator.And(crop, new BinaryOperator("IsActive", true)));
            cursor.PageSize = 150;
            int counter = 0;
            foreach (object from in cursor)
            {
                object dest = Activator.CreateInstance(destinationType, destinationUow);
                foreach (KeyValuePair<PropertyInfo, PropertyInfo> pair in propertyDictionary)
                {
                    try
                    {
                        object prop = pair.Key.GetValue(from, null);
                        if (prop == null)
                        {
                            pair.Value.SetValue(dest, null, null);
                        }
                        else if (pair.Key.PropertyType.IsSubclassOf(typeof(ITS.Retail.Model.BaseObj)))
                        {
                            if (pair.Value.PropertyType == typeof(Guid))
                            {
                                pair.Value.SetValue(dest, ((ITS.Retail.Model.BaseObj)pair.Key.GetValue(from, null)).Oid, null);
                            }
                            else if (pair.Value.PropertyType.IsSubclassOf(typeof(ITS.POS.Model.Settings.BaseObj)))
                            {
                                Guid objOid = ((ITS.Retail.Model.BaseObj)pair.Key.GetValue(from, null)).Oid;
                                pair.Value.SetValue(dest, destinationUow.GetObjectByKey(pair.Value.PropertyType, objOid), null);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else if (pair.Value.PropertyType.IsAssignableFrom(pair.Key.PropertyType))
                        {
                            pair.Value.SetValue(dest, pair.Key.GetValue(from, null), null);
                        }

                        else if (pair.Key.PropertyType.IsGenericType && pair.Key.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && pair.Value.PropertyType.IsAssignableFrom(Nullable.GetUnderlyingType(pair.Key.PropertyType)))
                        {
                            pair.Value.SetValue(dest, pair.Key.GetValue(from, null), null);

                        }
                        else if (pair.Value.PropertyType == typeof(decimal) &&
                            numericTypes.Contains(pair.Key.PropertyType))
                        {
                            decimal value = Convert.ToDecimal(pair.Key.GetValue(from, null), CultureInfo.InvariantCulture.NumberFormat);
                            pair.Value.SetValue(dest, value, null);
                        }
                        else
                        {
                            //MvcApplication.Log.Debug(String.Format("Property {0} ({1}) of type {2} cannot be assigned to property {3} ({4}) of type {5}",
                            //pair.Key.Name, pair.Key.PropertyType.FullName, sourceType.FullName, pair.Value.Name, pair.Value.PropertyType.FullName, destinationType.FullName));
                            MvcApplication.WRMLogModule.Log(String.Format("Property {0} ({1}) of type {2} cannot be assigned to property {3} ({4}) of type {5}",
                                                                            pair.Key.Name,
                                                                            pair.Key.PropertyType.FullName,
                                                                            sourceType.FullName,
                                                                            pair.Value.Name,
                                                                            pair.Value.PropertyType.FullName,
                                                                            destinationType.FullName
                                                                            ),
                                                                            KernelLogLevel.Debug
                                                                            );
                            throw new NotImplementedException();
                        }
                    }
                    catch (Exception)
                    {
                        //MvcApplication.Log.Debug(String.Format("Property {0} ({1}) of type {2} cannot be assigned to property {3} ({4}) of type {5}",
                        //        pair.Key.Name, pair.Key.PropertyType.FullName, sourceType.FullName, pair.Value.Name, pair.Value.PropertyType.FullName, destinationType.FullName));
                        MvcApplication.WRMLogModule.Log(String.Format("Property {0} ({1}) of type {2} cannot be assigned to property {3} ({4}) of type {5}",
                                                                      pair.Key.Name,
                                                                      pair.Key.PropertyType.FullName,
                                                                      sourceType.FullName,
                                                                      pair.Value.Name,
                                                                      pair.Value.PropertyType.FullName,
                                                                      destinationType.FullName),
                                                          KernelLogLevel.Error);
                    }
                }
                counter++;
                if (counter % MvcApplication.UpdaterBatchSize == 0)
                {
                    XpoHelper.CommitTransaction(destinationUow);
                    destinationUow.Dispose();
                    destinationUow = GetNewUnitOfWork();
                    System.GC.Collect();
                }
            }
            destinationUow.CommitChanges();
            destinationUow.Dispose();
        }


    }
}