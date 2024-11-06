using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Extensions
{
    public class AjaxOrChildActionOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            /*if (!filterContext.HttpContext.Request.IsAjaxRequest() &&
                !filterContext.IsChildAction
            )
            {
                filterContext.Result = new HttpNotFoundResult();
            }*/
        }
    }
}