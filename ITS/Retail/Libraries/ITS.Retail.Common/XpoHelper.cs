using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

public enum DBType
{
    SQLServer,
    Memory,
    postgres,
    sqlite,
    MySQL,
    Oracle,
    Remote
}

namespace ITS.Retail.Common
{
    [Description("Ρυθμίσεις σύνδεσης")]
    public static class XpoHelper
    {
        [Description("Database server")]
        public static string sqlserver { get; set; }

        private static string _uname;
        [Description("Username")]
        public static string username
        {
            get
            {
                return _uname;
            }
            set
            {
                _uname = value;
            }
        }

        [Description("Password")]
        public static string pass { get; set; }

        [Description("Database Name")]
        public static string database { get; set; }

        [Browsable(false)]
        public static string caburi;

        [Description("Db Type")]//: SQLServer, Memory, postgres, sqlite, MySQL, Oracle")]
        public static DBType databasetype { get; set; }

        [Browsable(false)]
        public static long IISCache;

        [Browsable(false)]
        public static bool DisableCache;

        public static string WEBServiceURI { get; set; }

        public static string HQURI { get; set; }

        public static void CommitChanges(UnitOfWork uow)
        {
            try
            {
                var v = uow.GetObjectsToSave().Cast<object>().Where(g => g is BaseObj).Cast<BaseObj>().ToList();
                foreach (BaseObj o in v)
                {
                    if (o.IsDeleted && uow.IsNewObject(o))
                    {
                        uow.RemoveFromSaveList(o);
                    }
                    else if (o.IsDeleted && TypeToPhysicallyDeleted(o.GetType()))
                    {
                        (uow.GetObjectsToDelete() as DevExpress.Xpo.Helpers.ObjectSet).Add(o);
                    }
                }
                uow.CommitChanges();
            }
            catch (LockingException ex)
            {
                string error = ex.GetFullMessage();
            }
            catch (ObjectDisposedException ex)
            {
                string error = ex.GetFullMessage();
            }

        }

        private static bool TypeToPhysicallyDeleted(Type t)
        {
            return (t.GetCustomAttributes(typeof(PhysicalDeleteAttribute), false).Count() != 0);
        }

        public static void CommitTransaction(Session uow)
        {
            try
            {
                List<object> objectsToSave = uow.GetObjectsToSave().Cast<object>().Where(G => G is BaseObj).ToList();
                foreach (BaseObj o in objectsToSave)
                {
                    if (o.IsDeleted && uow.IsNewObject(o))
                    {
                        uow.RemoveFromSaveList(o);
                    }
                    else if (o.IsDeleted && TypeToPhysicallyDeleted(o.GetType()))
                    {
                        (uow.GetObjectsToDelete() as DevExpress.Xpo.Helpers.ObjectSet).Add(o);
                    }
                }
                uow.CommitTransaction();
            }
            catch (LockingException ex)
            {
                string error = ex.GetFullMessage();
            }
            catch (ObjectDisposedException ex)
            {
                string error = ex.GetFullMessage();
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

        public static UnitOfWork GetReadOnlyUnitOfWork()
        {
            UnitOfWork Session = new UnitOfWork(DataLayer);
            Session.IsObjectModifiedOnNonPersistentPropertyChange = false;
            Session.LockingOption = LockingOption.None;
            Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.Ignore;
            Session.TrackPropertiesModifications = false;
            Session.IdentityMapBehavior = IdentityMapBehavior.Weak;
            return Session;
        }

        private readonly static object lockObject = new object();

        public static void ResetDataLayer()
        {
            lock (lockObject)
            {
                if (fDataLayer != null)
                {
                    fDataLayer = null;
                }
            }
        }

        static volatile IDataLayer fDataLayer;
        [Browsable(false)]
        public static IDataLayer DataLayer
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

        [Browsable(false)]
        public static XPDictionary dict
        {
            get;
            private set;
        }

        private static string GetConnectionString()
        {
            switch (databasetype)
            {
                case DBType.Oracle:
                    return @"XpoProvider=Oracle;" + string.Format(@"User Id={2};Password={3};Data Source=" + @" (DESCRIPTION = (ADDRESS = (PROTOCOL = tcp)(HOST = {0})(PORT = 1521))(CONNECT_DATA = (SERVICE_NAME = {1})))", XpoHelper.sqlserver, XpoHelper.database, XpoHelper.username, XpoHelper.pass);
                case DBType.MySQL:
                    return MySqlConnectionProvider.GetConnectionString(sqlserver, username, pass, database);
                case DBType.sqlite:
                    return SQLiteConnectionProvider.GetConnectionString(database, pass);
                case DBType.postgres:
                    return CustomPostgreSqlConnectionProvider.GetConnectionString(sqlserver, username, pass, database);
                case DBType.SQLServer:
                default:
                    return MSSqlConnectionProvider.GetConnectionString(sqlserver, username, pass, database);
            }
        }

        [Browsable(false)]
        public static DataCacheRoot CacheRoot { get; private set; }

        private static IDataLayer GetDataLayer()
        {
            try
            {
                if (databasetype == DBType.Remote)
                {
                    return XpoDefault.GetDataLayer(WEBServiceURI, AutoCreateOption.None);
                }
                CustomPostgreSqlConnectionProvider.Register();
                XpoDefault.Session = null;
                string conn = "";
                IDataStore store = null;

                if (databasetype == DBType.Memory)
                {
                    store = new InMemoryDataStore();
                }
                else
                {
                    conn = GetConnectionString();
                }
                dict = new ReflectionDictionary();
                if (store == null)
                {
                    store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.DatabaseAndSchema);//???
                }

                if (DisableCache)
                {
                    IDataLayer dl = new ThreadSafeDataLayer(dict, store);
                    return dl;
                }
                else
                {
                    CacheRoot = new DataCacheRoot(store);
                    DataCacheConfiguration dcc = new DataCacheConfiguration();
                    DataCacheNode cacheNode1 = new DataCacheNode(CacheRoot);

                    // TODO να μπει σαν setting

                    if (IISCache > 0)
                    {
                        cacheNode1.TotalMemoryPurgeThreshold = IISCache * 1024 * 1024L;
                    }
                    Assembly asm = Assembly.GetAssembly(typeof(Customer));
                    dict.GetDataStoreSchema(typeof(Customer).Assembly);
                    IDataLayer dl = new ThreadSafeDataLayer(dict, cacheNode1);
                    return dl;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void UpdateDatabase()
        {
            CustomPostgreSqlConnectionProvider.Register();
            using (IDataLayer dal = XpoDefault.GetDataLayer(GetConnectionString(), AutoCreateOption.DatabaseAndSchema))
            {
                using (UnitOfWork session = new UnitOfWork(dal))
                {
                    Assembly asm = Assembly.GetAssembly(typeof(Customer));
                    session.UpdateSchema(asm);
                    session.CreateObjectTypeRecords(asm);
                    session.FlushChanges();
                    session.CommitChanges();
                }
            }
        }
    }
}
