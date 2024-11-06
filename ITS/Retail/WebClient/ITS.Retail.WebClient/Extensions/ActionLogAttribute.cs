using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Controllers;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Enumerations;
using NLog;
using System.Threading.Tasks;

namespace ITS.Retail.WebClient.Extensions
{
    public class ActionLogAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public ITS.Retail.Platform.Enumerations.LogLevel LogLevel { get; set; }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            bool performLog = ((int)(AppiSettings.LoggingLevel) >= (int)LogLevel && (int)LogLevel >= 0);//|| filterContext.ExceptionHandled ==false;

            if (performLog)
            {
                string message = filterContext.Exception == null || filterContext.ExceptionHandled ? "Sucess" : "Error";
                KernelLogLevel level = filterContext.Exception == null || filterContext.ExceptionHandled ? KernelLogLevel.Debug : KernelLogLevel.Error;
                BaseController controller = (filterContext.Controller as BaseController);
                Guid? currentUserId = controller == null || controller.CurrentUser == null ? null : (Guid?)controller.CurrentUser.Oid;
                MvcApplication.WRMLogModule.Log(
                    filterContext.Exception, "", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName,
                    filterContext.HttpContext.Request.UserAgent, filterContext.HttpContext.Request.UserHostAddress, message, currentUserId, level);
            }
        }
        

        public void OnException(ExceptionContext filterContext)
        {
            BaseController controller = (filterContext.Controller as BaseController);
            Guid? currentUserId = controller == null || controller.CurrentUser == null ? null : (Guid?)controller.CurrentUser.Oid;
            MvcApplication.WRMLogModule.Log(filterContext.Exception, "",
                filterContext.RequestContext.HttpContext.Request.RawUrl, "",
                filterContext.RequestContext.HttpContext.Request.UserAgent,
                filterContext.RequestContext.HttpContext.Request.UserHostAddress,
                "Error", currentUserId, 
                KernelLogLevel.Error);
        }


    }
}