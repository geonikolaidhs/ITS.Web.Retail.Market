using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Common;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient
{
    public static class POSVersionDBPreparationHelper
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
            XpoDefault.Session = null;
            XPDictionary dict = new ReflectionDictionary();
            IDataStore store = XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.DatabaseAndSchema, out _objectsToDisposeOnDisconnect);
            dict.GetDataStoreSchema(typeof(ITS.POS.Model.Versions.TableVersions).Assembly);

            DataCacheRoot cacheRoot = new DataCacheRoot(store);
            DataCacheConfiguration dcc = new DataCacheConfiguration();
            DataCacheNode cacheNode1 = new DataCacheNode(cacheRoot);
            IDataLayer dl = new ThreadSafeDataLayer(dict, cacheNode1);
            return dl;
        }
        public static string ConnectionString = @"XpoProvider=SQLite;Data Source=POSVersions";

        public static string PreparePOSVersion(Guid Store, string path, Dictionary<string, string> values)
        {
            string VersionDBFile = path + "\\POSVersions";
            try
            {
                if (values.Count <= 0)
                {
                    throw new Exception(ResourcesLib.Resources.AnErrorOccurred);
                }

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }



                if (!File.Exists(VersionDBFile))
                {
                    //File.Delete(VersionDBFile);

                    using (FileStream fs = File.Create(VersionDBFile))
                    {
                    }
                }

                lock (lockObjectGeneral)
                {
                    //Step 1 Prepare database connection
                    ConnectionString = @"XpoProvider=SQLite;Data Source=" + VersionDBFile;

                    //Step 2 Reset connection
                    if (fDataLayer != null)
                    {
                        fDataLayer.Dispose();
                        fDataLayer = null;
                    }

                    //Save Versions
                    using (UnitOfWork uow = GetNewUnitOfWork())
                    {
                        foreach (KeyValuePair<string, string> pair in values)
                        {
                            ITS.POS.Model.Versions.TableVersions version = new ITS.POS.Model.Versions.TableVersions(uow);
                            version.EntityName = pair.Key;
                            version.Number = new DateTime(long.Parse(pair.Value));
                            version.Save();
                        }
                        XpoHelper.CommitChanges(uow);
                    }
                }
            }
            catch (Exception e)
            {
                MvcApplication.WRMLogModule.Log(e, "POSVersionDBPreparationHelper:PreparePOSVersion",KernelLogLevel.Error);
                throw;
            }
            finally
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
            return VersionDBFile;
        }
    }
}