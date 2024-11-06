using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Linq;
using ITS.Retail.Platform.Kernel.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using System.Collections.Generic;
using DevExpress.Data.Filtering;

namespace ITS.Retail.WRM.Kernel
{
    public class WRMDbModule : IWRMDbModule, IDisposable
    {
        UnitOfWork unitOfWork = null;
        public WRMDbModule()
        {
            XpoHelper.DisableCache = true;
            unitOfWork = XpoHelper.GetNewUnitOfWork();
        }

        public void CommitChanges()
        {
            unitOfWork.CommitChanges();
        }

        public object CreateObject(Type type)
        {
            if (typeof(XPBaseObject).IsAssignableFrom(type))
            {
                return Activator.CreateInstance(type, unitOfWork);
            }
            throw new ArgumentException("Type " + type.Name + " is invalid");
        }

        public T CreateObject<T>() where T : IPersistentObject
        {
            return (T)CreateObject(typeof(T));
        }

        public void Dispose()
        {
            if (unitOfWork != null)
            {
                unitOfWork.Dispose();
            }
        }

        public object GetObjectByKey(Guid key, Type type)
        {
            CriteriaOperator KeyCriteria = CriteriaOperator.And(new BinaryOperator("Oid", key, BinaryOperatorType.Equal));
            return unitOfWork.FindObject(type, KeyCriteria);
        }

        public T GetObjectByKey<T>(Guid key) where T : IPersistentObject
        {
            CriteriaOperator KeyCriteria = CriteriaOperator.And(new BinaryOperator("Oid", key, BinaryOperatorType.Equal));
            return unitOfWork.FindObject<T>(KeyCriteria);
        }

        public IQueryable<T> Query<T>() where T : IPersistentObject
        {
            return unitOfWork.Query<T>();
        }

        public IList<T> GetList<T>(CriteriaOperator crop) where T : IPersistentObject
        {
            return new XPCollection<T>(unitOfWork, crop);
        }

        public UnitOfWork GetUnitOfWork()
        {
            XpoHelper.DisableCache = true;
            unitOfWork = XpoHelper.GetNewUnitOfWork();
            return this.unitOfWork;
        }




    }
}
