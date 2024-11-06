using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using ITS.Retail.Common;
using System.Security.Claims;
using ITS.Retail.Model;

namespace ITS.Retail.Api.Authentication
{
    public class UserStore : IUserStore<IdentityUser, Guid>,
        IUserLoginStore<IdentityUser, Guid>,
        IUserClaimStore<IdentityUser, Guid>,
        IUserRoleStore<IdentityUser, Guid>,
        IUserPasswordStore<IdentityUser, Guid>,
        IUserSecurityStampStore<IdentityUser, Guid>

    {
        /// <summary>
        /// Set to true if object has been disposed
        /// </summary>
        private bool _disposed;


        public UnitOfWork UnitOfWork { get; private set; }

        public UserStore()
        {
            UnitOfWork = XpoHelper.GetNewUnitOfWork();
            _disposed = false;
        }
        public Task CreateAsync(IdentityUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(IdentityUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (UnitOfWork != null)
            {
                UnitOfWork.Dispose();
            }
            UnitOfWork = null;
            _disposed = true;
        }

        public Task<IdentityUser> FindByIdAsync(Guid userId)
        {
            User user = UnitOfWork.Query<User>().FirstOrDefault(usr => usr.Oid == userId);
            if (user == null)
            {
                return Task.FromResult<IdentityUser>(null);
            }
            return Task.FromResult(new IdentityUser(user));
        }

        public Task<IdentityUser> FindByNameAsync(string userName)
        {
            User user = UnitOfWork.Query<User>().FirstOrDefault(usr => usr.UserName == userName);
            if (user == null)
            {
                return Task.FromResult<IdentityUser>(null);
            }
            return Task.FromResult(new IdentityUser(user));
        }

        public Task UpdateAsync(IdentityUser user)
        {
            throw new NotImplementedException();
        }


        public static UserStore Create()
        {
            return new UserStore();
        }

        public Task AddLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(IdentityUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            IList<Claim> claims = user.Claims.Select(claim => new Claim(claim.ClaimType, claim.ClaimValue)).ToList();
            return Task.FromResult(claims);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public Task AddClaimAsync(IdentityUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(IdentityUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(IdentityUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(IdentityUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(IdentityUser user)
        {
            IList<string> roles = new List<string>();
            roles.Add(user.Role.Description);
            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(IdentityUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(IdentityUser user)
        {
            return Task.FromResult(user.PasswordHash);
            //throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(IdentityUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(IdentityUser user, string stamp)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult<int>(0);
        }

        public Task<string> GetSecurityStampAsync(IdentityUser user)
        {
            if (string.IsNullOrWhiteSpace(user.SecurityStamp))
            {
                SetSecurityStampAsync(user, GetSecurityStamp(user));
            }
            return Task.FromResult(user.SecurityStamp);
        }

        private string GetSecurityStamp(IdentityUser user)
        {
            return Guid.NewGuid().ToString();
        }

    }
}
