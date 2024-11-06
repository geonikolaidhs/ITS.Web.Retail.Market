using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using ITS.Licensing.LicenseModel;
using System.Reflection;
using System.ComponentModel;
using DevExpress.Data.Filtering;

namespace ITS.Licensing.Web
{
    public static class XpoHelper
    {

        public static string sqlserver;
        public static string username;
        public static string pass;
        public static string database;

        public static Session GetNewSession()
        {
            return new Session(DataLayer);
        }

        public static UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }

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


        private static IDataLayer GetDataLayer()
        {
            XpoDefault.Session = null;
            string conn = MSSqlConnectionProvider.GetConnectionString(sqlserver, username, pass, database);
            XPDictionary dict = new ReflectionDictionary();
            IDataStore store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.DatabaseAndSchema);
            DataCacheRoot cacheRoot = new DataCacheRoot(store);
            DataCacheNode cacheNode1 = new DataCacheNode(cacheRoot);

            dict.GetDataStoreSchema(typeof(Customer).Assembly);
            IDataLayer dl = new ThreadSafeDataLayer(dict, cacheNode1);
            return dl;
        }

        public static void UpdateDatabase()
        {
            using (IDataLayer dal = XpoDefault.GetDataLayer(MSSqlConnectionProvider.GetConnectionString(XpoHelper.sqlserver, XpoHelper.username, XpoHelper.pass, XpoHelper.database),
                AutoCreateOption.DatabaseAndSchema))
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
    }
}