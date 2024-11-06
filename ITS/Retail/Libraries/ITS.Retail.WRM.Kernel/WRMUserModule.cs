using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System.Security.Cryptography;

using DevExpress.Xpo;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.WRM.Kernel
{
    public class WRMUserModule : IWRMUserModule
    {
        IWRMDbModule wrmDbModule = null;
        public WRMUserModule(IWRMDbModule wrmDbModule)
        {
            this.wrmDbModule = wrmDbModule;
        }
        public bool CheckPasswordStrength(string password)
        {
            return password.Length >= 4;
        }

        public void Dispose()
        {
            if (wrmDbModule != null && wrmDbModule is IDisposable)
            {
                (wrmDbModule as IDisposable).Dispose();
                wrmDbModule = null;
            }
        }

        public string EncodePassword(string originalPassword)
        {
            //Declarations
            byte[] originalBytes;
            byte[] encodedBytes;
            byte[] doubleEncodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            using (md5 = new MD5CryptoServiceProvider())
            {
                originalBytes = Encoding.Default.GetBytes(originalPassword);
                encodedBytes = md5.ComputeHash(originalBytes);
            }

            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                doubleEncodedBytes = sha1.ComputeHash(encodedBytes);
            }
            return BitConverter.ToString(doubleEncodedBytes);
        }

        public string GeneratePassword(out string passphrase)
        {
            int characters = 8;
            string newPassword = Guid.NewGuid().ToString();
            newPassword = newPassword.Substring(0, characters);
            passphrase = newPassword;
            newPassword = EncodePassword(newPassword);
            return newPassword;
        }

        public ICompany GetCompany(IUser user)
        {
            XPCollection<CompanyNew> sup = GetUserEntities<CompanyNew>(user.Session, user);
            return sup.FirstOrDefault();
        }

        public ICustomer GetCustomer(IUser user)
        {
            return GetUserEntities<Customer>(user.Session, user).FirstOrDefault();
        }

        public IEnumerable<IStore> GetStoresThatUserOwns(IUser user)
        {
            IEnumerable<ICompany> userCompanies = this.GetUserCompanies(user);
            if (userCompanies == null || userCompanies.Count() == 0)
            {
                return new List<IStore>();
            }
            else
            {
                return GetUserEntities<Store>(user.Session, user).Where(g => g.Owner != null && userCompanies.Select(cp => cp.Oid).Contains(g.Owner.Oid));
            }
        }

        public IEnumerable<IStore> GetStoresWhereUserBuysFrom(IUser user)
        {
            ICompany userSupplier = GetCompany(user);
            if (userSupplier == null)
            {
                return new List<IStore>();
            }
            else
            {
                return GetUserEntities<Store>(user.Session, user).Where(g => g.Owner != null && g.Owner.Oid == userSupplier.Oid).Cast<IStore>();
            }
        }

        public ePermition GetUserEntityPermition(IUser user, string EntityType)
        {
            ePermition permition = new ePermition();

            if (user.Role == null)
            {
                return permition;
            }
            EntityAccessPermision eat = user.Session.FindObject<EntityAccessPermision>(CriteriaOperator.And(new BinaryOperator("EntityType", EntityType),
                                         new ContainsOperator("RoleEntityAccessPermisions", new BinaryOperator("Role.Oid", user.Role.Oid))));

            if (eat == null)
            {
                permition = (ePermition.Visible | ePermition.Insert | ePermition.Update | ePermition.Delete);
            }
            else
            {
                if (eat.CanDelete)
                {
                    permition = permition | ePermition.Delete;
                }
                if (eat.CanInsert)
                {
                    permition = permition | ePermition.Insert;
                }
                if (eat.CanUpdate)
                {
                    permition = permition | ePermition.Update;
                }
                if (eat.Visible)
                {
                    permition = permition | ePermition.Visible;
                }
            }

            return permition;
        }

        public IEnumerable<ICompany> GetUserCompanies(IUser user)
        {
            if (user.Role.Type != eRoleType.SystemAdministrator)
            {
                return GetUserEntities<CompanyNew>(user.Session, user);
            }
            return wrmDbModule.Query<CompanyNew>();
        }

        public UserType GetUserType(IUser user)
        {
            if (user.Role.Type == eRoleType.Customer || user.Role.Type == eRoleType.Supplier)
            {
                return UserType.TRADER;
            }
            else if (user.Role.Type == eRoleType.CompanyUser)
            {
                return UserType.COMPANYUSER;
            }
            else if (user.Role.Type == eRoleType.CompanyAdministrator || user.Role.Type == eRoleType.SystemAdministrator)
            {
                return UserType.ADMIN;
            }
            return UserType.NONE;
        }

        public bool IsAdmin(IUser user)
        {
            return IsSystemAdmin(user) || IsCompanyAdmin(user);
        }

        public bool IsSystemAdmin(IUser user)
        {
            return user != null &&
                 user.Role.Type == eRoleType.SystemAdministrator;
        }

        public bool UserCanLoginToCurrentStore(IUser user, eApplicationInstance applicationInstance, Guid storeOid, out string message)
        {
            message = "";
            if (IsSystemAdmin(user) || IsCompanyAdmin(user))
            {
                return true;
            }
            else if (applicationInstance == eApplicationInstance.RETAIL)
            {
                message = Resources.NoStoreAccess;
                throw new NotImplementedException();
                //return BOApplicationHelper.GetUserEntities<Store>(user.Session, user).Count() > 0;
            }
            else
            {
                message = Resources.YouDoNotHaveAccessToThisStore;
                throw new NotImplementedException();
                //return !IsCustomer(user) && BOApplicationHelper.GetEntityCollectionUsers(user.Session, new List<Guid>() { storeOid }).Contains(user);
            }
        }



        private bool IsCompanyAdmin(IUser user)
        {
            return user != null &&
                    user.Role.Type == eRoleType.CompanyAdministrator;

        }


        private static XPCollection<T> GetUserEntities<T>(Session session, IUser user = null)
        {
            IQueryable<UserTypeAccess> uta = new XPQuery<UserTypeAccess>(session);
            if (user != null)
            {
                uta = uta.Where(ut => ut.User.Oid == user.Oid);
            }
            uta = uta.Where(ut => ut.EntityType == typeof(T).ToString());

            List<Guid> guids = uta.Select(ut => ut.EntityOid).ToList();

            return new XPCollection<T>(session, new InOperator("Oid", guids));
        }

    }
}
