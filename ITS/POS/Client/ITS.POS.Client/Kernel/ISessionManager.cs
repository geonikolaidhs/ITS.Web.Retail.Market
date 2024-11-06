using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface ISessionManager : IKernelModule
    {
        UnitOfWork CreateSession(Type type);
        UnitOfWork GetSession<T>() where T : BasicObj;
        UnitOfWork GetSession(Type type);
        T GetObjectByKey<T>(Guid? id) where T : BasicObj;
        T FindObject<T>(CriteriaOperator criteria) where T : BasicObj;
        T FindObject<T>(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, CriteriaOperator criteria) where T : BasicObj;
        UnitOfWork MemorySettings { get; }
        void CommitTransactionsChanges();
        void ClearMasterSession();
        void ClearSettingsSession();
        void ClearTransactionsSession();
        void ClearAllSessions();
        void FillDenormalizedFields(BasicObj obj);
        void ReconnectToNewFile(IDataLayer fDataLayer, IEnumerable<IDisposable> _objectsToDisposeOnDisconnect);
    }
}
