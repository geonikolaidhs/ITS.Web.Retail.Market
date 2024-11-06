using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.WRM.Kernel.Interface
{
    public interface IWRMDbModule : IKernelModule
    {
        IQueryable<T> Query<T>() where T : IPersistentObject;

        T GetObjectByKey<T>(Guid key) where T : IPersistentObject;

        void CommitChanges();

        object GetObjectByKey(Guid key, Type type);
        object CreateObject(Type type);

        T CreateObject<T>() where T : IPersistentObject;

        IList<T> GetList<T>(CriteriaOperator crop) where T : IPersistentObject;

        UnitOfWork GetUnitOfWork();

        void Dispose();
    }
}
