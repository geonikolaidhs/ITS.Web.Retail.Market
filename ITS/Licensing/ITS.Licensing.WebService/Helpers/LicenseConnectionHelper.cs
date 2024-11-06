using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using ITS.Licensing.Web.AuxilliaryClasses;
using System.Reflection;
using DevExpress.Data.Filtering;

namespace ITS.Licensing.Web.Helpers
{
    public static class LicenseConnectionHelper
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
        public static string ConnectionString = ConnectionSettings.LicenseDB.ConnectionString();//@"XpoProvider=SQLite;Data Source=PosMaster";//TODO PATATA
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
            dict.GetDataStoreSchema(typeof(LicenseModel.License).Assembly);
            IDataLayer dl = new ThreadSafeDataLayer(dict, store);
            return dl;
        }
        public static Session GetNewSession()
        {
            return new Session(DataLayer);
        }


        public static void CopyObject<T>(T input, ref T output) where T : XPCustomObject
        {
            IList<PropertyInfo> props = new List<PropertyInfo>(input.GetType().GetProperties());
            foreach (PropertyInfo prop in props)
            {
                string propName = prop.Name;
                if (propName != "Session" && propName != "ClassInfo" && propName != "Loading" && propName != "IsLoading" && propName != "IsDeleted" && propName != "OptimisticLockField" && propName != "This")
                {
                    object propValue = prop.GetValue(input, null);
                    if (!(propValue is XPBaseCollection))
                    {
                        if (propValue is XPCustomObject)
                        {

                            XPCustomObject x = output.Session.FindObject(propValue.GetType(), new BinaryOperator("Oid", propValue.GetType().GetProperty("Oid").GetValue(propValue, null))) as XPCustomObject;
                            prop.SetValue(output, x, null);
                        }
                        else
                        {
                            prop.SetValue(output, propValue, null);
                        }
                    }
                }
            }
        }

        public static UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }
    }
}