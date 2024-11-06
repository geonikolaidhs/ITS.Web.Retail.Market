using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Common;
using ITS.Retail.Model;
using StoreControllerReconstructor.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace StoreControllerReconstructor.ConnectionHelpers
{
    public class XpoLayerHelper
    {
        public string SQLServer { get; set; }
        
        public string Username { get; set; }

        
        public string Password { get; set; }

        
        public string Database { get; set; }

        public DatabaseType Databasetype { get; set; }

        public XpoLayerHelper(SQLConnectionSettings sqlConnectionSettings)
        {
            this.Databasetype = sqlConnectionSettings.DatabaseType;
            this.SQLServer = sqlConnectionSettings.Server;
            this.Database = sqlConnectionSettings.Database;
            this.Username = sqlConnectionSettings.Username;
            this.Password = sqlConnectionSettings.Password;            
        }

        public void CommitChanges(UnitOfWork uow)
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

        private bool TypeToPhysicallyDeleted(Type t)
        {
            return (t.GetCustomAttributes(typeof(PhysicalDeleteAttribute), false).Count() != 0);
        }

        public void CommitTransaction(Session uow)
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

        public Session GetNewSession()
        {
            return new Session(DataLayer);
        }

        public UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }

        private readonly object lockObject = new object();

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
                            bool isTestConnectionCheck = false;
                            fDataLayer = GetDataLayer(isTestConnectionCheck);
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
            switch (Databasetype)
            {
                case DatabaseType.Postgres:
                    return PostgreSqlConnectionProvider.GetConnectionString(SQLServer, Username, Password, Database);
                case DatabaseType.MS_SQL:
                default:
                    return CustomMSSQLServerConnectionProvider.GetConnectionString(SQLServer, Username, Password, Database);
            }
        }
        public DataCacheRoot CacheRoot { get; private set; }

        public IDataLayer GetDataLayer(bool isTestConnectionCheck)
        {
            try
            {
                XpoDefault.Session = null;
                string conn = GetConnectionString();
                
                dict = new ReflectionDictionary();
                IDataStore store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.None);

                if (isTestConnectionCheck == true) // multiple message boxes fix
                {
                    MessageBox.Show("Connection success.", "Connection Information");
                }
                return new ThreadSafeDataLayer(dict, store);                              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed.", "Connection Information");
                throw;               
            }
        }

        private void ClearDataLayer()
        {
            if (this.fDataLayer != null)
            {
                this.fDataLayer.Dispose();
                this.fDataLayer = null;
            }
        }

        public void UpdateDatabase()
        {
            this.ClearDataLayer();
            using (IDataLayer dal = XpoDefault.GetDataLayer(GetConnectionString(), AutoCreateOption.DatabaseAndSchema))
            {
                using (UnitOfWork session = new UnitOfWork(dal))
                {
                    Assembly asm = Assembly.GetAssembly(typeof(Customer));
                    session.UpdateSchema(asm);
                    session.CreateObjectTypeRecords(asm);
                    session.FlushChanges();
                    session.CommitChanges();
                    this.ClearDataLayer();
                }
            }
        }
    }
}
