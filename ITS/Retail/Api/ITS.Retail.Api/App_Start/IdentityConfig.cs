using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using ITS.Retail.Api.Models;
using System;
using ITS.Retail.Api.Authentication;
using ITS.Retail.WRM.Kernel.Interface;
using Microsoft.Owin.Security.DataProtection;

namespace ITS.Retail.Api
{
    /// <summary>
    /// ITS WRM Api Application User Manager
    /// </summary>
    public class ApplicationUserManager : UserManager<IdentityUser, Guid>
    {

        private IWRMUserModule _wrmUserModule;

        /// <summary>
        /// Creates a new instance of ApplicationUserManager
        /// </summary>
        /// <param name="store">The user store</param>
        /// <param name="wrmUserModule">the wrm user module</param>
        public ApplicationUserManager(IUserStore<IdentityUser, Guid> store, IWRMUserModule wrmUserModule)
            : base(store)
        {
            _wrmUserModule = wrmUserModule;
        }


        /// <summary>
        /// Creates a new instance of ApplicationUserManager
        /// </summary>
        /// <param name="options">The user manager options</param>
        /// <param name="context">the current owin conetxt</param>
        /// <returns></returns>
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            ApplicationUserManager applicationUserManager = new ApplicationUserManager(new UserStore(), context.Get<IWRMUserModule>());

            applicationUserManager.UserValidator = new UserValidator<IdentityUser,Guid>(applicationUserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            applicationUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            if (options.DataProtectionProvider != null)
            {
                IDataProtector dataProtector = options.DataProtectionProvider.Create("ASP.NET Identity");
                DataProtectorTokenProvider<IdentityUser, Guid> dataTokenProvider = new DataProtectorTokenProvider<IdentityUser, Guid>(dataProtector);
                
                dataTokenProvider.TokenLifespan = Startup.OAuthOptions.AccessTokenExpireTimeSpan;
                applicationUserManager.UserTokenProvider = dataTokenProvider;
               
            }
            return applicationUserManager;
        }

        /// <summary>
        /// Check if user provided password is corrent
        /// </summary>
        /// <param name="user">The user object</param>
        /// <param name="password">The password</param>
        /// <returns></returns>
        public override Task<bool> CheckPasswordAsync(IdentityUser user, string password)
        {
            string encodedPassword = _wrmUserModule.EncodePassword(password);
            return Task.FromResult<bool>(user.PasswordHash == encodedPassword);
        }
    }
}
