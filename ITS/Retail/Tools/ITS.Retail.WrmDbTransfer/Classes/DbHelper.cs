using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Common;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ITS.Retail.WrmDbTransfer.Classes
{
    public class DbHelper
    {
        private readonly static object lockObject = new object();

        public string Server { get; set; }
        public DatabaseType DBType { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public DbHelper(string server, string database, string username, string password, DatabaseType type)
        {
            Server = server;
            Database = database;
            Username = username;
            Password = password;
            DBType = type;
        }

        public UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }

        public void ResetDataLayer()
        {
            lock (lockObject)
            {
                if (fDataLayer != null)
                {
                    fDataLayer = null;
                }
            }
        }



        volatile IDataLayer fDataLayer;

        public IDataLayer DataLayer
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

        public XPDictionary dict
        {
            get;
            private set;
        }


        private string GetConnectionString()
        {
            switch (DBType)
            {
                case DatabaseType.Oracle:
                    string conString = string.Format(@"User Id={2};Password={3};Data Source=" + @" (DESCRIPTION = (ADDRESS = (PROTOCOL = tcp)(HOST = {0})(PORT = 1521))(CONNECT_DATA = (SERVICE_NAME = {1})))", Server, Database, Username, Password);
                    conString = "XpoProvider=Oracle;" + conString;
                    return conString; ;
                case DatabaseType.MySQL:
                    return MySqlConnectionProvider.GetConnectionString(Server, Username, Password, Database);
                case DatabaseType.Sqlite:
                    return SQLiteConnectionProvider.GetConnectionString(Database, Password);
                case DatabaseType.Postgres:
                    return CustomPostgreSqlConnectionProvider.GetConnectionString(Server, Username, Password, Database);
                case DatabaseType.SQLServer:
                default:
                    return CustomMSSQLServerConnectionProvider.GetConnectionString(Server, Username, Password, Database);
            }
        }


        public DataCacheRoot CacheRoot { get; private set; }

        private IDataLayer GetDataLayer()
        {
            try
            {
                CustomPostgreSqlConnectionProvider.Register();
                XpoDefault.Session = null;
                string conn = "";
                IDataStore store = null;
                conn = GetConnectionString();
                dict = new ReflectionDictionary();
                if (store == null)
                {
                    store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.DatabaseAndSchema);//???
                }

                CacheRoot = new DataCacheRoot(store);
                DataCacheConfiguration dcc = new DataCacheConfiguration();
                DataCacheNode cacheNode1 = new DataCacheNode(CacheRoot);
                //cacheNode1.TotalMemoryPurgeThreshold = 512 * 1024 * 1024L;
                Assembly asm = Assembly.GetAssembly(typeof(Customer));
                dict.GetDataStoreSchema(typeof(Customer).Assembly);
                IDataLayer dl = new ThreadSafeDataLayer(dict, cacheNode1);
                return dl;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public void UpdateDatabase()
        {
            CustomPostgreSqlConnectionProvider.Register();
            string connectionString = GetConnectionString();
            using (IDataLayer dal = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema))
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

        public IDbConnection GetConnection()
        {
            string connection_string = GetConnectionStringForIDbConnection();
            switch (DBType)
            {
                case DatabaseType.Postgres:
                    return CustomPostgreSqlConnectionProvider.CreateConnection(connection_string);
                case DatabaseType.Oracle:
                    return new OracleConnection() { ConnectionString = connection_string };
                case DatabaseType.MySQL:
                    return new SqlConnection(connection_string);
                default:
                    return new SqlConnection(connection_string);
            }
        }

        public string GetConnectionStringForIDbConnection()
        {
            string connstring = GetConnectionString();
            String[] toks = connstring.Split(';');
            return toks.Where(g => g.Contains("XpoProvider") == false).Aggregate((f, s) => f + ";" + s);
        }


    }
}
