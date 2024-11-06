using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    /// <summary>
    /// Exposes an interface for a common, platform-wide access to the database.
    /// </summary>
    public interface IIntermediateModelManager : IKernelModule
    {
        T GetObjectByKey<T>(Guid? id) where T : IPersistentObject;
        T FindObject<T>(CriteriaOperator criteria) where T : IPersistentObject;
        T FindObject<T>(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, CriteriaOperator criteria) where T : IPersistentObject;
        T CreatePersistentObject<T>() where T : IPersistentObject;
        IEnumerable<T> GetCollection<T>(CriteriaOperator criteria) where T : IPersistentObject;
    }
}
