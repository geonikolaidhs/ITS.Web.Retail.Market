using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.DB;
using ITS.POS.Model.Transactions;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public static class TransactionConnectionHelper
    {
        private readonly static object lockObject = new object();

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

        public static void ClearDataLayer()
        {
            if (fDataLayer != null)
            {
                lock (lockObject)
                {
                    fDataLayer = null;
                }
            }
        }

        private static string ConnectionString;
        public static UnitOfWork OpenFile(string file)
        {
            SetTransactionFile(file);
            ClearDataLayer();
            return GetNewUnitOfWork();
        }

        public static void SetTransactionFile(string file)
        {
            ConnectionString = @"XpoProvider=SQLite;Data Source=" + file;
        }

        public static void Connect(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption)
        {
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(ConnectionString, autoCreateOption);
            XpoDefault.Session = null;
        }
        public static IDataStore GetConnectionProvider(AutoCreateOption autoCreateOption)
        {
            return XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption);
        }
        public static IDataStore GetConnectionProvider(AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect)
        {
            return XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
        }
        public static IDataLayer GetDataLayer(AutoCreateOption autoCreateOption)
        {
            return XpoDefault.GetDataLayer(ConnectionString, autoCreateOption);
        }

        public static XPDictionary dictionary
        {
            get;
            private set;
        }

        private static IDataLayer GetDataLayer()
        {
            XpoDefault.Session = null;
            dictionary = new ReflectionDictionary();
            IDataStore store = XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.DatabaseAndSchema, out _objectsToDisposeOnDisconnect);
            dictionary.GetDataStoreSchema(typeof(DocumentHeader).Assembly);
            IDataLayer dl = new ThreadSafeDataLayer(dictionary, store);
            return dl;
        }
        public static Session GetNewSession()
        {
            return new Session(DataLayer);
        }

        public static UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }

        #region Release resources
        public static void ReleaseResources(){
            fDataLayer.Dispose();
            foreach (IDisposable disp in _objectsToDisposeOnDisconnect)
            {
                disp.Dispose();
            }
        }
        #endregion
    }
}
