using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering.Helpers;
using System.Reflection;
using DevExpress.Xpo;

namespace ITS.Retail.WRM.Kernel
{
    public class WRMUserDbModule : IWRMUserDbModule
    {
        IWRMDbModule WrmDbModule { get; set; }
        IWRMUserModule WrmUserModule { get; set; }
        public WRMUserDbModule(IWRMDbModule wrmDbModule, IWRMUserModule wrmUserModule)
        {
            WrmUserModule = wrmUserModule;
            WrmDbModule = wrmDbModule;
        }

        public IList<T> GetList<T>(CriteriaOperator crop) where T : IPersistentObject
        {
            return WrmDbModule.GetList<T>(crop);
        }

        public T GetObjectByKey<T>(Guid key) where T : IPersistentObject
        {
            return WrmDbModule.GetObjectByKey<T>(key);
        }

        public IQueryable<T> Query<T>() where T : IPersistentObject
        {
            return WrmDbModule.Query<T>();
        }


        public IQueryable<T> Query<T>(IUser requestedUser) where T : IPersistentObject
        {
            //TODO apply restrictions     
            CriteriaOperator criteria = GetUserReadRestrictions(typeof(T), requestedUser);
            //return unitOfWork.Query<T>();

            return criteria == null ? WrmDbModule.Query<T>() : WrmDbModule.Query<T>().AppendWhere(new CriteriaToExpressionConverter(), criteria).Cast<T>();
        }

        private CriteriaOperator GetUserReadRestrictions(Type type, IUser requestedUser)
        {
            // TODO add role permissions

            CriteriaOperator owner = GetOwnerRestrictions(type, requestedUser);
            CriteriaOperator store = GetStoreRestrictions(type, requestedUser as User);
            CriteriaOperator role = GetRoleReadRestrictions(type, requestedUser as User);
            return CriteriaOperator.And(owner, store, role);
        }

        private CriteriaOperator GetRoleReadRestrictions(Type type, User user)
        {
            RoleEntityAccessPermision permission = user.Role.RoleEntityAccessPermisions.FirstOrDefault(rolePerm => rolePerm.EnityAccessPermision.EntityType == type.Name);
            if (permission != null && permission.EnityAccessPermision.Visible == false)
            {
                return new BinaryOperator("Oid", Guid.Empty);
            }
            return null;
        }

        private CriteriaOperator GetStoreRestrictions(Type type, User user)
        {
            //throw new NotImplementedException();
            return null; //TODO
        }

        private CriteriaOperator GetOwnerRestrictions(Type type, IUser requestedUser)
        {
            if (typeof(IOwner).IsAssignableFrom(type))
            {
                if (type.GetProperty("Owner").CanWrite == false)
                {
                    return null;
                }
                List<Guid> companiesOids = WrmUserModule.GetUserCompanies(requestedUser).Select(company => company.Oid).ToList();
                if (typeof(IRequiredOwner).IsAssignableFrom(type))
                {
                    return new InOperator("Owner.Oid", companiesOids);
                }
                return CriteriaOperator.Or(new NullOperator("Owner"), new InOperator("Owner.Oid", companiesOids));
            }
            return null;
        }

        public void CommitChanges()
        {
            this.WrmDbModule.CommitChanges();
        }

        public ePermition Access<T>(T obj, IUser requestedUser)
        {
            User currentUser = requestedUser as User;
            RoleEntityAccessPermision permision = currentUser.Role.RoleEntityAccessPermisions.FirstOrDefault(rolePerm => rolePerm.EnityAccessPermision.EntityType == typeof(T).Name);
            if (permision == null)
            {
                return ePermition.Delete | ePermition.Insert | ePermition.Update | ePermition.Visible;
            }
            ePermition returnValue = ePermition.None;
            if (permision.EnityAccessPermision.CanDelete)
            {
                returnValue |= ePermition.Delete;
            }
            if (permision.EnityAccessPermision.CanInsert)
            {
                returnValue |= ePermition.Insert;
            }
            if (permision.EnityAccessPermision.CanUpdate)
            {
                returnValue |= ePermition.Update;
            }
            if (permision.EnityAccessPermision.Visible)
            {
                returnValue |= ePermition.Visible;
            }
            return returnValue;
        }

        public T GetObjectByKey<T>(IUser requestedUser, Guid key) where T : IPersistentObject
        {
            T objectToReturn = this.GetObjectByKey<T>(key);
            if (objectToReturn == null ||
                    new ExpressionEvaluator(
                                    new EvaluatorContextDescriptorDefault(typeof(T)),
                                    GetUserReadRestrictions(typeof(T), requestedUser)
                   ).Fit(objectToReturn) == false)
            {
                return default(T);
            }
            return objectToReturn;
        }

        public object GetObjectByKey(Guid key, Type type)
        {
            return this.WrmDbModule.GetObjectByKey(key, type);
        }

        public object CreateObject(Type type)
        {
            return this.WrmDbModule.CreateObject(type);
        }

        public T CreateObject<T>() where T : IPersistentObject
        {
            return this.WrmDbModule.CreateObject<T>();
        }

        public object CreateObject(Type type, IUser requestedUser)
        {
            BasicObj obj = CreateObject(type) as BasicObj;
            if (obj != null)
            {
                User user = this.WrmDbModule.GetObjectByKey<User>(requestedUser.Oid);
                obj.CreatedBy = user;
                obj.UpdatedBy = user;
                obj.CreatedOnTicks = obj.UpdatedOnTicks = DateTime.Now.Ticks;
            }
            return obj;
        }

        public T CreateObject<T>(IUser requestedUser) where T : IPersistentObject
        {
            T obj = CreateObject<T>();
            User user = this.WrmDbModule.GetObjectByKey<User>(requestedUser.Oid);
            (obj as BasicObj).CreatedBy = user;
            (obj as BasicObj).UpdatedBy = user;
            (obj as BasicObj).CreatedOnTicks = (obj as BasicObj).UpdatedOnTicks = DateTime.Now.Ticks;
            return obj;
        }

        public void AssignOwner<T>(T obj, IUser requestedUser, Guid owner) where T : IPersistentObject
        {
            Type type = obj.GetType();
            if (typeof(IOwner).IsAssignableFrom(type))
            {
                PropertyInfo ownerProperty = type.GetProperty("Owner");
                if (ownerProperty.CanWrite)
                {
                    CompanyNew ownerToAssign = null;
                    IEnumerable<ICompany> activeCompanies = this.WrmUserModule.GetUserCompanies(requestedUser);
                    if (activeCompanies.Count() == 1)
                    {
                        ownerToAssign = obj.Session.GetObjectByKey<CompanyNew>(activeCompanies.First().Oid);
                    }
                    else if (activeCompanies.Any(comp => comp.Oid == owner))
                    {
                        ownerToAssign = obj.Session.GetObjectByKey<CompanyNew>(owner);
                    }
                    else
                    {
                        throw new Exception("Owner cannot be determined");
                    }
                    ownerProperty.SetValue(obj, ownerToAssign, null);
                }
            }
        }

        public void Dispose()
        {
            if (WrmDbModule != null)
            {
                WrmDbModule.Dispose();
                WrmDbModule = null;
            }
        }

        public UnitOfWork GetUnitOfWork()
        {
            if (WrmDbModule != null)
            {
                IWRMDbModule module = (IWRMDbModule)WrmDbModule;
                return module.GetUnitOfWork();
            }
            return null;
        }
    }
}
