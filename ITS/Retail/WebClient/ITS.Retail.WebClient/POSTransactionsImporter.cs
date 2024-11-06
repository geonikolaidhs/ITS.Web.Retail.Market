using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ITS.Retail.WebClient
{
    public class POSTransactionsImporter
    {
        private readonly static object lockObject = new object();
        private readonly static object lockObjectGeneral = new object();

        volatile IDataLayer fDataLayer;
        IDataLayer DataLayer
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

        public Session GetNewSession()
        {
            return new Session(DataLayer);
        }

        public UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }
        private IDataLayer GetDataLayer()
        {
            XpoDefault.Session = null;
            XPDictionary dict = new ReflectionDictionary();
            IDataStore store = XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.DatabaseAndSchema);
            dict.GetDataStoreSchema(typeof(ITS.POS.Model.Transactions.DocumentPayment).Assembly);

            DataCacheRoot cacheRoot = new DataCacheRoot(store);
            DataCacheConfiguration dcc = new DataCacheConfiguration();
            DataCacheNode cacheNode1 = new DataCacheNode(cacheRoot);
            IDataLayer dl = new ThreadSafeDataLayer(dict, cacheNode1);
            return dl;
        }
        protected string ConnectionString;


        public bool ImportTransactions(UnitOfWork localUow, String fileName, ITS.Retail.Model.POS localPOS)
        {
            //
            ConnectionString = @"XpoProvider=SQLite;Data Source=" + fileName;


            UnitOfWork posUow = this.GetNewUnitOfWork();
            XPCollection<ITS.POS.Model.Transactions.DailyTotals> fileDailyTotals = new XPCollection<ITS.POS.Model.Transactions.DailyTotals>(posUow);
            XPCollection<ITS.POS.Model.Transactions.DocumentHeader> fileDocuments = new XPCollection<ITS.POS.Model.Transactions.DocumentHeader>(posUow);
            XPCollection<ITS.POS.Model.Transactions.UserDailyTotals> fileUserDailyTotals = new XPCollection<ITS.POS.Model.Transactions.UserDailyTotals>(posUow);
            XPCollection<ITS.POS.Model.Transactions.DailyTotalsDetail> fileDailyTotalsDetails = new XPCollection<ITS.POS.Model.Transactions.DailyTotalsDetail>(posUow);
            XPCollection<ITS.POS.Model.Transactions.UserDailyTotalsDetail> fileUserDailyTotalsDetails = new XPCollection<ITS.POS.Model.Transactions.UserDailyTotalsDetail>(posUow);
            XPCollection<ITS.POS.Model.Transactions.DocumentDetail> fileDocumentsDetails = new XPCollection<ITS.POS.Model.Transactions.DocumentDetail>(posUow);
            XPCollection<ITS.POS.Model.Transactions.DocumentPayment> fileDocumentPayments = new XPCollection<ITS.POS.Model.Transactions.DocumentPayment>(posUow);



            InsertOrUpdate<ITS.POS.Model.Transactions.DailyTotals, ITS.Retail.Model.DailyTotals>(localUow, fileDailyTotals);
            InsertOrUpdate<ITS.POS.Model.Transactions.DailyTotalsDetail, ITS.Retail.Model.DailyTotalsDetail>(localUow, fileDailyTotalsDetails);
            InsertOrUpdate<ITS.POS.Model.Transactions.DocumentHeader, ITS.Retail.Model.DocumentHeader>(localUow, fileDocuments);
            InsertOrUpdate<ITS.POS.Model.Transactions.DocumentDetail, ITS.Retail.Model.DocumentDetail>(localUow, fileDocumentsDetails);
            InsertOrUpdate<ITS.POS.Model.Transactions.DocumentPayment, ITS.Retail.Model.DocumentPayment>(localUow, fileDocumentPayments);
            InsertOrUpdate<ITS.POS.Model.Transactions.UserDailyTotals, ITS.Retail.Model.UserDailyTotals>(localUow, fileUserDailyTotals);
            InsertOrUpdate<ITS.POS.Model.Transactions.UserDailyTotalsDetail, ITS.Retail.Model.UserDailyTotalsDetail>(localUow, fileUserDailyTotalsDetails);

            return false;
        }


        private void InsertOrUpdate<T,T1>(UnitOfWork localUow, XPCollection<T> collection)  where T : ITS.POS.Model.Settings.BaseObj where  T1: ITS.Retail.Model.BaseObj
        {
            foreach (T obj in collection)
            {
                InsertOrUpdate<T, T1>(localUow, obj);
            }
        }

        private void InsertOrUpdate<T, T1>(UnitOfWork localUow, T obj)
            where T : ITS.POS.Model.Settings.BaseObj
            where T1 : ITS.Retail.Model.BaseObj
        {
            T1 lObj = localUow.GetObjectByKey<T1>(obj.Oid);
            if (lObj == null)
            {
                lObj = Activator.CreateInstance(typeof(T1), localUow) as T1;
                lObj.Oid = obj.Oid;
            }
            PropertyInfo[] posProperties, retailProperties;
            posProperties = typeof(T).GetProperties();
            retailProperties = typeof(T1).GetProperties();
            foreach (PropertyInfo retailProperty in retailProperties)
            {
                if (retailProperty.GetCustomAttributes(typeof(ITS.Retail.Model.UpdaterIgnoreFieldAttribute), false).Count()>0)
                {
                    continue;
                }
                IEnumerable<PropertyInfo> posPropsI = posProperties.Where(g => g.Name == retailProperty.Name);
                if (posPropsI.Count() == 1)
                {
                    PropertyInfo posProperty = posPropsI.First();
                    if (posProperty.PropertyType == retailProperty.PropertyType && retailProperty.CanWrite)
                    {
                        try
                        {
                            retailProperty.SetValue(lObj, posProperty.GetValue(obj, null), null);
                        }
                        catch (Exception )
                        {

                        }
                    }
                    else if (posProperty.PropertyType == typeof(Guid) && retailProperty.PropertyType.IsSubclassOf(typeof(ITS.Retail.Model.BaseObj)) && retailProperty.CanWrite)
                    {
                        Guid g = (Guid)posProperty.GetValue(obj, null);
                        object o = localUow.GetObjectByKey(retailProperty.PropertyType, g);
                        retailProperty.SetValue(lObj, o, null);
                    }
                    else if (posProperty.PropertyType.IsSubclassOf(typeof(ITS.POS.Model.Settings.BaseObj)) && retailProperty.PropertyType.IsSubclassOf(typeof(ITS.Retail.Model.BaseObj)) && retailProperty.CanWrite)
                    {
                        ITS.POS.Model.Settings.BaseObj g = (ITS.POS.Model.Settings.BaseObj)posProperty.GetValue(obj, null);
                        object o = localUow.GetObjectByKey(retailProperty.PropertyType, g.Oid);
                        retailProperty.SetValue(lObj, o, null);
                    }
                    else if (posProperty.PropertyType.IsSubclassOf(typeof(XPBaseCollection)) && retailProperty.PropertyType.IsSubclassOf(typeof(XPBaseCollection)))
                    {
                    }
                }
            }
            lObj.SetMemberValue("GCRecord", obj.GetMemberValue("GCRecord"));
            lObj.Save();
        }


    }
}