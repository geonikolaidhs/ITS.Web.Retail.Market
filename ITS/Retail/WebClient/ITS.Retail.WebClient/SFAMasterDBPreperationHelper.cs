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
using System.Collections;
using Ionic.Zip;
using System.Threading;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using DevExpress.Utils;

using System.Diagnostics;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using SFA_Model;

namespace ITS.Retail.WebClient
{
    internal class CustomSqliteSFAConnectionProvider : SQLiteConnectionProvider
    {


        public CustomSqliteSFAConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
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
                return new CustomSqliteSFAConnectionProvider(connection, autoCreateOption);
            }
            else
            {
                return null;
            }
        }

        public new static string GetConnectionString(string database)
        {
            string pgIntConnString = SQLiteConnectionProvider.GetConnectionString(database);
            string pgFinConnString = pgIntConnString.Replace(SQLiteConnectionProvider.XpoProviderTypeString, CustomSqliteSFAConnectionProvider.XpoProviderTypeString);
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

    public static class SFAMasterDBPreparationHelper
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
        public static IDataLayer DataLayer
        {
            get
            {
                if (SFAMasterDBPreparationHelper.fDataLayer == null)
                {
                    lock (lockObject)
                    {
                        if (SFAMasterDBPreparationHelper.fDataLayer == null)
                        {
                            SFAMasterDBPreparationHelper.fDataLayer = GetDataLayer();
                        }
                    }
                }
                return fDataLayer;
            }
        }

        public static Session GetNewSession()
        {
            return new Session(SFAMasterDBPreparationHelper.DataLayer);
        }

        public static UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(SFAMasterDBPreparationHelper.DataLayer);
        }
        public static IDataLayer GetDataLayer()
        {
            CustomSqliteConnectionProvider.Register();
            XpoDefault.Session = null;
            XPDictionary dict = new ReflectionDictionary();
            IDataStore store = XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.DatabaseAndSchema, out _objectsToDisposeOnDisconnect);
            dict.GetDataStoreSchema(typeof(SFA_Model.Address).Assembly);

            DataCacheRoot cacheRoot = new DataCacheRoot(store);
            DataCacheConfiguration dcc = new DataCacheConfiguration();
            DataCacheNode cacheNode1 = new DataCacheNode(cacheRoot);
            IDataLayer dl = new ThreadSafeDataLayer(dict, cacheNode1);
            return dl;
        }
        public static string ConnectionString = @"CustomSqlite=SQLite;Data Source=ITS_WRM_SFA.db";

        public static bool IsProcessing
        {
            get
            {
                return _IsProcessing;
            }
        }

        private static volatile bool _IsProcessing;

        public static string PrepareSFAMaster(Guid Store, string path)
        {
            string SFAMasterFile = path + "\\ITS_WRM_SFA.db";
            //string POSVersionFile = null;//the filename is the output of the relevant function call
            string zipFile = path + "\\SFADatabase.zip";
            MvcApplication.WRMLogModule.Log("CustomSqliteSFAConnectionProvider PrepareSFAMaster Start", KernelLogLevel.Info);
            try
            {
                _IsProcessing = true;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!File.Exists(SFAMasterFile))
                {
                    using (FileStream fs = File.Create(SFAMasterFile))
                    {

                    }
                }

                lock (lockObjectGeneral)
                {
                    // Step 1 Prepare master database connection
                    ConnectionString = @"XpoProvider=CustomSqlite;Data Source=" + SFAMasterFile;

                    //  Step 2 Clear old connection (if any)
                    if (fDataLayer != null)
                    {
                        fDataLayer.Dispose();
                        fDataLayer = null;
                    }

                    //Step 3 Type mapping                    
                    Dictionary<Type, Type> mappingDictionary = new Dictionary<Type, Type>();
                    mappingDictionary[typeof(SFA_Model.SFA)] = typeof(ITS.Retail.Model.SFA);
                    mappingDictionary[typeof(SFA_Model.Address)] = typeof(ITS.Retail.Model.Address);
                    mappingDictionary[typeof(SFA_Model.Trader)] = typeof(ITS.Retail.Model.Trader);
                    mappingDictionary[typeof(SFA_Model.AddressType)] = typeof(ITS.Retail.Model.AddressType);
                    mappingDictionary[typeof(SFA_Model.Store)] = typeof(ITS.Retail.Model.Store);
                    mappingDictionary[typeof(SFA_Model.StoreDocumentSeriesType)] = typeof(ITS.Retail.Model.StoreDocumentSeriesType);
                    mappingDictionary[typeof(SFA_Model.ItemBarcode)] = typeof(ITS.Retail.Model.ItemBarcode);
                    mappingDictionary[typeof(SFA_Model.Address)] = typeof(ITS.Retail.Model.Address);
                    mappingDictionary[typeof(SFA_Model.Barcode)] = typeof(ITS.Retail.Model.Barcode);
                    mappingDictionary[typeof(SFA_Model.BarcodeType)] = typeof(ITS.Retail.Model.BarcodeType);
                    mappingDictionary[typeof(SFA_Model.Buyer)] = typeof(ITS.Retail.Model.Buyer);
                    mappingDictionary[typeof(SFA_Model.CategoryNode)] = typeof(ITS.Retail.Model.CategoryNode);
                    mappingDictionary[typeof(SFA_Model.CompanyNew)] = typeof(ITS.Retail.Model.CompanyNew);
                    mappingDictionary[typeof(SFA_Model.CouponCategory)] = typeof(ITS.Retail.Model.CouponCategory);
                    mappingDictionary[typeof(SFA_Model.CouponMask)] = typeof(ITS.Retail.Model.CouponMask);
                    mappingDictionary[typeof(SFA_Model.CustomEnumerationDefinition)] = typeof(ITS.Retail.Model.CustomEnumerationDefinition);
                    mappingDictionary[typeof(SFA_Model.CustomEnumerationValue)] = typeof(ITS.Retail.Model.CustomEnumerationValue);
                    mappingDictionary[typeof(SFA_Model.Customer)] = typeof(ITS.Retail.Model.Customer);
                    mappingDictionary[typeof(SFA_Model.DiscountType)] = typeof(ITS.Retail.Model.DiscountType);
                    mappingDictionary[typeof(SFA_Model.Division)] = typeof(ITS.Retail.Model.Division);
                    mappingDictionary[typeof(SFA_Model.DocumentSeries)] = typeof(ITS.Retail.Model.DocumentSeries);
                    mappingDictionary[typeof(SFA_Model.DocumentStatus)] = typeof(ITS.Retail.Model.DocumentStatus);
                    mappingDictionary[typeof(SFA_Model.DocumentType)] = typeof(ITS.Retail.Model.DocumentType);
                    mappingDictionary[typeof(SFA_Model.Item)] = typeof(ITS.Retail.Model.Item);
                    mappingDictionary[typeof(SFA_Model.ItemAnalyticTree)] = typeof(ITS.Retail.Model.ItemAnalyticTree);
                    mappingDictionary[typeof(SFA_Model.ItemCategory)] = typeof(ITS.Retail.Model.ItemCategory);
                    mappingDictionary[typeof(SFA_Model.ItemExtraInfo)] = typeof(ITS.Retail.Model.ItemExtraInfo);
                    mappingDictionary[typeof(SFA_Model.LinkedItem)] = typeof(ITS.Retail.Model.LinkedItem);
                    mappingDictionary[typeof(SFA_Model.MeasurementUnit)] = typeof(ITS.Retail.Model.MeasurementUnit);
                    mappingDictionary[typeof(SFA_Model.MinistryDocumentType)] = typeof(ITS.Retail.Model.MinistryDocumentType);
                    mappingDictionary[typeof(SFA_Model.OwnerApplicationSettings)] = typeof(ITS.Retail.Model.OwnerApplicationSettings);
                    mappingDictionary[typeof(SFA_Model.PaymentMethod)] = typeof(ITS.Retail.Model.PaymentMethod);
                    mappingDictionary[typeof(SFA_Model.PaymentMethodField)] = typeof(ITS.Retail.Model.PaymentMethodField);
                    mappingDictionary[typeof(SFA_Model.Phone)] = typeof(ITS.Retail.Model.Phone);
                    mappingDictionary[typeof(SFA_Model.PhoneType)] = typeof(ITS.Retail.Model.PhoneType);
                    mappingDictionary[typeof(SFA_Model.PriceCatalog)] = typeof(ITS.Retail.Model.PriceCatalog);
                    mappingDictionary[typeof(SFA_Model.PriceCatalogDetail)] = typeof(ITS.Retail.Model.PriceCatalogDetail);
                    mappingDictionary[typeof(SFA_Model.PriceCatalogDetailTimeValue)] = typeof(ITS.Retail.Model.PriceCatalogDetailTimeValue);
                    mappingDictionary[typeof(SFA_Model.PriceCatalogPolicy)] = typeof(ITS.Retail.Model.PriceCatalogPolicy);
                    mappingDictionary[typeof(SFA_Model.PriceCatalogPolicyDetail)] = typeof(ITS.Retail.Model.PriceCatalogPolicyDetail);
                    mappingDictionary[typeof(SFA_Model.Reason)] = typeof(ITS.Retail.Model.Reason);
                    mappingDictionary[typeof(SFA_Model.ReasonCategory)] = typeof(ITS.Retail.Model.ReasonCategory);
                    mappingDictionary[typeof(SFA_Model.SpecialItem)] = typeof(ITS.Retail.Model.SpecialItem);
                    mappingDictionary[typeof(SFA_Model.StorePriceCatalogPolicy)] = typeof(ITS.Retail.Model.StorePriceCatalogPolicy);
                    mappingDictionary[typeof(SFA_Model.StorePriceList)] = typeof(ITS.Retail.Model.StorePriceList);
                    mappingDictionary[typeof(SFA_Model.Supplier)] = typeof(ITS.Retail.Model.SupplierNew);
                    mappingDictionary[typeof(SFA_Model.TaxOffice)] = typeof(ITS.Retail.Model.TaxOffice);
                    mappingDictionary[typeof(SFA_Model.TransactionCoupon)] = typeof(ITS.Retail.Model.TransactionCoupon);
                    mappingDictionary[typeof(SFA_Model.User)] = typeof(ITS.Retail.Model.User);
                    mappingDictionary[typeof(SFA_Model.VatCategory)] = typeof(ITS.Retail.Model.VatCategory);
                    mappingDictionary[typeof(SFA_Model.VatFactor)] = typeof(ITS.Retail.Model.VatFactor);
                    mappingDictionary[typeof(SFA_Model.VatLevel)] = typeof(ITS.Retail.Model.VatLevel);
                    mappingDictionary[typeof(SFA_Model.POSPrintFormat)] = typeof(ITS.Retail.Model.POSPrintFormat);
                    mappingDictionary[typeof(SFA_Model.SpecialItem)] = typeof(ITS.Retail.Model.SpecialItem);

                    foreach (KeyValuePair<Type, Type> pair in mappingDictionary.Where(p => p.Key.GetCustomAttributes<CreateOrUpdaterOrderAttribute>().Count() > 0).OrderBy(typ => typ.Key.GetCustomAttributes<CreateOrUpdaterOrderAttribute>().First().Order).ToList())
                    {

                        using (UnitOfWork sourceUow = XpoHelper.GetReadOnlyUnitOfWork())
                        {
                            TransferObjects(pair.Value, pair.Key, sourceUow, Store, eUpdateDirection.MASTER_TO_SFA);
                        }
                    }
                }

                //Create Zip File
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }
                try
                {
                    if (fDataLayer != null)
                    {
                        fDataLayer.Dispose();
                    }
                    fDataLayer = null;
                    foreach (IDisposable disp in _objectsToDisposeOnDisconnect)
                    {
                        disp.Dispose();
                    }
                }
                catch (Exception ex) { }
                // Step 1 Prepare master database connection
                ConnectionString = @"XpoProvider=SQLite;Data Source=" + SFAMasterFile;
                using (ZipFile zip = new ZipFile(zipFile))
                {
                    Stream stream = null;
                    stream = File.OpenRead(SFAMasterFile);
                    ZipEntry z = zip.AddEntry("ITS_WRM_SFA.db", stream);
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

                MvcApplication.WRMLogModule.Log(e);
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
                if (_objectsToDisposeOnDisconnect != null)
                {
                    foreach (IDisposable disp in _objectsToDisposeOnDisconnect)
                    {
                        disp.Dispose();
                    }

                }
                if (File.Exists(SFAMasterFile))
                {
                    File.Delete(SFAMasterFile);
                }

            }
            MvcApplication.WRMLogModule.Log("CustomSqliteSFAConnectionProvider: PrepareSFAMaster End", KernelLogLevel.Info);
            return zipFile;
        }

        private static Type[] numericTypes = new Type[] { typeof(int), typeof(short), typeof(long), typeof(float), typeof(double) };

        public static void TransferObjects(Type sourceType, Type destinationType, UnitOfWork sourceUow, Guid store, eUpdateDirection updateDirection)
        {
            List<string> ignoreProperties = new List<string>()
            {
                "Session",
                "ClassInfo",
                "IsLoading",
                "Loading",
                "MasterObjOid",
                "SkipOnSavingProcess",
                "Loading",
                "IsLoading",
                "TempObjExists",
                "CreatedOn",
                "UpdatedOn",
                "ObjectSignature",
                "IsDeleted",
                "MLValues",
                "GCRecord",
                "OptimisticLockField",
            };

            // Step 1 Property mapping
            UnitOfWork destinationUow = GetNewUnitOfWork();
            PropertyInfo[] sourceProperties = sourceType.GetProperties().Where(x => ignoreProperties.Contains(x.Name) == false && x.CanWrite == true).ToArray();
            PropertyInfo[] destinationProperties = destinationType.GetProperties().Where(x => ignoreProperties.Contains(x.Name) == false).ToArray();

            Dictionary<PropertyInfo, PropertyInfo> propertyDictionary = sourceProperties.Where(g => (g.GetCustomAttributes(typeof(Model.UpdaterIgnoreFieldAttribute), false).Count() == 0
            || (g.GetCustomAttributes(typeof(Model.UpdaterIgnoreFieldAttribute), false)
                 .FirstOrDefault() as Model.UpdaterIgnoreFieldAttribute).IgnoreWhenDirection.HasFlag(updateDirection) == false) &&
                g.GetCustomAttributes(typeof(MemberDesignTimeVisibilityAttribute), false).Count() == 0 && destinationProperties.Where(x => x.Name == g.Name).Count() > 0).
                ToDictionary(g => g, g => destinationProperties.Where(x => x.Name == g.Name).First());

            Model.Store Store = sourceUow.GetObjectByKey<Model.Store>(store);
            Model.POS firstPos = sourceUow.FindObject<Model.POS>(new BinaryOperator("Store.Oid", store));

            CriteriaOperator crop = sourceType.GetMethod("GetUpdaterCriteria", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, new object[]
                                                        { eUpdateDirection.MASTER_TO_SFA, Store.Owner, Store, firstPos == null ? Guid.Empty.ToString() : firstPos.Oid.ToString() }) as CriteriaOperator;


            XPCursor cursor = new XPCursor(sourceUow, sourceType, CriteriaOperator.And(crop, new BinaryOperator("IsActive", true)));
            cursor.PageSize = 150;
            Type[] args = new Type[] { typeof(Session) };
            var myConstructor = BasicObj.CreateConstructor(destinationType, args);
            int counter = 0;
            long maxVersion = 0;
            TableVersion tversion = null;
            foreach (object from in cursor)
            {

                object dest = myConstructor(destinationUow);
                foreach (KeyValuePair<PropertyInfo, PropertyInfo> pair in propertyDictionary)
                {
                    try
                    {
                        if (pair.Key.Name == "UpdatedOnTicks")
                        {
                            long objVersion = ((ITS.Retail.Model.BaseObj)from as ITS.Retail.Model.BaseObj)?.UpdatedOnTicks ?? 0;
                            maxVersion = objVersion > maxVersion ? objVersion : maxVersion;
                        }
                        object prop = pair.Key.GetValue(from, null);
                        if (prop == null)
                        {
                            pair.Value.SetValue(dest, null, null);
                        }
                        if (ignoreProperties.Contains(pair.Key.Name) || ignoreProperties.Contains(pair.Value.Name))
                        {
                            continue;
                        }
                        else if (pair.Key.PropertyType.IsSubclassOf(typeof(ITS.Retail.Model.BaseObj)))
                        {
                            if (pair.Key.Name.Contains("DefaultCustomReport"))
                            {
                                continue;
                            }
                            if (pair.Value.PropertyType == typeof(Guid))
                            {
                                if (pair.Value.Name == "CreatedBy")
                                {
                                    pair.Value.SetValue(dest, ((ITS.Retail.Model.BaseObj)from as ITS.Retail.Model.BaseObj)?.CreatedBy?.Oid ?? Guid.Empty, null);
                                }
                                else if (pair.Value.Name == "UpdatedBy")
                                {
                                    ITS.Retail.Model.BaseObj baseobj = ((ITS.Retail.Model.BaseObj)pair.Key.GetValue(from, null)) as ITS.Retail.Model.BaseObj;
                                    if (baseobj == null || baseobj.UpdatedBy == null || baseobj.UpdatedBy.Oid == Guid.Empty)
                                        continue;
                                    pair.Value.SetValue(dest, baseobj.UpdatedBy.Oid, null);
                                }
                                else
                                {
                                    ITS.Retail.Model.BaseObj baseobj = ((ITS.Retail.Model.BaseObj)pair.Key.GetValue(from, null)) as ITS.Retail.Model.BaseObj;
                                    if (baseobj == null || baseobj.Oid == Guid.Empty || baseobj.Oid == null)
                                        continue;
                                    pair.Value.SetValue(dest, baseobj.Oid, null);
                                }
                            }
                            else// if (pair.Value.PropertyType.IsSubclassOf(typeof(ITS.Retail.WebClient.SFA_Model.NonPersistant.BaseObj)))
                            {
                                if (pair.Key.Name.Contains("DefaultCustomReport"))
                                {
                                    continue;
                                }
                                ITS.Retail.Model.BaseObj baseobj = ((ITS.Retail.Model.BaseObj)pair.Key.GetValue(from, null)) as ITS.Retail.Model.BaseObj;
                                if (baseobj == null || baseobj.Oid == Guid.Empty || baseobj.Oid == null)
                                    continue;

                                pair.Value.SetValue(dest, baseobj.Oid, null);
                            }
                            //else
                            //{
                            //    throw new NotImplementedException();
                            //}
                        }
                        else if (pair.Value.PropertyType.IsAssignableFrom(pair.Key.PropertyType))
                        {
                            object value = pair.Key.GetValue(from, null);
                            Type nullableUnderlyingType = Nullable.GetUnderlyingType(pair.Value.PropertyType);
                            if (nullableUnderlyingType != null)
                            {
                                //Type propertyType = nullableUnderlyingType ?? pair.Value.PropertyType;
                                object safeValue = null;
                                try
                                {
                                    safeValue = (value == null) ? null : Convert.ChangeType(value, nullableUnderlyingType);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(String.Format("Conversion of value '{0}' failed. Expected Type '{1}', Actual Type '{2}', Error: '{3}'",
                                                                       value, nullableUnderlyingType.Name, (value == null ? "null" : value.GetType().Name), ex.Message), ex);
                                }
                                pair.Value.SetValue(dest, safeValue, null);
                            }
                            else
                            {
                                pair.Value.SetValue(dest, pair.Key.GetValue(from, null), null);
                            }
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
                            throw new NotImplementedException();
                        }
                    }
                    catch (Exception e)
                    {
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
                if (counter % 1000 == 0)
                {

                    destinationUow.CommitChanges();
                    destinationUow.Dispose();
                    GC.Collect();
                    destinationUow = GetNewUnitOfWork();
                }
            }

            destinationUow.CommitChanges();
            destinationUow.Dispose();
            destinationUow = GetNewUnitOfWork();
            tversion = destinationUow.FindObject<TableVersion>(CriteriaOperator.And("TableName", destinationType.Name));
            if (tversion == null)
            {
                tversion = new TableVersion(destinationUow) { Version = maxVersion, TableName = destinationType.Name, Oid = Guid.NewGuid(), UpdatedOnTicks = DateTime.Now.Ticks };
            }
            else
            {
                tversion.UpdatedOnTicks = DateTime.Now.Ticks;
                tversion.Version = maxVersion;
            }
            destinationUow.CommitChanges();
            destinationUow.Dispose();
            System.GC.Collect();
        }


    }


}
