using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common.Helpers;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ITS.Retail.WebClient.APIService
{
    public class AuthorizationService
    {
        private IObjectLayer ObjectLayer { get; set; }
        public AuthorizationService(IObjectLayer xpoObjectLayer)
        {
            this.ObjectLayer = xpoObjectLayer;
        }

        /// <summary>
        /// Returns the Guid of the user that is encrypted in auth cookie
        /// </summary>
        /// <param name="authCookie">The authentication cookie</param>
        /// <returns></returns>
        public UserVariables Authorize(string authCookie)
        {
            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie);
                if (ticket == null || string.IsNullOrWhiteSpace(ticket.Name) || ticket.Expired)
                {
                    return null;
                }

                using (UnitOfWork uow = new UnitOfWork(ObjectLayer))
                {
                    User user = uow.Query<User>().Where(usr => usr.UserName == ticket.Name).FirstOrDefault();
                    return PrepareUserVariables(user);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private UserVariables PrepareUserVariables(User user)
        {
            if (user == null)
            {
                return null;
            }
            UserVariables variables = new UserVariables()
            {
                Oid = user.Oid
            };
            if (UserHelper.IsSystemAdmin(user))
            {
                variables.AllowedCompanies = user.Session.Query<CompanyNew>().Select(comp => comp.Oid).ToList();
                variables.Company = variables.AllowedCompanies.Count == 1 ? variables.AllowedCompanies.First() : Guid.Empty;
            }
            else
            {
                CompanyNew company = UserHelper.GetCompany(user);
                variables.Company = company.Oid;
                variables.AllowedCompanies = BOApplicationHelper.GetUserEntities<CompanyNew>(user.Session, user).Select(x => x.Oid).ToList();
            }
            return variables;
        }
    }
}