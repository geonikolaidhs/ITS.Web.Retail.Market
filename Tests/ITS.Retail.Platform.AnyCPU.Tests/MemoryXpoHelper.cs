using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Platform.Tests
{
    public static class MemoryXpoHelper
    {
        private readonly static object lockObject = new object();

        private static Dictionary<Assembly, IDataLayer> dataLayers = new Dictionary<Assembly, IDataLayer>();
        public static Dictionary<Assembly, IDataLayer> AllActiveDataLayers
        {
            get
            {
                return dataLayers;
            }
        }

        private static IDataLayer GetDataLayer(Type type)
        {
            Assembly assembly = type.Assembly;
            if(dataLayers.ContainsKey(assembly))
            {
                return dataLayers[assembly];
            }

            XpoDefault.Session = null;
            XPDictionary dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(assembly);
            IDataLayer dl = new SimpleDataLayer(dict,new InMemoryDataStore());
            dataLayers.Add(assembly, dl);
            return dl;
        }

        public static void ClearDataLayer(Type type)
        {
            Assembly assembly = type.Assembly;
            if (dataLayers.ContainsKey(assembly))
            {
                IDataLayer dl = dataLayers[assembly];
                dl.Dispose();
                dataLayers.Remove(assembly);
            }
        }

        public static void ClearDataLayer<T>()
        {
            ClearDataLayer(typeof(T));
        }

        public static Session GetNewSession<T>()
        {
            return new Session(GetDataLayer(typeof(T)));
        }

        public static Session GetNewSession(Type type)
        {
            return new Session(GetDataLayer(type));
        }

        public static UnitOfWork GetNewUnitOfWork<T>()
        {
            return new UnitOfWork(GetDataLayer(typeof(T)));
        }

        public static UnitOfWork GetNewUnitOfWork(Type type)
        {
            return new UnitOfWork(GetDataLayer(type));
        }

    }
}
