using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Controllers;
using ITS.Retail.WebClient.Helpers;

namespace ITS.Retail.WebClient.Extensions
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Contains a map for Entitities that their Authorisation Rules MUST be consistent with another Entity.
        /// They Discitonary key is the Entity that has different  Authorisation rules, and the value is the relative entity name that we should retrieve the Authorisation rules from instead.
        /// </summary>
        private Dictionary<string, string> AuthorisationRuleModelNameMismatches = new Dictionary<string, string>(){
            {typeof(ItemBarcode).FullName.Split('.').Last(), typeof(Item).FullName.Split('.').Last()}
        };

        protected AuthorizationContext filterContext;
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext) == false)
            {
                return false;
            }
            User usr = (filterContext.Controller as BaseController).CurrentUser;
            if (usr == null)
            {
                return false;
            }
            
            bool returnValue = true;        
            Role rl = usr.Role;

            List<String> forbiddenStringList = usr.Role.RoleEntityAccessPermisions.Where(reap => reap.EnityAccessPermision.Visible == false)
                .Select(reap => reap.EnityAccessPermision.EntityType.Substring(reap.EnityAccessPermision.EntityType.LastIndexOf('.') + 1)).ToList();
            
            if (!UserHelper.IsSystemAdmin(usr))
            {
                forbiddenStringList.Add("POSRecovery");
                if (usr.Role.GDPRActions==false)
                {
                    forbiddenStringList.Add("GDPRActions");
                }

            }
            
            httpContext.Session["LayoutForbidden"] = forbiddenStringList;

            
            object[] attributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(SecurityAttribute),true);            
            if ((attributes.Length == 0 || attributes.Cast<SecurityAttribute>().First().OverrideSecurity == false))
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    string entityType = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;                    
                    if(AuthorisationRuleModelNameMismatches.Keys.Contains(entityType))
                    {
                        entityType = AuthorisationRuleModelNameMismatches[entityType];
                    }
                    RoleEntityAccessPermision rolePermissions = uow.FindObject<RoleEntityAccessPermision>(CriteriaOperator.And(new BinaryOperator("Role.Oid", rl.Oid),
                        new BinaryOperator("EnityAccessPermision.EntityType", entityType)
                        ));

                    if (rolePermissions == null)
                    {
                        returnValue = true;
                    }
                    else if (rolePermissions.EnityAccessPermision.Visible == true)
                    {
                        returnValue = true;
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
            }
            return returnValue;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            this.filterContext = filterContext;
            base.OnAuthorization(filterContext);            
        }

    }
}