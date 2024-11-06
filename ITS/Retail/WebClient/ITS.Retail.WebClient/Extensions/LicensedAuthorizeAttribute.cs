using ITS.Licensing.Enumerations;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.AuxillaryClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Extensions
{
    public class LicensedAuthorizeAttribute : AuthorizeAttribute
    {


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            
            if(filterContext.IsChildAction)
            {
                return;
            }

            if ( MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER )
            {
                return;
            }

            if (LicenseExists == false)
            {
                if (filterContext.RequestContext.HttpContext.Request.IsLocal)
                {
                    filterContext.Result = new RedirectResult("~/License/Activate");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/License/NotActivated");
                }
            }
            else if (IsLicenseValid == false)
            {
                if (filterContext.RequestContext.HttpContext.Request.IsLocal)
                {
                    filterContext.Result = new RedirectResult("~/License/Index");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/License/NotActivated");
                }
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return IsLicenseValid;
        }

        protected bool LicenseExists
        {
            get
            {
                return (MvcApplication.USES_LICENSE == false) || File.Exists(MvcApplication.LICENSE_FILE);
            }
        }

        protected bool IsLicenseValid
        {
            get
            {
                return (MvcApplication.USES_LICENSE == false)
                    || MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER
                    || (LicenseExists
                           && (MvcApplication.LicenseStatus == LicenseStatus.Valid || MvcApplication.LicenseStatus == LicenseStatus.Warning)
                       );
            }
        }
    }
}