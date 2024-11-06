using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Licensing.Web.Controllers
{
    public abstract class BaseObjController : BasicController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            bool valid = IsValidUserRequest(filterContext);

            base.OnActionExecuting(filterContext);
            if (valid == false)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
            }
        }

    }
}
