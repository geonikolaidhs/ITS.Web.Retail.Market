using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;

namespace ITS.Retail.ScanTool.Model
{
    [Description("Ρυθμίσεις σύνδεσης")]
    public static class XpoHelper
    {



        public static Session GetNewSession()
        {
            return new Session(DataLayer);
        }

        public static UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
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

        [Browsable(false)]
        public static XPDictionary dict
        {
            get;
            private set;
        }

        private static string GetConnectionString()
        {
            return SQLiteConnectionProvider.GetConnectionString("ITS.Retail.ScanTool.sqlite");
        }

        private static IDataLayer GetDataLayer()
        {
            XpoDefault.Session = null;
            string conn = "";
            IDataStore store = null;

            conn = GetConnectionString();
            dict = new ReflectionDictionary();
            if (store == null)
            {
                store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.DatabaseAndSchema);//???
            }
           

            dict.GetDataStoreSchema(typeof(ScannedDocumentHeader).Assembly);
            IDataLayer dl = new ThreadSafeDataLayer(dict, store);
            return dl;
        }

        public static void UpdateDatabase()
        {

            using (IDataLayer dal = XpoDefault.GetDataLayer(GetConnectionString(), AutoCreateOption.DatabaseAndSchema))
            {
                using (UnitOfWork session = new UnitOfWork(dal))
                {
                    Assembly asm = Assembly.GetAssembly(typeof(ScannedDocumentHeader));
                    session.UpdateSchema(asm);
                    session.CreateObjectTypeRecords(asm);
                    session.FlushChanges();
                    session.CommitChanges();

                }
            }
        }



        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return value.ToString();
        }
    }
}

