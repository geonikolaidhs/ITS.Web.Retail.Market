using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Provides POS specific implementation for model access, that is used in common platform services
    /// </summary>
    public class IntermediateModelManager : IIntermediateModelManager
    {
        private ISessionManager SessionManager { get; set; }
        private IPlatformPersistentObjectMap PersistentObjectMap { get; set; }

        public IntermediateModelManager(ISessionManager sessionManager, IPlatformPersistentObjectMap persistentObjectMap)
        {
            SessionManager = sessionManager;
            PersistentObjectMap = persistentObjectMap;
        }

        public T GetObjectByKey<T>(Guid? id) where T : IPersistentObject
        {
            Type concreteType = PersistentObjectMap.ResolveType<T>();
            UnitOfWork uow = SessionManager.GetSession(concreteType);
            T result = (T)uow.GetObjectByKey(concreteType, id);
            return result;
        }

        public T FindObject<T>(CriteriaOperator criteria) where T : IPersistentObject
        {
            Type concreteType = PersistentObjectMap.ResolveType<T>();
            UnitOfWork uow = SessionManager.GetSession(concreteType);
            T result = (T)uow.FindObject(concreteType, criteria);
            return result;
        }

        public T FindObject<T>(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, CriteriaOperator criteria) where T : IPersistentObject
        {
            Type concreteType = PersistentObjectMap.ResolveType<T>();
            UnitOfWork uow = SessionManager.GetSession(concreteType);
            T result = (T)uow.FindObject(criteriaEvaluationBehavior, concreteType, criteria);
            return result;
        }

        public T CreatePersistentObject<T>() where T : IPersistentObject
        {
            Type concreteType = PersistentObjectMap.ResolveType<T>();
            UnitOfWork uow = SessionManager.GetSession(concreteType);
            return (T)Activator.CreateInstance(concreteType, (Session)uow);
        }

        public IEnumerable<T> GetCollection<T>(CriteriaOperator criteria) where T : IPersistentObject
        {
            Type concreteType = PersistentObjectMap.ResolveType<T>();
            UnitOfWork uow = SessionManager.GetSession(concreteType);

            return new XPCollection(uow, concreteType, criteria).Cast<T>();
        }
    }
}
