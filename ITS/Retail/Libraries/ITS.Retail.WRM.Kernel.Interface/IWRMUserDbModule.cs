using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.WRM.Kernel.Interface
{
    public interface IWRMUserDbModule : IWRMDbModule
    {
        /// <summary>
        /// Gets a query of T after applying the user restrictions
        /// </summary>
        /// <typeparam name="T">The object type of the query returned</typeparam>
        /// <param name="requestedUser">The user requesting the query</param>
        /// <returns></returns>
        IQueryable<T> Query<T>(IUser requestedUser) where T : IPersistentObject;

        T GetObjectByKey<T>(IUser requestedUser, Guid key) where T : IPersistentObject;

        /// <summary>
        /// Returns the permissions of a user for specific object
        /// </summary>
        /// <typeparam name="T">The type of the object to check</typeparam>
        /// <param name="obj">The object to check</param>
        /// <param name="requestedUser">The user to check</param>
        /// <returns></returns>
        ePermition Access<T>(T obj, IUser requestedUser);

        object CreateObject(Type type, IUser requestedUser);

        T CreateObject<T>(IUser requestedUser) where T : IPersistentObject;
        IList<T> GetList<T>(CriteriaOperator crop) where T : IPersistentObject;

        void AssignOwner<T>(T obj, IUser requestedUser, Guid owner) where T : IPersistentObject;

        //UnitOfWork GetUserUnitOfWork();


    }
}
