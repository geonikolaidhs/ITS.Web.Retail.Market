using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient
{
    public class DisplayAttribute : ActionFilterAttribute
    {
        public bool ShowSettings { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            filterContext.Controller.ViewBag.ShowSettings = this.ShowSettings;
        }
        
    }
}