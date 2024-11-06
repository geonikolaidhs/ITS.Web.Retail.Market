using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Licensing.Web.AuxilliaryClasses;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace ITS.Licensing.Web.Helpers
{
    public static class RetailConnectionHelper
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
        public static string ConnectionString = ConnectionSettings.RetailDB.ConnectionString();//@"XpoProvider=SQLite;Data Source=PosMaster";//TODO PATATA
        public static void Connect(AutoCreateOption autoCreateOption)
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
            IDataStore store = XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.DatabaseAndSchema);
            dict.GetDataStoreSchema(typeof(RetailModel.Barcode).Assembly);
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