using ITS.Retail.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ITS.Retail.Api.Authentication
{
    /// <summary>
    /// Custom WRM Authorization Attribute: It affects only the controllers that derived <see cref="CustomODataController"/>. After successful authorization, 
    /// the <see cref="CustomODataController.AfterAuthorization"/> is invoked
    /// </summary>
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// The main authorization process is inherited from <see cref="AuthorizeAttribute"/>. After successful authorization, the
        /// <see cref="CustomODataController.AfterAuthorization"/> is invoked if the Controller derrived from <see cref="CustomODataController"/>.
        /// </summary>
        /// <param name="actionContext">The action context of current request</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
            if (actionContext.ControllerContext.Controller as CustomODataController != null &&
                    (actionContext.ControllerContext.Controller as CustomODataController).User.Identity.IsAuthenticated)
            {
                try
                {
                    (actionContext.ControllerContext.Controller as CustomODataController).AfterAuthorization();
                }
                catch (Exception ex)
                {

                }
            }

            if (actionContext.ControllerContext.Controller as UpdaterController != null &&
                   (actionContext.ControllerContext.Controller as UpdaterController).User.Identity.IsAuthenticated)
            {
                try
                {
                    (actionContext.ControllerContext.Controller as UpdaterController).AfterAuthorization();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}