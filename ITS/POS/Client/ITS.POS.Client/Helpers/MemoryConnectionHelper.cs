using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.DB;
using ITS.POS.Model.Transactions;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Helpers
{
    public static class MemoryConnectionHelper
    {
        private readonly static object lockObject = new object();

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
        public const string ConnectionString = @"XpoProvider=SQLite;Data Source=Memory";
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
        private static IDataLayer GetDataLayer()
        {
            XpoDefault.Session = null;
            XPDictionary dict = new ReflectionDictionary();
            IDataStore store = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema);
            dict.CollectClassInfos(typeof(POSKeyMapping).Assembly);
            IDataLayer dl = new ThreadSafeDataLayer(dict, store);
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
    }
}
