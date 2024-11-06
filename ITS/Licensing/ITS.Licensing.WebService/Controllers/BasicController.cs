using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Licensing.Web.Controllers
{
    public abstract class BasicController : Controller
    {

        protected bool IsValidUserRequest(ActionExecutingContext filterContext)
        {
            return Session["USER"] != null;
        }

    }
}
