using DevExpress.Xpo;
using ITS.Retail.Model;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ITS.Retail.Api.Authentication
{
    public class IdentityUser : IUser<Guid>
    {
        //TODO
        private User User { get; set; }

        public string CompanyTaxCode
        {
            get
            {
                if(this.User == null)
                {
                    return String.Empty;
                }

                UserTypeAccess userTypeAccess = User.UserTypeAccesses.FirstOrDefault(ace => ace.EntityType == typeof(CompanyNew).FullName);
                if ( userTypeAccess == null )
                {
                    return String.Empty;
                }

                CompanyNew company = this.User.Session.GetObjectByKey<CompanyNew>(userTypeAccess.EntityOid);
                if ( company == null)
                {
                    return String.Empty;
                }
                return company.Trader.TaxCode;
            }
        }

        public Guid Id
        {
            get
            {
                return User.Oid;
            }
        }

        public string UserName
        {
            get
            {
                return User.UserName;
            }

            set
            {
                //TODO
                User.UserName = value;
            }
        }

        public string PasswordHash
        {
            get
            {
                return User.Password;
            }

        }


        public string SecurityStamp
        {
            get
            {
                return User.AuthToken;
            }
            set
            {
                User.AuthToken = value;
                User.Save();
                (User.Session as UnitOfWork).CommitChanges();
            }
        }

        public ITS.Retail.Platform.Kernel.Model.IRole Role
        {
            get
            {
                return User.Role;
            }
        }
        public virtual List<IdentityUserClaim> Claims { get; private set; }

        public IdentityUser(User user) : this()
        {
            this.User = user;
        }

        public IdentityUser(Guid id, UnitOfWork uow) : this()
        {
            this.User = uow.Query<User>().FirstOrDefault(x => x.Oid == id);
        }

        public IdentityUser(Guid id, UserStore userStore) : this()
        {
            this.User = userStore.UnitOfWork.Query<User>().FirstOrDefault(x => x.Oid == id);
        }

        private IdentityUser()
        {
            this.Claims = new List<IdentityUserClaim>();
            //this.Roles = new List<string>();
            //this.Logins = new List<UserLoginInfo>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser,Guid> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}